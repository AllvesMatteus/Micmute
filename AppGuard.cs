using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace MicMute
{
    /// <summary>
    /// Simple file logger (%LocalAppData%\MicMute\logs\app.log). Never throws.
    /// </summary>
    internal static class AppLog
    {
        private static readonly object Gate = new object();
        private const long MaxBytes = 512 * 1024;

        public static string LogsDir
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "MicMute", "logs");
            }
        }

        public static void Write(string message, Exception ex = null)
        {
            try
            {
                lock (Gate)
                {
                    Directory.CreateDirectory(LogsDir);
                    string path = Path.Combine(LogsDir, "app.log");
                    if (File.Exists(path) && new FileInfo(path).Length > MaxBytes)
                    {
                        string old = Path.Combine(LogsDir, "app.old.log");
                        if (File.Exists(old)) File.Delete(old);
                        File.Move(path, old);
                    }
                    string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [" + Process.GetCurrentProcess().Id + "] " + message;
                    if (ex != null) line += Environment.NewLine + ex;
                    File.AppendAllText(path, line + Environment.NewLine);
                }
            }
            catch { }
        }
    }

    /// <summary>
    /// Guarantees a single MicMute process per user session.
    /// - A 2nd launch asks the 1st one to show its window and waits for an acknowledgement.
    /// - If the 1st instance does not answer (UI thread frozen, e.g. after a USB audio hub
    ///   reconnect), it is considered hung, killed, and the new instance takes over.
    /// </summary>
    internal static class SingleInstance
    {
        private const string Id = "{0FD9EB53-0332-48A1-BFB3-4CBA95F37FDC}";
        private static Mutex mutex;
        private static bool owned;
        private static EventWaitHandle showEvent;
        private static EventWaitHandle ackEvent;

        /// <summary>Number of watchdog restarts that led to this process (--restarted=N).</summary>
        public static int RestartCount { get; private set; }

        public static bool TryAcquire(string[] args)
        {
            bool restarted = false;
            foreach (string a in args ?? new string[0])
            {
                if (a.StartsWith("--restarted", StringComparison.OrdinalIgnoreCase))
                {
                    restarted = true;
                    int n;
                    int eq = a.IndexOf('=');
                    if (eq > 0 && int.TryParse(a.Substring(eq + 1), out n)) RestartCount = n;
                    else RestartCount = 1;
                }
            }

            bool createdNew;
            mutex = new Mutex(true, @"Local\MicMute_Mutex_" + Id, out createdNew);
            showEvent = new EventWaitHandle(false, EventResetMode.AutoReset, @"Local\MicMute_Show_" + Id);
            ackEvent = new EventWaitHandle(false, EventResetMode.AutoReset, @"Local\MicMute_Ack_" + Id);

            if (createdNew)
            {
                owned = true;
                return true;
            }

            // Launched by the watchdog/updater: the previous process is dying, just wait for it.
            if (restarted && WaitForMutex(20000))
                return true;

            // Ask the running instance to show itself and answer.
            try
            {
                ackEvent.Reset();
                showEvent.Set();
                if (ackEvent.WaitOne(5000))
                    return false; // healthy instance is already running
            }
            catch { }

            // No answer: the running instance is frozen. Kill it and take over.
            AppLog.Write("Existing instance is not responding; terminating it.");
            KillOtherInstances();
            return WaitForMutex(8000);
        }

        private static bool WaitForMutex(int ms)
        {
            try
            {
                owned = mutex.WaitOne(ms);
            }
            catch (AbandonedMutexException)
            {
                owned = true; // previous owner was killed; we now own it
            }
            catch { owned = false; }
            return owned;
        }

        private static void KillOtherInstances()
        {
            try
            {
                var me = Process.GetCurrentProcess();
                foreach (var p in Process.GetProcessesByName(me.ProcessName).Where(x => x.Id != me.Id))
                {
                    try
                    {
                        p.Kill();
                        p.WaitForExit(3000);
                    }
                    catch (Exception ex) { AppLog.Write("Could not kill pid " + p.Id, ex); }
                }
            }
            catch (Exception ex) { AppLog.Write("KillOtherInstances failed", ex); }
        }

        /// <summary>
        /// Starts the background listener that answers other launches.
        /// <paramref name="onShow"/> runs on the UI thread; the ack is only sent after it ran,
        /// so a frozen UI thread never acknowledges (which is how the hang is detected).
        /// </summary>
        public static void StartListener(Control ui, Action onShow)
        {
            var t = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        showEvent.WaitOne();
                        if (ui.IsDisposed) return;
                        ui.BeginInvoke(new Action(() =>
                        {
                            try { onShow(); }
                            catch (Exception ex) { AppLog.Write("onShow failed", ex); }
                            finally { try { ackEvent.Set(); } catch { } }
                        }));
                    }
                    catch (ObjectDisposedException) { return; }
                    catch (InvalidOperationException) { Thread.Sleep(500); } // handle not ready yet
                    catch (Exception ex) { AppLog.Write("Listener error", ex); Thread.Sleep(500); }
                }
            });
            t.IsBackground = true;
            t.Name = "MicMute.SingleInstanceListener";
            t.Start();
        }

        public static void Release()
        {
            try { if (owned && mutex != null) mutex.ReleaseMutex(); } catch { }
        }
    }

    /// <summary>
    /// Detects a frozen UI thread and restarts the application automatically,
    /// so a hung tray icon (e.g. after USB/Bluetooth audio devices reconnect) heals itself.
    /// </summary>
    internal static class Watchdog
    {
        private const int HeartbeatMs = 2000;
        private const int CheckMs = 5000;
        private const int StaleThresholdMs = 15000;
        private const int MaxAutoRestarts = 3;

        private static int lastBeat = Environment.TickCount;
        private static System.Windows.Forms.Timer heartbeatTimer;
        private static Thread thread;
        private static volatile bool stopped;

        public static void Start()
        {
            Beat();

            // Runs on the UI thread: proves the message loop is alive.
            heartbeatTimer = new System.Windows.Forms.Timer { Interval = HeartbeatMs };
            heartbeatTimer.Tick += delegate { Beat(); };
            heartbeatTimer.Start();

            // After sleep/hibernate the tick counter is stale; do not treat it as a hang.
            SystemEvents.PowerModeChanged += delegate(object s, PowerModeChangedEventArgs e)
            {
                if (e.Mode == PowerModes.Resume) Beat();
            };
            SystemEvents.SessionSwitch += delegate { Beat(); };

            thread = new Thread(Run) { IsBackground = true, Name = "MicMute.Watchdog" };
            thread.Start();
        }

        public static void Stop()
        {
            stopped = true;
            try { if (heartbeatTimer != null) heartbeatTimer.Stop(); } catch { }
        }

        private static void Beat()
        {
            Interlocked.Exchange(ref lastBeat, Environment.TickCount);
        }

        private static void Run()
        {
            int strikes = 0;
            while (!stopped)
            {
                Thread.Sleep(CheckMs);
                if (stopped) return;

                int stale = unchecked(Environment.TickCount - Thread.VolatileRead(ref lastBeat));
                strikes = stale > StaleThresholdMs ? strikes + 1 : 0;

                if (strikes >= 2)
                {
                    HandleHang(stale);
                    strikes = 0;
                }
            }
        }

        private static void HandleHang(int staleMs)
        {
            AppLog.Write("Watchdog: UI thread unresponsive for " + staleMs + " ms.");

            if (SingleInstance.RestartCount >= MaxAutoRestarts)
            {
                AppLog.Write("Watchdog: restart limit reached, not restarting again.");
                return;
            }

            try
            {
                var psi = new ProcessStartInfo(Application.ExecutablePath, "--restarted=" + (SingleInstance.RestartCount + 1))
                {
                    UseShellExecute = false
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                AppLog.Write("Watchdog: failed to start replacement process", ex);
                return;
            }

            // Environment.Exit can itself block while the UI thread is stuck in COM: kill instead.
            Process.GetCurrentProcess().Kill();
        }
    }
}
