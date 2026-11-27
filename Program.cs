using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicMute
{
    static class Program
    {
        public static MainForm mf = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Route UI-thread exceptions to the log instead of the blocking .NET crash dialog
            // (a dialog nobody sees would leave a frozen, zombie tray process).
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => AppLog.Write("UI thread exception", e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => AppLog.Write("Unhandled exception", e.ExceptionObject as Exception);
            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                AppLog.Write("Unobserved task exception", e.Exception);
                e.SetObserved();
            };

            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            }
            catch { }

            // Only one MicMute per session. A frozen previous instance is killed and replaced.
            if (!SingleInstance.TryAcquire(args))
                return;

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                mf = new MainForm();
                Application.Run(mf);
            }
            catch (Exception ex)
            {
                AppLog.Write("Fatal error", ex);
                try
                {
                    string logsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                    System.IO.Directory.CreateDirectory(logsDir);
                    System.IO.File.WriteAllText(System.IO.Path.Combine(logsDir, "crash.txt"), ex.ToString());
                }
                catch { }
            }
            finally
            {
                Watchdog.Stop();
                SingleInstance.Release();
            }
        }
    }
}
