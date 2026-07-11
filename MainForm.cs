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
using System.Runtime.Serialization.Json;
using System.Text;
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
        private bool isInitializing = true; // bloqueia saves prematuros durante o Load
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

        // P/Invoke — DWM: atributos de janela
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int attrLen);

        // DWMWA_WINDOW_CORNER_PREFERENCE (atributo 33, Windows 11+)
        private const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;

        // Valores de DWM_WINDOW_CORNER_PREFERENCE
        private const int DWMWCP_DEFAULT    = 0; // SO decide
        private const int DWMWCP_DONOTROUND = 1; // Sem arredondamento
        private const int DWMWCP_ROUND      = 2; // Arredondado (raio maior, padrão Win11)
        private const int DWMWCP_ROUNDSMALL = 3; // Arredondado pequeno

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

            // ── Menu de contexto da bandeja ─────────────────────────────────────────
            iconContextMenu.Renderer = new FluentMenuRenderer();
            iconContextMenu.ShowImageMargin = false;
            iconContextMenu.ShowCheckMargin = false;
            // Padding vertical do strip: 4px topo/base para não ficar "gordo"
            iconContextMenu.Padding = new Padding(0, 4, 0, 4);
            iconContextMenu.AutoSize = true;
            iconContextMenu.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            foreach (ToolStripItem item in iconContextMenu.Items)
            {
                // Horizontal: 16px para texto não encostar na borda.
                // Vertical: 5px — suficiente para clique confortável sem "gordura" extra.
                item.Padding = new Padding(16, 5, 16, 5);
                item.Margin  = new Padding(0);
            }

            // Arredondamento do menu ao abrir
            iconContextMenu.Opened += (s, ev) =>
            {
                // ── Nativo Windows 11: DWM arredonda a janela do próprio OS ──────────
                // DWMWCP_ROUND dá raio ~8px — visual consistente com menus do Shell.
                // Em Windows 10 essa chamada é ignorada silenciosamente.
                try
                {
                    int pref = DWMWCP_ROUND;
                    DwmSetWindowAttribute(iconContextMenu.Handle, DWMWA_WINDOW_CORNER_PREFERENCE,
                        ref pref, sizeof(int));
                }
                catch { }

                // ── Fallback Region para Windows 10 (e para clip do fundo desenhado) ─
                // Aplicamos DEPOIS do DWM para que no Win11 o clip do GDI+ fique
                // alinhado ao arredondamento nativo (raio 8). No Win10 é o único
                // mecanismo de arredondamento disponível.
                try
                {
                    // Usa as dimensões reais após AutoSize calcular o layout
                    var rect = new Rectangle(0, 0, iconContextMenu.Width, iconContextMenu.Height);
                    iconContextMenu.Region = new Region(FluentTheme.GetRoundedPath(rect, 8));
                }
                catch { }
            };

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

            // Register dynamic TextChanged translators
            hotkeyTextBox.TextChanged += (s, ev) => {
                string translated = TranslateHotkeyText(hotkeyTextBox.Text, currentLang);
                if (hotkeyTextBox.Text != translated) hotkeyTextBox.Text = translated;
            };
            muteTextBox.TextChanged += (s, ev) => {
                string translated = TranslateHotkeyText(muteTextBox.Text, currentLang);
                if (muteTextBox.Text != translated) muteTextBox.Text = translated;
            };
            unmuteTextBox.TextChanged += (s, ev) => {
                string translated = TranslateHotkeyText(unmuteTextBox.Text, currentLang);
                if (unmuteTextBox.Text != translated) unmuteTextBox.Text = translated;
            };

            // Salva configurações ao perder o foco de qualquer caixa de hotkey
            hotkeyTextBox.Leave += (s, ev) => { if (!isInitializing) SaveSettingsToFile(); };
            muteTextBox.Leave   += (s, ev) => { if (!isInitializing) SaveSettingsToFile(); };
            unmuteTextBox.Leave += (s, ev) => { if (!isInitializing) SaveSettingsToFile(); };

            // Detecta se é a primeira execução (sem JSON e sem chaves no registry)
            bool isFirstRun = !File.Exists(SettingsManager.SettingsFilePath) && (registryKey.ValueCount == 0);

            if (isFirstRun)
            {
                selectedDeviceId = "";
                selectedDeviceName = DEFAULT_RECORDING_DEVICE;
                playSoundOnMute = true;
                playSoundOnUnmute = true;

                string muteMp3 = @"assets\sounds\muted.mp3";
                soundMutePath = File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, muteMp3)) ? muteMp3 : @"assets\sounds\muted.wav";

                string unmuteMp3 = @"assets\sounds\unmuted.mp3";
                soundUnmutePath = File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, unmuteMp3)) ? unmuteMp3 : @"assets\sounds\unmuted.wav";

                soundVolume = 100;
                currentLang = "PT";
            }
            else
            {
                selectedDeviceId = (string)registryKey.GetValue(registryDeviceId) ?? "";
                selectedDeviceName = (string)registryKey.GetValue(registryDeviceName) ?? DEFAULT_RECORDING_DEVICE;

                playSoundOnMute = Convert.ToInt32(registryKey.GetValue(registryPlayMute) ?? 1) == 1;
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
            }

            trackBarVolume.Value = soundVolume;
            lblVolumeValue.Text = soundVolume + "%";
            cbLanguage.SelectedIndex = (currentLang == "EN") ? 1 : 0;

            chkPlayMute.Checked = playSoundOnMute;
            chkPlayUnmute.Checked = playSoundOnUnmute;
            txtMutePath.Text = soundMutePath;
            txtUnmutePath.Text = soundUnmutePath;

            // Display short names for files
            lblMuteFile.Text = string.IsNullOrEmpty(soundMutePath) ? (currentLang == "EN" ? "No sound" : "Nenhum som") : Path.GetFileName(soundMutePath);
            lblUnmuteFile.Text = string.IsNullOrEmpty(soundUnmutePath) ? (currentLang == "EN" ? "No sound" : "Nenhum som") : Path.GetFileName(soundUnmutePath);

            // Wire custom ToggleSwitch events — atualiza variável e salva imediatamente
            chkPlayMute.CheckedChanged += (s, ev) => { if (isInitializing) return; playSoundOnMute = chkPlayMute.Checked; SaveSettingsToFile(); };
            chkPlayUnmute.CheckedChanged += (s, ev) => { if (isInitializing) return; playSoundOnUnmute = chkPlayUnmute.Checked; SaveSettingsToFile(); };

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
            chkStartWithWindows.CheckedChanged += (s, ev) => { if (isInitializing) return; SaveStartupSettings(); SaveSettingsToFile(); };

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

            // Carrega configurações do arquivo JSON (sobrepõe Registry se arquivo existir)
            var savedSettings = LoadSettingsFromFile();
            if (savedSettings != null)
            {
                ApplySettingsToUI(savedSettings);
            }

            // A partir daqui os eventos de UI podem disparar saves normalmente
            isInitializing = false;

            if (isFirstRun && savedSettings == null)
            {
                SaveSettingsToFile(); // persiste os padrões na primeira execução
            }
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
                    {
                        string savedPath = key.GetValue("MicMute") as string;
                        // Valida se o executável registrado ainda existe no disco
                        bool isValid = !string.IsNullOrEmpty(savedPath) &&
                                       File.Exists(savedPath.Trim('"'));
                        chkStartWithWindows.Checked = isValid;

                        // Remove entrada inválida para não confundir o usuário
                        if (!isValid && savedPath != null)
                        {
                            using (RegistryKey writable = Registry.CurrentUser.OpenSubKey(runKey, true))
                                writable?.DeleteValue("MicMute", false);
                        }
                    }
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

        // ─── Persistência JSON ────────────────────────────────────────────────────

        /// <summary>
        /// Captura o estado atual de todos os controles de configuração e retorna
        /// um objeto MicMuteSettings pronto para ser serializado.
        /// </summary>
        private MicMuteSettings CaptureSettingsFromUI()
        {
            // Sincroniza campos internos antes de capturar
            hotkey      = hotkeyTextBox.Hotkey;
            muteHotkey  = muteTextBox.Hotkey;
            unMuteHotkey = unmuteTextBox.Hotkey;
            playSoundOnMute   = chkPlayMute.Checked;
            playSoundOnUnmute = chkPlayUnmute.Checked;
            soundMutePath   = txtMutePath.Text;
            soundUnmutePath = txtUnmutePath.Text;
            soundVolume     = trackBarVolume.Value;

            return new MicMuteSettings
            {
                DeviceId        = selectedDeviceId ?? "",
                DeviceName      = selectedDeviceName ?? "",
                Hotkey          = hotkey?.ToString(),
                HotkeyMute      = muteHotkey?.ToString(),
                HotkeyUnmute    = unMuteHotkey?.ToString(),
                PlayMute        = playSoundOnMute,
                PlayUnmute      = playSoundOnUnmute,
                SoundMutePath   = soundMutePath ?? "",
                SoundUnmutePath = soundUnmutePath ?? "",
                SoundVolume     = soundVolume,
                Language        = currentLang ?? "PT",
                StartWithWindows = chkStartWithWindows.Checked
            };
        }

        /// <summary>
        /// Aplica um objeto MicMuteSettings nos controles da UI e nas variáveis internas.
        /// Chamado no Load, após ler o arquivo JSON (para sobrepor o Registry).
        /// </summary>
        private void ApplySettingsToUI(MicMuteSettings s)
        {
            if (s == null) return;
            try
            {
                // Dispositivo
                if (s.DeviceId != null) selectedDeviceId = s.DeviceId;
                if (s.DeviceName != null) selectedDeviceName = s.DeviceName;

                // Hotkeys
                var converter = new Shortcut.Forms.HotkeyConverter();
                if (!string.IsNullOrEmpty(s.Hotkey))
                {
                    hotkey = (Hotkey)converter.ConvertFromString(s.Hotkey);
                    hotkeyTextBox.Hotkey = hotkey;
                }
                if (!string.IsNullOrEmpty(s.HotkeyMute))
                {
                    muteHotkey = (Hotkey)converter.ConvertFromString(s.HotkeyMute);
                    muteTextBox.Hotkey = muteHotkey;
                }
                if (!string.IsNullOrEmpty(s.HotkeyUnmute))
                {
                    unMuteHotkey = (Hotkey)converter.ConvertFromString(s.HotkeyUnmute);
                    unmuteTextBox.Hotkey = unMuteHotkey;
                }

                // Feedback sonoro
                playSoundOnMute   = s.PlayMute;
                playSoundOnUnmute = s.PlayUnmute;
                soundMutePath   = s.SoundMutePath ?? "";
                soundUnmutePath = s.SoundUnmutePath ?? "";

                chkPlayMute.Checked   = playSoundOnMute;
                chkPlayUnmute.Checked = playSoundOnUnmute;
                txtMutePath.Text   = soundMutePath;
                txtUnmutePath.Text = soundUnmutePath;

                lblMuteFile.Text   = string.IsNullOrEmpty(soundMutePath)
                    ? (currentLang == "EN" ? "No sound" : "Nenhum som")
                    : Path.GetFileName(soundMutePath);
                lblUnmuteFile.Text = string.IsNullOrEmpty(soundUnmutePath)
                    ? (currentLang == "EN" ? "No sound" : "Nenhum som")
                    : Path.GetFileName(soundUnmutePath);

                // Volume
                soundVolume = s.SoundVolume;
                trackBarVolume.Value = soundVolume;
                lblVolumeValue.Text = soundVolume + "%";

                // Idioma (só altera o índice do combo; ApplyLanguage já foi chamada no Load;
                // aqui apenas sincronizamos a variável e o combo sem disparar o evento)
                if (!string.IsNullOrEmpty(s.Language) && s.Language != currentLang)
                {
                    currentLang = s.Language;
                    int langIdx = (currentLang == "EN") ? 1 : 0;
                    if (cbLanguage.SelectedIndex != langIdx)
                    {
                        cbLanguage.SelectedIndexChanged -= CbLanguage_SelectedIndexChanged;
                        cbLanguage.SelectedIndex = langIdx;
                        cbLanguage.SelectedIndexChanged += CbLanguage_SelectedIndexChanged;
                    }
                    ApplyLanguage(currentLang);
                }

                // Iniciar com Windows — isInitializing=true garante que o evento não dispara saves durante Apply
                bool startWithWin = s.StartWithWindows;
                // Lê a realidade do Registry para decidir se precisa mudar
                string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
                bool currentlyInRun = false;
                using (var rk = Registry.CurrentUser.OpenSubKey(runKey, false))
                    currentlyInRun = rk?.GetValue("MicMute") != null;

                // Aplica apenas se diferente para não disparar saves desnecessários
                if (chkStartWithWindows.Checked != startWithWin)
                    chkStartWithWindows.Checked = startWithWin;

                // Reload do dropdown para refletir o dispositivo salvo
                LoadMicsDropdown();
                UpdateSelectedDevice();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ApplySettingsToUI] Erro: " + ex.Message);
            }
        }

        /// <summary>
        /// Carrega as configurações do arquivo JSON via SettingsManager.
        /// Retorna null se o arquivo não existir ou falhar.
        /// </summary>
        private MicMuteSettings LoadSettingsFromFile()
        {
            return SettingsManager.Load();
        }

        /// <summary>
        /// Captura a UI, salva no arquivo JSON e sincroniza as chaves relevantes no Registry.
        /// Este é o único ponto de escrita; substitui a lógica dispersa anterior.
        /// </summary>
        private void SaveSettingsToFile()
        {
            try
            {
                MicMuteSettings s = CaptureSettingsFromUI();

                // 1) Grava o arquivo JSON
                SettingsManager.Save(s);

                // 2) Mantém o Registry sincronizado (compatibilidade com código externo)
                if (s.Hotkey != null)
                    registryKey.SetValue(registryKeyName, s.Hotkey);
                else
                    registryKey.DeleteValue(registryKeyName, false);

                if (s.HotkeyMute != null)
                    registryKey.SetValue(registryKeyMute, s.HotkeyMute);
                else
                    registryKey.DeleteValue(registryKeyMute, false);

                if (s.HotkeyUnmute != null)
                    registryKey.SetValue(registryKeyUnmute, s.HotkeyUnmute);
                else
                    registryKey.DeleteValue(registryKeyUnmute, false);

                registryKey.SetValue(registryDeviceId,         s.DeviceId);
                registryKey.SetValue(registryDeviceName,       s.DeviceName);
                registryKey.SetValue(registryPlayMute,         s.PlayMute ? 1 : 0);
                registryKey.SetValue(registryPlayUnmute,       s.PlayUnmute ? 1 : 0);
                registryKey.SetValue(registrySoundMutePath,    s.SoundMutePath);
                registryKey.SetValue(registrySoundUnmutePath,  s.SoundUnmutePath);
                registryKey.SetValue("SoundVolume",            s.SoundVolume, RegistryValueKind.DWord);
                registryKey.SetValue("Language",               s.Language,    RegistryValueKind.String);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[SaveSettingsToFile] Erro: " + ex.Message);
            }
        }

        /// <summary>
        /// Salva todas as configurações (JSON + Registry). Mantido para compatibilidade
        /// com chamadas existentes (FormClosing, BtnVolDown/Up, CbLanguage).
        /// </summary>
        private void SaveSettings()
        {
            SaveSettingsToFile();
            SaveStartupSettings();
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
            hotkeyTextBox.Text = currentLang == "EN" ? "None" : "Nenhum";
        }
        
        private void muteReset_Click(object sender, EventArgs e)
        {
            muteTextBox.Hotkey = null;
            muteTextBox.Text = currentLang == "EN" ? "None" : "Nenhum";
        }

        private void unmuteReset_Click(object sender, EventArgs e)
        {
            unmuteTextBox.Hotkey = null;
            unmuteTextBox.Text = currentLang == "EN" ? "None" : "Nenhum";
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
                selectedDeviceName = selectedItem.Text;
                selectedDeviceId   = selectedItem.deviceId;
                UpdateSelectedDevice();
                SaveSettingsToFile(); // persiste imediatamente
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
                string soundsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "sounds");
                if (Directory.Exists(soundsDir))
                    openFileDialog.InitialDirectory = soundsDir;

                string filterLabel = currentLang == "EN" ? "Audio files" : "Arquivos de áudio";
                string allLabel = currentLang == "EN" ? "All files" : "Todos os arquivos";
                openFileDialog.Filter = $"{filterLabel} (*.mp3;*.wav)|*.mp3;*.wav|{allLabel} (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtMutePath.Text = openFileDialog.FileName;
                    soundMutePath    = openFileDialog.FileName;
                    lblMuteFile.Text = Path.GetFileName(openFileDialog.FileName);
                    SaveSettingsToFile(); // persiste imediatamente
                }
            }
        }

        private void BtnBrowseUnmute_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                string soundsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "sounds");
                if (Directory.Exists(soundsDir))
                    openFileDialog.InitialDirectory = soundsDir;

                string filterLabel = currentLang == "EN" ? "Audio files" : "Arquivos de áudio";
                string allLabel = currentLang == "EN" ? "All files" : "Todos os arquivos";
                openFileDialog.Filter = $"{filterLabel} (*.mp3;*.wav)|*.mp3;*.wav|{allLabel} (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtUnmutePath.Text = openFileDialog.FileName;
                    soundUnmutePath    = openFileDialog.FileName;
                    lblUnmuteFile.Text = Path.GetFileName(openFileDialog.FileName);
                    SaveSettingsToFile(); // persiste imediatamente
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

            // Translate the textbox text values
            hotkeyTextBox.Text = TranslateHotkeyText(hotkeyTextBox.Text, lang);
            muteTextBox.Text = TranslateHotkeyText(muteTextBox.Text, lang);
            unmuteTextBox.Text = TranslateHotkeyText(unmuteTextBox.Text, lang);

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

        private void TrackBarVolume_Scroll(object sender, EventArgs e)
        {
            soundVolume = trackBarVolume.Value;
            lblVolumeValue.Text = soundVolume + "%";
            SaveSettingsToFile(); // Salva a alteração imediatamente
        }

        private string TranslateHotkeyText(string text, string lang)
        {
            if (string.IsNullOrEmpty(text)) return lang == "EN" ? "None" : "Nenhum";
            if (text == "Nenhum" && lang == "EN") return "None";
            if (text == "None" && lang == "PT") return "Nenhum";

            if (lang == "EN")
            {
                string result = text
                    .Replace("Nenhum", "None")
                    .Replace("Control", "Ctrl")
                    .Replace("Retorno", "Enter")
                    .Replace("Retroceder", "Backspace")
                    .Replace("Espaço", "Space")
                    .Replace("Esquerda", "Left")
                    .Replace("Direita", "Right")
                    .Replace("Acima", "Up")
                    .Replace("Abaixo", "Down")
                    .Replace("Menu", "Menu")
                    .Replace("Acento til", "Tilde")
                    .Replace("Acento circunflexo", "Circumflex")
                    .Replace("Acento agudo", "Acute")
                    .Replace("Vírgula", "Comma")
                    .Replace("Ponto", "Period")
                    .Replace("Barra", "Slash")
                    .Replace("Barra invertida", "Backslash")
                    .Replace("Mais", "Plus")
                    .Replace("Menos", "Minus")
                    .Replace("Igual", "Equal")
                    .Replace("Multiplicar", "Multiply")
                    .Replace("Dividir", "Divide")
                    .Replace("Somar", "Add")
                    .Replace("Subtrair", "Subtract")
                    .Replace("Decimal", "Decimal");
                return result;
            }
            else
            {
                string result = text
                    .Replace("None", "Nenhum")
                    .Replace("Ctrl", "Control")
                    .Replace("Enter", "Retorno")
                    .Replace("Backspace", "Retroceder")
                    .Replace("Space", "Espaço")
                    .Replace("Left", "Esquerda")
                    .Replace("Right", "Direita")
                    .Replace("Up", "Acima")
                    .Replace("Down", "Abaixo")
                    .Replace("Tilde", "Acento til")
                    .Replace("Circumflex", "Acento circunflexo")
                    .Replace("Acute", "Acento agudo")
                    .Replace("Comma", "Vírgula")
                    .Replace("Period", "Ponto")
                    .Replace("Slash", "Barra")
                    .Replace("Backslash", "Barra invertida")
                    .Replace("Plus", "Mais")
                    .Replace("Minus", "Menos")
                    .Replace("Equal", "Igual")
                    .Replace("Multiply", "Multiplicar")
                    .Replace("Divide", "Dividir")
                    .Replace("Add", "Somar")
                    .Replace("Subtract", "Subtrair");
                return result;
            }
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
        // Raio dos cantos — deve ser idêntico ao usado na Region e na borda
        private const int MenuRadius   = 8;
        // Raio do highlight por item — menor que o do menu
        private const int ItemRadius   = 4;
        // Cor de fundo do menu
        private static readonly Color BgColor       = Color.FromArgb(45, 45, 45);
        // Cor de highlight ao passar o mouse
        private static readonly Color HoverColor     = Color.FromArgb(68, 68, 78);

        public FluentMenuRenderer() : base(new FluentColorTable()) { }

        // ── Fundo geral do strip ─────────────────────────────────────────────────
        // Usa FillPath em vez de FillRectangle para que o preenchimento
        // respeite a Region arredondada, evitando pixels "quadrados" nos cantos.
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, e.ToolStrip.Width, e.ToolStrip.Height);
            using (GraphicsPath path = FluentTheme.GetRoundedPath(rect, MenuRadius))
            using (SolidBrush brush = new SolidBrush(BgColor))
            {
                e.Graphics.FillPath(brush, path);
            }
        }

        // ── Highlight do item selecionado ────────────────────────────────────────
        // Inset horizontal fixo (4px) + inset vertical proporcional (2px),
        // garantindo que o pill não toque nas bordas do menu.
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (!e.Item.Selected) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // inset: 4px laterais, 2px topo/base — visual "pill" compacto
            const int hInset = 4;
            const int vInset = 2;
            Rectangle rect = new Rectangle(
                hInset,
                vInset,
                e.Item.Width  - hInset * 2,
                e.Item.Height - vInset * 2);

            using (GraphicsPath path = FluentTheme.GetRoundedPath(rect, ItemRadius))
            using (SolidBrush brush = new SolidBrush(HoverColor))
            {
                e.Graphics.FillPath(brush, path);
            }
        }

        // ── Texto dos itens ──────────────────────────────────────────────────────
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = FluentTheme.TextPrimary;
            base.OnRenderItemText(e);
        }

        // ── Margem de imagem (desativada, mas precisa ser pintada para não vazar) ─
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            // ShowImageMargin = false, mas o renderer ainda chama este método;
            // pintar com a mesma cor de fundo evita artefatos.
            using (SolidBrush brush = new SolidBrush(BgColor))
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
        }

        // ── Borda externa do menu ────────────────────────────────────────────────
        // Raio idêntico ao da Region (8px) para que a borda desenhada pelo GDI+
        // fique alinhada ao clip, sem "borda quadrada" visível além do arredondamento.
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            // Recuo de 0.5px para a borda cair dentro do clip da Region
            Rectangle rect = new Rectangle(0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
            using (GraphicsPath path = FluentTheme.GetRoundedPath(rect, MenuRadius))
            using (Pen pen = new Pen(FluentTheme.CardBorder, 1f))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        // ── Separador ────────────────────────────────────────────────────────────
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            int midY = e.Item.Height / 2;
            using (Pen pen = new Pen(FluentTheme.CardBorder, 1f))
            {
                // Recuo horizontal de 8px para o separador não tocar as bordas arredondadas
                e.Graphics.DrawLine(pen, 8, midY, e.ToolStrip.Width - 8, midY);
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
