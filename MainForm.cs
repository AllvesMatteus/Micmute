using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using Microsoft.Win32;
using Shortcut;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Media;
using System.Reactive;
using System.Windows.Forms;

namespace MicMute
{
    public partial class MainForm : Form
    {
        private string DEFAULT_RECORDING_DEVICE = "Dispositivo de gravação padrão";
        public CoreAudioController AudioController = new CoreAudioController();
        private readonly RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\MicMute");
        private readonly GlobalKeyboardHook globalHook = new GlobalKeyboardHook();

        private bool isExiting = false;
        private Image imgOn = null;
        private Image imgOff = null;
        private Icon iconOn = null;
        private Icon iconOff = null;
        private Icon iconError = null;
        
        // toggle
        private readonly string registryKeyName = "Hotkey";
        private Hotkey hotkey;

        // mute
        private readonly string registryKeyMute = "HotkeyMute";
        private Hotkey muteHotkey;

        // unmute
        private readonly string registryKeyUnmute = "HotkeyUnmute";
        private Hotkey unMuteHotkey;

        private readonly string registryDeviceId = "DeviceId";
        private readonly string registryDeviceName = "DeviceName";

        private readonly string registryPlayMute = "PlayMute";
        private readonly string registryPlayUnmute = "PlayUnmute";
        private readonly string registrySoundMutePath = "SoundMutePath";
        private readonly string registrySoundUnmutePath = "SoundUnmutePath";

        private bool playSoundOnMute;
        private bool playSoundOnUnmute;
        private string soundMutePath;
        private string soundUnmutePath;

        private string selectedDeviceId;
        private string selectedDeviceName;

        enum MicStatus
        {
            Initial, On, Off, Error
        }
        private MicStatus currentStatus = MicStatus.Initial;

        private bool myVisible; 
        public bool MyVisible
        {
            get { return myVisible; }
            set { myVisible = value; Visible = value; }
        }

        // P/Invoke for Immersive Dark Mode and Icon Disposal
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int attrLen);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private Icon currentTrayIcon = null;
        private int soundVolume = 100;
        private string currentLang = "PT";

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyWindowTheme();
        }

        private void OnNextDevice(DeviceChangedArgs next)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateSelectedDevice));
            }
            else
            {
                UpdateSelectedDevice();
            }
        }

        private void MyHide()
        {
            ShowInTaskbar = false;
            Location = new Point(-10000, -10000);
            MyVisible = false;
        }

        private bool isLoadingMics = false;
        private void LoadMicsDropdown()
        {
            isLoadingMics = true;
            cbMics.Items.Clear();

            ComboboxItem defaultItem = new ComboboxItem();
            defaultItem.Text = DEFAULT_RECORDING_DEVICE;
            defaultItem.deviceId = "";
            cbMics.Items.Add(defaultItem);

            int selectedIndex = 0;
            int index = 1;

            foreach (CoreAudioDevice device in AudioController.GetCaptureDevices())
            {
                if (device.State == DeviceState.Active)
                {
                    ComboboxItem item = new ComboboxItem();
                    item.Text = device.FullName;
                    item.deviceId = device.Id.ToString();
                    cbMics.Items.Add(item);

                    if (item.deviceId == selectedDeviceId)
                    {
                        selectedIndex = index;
                    }
                    index++;
                }
            }

            cbMics.SelectedIndex = selectedIndex;
            isLoadingMics = false;
        }

        private void MyShow()
        {
            MyVisible = true;
            ShowInTaskbar = true;
            LoadMicsDropdown();
            UpdateSelectedDevice();
            CenterToScreen();
            
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            MyHide();

            // Apply DWM window attributes dynamically based on light/dark system theme
            ApplyWindowTheme();

            // Apply custom professional dark context menu renderer & remove margins
            iconContextMenu.Renderer = new FluentMenuRenderer();
            iconContextMenu.ShowImageMargin = false;
            iconContextMenu.ShowCheckMargin = false;

            // Setup modern container wrappers for Hotkey inputs inside the right column panel
            this.hotkeyTextBox.Parent = null;
            this.muteTextBox.Parent = null;
            this.unmuteTextBox.Parent = null;

            var hotkeyContainer = new ModernTextBoxContainer(hotkeyTextBox) { Location = new Point(15, 60), Size = new Size(145, 34) };
            var muteContainer = new ModernTextBoxContainer(muteTextBox) { Location = new Point(15, 145), Size = new Size(145, 34) };
            var unmuteContainer = new ModernTextBoxContainer(unmuteTextBox) { Location = new Point(15, 230), Size = new Size(145, 34) };

            panelHotkeys.Controls.Add(hotkeyContainer);
            panelHotkeys.Controls.Add(muteContainer);
            panelHotkeys.Controls.Add(unmuteContainer);

            // Load registry values
            selectedDeviceId = (string)registryKey.GetValue(registryDeviceId) ?? "";
            selectedDeviceName = (string)registryKey.GetValue(registryDeviceName) ?? DEFAULT_RECORDING_DEVICE;

            playSoundOnMute = Convert.ToInt32(registryKey.GetValue(registryPlayMute) ?? 0) == 1;
            playSoundOnUnmute = Convert.ToInt32(registryKey.GetValue(registryPlayUnmute) ?? 0) == 1;

            soundMutePath = (string)registryKey.GetValue(registrySoundMutePath);
            if (string.IsNullOrEmpty(soundMutePath) || !File.Exists(ResolveSoundPath(soundMutePath)) || soundMutePath == @"assets\sounds\muted.wav")
            {
                string mp3Path = @"assets\sounds\muted.mp3";
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mp3Path)))
                {
                    soundMutePath = mp3Path;
                }
                else
                {
                    soundMutePath = @"assets\sounds\muted.wav";
                }
            }
            soundUnmutePath = (string)registryKey.GetValue(registrySoundUnmutePath);
            if (string.IsNullOrEmpty(soundUnmutePath) || !File.Exists(ResolveSoundPath(soundUnmutePath)) || soundUnmutePath == @"assets\sounds\unmuted.wav")
            {
                string mp3Path = @"assets\sounds\unmuted.mp3";
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mp3Path)))
                {
                    soundUnmutePath = mp3Path;
                }
                else
                {
                    soundUnmutePath = @"assets\sounds\unmuted.wav";
                }
            }

            soundVolume = registryKey.GetValue("SoundVolume") != null ? Convert.ToInt32(registryKey.GetValue("SoundVolume")) : 100;
            currentLang = (string)registryKey.GetValue("Language") ?? "PT";

            lblVolumeValue.Text = soundVolume + "%";
            cbLanguage.SelectedIndex = (currentLang == "EN") ? 1 : 0;

            chkPlayMute.Checked = playSoundOnMute;
            chkPlayUnmute.Checked = playSoundOnUnmute;
            txtMutePath.Text = soundMutePath;
            txtUnmutePath.Text = soundUnmutePath;

            // Display short names for files
            lblMuteFile.Text = string.IsNullOrEmpty(soundMutePath) ? (currentLang == "EN" ? "No sound" : "Nenhum som") : Path.GetFileName(soundMutePath);
            lblUnmuteFile.Text = string.IsNullOrEmpty(soundUnmutePath) ? (currentLang == "EN" ? "No sound" : "Nenhum som") : Path.GetFileName(soundUnmutePath);

            // Wire custom ToggleSwitch events
            chkPlayMute.CheckedChanged += (s, ev) => playSoundOnMute = chkPlayMute.Checked;
            chkPlayUnmute.CheckedChanged += (s, ev) => playSoundOnUnmute = chkPlayUnmute.Checked;

            // Load custom state icons
            string iconOnPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"assets\icons\micon.ico");
            string iconOffPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"assets\icons\micmute.ico");
            string imgOnPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"assets\icons\micon.png");
            string imgOffPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"assets\icons\micmute.png");

            try
            {
                if (File.Exists(iconOnPath)) iconOn = new Icon(iconOnPath);
                else iconOn = CreateDynamicIcon("\uE720", FluentTheme.UnmuteGreen);

                if (File.Exists(iconOffPath)) iconOff = new Icon(iconOffPath);
                else iconOff = CreateDynamicIcon("\uE721", FluentTheme.MuteRed);

                iconError = CreateDynamicIcon("\uE7BA", FluentTheme.ErrorOrange);

                if (File.Exists(imgOnPath)) imgOn = Image.FromFile(imgOnPath);
                if (File.Exists(imgOffPath)) imgOff = Image.FromFile(imgOffPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading icons: " + ex.Message);
            }

            if (iconOn != null) this.Icon = iconOn;

            // Load auto-start state
            LoadStartupSettings();
            chkStartWithWindows.CheckedChanged += (s, ev) => SaveStartupSettings();

            LoadMicsDropdown();
            UpdateSelectedDevice();
            
            AudioController.AudioDeviceChanged.Subscribe(OnNextDevice);
            
            // Register Keyboard Hook
            globalHook.KeyDown += GlobalHook_KeyDown;
            globalHook.Hook();

            // Load saved hotkeys
            var hotkeyValue = registryKey.GetValue(registryKeyName);
            if (hotkeyValue != null)
            {
                var converter = new Shortcut.Forms.HotkeyConverter();
                hotkey = (Hotkey)converter.ConvertFromString(hotkeyValue.ToString());
            }

            hotkeyValue = registryKey.GetValue(registryKeyMute);
            if (hotkeyValue != null)
            {
                var converter = new Shortcut.Forms.HotkeyConverter();
                muteHotkey = (Hotkey)converter.ConvertFromString(hotkeyValue.ToString());
            }

            hotkeyValue = registryKey.GetValue(registryKeyUnmute);
            if (hotkeyValue != null)
            {
                var converter = new Shortcut.Forms.HotkeyConverter();
                unMuteHotkey = (Hotkey)converter.ConvertFromString(hotkeyValue.ToString());
            }

            ApplyLanguage(currentLang);
        }

        private void GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (MatchesHotkey(hotkey, e))
            {
                ToggleMicStatus();
                e.Handled = true;
            }
            else if (MatchesHotkey(muteHotkey, e))
            {
                MuteMicStatus();
                e.Handled = true;
            }
            else if (MatchesHotkey(unMuteHotkey, e))
            {
                UnMuteMicStatus();
                e.Handled = true;
            }
        }

        private bool MatchesHotkey(Hotkey hk, KeyEventArgs e)
        {
            if (hk == null) return false;

            Keys targetKey = hk.Key;
            Modifiers targetMod = hk.Modifier;

            Keys pressedKey = e.KeyCode;
            
            // Normalize modifier key values triggered on separate KeyDowns
            if (pressedKey == Keys.LControlKey || pressedKey == Keys.RControlKey) pressedKey = Keys.ControlKey;
            if (pressedKey == Keys.LMenu || pressedKey == Keys.RMenu || pressedKey == Keys.Alt) pressedKey = Keys.Menu;
            if (pressedKey == Keys.LShiftKey || pressedKey == Keys.RShiftKey) pressedKey = Keys.ShiftKey;

            Keys normalizedTargetKey = targetKey;
            if (normalizedTargetKey == Keys.LControlKey || normalizedTargetKey == Keys.RControlKey) normalizedTargetKey = Keys.ControlKey;
            if (normalizedTargetKey == Keys.LMenu || normalizedTargetKey == Keys.RMenu || normalizedTargetKey == Keys.Alt) normalizedTargetKey = Keys.Menu;
            if (normalizedTargetKey == Keys.LShiftKey || normalizedTargetKey == Keys.RShiftKey) normalizedTargetKey = Keys.ShiftKey;

            if (pressedKey != normalizedTargetKey) return false;

            bool shiftPressed = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool ctrlPressed = (e.Modifiers & Keys.Control) == Keys.Control;
            bool altPressed = (e.Modifiers & Keys.Alt) == Keys.Alt;

            bool shiftMatch = ((targetMod & Modifiers.Shift) == Modifiers.Shift) == shiftPressed;
            bool ctrlMatch = ((targetMod & Modifiers.Control) == Modifiers.Control) == ctrlPressed;
            bool altMatch = ((targetMod & Modifiers.Alt) == Modifiers.Alt) == altPressed;
            
            bool winPressed = (GetKeyState(0x5B) & 0x8000) != 0 || (GetKeyState(0x5C) & 0x8000) != 0;
            bool winMatch = ((targetMod & Modifiers.Win) == Modifiers.Win) == winPressed;

            return shiftMatch && ctrlMatch && altMatch && winMatch;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int keyCode);

        private void OnMuteChanged(DeviceMuteChangedArgs next)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateStatus(next.Device)));
            }
            else
            {
                UpdateStatus(next.Device);
            }
        }

        IDisposable muteChangedSubscription;
        public void UpdateDevice(IDevice device)
        {
            muteChangedSubscription?.Dispose();
            muteChangedSubscription = device?.MuteChanged.Subscribe(OnMuteChanged);
            UpdateStatus(device);
        }

        public IDevice getSelectedDevice()
        {
            try
            {
                return selectedDeviceId == "" ? AudioController.DefaultCaptureDevice : AudioController.GetDevice(new Guid(selectedDeviceId), DeviceState.Active);
            }
            catch
            {
                return AudioController.DefaultCaptureDevice;
            }
        }

        public void UpdateSelectedDevice()
        {
            UpdateDevice(getSelectedDevice());
        }

        public string ResolveSoundPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            if (Path.IsPathRooted(path))
            {
                if (File.Exists(path)) return path;
                string relativeName = Path.GetFileName(path);
                string localFallback = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "sounds", relativeName);
                if (File.Exists(localFallback)) return localFallback;
            }
            else
            {
                string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                if (File.Exists(localPath)) return localPath;
            }
            return path;
        }

        private void LoadStartupSettings()
        {
            try
            {
                string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(runKey, false))
                {
                    if (key != null)
                        chkStartWithWindows.Checked = key.GetValue("MicMute") != null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading startup registry: " + ex.Message);
            }
        }

        private void SaveStartupSettings()
        {
            try
            {
                string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(runKey, true))
                {
                    if (key != null)
                    {
                        if (chkStartWithWindows.Checked)
                            key.SetValue("MicMute", "\"" + Application.ExecutablePath + "\"");
                        else
                            key.DeleteValue("MicMute", false);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving startup registry: " + ex.Message);
            }
        }

        [System.Runtime.InteropServices.DllImport("winmm.dll")]
        private static extern long mciSendString(string command, System.Text.StringBuilder returnValue, int returnLength, IntPtr winhandle);

        private int GetWindowsVolume()
        {
            try
            {
                var controller = new AudioSwitcher.AudioApi.CoreAudio.CoreAudioController();
                var dev = controller.DefaultPlaybackDevice;
                if (dev != null)
                {
                    return (int)dev.Volume;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading Windows volume: " + ex.Message);
            }
            return 100;
        }

        public void PlaySoundFile(string filePath)
        {
            string resolvedPath = ResolveSoundPath(filePath);
            if (string.IsNullOrEmpty(resolvedPath) || !File.Exists(resolvedPath))
            {
                return;
            }

            try
            {
                mciSendString("close micMuteSound", null, 0, IntPtr.Zero);
                mciSendString(string.Format("open \"{0}\" type mpegvideo alias micMuteSound", resolvedPath), null, 0, IntPtr.Zero);
                
                // Scale user preference volume by current Windows master volume
                int winVol = GetWindowsVolume();
                int finalVol = (int)((soundVolume / 100.0) * winVol * 10);
                mciSendString(string.Format("setaudio micMuteSound volume to {0}", finalVol), null, 0, IntPtr.Zero);
                
                mciSendString("play micMuteSound", null, 0, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error playing sound: " + ex.Message);
            }
        }

        private Icon CreateDynamicIcon(string glyph, Color accentColor)
        {
            try
            {
                int size = 32;
                Bitmap bmp = new Bitmap(size, size);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                    g.Clear(Color.Transparent);

                    using (SolidBrush brush = new SolidBrush(accentColor))
                    {
                        g.FillEllipse(brush, 1, 1, size - 2, size - 2);
                    }

                    using (Font font = new Font("Segoe MDL2 Assets", 15F, FontStyle.Regular, GraphicsUnit.Pixel))
                    using (Brush textBrush = new SolidBrush(Color.White))
                    {
                        SizeF glyphSize = g.MeasureString(glyph, font);
                        float x = (size - glyphSize.Width) / 2f;
                        float y = (size - glyphSize.Height) / 2f;
                        g.DrawString(glyph, font, textBrush, x, y);
                    }
                }
                
                IntPtr hIcon = bmp.GetHicon();
                bmp.Dispose();
                return Icon.FromHandle(hIcon);
            }
            catch
            {
                return SystemIcons.Application;
            }
        }

        private void SetTrayIcon(Icon newIcon)
        {
            Icon oldIcon = currentTrayIcon;
            currentTrayIcon = newIcon;
            this.icon.Icon = newIcon;
            if (oldIcon != null && oldIcon != iconOn && oldIcon != iconOff && oldIcon != iconError)
            {
                DestroyIcon(oldIcon.Handle);
                oldIcon.Dispose();
            }
        }

        public void UpdateStatus(IDevice device)
        {
            MicStatus newStatus = (device != null) ? (device.IsMuted ? MicStatus.Off : MicStatus.On) : MicStatus.Error;
            bool playSound = currentStatus != MicStatus.Initial && currentStatus != newStatus;
            currentStatus = newStatus;

            string statusText = "";
            Color statusColor = Color.Gray;
            string statusGlyph = "";

            switch (currentStatus)
            {
                case MicStatus.On:
                    statusText = currentLang == "EN" ? "ACTIVE" : "ATIVO";
                    statusColor = FluentTheme.UnmuteGreen;
                    statusGlyph = "\uE720"; // Microphone
                    
                    SetTrayIcon(iconOn ?? CreateDynamicIcon(statusGlyph, statusColor));
                    if (iconOn != null) this.Icon = iconOn;
                    this.icon.Text = device.FullName.Substring(0, Math.Min(device.FullName.Length, 63));
                    
                    if (playSound && playSoundOnUnmute) PlaySoundFile(soundUnmutePath);
                    break;
                    
                case MicStatus.Off:
                    statusText = currentLang == "EN" ? "MUTED" : "MUTADO";
                    statusColor = FluentTheme.MuteRed;
                    statusGlyph = "\uE721"; // Microphone off
                    
                    SetTrayIcon(iconOff ?? CreateDynamicIcon(statusGlyph, statusColor));
                    if (iconOff != null) this.Icon = iconOff;
                    this.icon.Text = device.FullName.Substring(0, Math.Min(device.FullName.Length, 63));
                    
                    if (playSound && playSoundOnMute) PlaySoundFile(soundMutePath);
                    break;
                    
                case MicStatus.Error:
                    statusText = currentLang == "EN" ? "ERROR" : "ERRO";
                    statusColor = FluentTheme.ErrorOrange;
                    statusGlyph = "\uE7BA"; // Warning
                    
                    SetTrayIcon(iconError ?? CreateDynamicIcon(statusGlyph, statusColor));
                    if (iconError != null) this.Icon = iconError;
                    this.icon.Text = currentLang == "EN" ? "< No device >" : "< Nenhum dispositivo >";
                    
                    if (playSound) PlaySoundFile("error.wav");
                    break;
            }

            // Update main window controls
            lblStatusText.Text = statusText;
            lblStatusText.ForeColor = statusColor;
            lblDeviceName.Text = device != null ? device.FullName : (currentLang == "EN" ? "< No device detected >" : "< Nenhum dispositivo detectado >");
            
            // Customize visual status button to use the dark theme background and a modern border
            btnToggleMic.Image = currentStatus == MicStatus.On ? imgOn : (currentStatus == MicStatus.Off ? imgOff : null);
            btnToggleMic.Text = btnToggleMic.Image == null ? statusGlyph : "";
            btnToggleMic.ForeColor = statusColor;
            btnToggleMic.CustomBackColor = System.Drawing.Color.FromArgb(32, 32, 32);
            btnToggleMic.CustomHoverColor = System.Drawing.Color.FromArgb(45, 45, 45);
            btnToggleMic.CustomPressedColor = System.Drawing.Color.FromArgb(20, 20, 20);
            btnToggleMic.CustomBorderColor = FluentTheme.CardBorder;
        }

        public async void ToggleMicStatus()
        {
            var dev = getSelectedDevice();
            if (dev != null)
            {
                await dev.ToggleMuteAsync();
            }
        }

        public async void MuteMicStatus()
        {
            var dev = getSelectedDevice();
            if (dev != null)
            {
                await dev.SetMuteAsync(true);
            }
        }

        public async void UnMuteMicStatus()
        {
            var dev = getSelectedDevice();
            if (dev != null)
            {
                await dev.SetMuteAsync(false);
            }
        }

        private void Icon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ToggleMicStatus();
            }
            else if (e.Button == MouseButtons.Right)
            {
                SetForegroundWindow(this.Handle);
                iconContextMenu.Show(Control.MousePosition);
            }
        }

        private void HotkeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set hotkeys inside textboxes
            if (hotkey != null) hotkeyTextBox.Hotkey = hotkey;
            if (muteHotkey != null) muteTextBox.Hotkey = muteHotkey;
            if (unMuteHotkey != null) unmuteTextBox.Hotkey = unMuteHotkey;

            MyShow();
        }

        private void SaveSettings()
        {
            try
            {
                // Save Toggle Hotkey
                hotkey = hotkeyTextBox.Hotkey;
                if (hotkey == null)
                {
                    registryKey.DeleteValue(registryKeyName, false);
                }
                else
                {
                    registryKey.SetValue(registryKeyName, hotkey);
                }

                // Save Mute Hotkey
                muteHotkey = muteTextBox.Hotkey;
                if (muteHotkey == null)
                {
                    registryKey.DeleteValue(registryKeyMute, false);
                }
                else
                {
                    registryKey.SetValue(registryKeyMute, muteHotkey);
                }

                // Save Unmute Hotkey
                unMuteHotkey = unmuteTextBox.Hotkey;
                if (unMuteHotkey == null)
                {
                    registryKey.DeleteValue(registryKeyUnmute, false);
                }
                else
                {
                    registryKey.SetValue(registryKeyUnmute, unMuteHotkey);
                }

                // Save audio feedback settings
                playSoundOnMute = chkPlayMute.Checked;
                playSoundOnUnmute = chkPlayUnmute.Checked;
                soundMutePath = txtMutePath.Text;
                soundUnmutePath = txtUnmutePath.Text;

                registryKey.SetValue(registryPlayMute, playSoundOnMute ? 1 : 0);
                registryKey.SetValue(registryPlayUnmute, playSoundOnUnmute ? 1 : 0);
                registryKey.SetValue(registrySoundMutePath, soundMutePath);
                registryKey.SetValue(registrySoundUnmutePath, soundUnmutePath);
                registryKey.SetValue("SoundVolume", soundVolume, RegistryValueKind.DWord);
                registryKey.SetValue("Language", currentLang, RegistryValueKind.String);

                SaveStartupSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving settings: " + ex.Message);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isExiting && MyVisible)
            {
                MyHide();
                e.Cancel = true;
                SaveSettings();
            }
            else if (isExiting)
            {
                SaveSettings();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            globalHook.Unhook();
            base.OnFormClosed(e);
        }

        private void ButtonReset_Click(object sender, EventArgs e)
        {
            hotkeyTextBox.Hotkey = null;
            hotkeyTextBox.Text = "Nenhum";
        }
        
        private void muteReset_Click(object sender, EventArgs e)
        {
            muteTextBox.Hotkey = null;
            muteTextBox.Text = "Nenhum";
        }

        private void unmuteReset_Click(object sender, EventArgs e)
        {
            unmuteTextBox.Hotkey = null;
            unmuteTextBox.Text = "Nenhum";
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            isExiting = true;
            globalHook.Unhook();
            SetTrayIcon(null);
            Application.Exit();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            MyShow();
            cbMics.Focus();
        }

        private void CbMics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoadingMics) return;

            ComboboxItem selectedItem = (ComboboxItem)cbMics.SelectedItem;
            if (selectedItem != null)
            {
                registryKey.SetValue(registryDeviceId, selectedItem.deviceId);
                registryKey.SetValue(registryDeviceName, selectedItem.Text);
                selectedDeviceName = selectedItem.Text;
                selectedDeviceId = selectedItem.deviceId;
                UpdateSelectedDevice();
            }
        }

        private void BtnToggleMic_Click(object sender, EventArgs e)
        {
            ToggleMicStatus();
        }

        private void BtnBrowseMute_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Arquivos de áudio (*.mp3;*.wav)|*.mp3;*.wav|Todos os arquivos (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtMutePath.Text = openFileDialog.FileName;
                    soundMutePath = openFileDialog.FileName;
                    lblMuteFile.Text = Path.GetFileName(openFileDialog.FileName);
                }
            }
        }

        private void BtnBrowseUnmute_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Arquivos de áudio (*.mp3;*.wav)|*.mp3;*.wav|Todos os arquivos (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtUnmutePath.Text = openFileDialog.FileName;
                    soundUnmutePath = openFileDialog.FileName;
                    lblUnmuteFile.Text = Path.GetFileName(openFileDialog.FileName);
                }
            }
        }

        private void BtnAbout_Click(object sender, EventArgs e)
        {
            using (AboutForm about = new AboutForm(currentLang))
            {
                about.ShowDialog(this);
            }
        }

        private bool IsSystemDarkTheme()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
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
                if (!this.IsHandleCreated) return;

                int useDark = 1;
                // Try attribute 20 (Windows 10 20H1+/Windows 11), fallback to 19 (older Windows 10)
                if (DwmSetWindowAttribute(this.Handle, 20, ref useDark, sizeof(int)) != 0)
                {
                    DwmSetWindowAttribute(this.Handle, 19, ref useDark, sizeof(int));
                }

                int colorVal = 0x202020;
                DwmSetWindowAttribute(this.Handle, 34, ref colorVal, sizeof(int));
                DwmSetWindowAttribute(this.Handle, 35, ref colorVal, sizeof(int));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error applying theme: " + ex.Message);
            }
        }

        private void ApplyLanguage(string lang)
        {
            if (lang == "EN")
            {
                DEFAULT_RECORDING_DEVICE = "Default recording device";
                lblStartWithWindows.Text = "Start with Windows";
                btnAbout.Text = "About";
                lblFeedbackTitle.Text = "Sound Feedback";
                lblPlayMuteText.Text = "Sound on mute";
                lblPlayUnmuteText.Text = "Sound on unmute";
                lblHotkeysTitle.Text = "Keyboard Shortcuts";
                lblToggleLabel.Text = "Toggle Hotkey:";
                lblMuteLabel.Text = "Mute Hotkey:";
                lblUnmuteLabel.Text = "Unmute Hotkey:";
                buttonReset.Text = "Clear";
                muteReset.Text = "Clear";
                unmuteReset.Text = "Clear";
                hotkeyToolStripMenuItem.Text = "Settings";
                toolStripMenuItem1.Text = "Close";
            }
            else
            {
                DEFAULT_RECORDING_DEVICE = "Dispositivo de gravação padrão";
                lblStartWithWindows.Text = "Iniciar com Windows";
                btnAbout.Text = "Sobre";
                lblFeedbackTitle.Text = "Feedback Sonoro";
                lblPlayMuteText.Text = "Som ao mutar";
                lblPlayUnmuteText.Text = "Som ao desmutar";
                lblHotkeysTitle.Text = "Teclas de Atalho";
                lblToggleLabel.Text = "Alternar (Toggle):";
                lblMuteLabel.Text = "Mutar (Mute):";
                lblUnmuteLabel.Text = "Desmutar (Unmute):";
                buttonReset.Text = "Limpar";
                muteReset.Text = "Limpar";
                unmuteReset.Text = "Limpar";
                hotkeyToolStripMenuItem.Text = "Configurações";
                toolStripMenuItem1.Text = "Fechar";
            }

            // Refresh device dropdown with translated default device name
            LoadMicsDropdown();

            var dev = getSelectedDevice();
            UpdateStatus(dev);
        }

        private void CbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentLang = (cbLanguage.SelectedIndex == 1) ? "EN" : "PT";
            ApplyLanguage(currentLang);
            SaveSettings();
        }

        private void BtnVolDown_Click(object sender, EventArgs e)
        {
            soundVolume = Math.Max(0, soundVolume - 10);
            lblVolumeValue.Text = soundVolume + "%";
            SaveSettings();
        }

        private void BtnVolUp_Click(object sender, EventArgs e)
        {
            soundVolume = Math.Min(100, soundVolume + 10);
            lblVolumeValue.Text = soundVolume + "%";
            SaveSettings();
        }
    }

    public class ComboboxItem
    {
        public string Text { get; set; }
        public string deviceId { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }

    public class FluentMenuRenderer : ToolStripProfessionalRenderer
    {
        public FluentMenuRenderer() : base(new FluentColorTable()) { }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected)
            {
                Rectangle rect = new Rectangle(4, 2, e.Item.Width - 8, e.Item.Height - 4);
                using (GraphicsPath path = FluentTheme.GetRoundedPath(rect, 4))
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(70, 70, 70)))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillPath(brush, path);
                }
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = FluentTheme.TextPrimary;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(45, 45, 45)))
            {
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            // Do not paint default border
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            using (Pen pen = new Pen(FluentTheme.CardBorder, 1))
            {
                e.Graphics.DrawLine(pen, 10, e.Item.Height / 2, e.ToolStrip.Width - 10, e.Item.Height / 2);
            }
        }
    }

    public class FluentColorTable : ProfessionalColorTable
    {
        public override Color ToolStripDropDownBackground => Color.FromArgb(45, 45, 45);
        public override Color MenuBorder => Color.FromArgb(58, 58, 58);
        public override Color MenuItemSelected => Color.FromArgb(70, 70, 70);
        public override Color MenuItemBorder => Color.Transparent;
    }

    public class GlobalKeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public event KeyEventHandler KeyDown;

        public GlobalKeyboardHook()
        {
            _proc = HookCallback;
        }

        public void Hook()
        {
            _hookID = SetHook(_proc);
        }

        public void Unhook()
        {
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
            }
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (System.Diagnostics.Process curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (System.Diagnostics.ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = System.Runtime.InteropServices.Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;

                KeyEventArgs args = new KeyEventArgs(key | ModifierKeys);
                KeyDown?.Invoke(this, args);
                if (args.Handled)
                {
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }


        private Keys ModifierKeys
        {
            get
            {
                Keys modifiers = Keys.None;
                if ((GetKeyState(0x10) & 0x8000) != 0) modifiers |= Keys.Shift;      // VK_SHIFT
                if ((GetKeyState(0x11) & 0x8000) != 0) modifiers |= Keys.Control;    // VK_CONTROL
                if ((GetKeyState(0x12) & 0x8000) != 0) modifiers |= Keys.Alt;        // VK_MENU
                return modifiers;
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int keyCode);
    }
}
