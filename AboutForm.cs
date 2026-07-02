using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MicMute
{
    public partial class AboutForm : Form
    {
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int attrLen);

        private string lang;

        public AboutForm(string lang)
        {
            this.lang = lang;
            InitializeComponent();
            
            // Apply theme dynamically matching system personalization settings
            ApplyWindowTheme();
        }

        private bool IsSystemDarkTheme()
        {
            try
            {
                using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        object value = key.GetValue("AppsUseLightTheme");
                        if (value != null)
                        {
                            return (int)value == 0;
                        }
                    }
                }
            }
            catch { }
            return true;
        }

        private void ApplyWindowTheme()
        {
            try
            {
                bool isDark = IsSystemDarkTheme();
                int useDark = isDark ? 1 : 0;
                DwmSetWindowAttribute(this.Handle, 20, ref useDark, sizeof(int));
                
                if (isDark)
                {
                    int colorVal = 0x202020;
                    DwmSetWindowAttribute(this.Handle, 34, ref colorVal, sizeof(int));
                    DwmSetWindowAttribute(this.Handle, 35, ref colorVal, sizeof(int));
                }
                else
                {
                    int defaultColor = -1;
                    DwmSetWindowAttribute(this.Handle, 34, ref defaultColor, sizeof(int));
                    DwmSetWindowAttribute(this.Handle, 35, ref defaultColor, sizeof(int));
                }
            }
            catch { }
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            // Apply translations
            if (lang == "EN")
            {
                this.Text = "About";
                lblAuthorLabel.Text = "Developed by:";
            }
            else
            {
                this.Text = "Sobre";
                lblAuthorLabel.Text = "Desenvolvido por:";
            }

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
                btnGithub.Image = Image.FromFile(githubPath);
            }
            if (File.Exists(linkedinPath))
            {
                btnLinkedin.Image = Image.FromFile(linkedinPath);
            }
        }

        private void BtnGithub_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/AllvesMatteus");
        }

        private void BtnLinkedin_Click(object sender, EventArgs e)
        {
            OpenUrl("https://www.linkedin.com/in/allves-matteus/");
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
