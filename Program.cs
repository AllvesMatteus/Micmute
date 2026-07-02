using System;
using System.Collections.Generic;
using System.Linq;
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
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                mf = new MainForm();
                Application.Run(mf);
            }
            catch (Exception ex)
            {
                try
                {
                    string logsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                    if (!System.IO.Directory.Exists(logsDir))
                    {
                        System.IO.Directory.CreateDirectory(logsDir);
                    }
                    string crashPath = System.IO.Path.Combine(logsDir, "crash.txt");
                    System.IO.File.WriteAllText(crashPath, ex.ToString());
                }
                catch
                {
                    try
                    {
                        string fallbackLogs = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MicMute", "logs");
                        System.IO.Directory.CreateDirectory(fallbackLogs);
                        string crashPath = System.IO.Path.Combine(fallbackLogs, "crash.txt");
                        System.IO.File.WriteAllText(crashPath, ex.ToString());
                    }
                    catch { }
                }
            }
        }
    }
}
