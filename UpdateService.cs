using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace MicMute
{
    public enum UpdateState
    {
        Idle, Checking, Available, Downloading, Downloaded, UpToDate, Error
    }

    [DataContract]
    internal class GhAsset
    {
        [DataMember(Name = "name")] public string Name { get; set; }
        [DataMember(Name = "size")] public long Size { get; set; }
        [DataMember(Name = "browser_download_url")] public string Url { get; set; }
        [DataMember(Name = "digest")] public string Digest { get; set; }
    }

    [DataContract]
    internal class GhRelease
    {
        [DataMember(Name = "tag_name")] public string TagName { get; set; }
        [DataMember(Name = "draft")] public bool Draft { get; set; }
        [DataMember(Name = "prerelease")] public bool Prerelease { get; set; }
        [DataMember(Name = "assets")] public List<GhAsset> Assets { get; set; }
    }

    /// <summary>
    /// Auto-updater based on GitHub Releases (same idea as electron-updater in the YouTube Music project):
    /// checks the latest release on startup and every 6 hours, downloads the installer in the
    /// background and then waits for the user to restart (one click) to apply it silently.
    /// </summary>
    public sealed class UpdateService
    {
        private const string Owner = "AllvesMatteus";
        private const string Repo = "Micmute";
        private static readonly string ApiUrl = "https://api.github.com/repos/" + Owner + "/" + Repo + "/releases/latest";
        private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(6);

        private readonly object gate = new object();
        private bool busy;
        private Timer timer;
        private string installerPath;

        /// <summary>Raised from a background thread whenever State/Progress changes.</summary>
        public event Action StateChanged;

        public UpdateState State { get; private set; }
        public int Progress { get; private set; }
        public string ErrorMessage { get; private set; }
        public Version LatestVersion { get; private set; }
        public Version CurrentVersion { get; private set; }

        public static string UpdatesDir
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "MicMute", "updates");
            }
        }

        public UpdateService()
        {
            CurrentVersion = Normalize(Assembly.GetExecutingAssembly().GetName().Version);
            State = UpdateState.Idle;
        }

        /// <summary>Starts periodic automatic checks (first one 10 s after startup).</summary>
        public void StartAutomatic()
        {
            if (timer != null) return;
            timer = new Timer(delegate { CheckAsync(false); }, null, TimeSpan.FromSeconds(10), CheckInterval);
        }

        public void CheckAsync(bool manual)
        {
            Task.Run(() => CheckCore(manual));
        }

        private async Task CheckCore(bool manual)
        {
            lock (gate)
            {
                if (busy) return;
                if (State == UpdateState.Downloading || State == UpdateState.Downloaded)
                {
                    if (manual) Raise();
                    return;
                }
                busy = true;
            }

            try
            {
                ErrorMessage = null;
                SetState(UpdateState.Checking, 0);

                GhRelease release = await FetchLatestAsync().ConfigureAwait(false);
                Version latest;
                if (release == null || release.Draft || release.Prerelease || !TryParseVersion(release.TagName, out latest))
                    throw new InvalidOperationException("Release inválida ou sem versão: " + (release == null ? "null" : release.TagName));

                if (latest <= CurrentVersion)
                {
                    SetState(UpdateState.UpToDate, 0);
                    return;
                }

                GhAsset asset = (release.Assets ?? new List<GhAsset>()).FirstOrDefault(a =>
                    a.Name != null &&
                    a.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) &&
                    a.Name.IndexOf("setup", StringComparison.OrdinalIgnoreCase) >= 0);
                if (asset == null || !IsTrustedUrl(asset.Url))
                    throw new InvalidOperationException("Instalador não encontrado na release " + release.TagName + ".");

                LatestVersion = latest;
                AppLog.Write("Update available: " + latest + " (current " + CurrentVersion + ")");
                SetState(UpdateState.Available, 0);

                await DownloadAsync(asset, latest).ConfigureAwait(false);
                AppLog.Write("Update downloaded: " + installerPath);
                SetState(UpdateState.Downloaded, 100);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                AppLog.Write("Update check failed", ex);
                SetState(UpdateState.Error, 0);
            }
            finally
            {
                lock (gate) busy = false;
            }
        }

        private static HttpClient CreateClient(TimeSpan timeout)
        {
            var http = new HttpClient { Timeout = timeout };
            http.DefaultRequestHeaders.UserAgent.ParseAdd("MicMute-Updater/1.0");
            http.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
            return http;
        }

        private static async Task<GhRelease> FetchLatestAsync()
        {
            using (var http = CreateClient(TimeSpan.FromSeconds(30)))
            using (var resp = await http.GetAsync(ApiUrl).ConfigureAwait(false))
            {
                resp.EnsureSuccessStatusCode();
                using (var stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    var ser = new DataContractJsonSerializer(typeof(GhRelease));
                    return (GhRelease)ser.ReadObject(stream);
                }
            }
        }

        private static bool IsTrustedUrl(string url)
        {
            Uri u;
            if (string.IsNullOrEmpty(url) || !Uri.TryCreate(url, UriKind.Absolute, out u)) return false;
            if (u.Scheme != Uri.UriSchemeHttps) return false;
            return u.Host.Equals("github.com", StringComparison.OrdinalIgnoreCase) ||
                   u.Host.EndsWith(".githubusercontent.com", StringComparison.OrdinalIgnoreCase);
        }

        private async Task DownloadAsync(GhAsset asset, Version version)
        {
            Directory.CreateDirectory(UpdatesDir);
            string final = Path.Combine(UpdatesDir, "MicMute_Setup_" + version + ".exe");
            string part = final + ".part";
            CleanOldInstallers(final);

            if (File.Exists(final) && (asset.Size <= 0 || new FileInfo(final).Length == asset.Size) && VerifyDigest(final, asset.Digest))
            {
                installerPath = final;
                return;
            }

            SetState(UpdateState.Downloading, 0);
            using (var http = CreateClient(TimeSpan.FromMinutes(30)))
            using (var resp = await http.GetAsync(asset.Url, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
            {
                resp.EnsureSuccessStatusCode();
                long total = resp.Content.Headers.ContentLength ?? asset.Size;
                using (var src = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var dst = new FileStream(part, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var buf = new byte[81920];
                    long read = 0;
                    int lastPct = -1;
                    int n;
                    while ((n = await src.ReadAsync(buf, 0, buf.Length).ConfigureAwait(false)) > 0)
                    {
                        await dst.WriteAsync(buf, 0, n).ConfigureAwait(false);
                        read += n;
                        if (total > 0)
                        {
                            int pct = (int)(read * 100 / total);
                            if (pct != lastPct) { lastPct = pct; SetState(UpdateState.Downloading, pct); }
                        }
                    }
                }
            }

            if (asset.Size > 0 && new FileInfo(part).Length != asset.Size)
            {
                File.Delete(part);
                throw new IOException("Download incompleto do instalador.");
            }
            if (!VerifyDigest(part, asset.Digest))
            {
                File.Delete(part);
                throw new IOException("Checksum do instalador não confere.");
            }

            if (File.Exists(final)) File.Delete(final);
            File.Move(part, final);
            installerPath = final;
        }

        private static bool VerifyDigest(string path, string digest)
        {
            // GitHub exposes "sha256:<hex>" for release assets. If absent, skip (size is still checked).
            if (string.IsNullOrEmpty(digest) || !digest.StartsWith("sha256:", StringComparison.OrdinalIgnoreCase))
                return true;
            using (var sha = SHA256.Create())
            using (var fs = File.OpenRead(path))
            {
                string actual = BitConverter.ToString(sha.ComputeHash(fs)).Replace("-", "");
                return actual.Equals(digest.Substring(7), StringComparison.OrdinalIgnoreCase);
            }
        }

        private static void CleanOldInstallers(string keep)
        {
            try
            {
                foreach (string f in Directory.GetFiles(UpdatesDir, "MicMute_Setup_*"))
                {
                    if (!string.Equals(f, keep, StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(f, keep + ".part", StringComparison.OrdinalIgnoreCase))
                    {
                        try { File.Delete(f); } catch { }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Launches the downloaded installer silently. The caller must exit the app right after.
        /// The installer relaunches MicMute when it finishes (see setup.iss).
        /// Returns false if there is nothing to install or the user denied the UAC prompt.
        /// </summary>
        public bool LaunchInstaller()
        {
            if (State != UpdateState.Downloaded || string.IsNullOrEmpty(installerPath) || !File.Exists(installerPath))
                return false;
            try
            {
                var psi = new ProcessStartInfo(installerPath, "/SILENT /SUPPRESSMSGBOXES /NORESTART /CLOSEAPPLICATIONS /FORCECLOSEAPPLICATIONS")
                {
                    UseShellExecute = true, // required to show the UAC elevation prompt
                    WorkingDirectory = UpdatesDir
                };
                Process.Start(psi);
                return true;
            }
            catch (Win32Exception ex)
            {
                AppLog.Write("Installer launch cancelled/failed", ex);
                return false;
            }
        }

        private void SetState(UpdateState state, int progress)
        {
            State = state;
            Progress = progress;
            Raise();
        }

        private void Raise()
        {
            try
            {
                var h = StateChanged;
                if (h != null) h();
            }
            catch (Exception ex) { AppLog.Write("StateChanged handler failed", ex); }
        }

        private static Version Normalize(Version v)
        {
            return new Version(Math.Max(v.Major, 0), Math.Max(v.Minor, 0), Math.Max(v.Build, 0), Math.Max(v.Revision, 0));
        }

        private static bool TryParseVersion(string tag, out Version version)
        {
            version = null;
            if (string.IsNullOrWhiteSpace(tag)) return false;
            string s = tag.Trim().TrimStart('v', 'V');
            int cut = s.IndexOfAny(new[] { '-', '+', ' ' });
            if (cut >= 0) s = s.Substring(0, cut);
            Version parsed;
            if (!Version.TryParse(s, out parsed)) return false;
            version = Normalize(parsed);
            return true;
        }
    }
}
