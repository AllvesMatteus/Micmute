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

        public AboutForm()
        {
            InitializeComponent();
            
            // Set Immersive Dark Mode for Title Bar (DWMWA_USE_IMMERSIVE_DARK_MODE = 20)
            int useDark = 1;
            DwmSetWindowAttribute(this.Handle, 20, ref useDark, sizeof(int));
            
            // Set Caption Background and Border Colors (DWMWA_BORDER_COLOR = 34, DWMWA_CAPTION_COLOR = 35) to #202020
            int colorVal = 0x202020;
            DwmSetWindowAttribute(this.Handle, 34, ref colorVal, sizeof(int));
            DwmSetWindowAttribute(this.Handle, 35, ref colorVal, sizeof(int));
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
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
                MessageBox.Show("Não foi possível abrir o link: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
