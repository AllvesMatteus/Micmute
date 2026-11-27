using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MicMute
{
    public partial class AboutForm : Form
    {
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int attrLen);

        private readonly string lang;
        private readonly UpdateService updater;

        public AboutForm(string lang, UpdateService updater = null)
        {
            this.lang = lang;
            this.updater = updater;
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyWindowTheme();
        }

        private void ApplyWindowTheme()
        {
            try
            {
                if (!this.IsHandleCreated) return;

                int useDark = 1;
                if (DwmSetWindowAttribute(this.Handle, 20, ref useDark, sizeof(int)) != 0)
                {
                    DwmSetWindowAttribute(this.Handle, 19, ref useDark, sizeof(int));
                }

                int colorVal = 0x202020;
                DwmSetWindowAttribute(this.Handle, 34, ref colorVal, sizeof(int));
                DwmSetWindowAttribute(this.Handle, 35, ref colorVal, sizeof(int));
            }
            catch { }
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            bool en = lang == "EN";
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            string shortVer = UpdateService.FormatShortVersion(v);

            lblTitle.Text = "MicMute v" + shortVer;
            this.Text = en ? "About MicMute" : "Sobre o MicMute";
            lblAuthorLabel.Text = en ? "Developed by:" : "Desenvolvido por:";
            lblAuthorName.Text = "AllvesMatteus";
            lblNotesTitle.Text = en ? "Version details:" : "Detalhes da versão:";
            btnReleasePage.Text = en ? "Repository" : "Repositório";

            string arch = Environment.Is64BitProcess ? "64-bit" : "32-bit";
            lblBuildInfo.Text = string.Format("Build {0} • .NET Framework 4.8 ({1})", v, arch);

            // Release date (from GitHub release metadata or executable build timestamp)
            DateTime? relDate = updater != null ? updater.ReleaseDate : null;
            if (!relDate.HasValue)
            {
                try
                {
                    string exePath = Assembly.GetExecutingAssembly().Location;
                    if (!string.IsNullOrEmpty(exePath) && File.Exists(exePath))
                        relDate = File.GetLastWriteTime(exePath);
                }
                catch { }
            }

            if (relDate.HasValue)
            {
                string formattedDate = en
                    ? relDate.Value.ToString("yyyy-MM-dd HH:mm")
                    : relDate.Value.ToString("dd/MM/yyyy 'às' HH:mm");
                lblReleaseDate.Text = (en ? "Released on: " : "Lançado em: ") + formattedDate;
            }
            else
            {
                lblReleaseDate.Text = en ? "Released on: N/A" : "Lançado em: N/D";
            }

            // Update status indicator
            UpdateState st = updater != null ? updater.State : UpdateState.Idle;
            if (st == UpdateState.Downloaded || st == UpdateState.Available || st == UpdateState.Downloading)
            {
                string latestStr = updater.LatestVersion != null ? UpdateService.FormatShortVersion(updater.LatestVersion) : "";
                lblVersionStatus.Text = en
                    ? "⬆ Update available: v" + latestStr
                    : "⬆ Nova versão disponível: v" + latestStr;
                lblVersionStatus.ForeColor = FluentTheme.AccentHover;
            }
            else
            {
                lblVersionStatus.Text = en
                    ? "✓ Latest version installed"
                    : "✓ Versão mais recente instalada";
                lblVersionStatus.ForeColor = Color.FromArgb(40, 200, 110);
            }

            // Release notes / details
            string notes = updater != null ? updater.ReleaseNotes : null;
            if (string.IsNullOrWhiteSpace(notes))
            {
                notes = en
                    ? "• Automatic updates via GitHub Releases\r\n• Single-instance protection & self-healing Watchdog\r\n• USB/Bluetooth audio reconnect freeze fix"
                    : "• Atualizador automático via GitHub Releases\r\n• Instância única com auto-recuperação (Watchdog)\r\n• Correção de travamento ao reconectar USB/Bluetooth";
            }
            lblNotesBody.Text = notes;

            // Set application icon if available
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"assets\icons\micon.ico");
            if (File.Exists(iconPath))
            {
                this.Icon = new Icon(iconPath);
            }

            // Load brand images
            string githubPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"assets\icons\github.png");
            string linkedinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"assets\icons\linkedin.png");

            if (File.Exists(githubPath))
            {
                btnGithub.Image = MakeWhiteIcon(Image.FromFile(githubPath));
            }
            if (File.Exists(linkedinPath))
            {
                btnLinkedin.Image = Image.FromFile(linkedinPath);
            }
        }

        private static Bitmap MakeWhiteIcon(Image src)
        {
            Bitmap bmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Bitmap orig = new Bitmap(src))
            {
                for (int y = 0; y < orig.Height; y++)
                {
                    for (int x = 0; x < orig.Width; x++)
                    {
                        Color c = orig.GetPixel(x, y);
                        if (c.A > 0)
                        {
                            bmp.SetPixel(x, y, Color.FromArgb(c.A, 255, 255, 255));
                        }
                    }
                }
            }
            src.Dispose();
            return bmp;
        }

        private void BtnGithub_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/AllvesMatteus");
        }

        private void BtnLinkedin_Click(object sender, EventArgs e)
        {
            OpenUrl("https://www.linkedin.com/in/allves-matteus/");
        }

        private void BtnReleasePage_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/AllvesMatteus/Micmute");
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                string errMsg = lang == "EN" ? "Could not open link: " : "Não foi possível abrir o link: ";
                string titleMsg = lang == "EN" ? "Error" : "Erro";
                MessageBox.Show(errMsg + ex.Message, titleMsg, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
