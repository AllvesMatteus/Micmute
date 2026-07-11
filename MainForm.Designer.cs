namespace MicMute
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            
            this.icon = new System.Windows.Forms.NotifyIcon(this.components);
            this.iconContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.hotkeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            
            // Cards
            this.panelStatus = new MicMute.ModernPanel();
            this.panelFeedback = new MicMute.ModernPanel();
            this.panelHotkeys = new MicMute.ModernPanel();
            this.panelGeneral = new MicMute.ModernPanel();
            
            // Card 1 (Status & Device) controls
            this.btnToggleMic = new MicMute.ModernButton();
            this.lblStatusText = new System.Windows.Forms.Label();
            this.lblDeviceName = new System.Windows.Forms.Label();
            this.cbMics = new MicMute.ModernComboBox();
            
            // Card 2 (Feedback) controls
            this.lblFeedbackTitle = new System.Windows.Forms.Label();
            this.chkPlayMute = new MicMute.ModernToggleSwitch();
            this.lblPlayMuteText = new System.Windows.Forms.Label();
            this.btnBrowseMute = new MicMute.ModernButton();
            this.lblMuteFile = new System.Windows.Forms.Label();
            
            this.chkPlayUnmute = new MicMute.ModernToggleSwitch();
            this.lblPlayUnmuteText = new System.Windows.Forms.Label();
            this.btnBrowseUnmute = new MicMute.ModernButton();
            this.lblUnmuteFile = new System.Windows.Forms.Label();
            
            // Card 3 (Hotkeys) controls
            this.lblHotkeysTitle = new System.Windows.Forms.Label();
            
            this.lblToggleLabel = new System.Windows.Forms.Label();
            this.hotkeyTextBox = new Shortcut.Forms.HotkeyTextBox();
            this.buttonReset = new MicMute.ModernButton();
            
            this.lblMuteLabel = new System.Windows.Forms.Label();
            this.muteTextBox = new Shortcut.Forms.HotkeyTextBox();
            this.muteReset = new MicMute.ModernButton();
            
            this.lblUnmuteLabel = new System.Windows.Forms.Label();
            this.unmuteTextBox = new Shortcut.Forms.HotkeyTextBox();
            this.unmuteReset = new MicMute.ModernButton();
            
            // Hidden textboxes for path values to preserve original logic bindings
            this.txtMutePath = new System.Windows.Forms.TextBox();
            this.txtUnmutePath = new System.Windows.Forms.TextBox();
            
            // General Settings controls
            this.lblStartWithWindows = new System.Windows.Forms.Label();
            this.chkStartWithWindows = new MicMute.ModernToggleSwitch();
            this.btnAbout = new MicMute.ModernButton();
            this.cbLanguage = new MicMute.ModernComboBox();
            
            // Volume controls
            this.lblVolumeIcon = new System.Windows.Forms.Label();
            this.lblVolumeValue = new System.Windows.Forms.Label();
            this.trackBarVolume = new System.Windows.Forms.TrackBar();
            
            this.iconContextMenu.SuspendLayout();
            this.panelStatus.SuspendLayout();
            this.panelFeedback.SuspendLayout();
            this.panelHotkeys.SuspendLayout();
            this.panelGeneral.SuspendLayout();
            this.SuspendLayout();
            
            // 
            // icon
            // 
            this.icon.ContextMenuStrip = null;
            this.icon.Text = "<Inicializando>";
            this.icon.Visible = true;
            this.icon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Icon_MouseClick);
            // 
            // iconContextMenu
            // 
            this.iconContextMenu.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.iconContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hotkeyToolStripMenuItem,
            this.toolStripMenuItem1});
            this.iconContextMenu.Name = "iconContextMenu";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(219, 40);
            this.toolStripMenuItem2.Text = "Selecionar microfone";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // hotkeyToolStripMenuItem
            // 
            this.hotkeyToolStripMenuItem.Name = "hotkeyToolStripMenuItem";
            this.hotkeyToolStripMenuItem.Size = new System.Drawing.Size(219, 40);
            this.hotkeyToolStripMenuItem.Text = "Configurações";
            this.hotkeyToolStripMenuItem.Click += new System.EventHandler(this.HotkeyToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(219, 40);
            this.toolStripMenuItem1.Text = "Fechar";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.ExitMenuItem_Click);
            // 
            // panelStatus
            // 
            this.panelStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.panelStatus.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.panelStatus.BorderWidth = 1;
            this.panelStatus.Controls.Add(this.btnToggleMic);
            this.panelStatus.Controls.Add(this.lblStatusText);
            this.panelStatus.Controls.Add(this.lblDeviceName);
            this.panelStatus.Controls.Add(this.cbMics);
            this.panelStatus.Location = new System.Drawing.Point(15, 15);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Radius = 8;
            this.panelStatus.Size = new System.Drawing.Size(240, 135);
            this.panelStatus.TabIndex = 0;
            // 
            // btnToggleMic
            // 
            this.btnToggleMic.BackColor = System.Drawing.Color.Transparent;
            this.btnToggleMic.CustomBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(212)))));
            this.btnToggleMic.CustomBorderColor = System.Drawing.Color.Transparent;
            this.btnToggleMic.CustomHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(142)))), ((int)(((byte)(230)))));
            this.btnToggleMic.CustomPressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(180)))));
            this.btnToggleMic.Font = new System.Drawing.Font("Segoe MDL2 Assets", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnToggleMic.ForeColor = System.Drawing.Color.White;
            this.btnToggleMic.IconGlyph = "";
            this.btnToggleMic.IconSize = 11F;
            this.btnToggleMic.Location = new System.Drawing.Point(15, 15);
            this.btnToggleMic.Name = "btnToggleMic";
            this.btnToggleMic.Radius = 24;
            this.btnToggleMic.Size = new System.Drawing.Size(48, 48);
            this.btnToggleMic.TabIndex = 0;
            this.btnToggleMic.Text = "\uE720";
            this.btnToggleMic.Click += new System.EventHandler(this.BtnToggleMic_Click);
            // 
            // lblStatusText
            // 
            this.lblStatusText.AutoSize = true;
            this.lblStatusText.BackColor = System.Drawing.Color.Transparent;
            this.lblStatusText.Font = new System.Drawing.Font("Segoe UI", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusText.ForeColor = System.Drawing.Color.White;
            this.lblStatusText.Location = new System.Drawing.Point(75, 16);
            this.lblStatusText.Name = "lblStatusText";
            this.lblStatusText.Size = new System.Drawing.Size(130, 25);
            this.lblStatusText.TabIndex = 1;
            this.lblStatusText.Text = "VERIFICANDO";
            // 
            // lblDeviceName
            // 
            this.lblDeviceName.BackColor = System.Drawing.Color.Transparent;
            this.lblDeviceName.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDeviceName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblDeviceName.Location = new System.Drawing.Point(75, 40);
            this.lblDeviceName.Name = "lblDeviceName";
            this.lblDeviceName.Size = new System.Drawing.Size(150, 18);
            this.lblDeviceName.TabIndex = 2;
            this.lblDeviceName.Text = "Buscando dispositivo...";
            this.lblDeviceName.AutoEllipsis = true;
            this.lblDeviceName.AutoSize = false;
            // 
            // cbMics
            // 
            this.cbMics.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.cbMics.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbMics.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMics.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbMics.ForeColor = System.Drawing.Color.White;
            this.cbMics.FormattingEnabled = true;
            this.cbMics.Location = new System.Drawing.Point(15, 80);
            this.cbMics.Name = "cbMics";
            this.cbMics.Size = new System.Drawing.Size(210, 24);
            this.cbMics.TabIndex = 3;
            this.cbMics.SelectedIndexChanged += new System.EventHandler(this.CbMics_SelectedIndexChanged);
            // 
            // panelFeedback
            // 
            this.panelFeedback.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.panelFeedback.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.panelFeedback.BorderWidth = 1;
            this.panelFeedback.Controls.Add(this.lblFeedbackTitle);
            this.panelFeedback.Controls.Add(this.chkPlayMute);
            this.panelFeedback.Controls.Add(this.lblPlayMuteText);
            this.panelFeedback.Controls.Add(this.btnBrowseMute);
            this.panelFeedback.Controls.Add(this.lblMuteFile);
            this.panelFeedback.Controls.Add(this.chkPlayUnmute);
            this.panelFeedback.Controls.Add(this.lblPlayUnmuteText);
            this.panelFeedback.Controls.Add(this.btnBrowseUnmute);
            this.panelFeedback.Controls.Add(this.lblUnmuteFile);
            this.panelFeedback.Controls.Add(this.lblVolumeIcon);
            this.panelFeedback.Controls.Add(this.lblVolumeValue);
            this.panelFeedback.Controls.Add(this.trackBarVolume);
            this.panelFeedback.Location = new System.Drawing.Point(15, 165);
            this.panelFeedback.Name = "panelFeedback";
            this.panelFeedback.Radius = 8;
            this.panelFeedback.Size = new System.Drawing.Size(240, 200);
            this.panelFeedback.TabIndex = 1;
            // 
            // lblFeedbackTitle
            // 
            this.lblFeedbackTitle.AutoSize = true;
            this.lblFeedbackTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblFeedbackTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFeedbackTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblFeedbackTitle.Location = new System.Drawing.Point(15, 10);
            this.lblFeedbackTitle.Name = "lblFeedbackTitle";
            this.lblFeedbackTitle.Size = new System.Drawing.Size(99, 15);
            this.lblFeedbackTitle.TabIndex = 0;
            this.lblFeedbackTitle.Text = "Feedback Sonoro";
            // 
            // chkPlayMute
            // 
            this.chkPlayMute.Checked = false;
            this.chkPlayMute.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkPlayMute.Location = new System.Drawing.Point(15, 35);
            this.chkPlayMute.Name = "chkPlayMute";
            this.chkPlayMute.Size = new System.Drawing.Size(50, 26);
            this.chkPlayMute.TabIndex = 1;
            // 
            // lblPlayMuteText
            // 
            this.lblPlayMuteText.AutoSize = true;
            this.lblPlayMuteText.BackColor = System.Drawing.Color.Transparent;
            this.lblPlayMuteText.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlayMuteText.ForeColor = System.Drawing.Color.White;
            this.lblPlayMuteText.Location = new System.Drawing.Point(70, 38);
            this.lblPlayMuteText.Name = "lblPlayMuteText";
            this.lblPlayMuteText.Size = new System.Drawing.Size(117, 15);
            this.lblPlayMuteText.TabIndex = 2;
            this.lblPlayMuteText.Text = "Som ao mutar";
            // 
            // btnBrowseMute
            // 
            this.btnBrowseMute.BackColor = System.Drawing.Color.Transparent;
            this.btnBrowseMute.CustomBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnBrowseMute.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnBrowseMute.CustomHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.btnBrowseMute.CustomPressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnBrowseMute.Font = new System.Drawing.Font("Segoe MDL2 Assets", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowseMute.ForeColor = System.Drawing.Color.White;
            this.btnBrowseMute.IconGlyph = "";
            this.btnBrowseMute.IconSize = 10F;
            this.btnBrowseMute.Location = new System.Drawing.Point(195, 34);
            this.btnBrowseMute.Name = "btnBrowseMute";
            this.btnBrowseMute.Radius = 4;
            this.btnBrowseMute.Size = new System.Drawing.Size(30, 26);
            this.btnBrowseMute.TabIndex = 3;
            this.btnBrowseMute.Text = "\uED25"; // Folder Open icon
            this.btnBrowseMute.Click += new System.EventHandler(this.BtnBrowseMute_Click);
            // 
            // lblMuteFile
            // 
            this.lblMuteFile.BackColor = System.Drawing.Color.Transparent;
            this.lblMuteFile.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMuteFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblMuteFile.Location = new System.Drawing.Point(70, 60);
            this.lblMuteFile.Name = "lblMuteFile";
            this.lblMuteFile.Size = new System.Drawing.Size(120, 16);
            this.lblMuteFile.TabIndex = 4;
            this.lblMuteFile.Text = "Nenhum som";
            this.lblMuteFile.AutoEllipsis = true;
            this.lblMuteFile.AutoSize = false;
            // 
            // chkPlayUnmute
            // 
            this.chkPlayUnmute.Checked = false;
            this.chkPlayUnmute.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkPlayUnmute.Location = new System.Drawing.Point(15, 90);
            this.chkPlayUnmute.Name = "chkPlayUnmute";
            this.chkPlayUnmute.Size = new System.Drawing.Size(50, 26);
            this.chkPlayUnmute.TabIndex = 5;
            // 
            // lblPlayUnmuteText
            // 
            this.lblPlayUnmuteText.AutoSize = true;
            this.lblPlayUnmuteText.BackColor = System.Drawing.Color.Transparent;
            this.lblPlayUnmuteText.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPlayUnmuteText.ForeColor = System.Drawing.Color.White;
            this.lblPlayUnmuteText.Location = new System.Drawing.Point(70, 93);
            this.lblPlayUnmuteText.Name = "lblPlayUnmuteText";
            this.lblPlayUnmuteText.Size = new System.Drawing.Size(117, 15);
            this.lblPlayUnmuteText.TabIndex = 6;
            this.lblPlayUnmuteText.Text = "Som ao desmutar";
            // 
            // btnBrowseUnmute
            // 
            this.btnBrowseUnmute.BackColor = System.Drawing.Color.Transparent;
            this.btnBrowseUnmute.CustomBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnBrowseUnmute.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnBrowseUnmute.CustomHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.btnBrowseUnmute.CustomPressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnBrowseUnmute.Font = new System.Drawing.Font("Segoe MDL2 Assets", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowseUnmute.ForeColor = System.Drawing.Color.White;
            this.btnBrowseUnmute.IconGlyph = "";
            this.btnBrowseUnmute.IconSize = 10F;
            this.btnBrowseUnmute.Location = new System.Drawing.Point(195, 89);
            this.btnBrowseUnmute.Name = "btnBrowseUnmute";
            this.btnBrowseUnmute.Radius = 4;
            this.btnBrowseUnmute.Size = new System.Drawing.Size(30, 26);
            this.btnBrowseUnmute.TabIndex = 7;
            this.btnBrowseUnmute.Text = "\uED25";
            this.btnBrowseUnmute.Click += new System.EventHandler(this.BtnBrowseUnmute_Click);
            // 
            // lblUnmuteFile
            // 
            this.lblUnmuteFile.BackColor = System.Drawing.Color.Transparent;
            this.lblUnmuteFile.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUnmuteFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblUnmuteFile.Location = new System.Drawing.Point(70, 115);
            this.lblUnmuteFile.Name = "lblUnmuteFile";
            this.lblUnmuteFile.Size = new System.Drawing.Size(120, 16);
            this.lblUnmuteFile.TabIndex = 8;
            this.lblUnmuteFile.Text = "Nenhum som";
            // 
            // lblVolumeIcon
            // 
            this.lblVolumeIcon.AutoSize = true;
            this.lblVolumeIcon.BackColor = System.Drawing.Color.Transparent;
            this.lblVolumeIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVolumeIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblVolumeIcon.Location = new System.Drawing.Point(15, 151);
            this.lblVolumeIcon.Name = "lblVolumeIcon";
            this.lblVolumeIcon.Size = new System.Drawing.Size(16, 15);
            this.lblVolumeIcon.TabIndex = 9;
            this.lblVolumeIcon.Text = "\uE767"; // Speaker icon
            // 
            // lblVolumeValue
            // 
            this.lblVolumeValue.AutoSize = true;
            this.lblVolumeValue.BackColor = System.Drawing.Color.Transparent;
            this.lblVolumeValue.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVolumeValue.ForeColor = System.Drawing.Color.White;
            this.lblVolumeValue.Location = new System.Drawing.Point(35, 152);
            this.lblVolumeValue.Name = "lblVolumeValue";
            this.lblVolumeValue.Size = new System.Drawing.Size(35, 15);
            this.lblVolumeValue.TabIndex = 10;
            this.lblVolumeValue.Text = "100%";
            //
            // trackBarVolume
            //
            this.trackBarVolume.Minimum = 0;
            this.trackBarVolume.Maximum = 100;
            this.trackBarVolume.Value = 100;
            this.trackBarVolume.TickFrequency = 10;
            this.trackBarVolume.LargeChange = 10;
            this.trackBarVolume.SmallChange = 1;
            this.trackBarVolume.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // Oculta os ticks visuais para visual mais limpo no tema escuro
            this.trackBarVolume.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarVolume.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.trackBarVolume.Location = new System.Drawing.Point(12, 168);
            this.trackBarVolume.Name = "trackBarVolume";
            this.trackBarVolume.Size = new System.Drawing.Size(216, 22);
            this.trackBarVolume.TabIndex = 11;
            this.trackBarVolume.Scroll += new System.EventHandler(this.TrackBarVolume_Scroll);
            this.lblUnmuteFile.AutoEllipsis = true;
            this.lblUnmuteFile.AutoSize = false;
            // 
            // panelHotkeys
            // 
            this.panelHotkeys.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.panelHotkeys.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.panelHotkeys.BorderWidth = 1;
            this.panelHotkeys.Controls.Add(this.lblHotkeysTitle);
            this.panelHotkeys.Controls.Add(this.lblToggleLabel);
            this.panelHotkeys.Controls.Add(this.buttonReset);
            this.panelHotkeys.Controls.Add(this.lblMuteLabel);
            this.panelHotkeys.Controls.Add(this.muteReset);
            this.panelHotkeys.Controls.Add(this.lblUnmuteLabel);
            this.panelHotkeys.Controls.Add(this.unmuteReset);
            this.panelHotkeys.Location = new System.Drawing.Point(265, 15);
            this.panelHotkeys.Name = "panelHotkeys";
            this.panelHotkeys.Radius = 8;
            this.panelHotkeys.Size = new System.Drawing.Size(240, 305);
            this.panelHotkeys.TabIndex = 2;
            // 
            // lblHotkeysTitle
            // 
            this.lblHotkeysTitle.AutoSize = true;
            this.lblHotkeysTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblHotkeysTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHotkeysTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblHotkeysTitle.Location = new System.Drawing.Point(15, 12);
            this.lblHotkeysTitle.Name = "lblHotkeysTitle";
            this.lblHotkeysTitle.Size = new System.Drawing.Size(107, 17);
            this.lblHotkeysTitle.TabIndex = 0;
            this.lblHotkeysTitle.Text = "Teclas de Atalho";
            // 
            // lblToggleLabel
            // 
            this.lblToggleLabel.AutoSize = true;
            this.lblToggleLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblToggleLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblToggleLabel.ForeColor = System.Drawing.Color.White;
            this.lblToggleLabel.Location = new System.Drawing.Point(15, 40);
            this.lblToggleLabel.Name = "lblToggleLabel";
            this.lblToggleLabel.Size = new System.Drawing.Size(100, 15);
            this.lblToggleLabel.TabIndex = 1;
            this.lblToggleLabel.Text = "Alternar (Toggle):";
            // 
            // buttonReset
            // 
            this.buttonReset.BackColor = System.Drawing.Color.Transparent;
            this.buttonReset.CustomBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.buttonReset.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.buttonReset.CustomHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.buttonReset.CustomPressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.buttonReset.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonReset.ForeColor = System.Drawing.Color.White;
            this.buttonReset.IconGlyph = "";
            this.buttonReset.IconSize = 10F;
            this.buttonReset.Location = new System.Drawing.Point(170, 60);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Radius = 4;
            this.buttonReset.Size = new System.Drawing.Size(55, 32);
            this.buttonReset.TabIndex = 3;
            this.buttonReset.Text = "Limpar";
            this.buttonReset.Click += new System.EventHandler(this.ButtonReset_Click);
            // 
            // lblMuteLabel
            // 
            this.lblMuteLabel.AutoSize = true;
            this.lblMuteLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblMuteLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMuteLabel.ForeColor = System.Drawing.Color.White;
            this.lblMuteLabel.Location = new System.Drawing.Point(15, 125);
            this.lblMuteLabel.Name = "lblMuteLabel";
            this.lblMuteLabel.Size = new System.Drawing.Size(81, 15);
            this.lblMuteLabel.TabIndex = 4;
            this.lblMuteLabel.Text = "Mutar (Mute):";
            // 
            // muteReset
            // 
            this.muteReset.BackColor = System.Drawing.Color.Transparent;
            this.muteReset.CustomBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.muteReset.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.muteReset.CustomHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.muteReset.CustomPressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.muteReset.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.muteReset.ForeColor = System.Drawing.Color.White;
            this.muteReset.IconGlyph = "";
            this.muteReset.IconSize = 10F;
            this.muteReset.Location = new System.Drawing.Point(170, 145);
            this.muteReset.Name = "muteReset";
            this.muteReset.Radius = 4;
            this.muteReset.Size = new System.Drawing.Size(55, 32);
            this.muteReset.TabIndex = 6;
            this.muteReset.Text = "Limpar";
            this.muteReset.Click += new System.EventHandler(this.muteReset_Click);
            // 
            // lblUnmuteLabel
            // 
            this.lblUnmuteLabel.AutoSize = true;
            this.lblUnmuteLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblUnmuteLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUnmuteLabel.ForeColor = System.Drawing.Color.White;
            this.lblUnmuteLabel.Location = new System.Drawing.Point(15, 210);
            this.lblUnmuteLabel.Name = "lblUnmuteLabel";
            this.lblUnmuteLabel.Size = new System.Drawing.Size(110, 15);
            this.lblUnmuteLabel.TabIndex = 7;
            this.lblUnmuteLabel.Text = "Desmutar (Unmute):";
            // 
            // unmuteReset
            // 
            this.unmuteReset.BackColor = System.Drawing.Color.Transparent;
            this.unmuteReset.CustomBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.unmuteReset.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.unmuteReset.CustomHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.unmuteReset.CustomPressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.unmuteReset.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.unmuteReset.ForeColor = System.Drawing.Color.White;
            this.unmuteReset.IconGlyph = "";
            this.unmuteReset.IconSize = 10F;
            this.unmuteReset.Location = new System.Drawing.Point(170, 230);
            this.unmuteReset.Name = "unmuteReset";
            this.unmuteReset.Radius = 4;
            this.unmuteReset.Size = new System.Drawing.Size(55, 32);
            this.unmuteReset.TabIndex = 9;
            this.unmuteReset.Text = "Limpar";
            this.unmuteReset.Click += new System.EventHandler(this.unmuteReset_Click);
            // 
            // hotkeyTextBox
            // 
            this.hotkeyTextBox.Hotkey = null;
            this.hotkeyTextBox.Name = "hotkeyTextBox";
            this.hotkeyTextBox.Text = "Nenhum";
            // 
            // muteTextBox
            // 
            this.muteTextBox.Hotkey = null;
            this.muteTextBox.Name = "muteTextBox";
            this.muteTextBox.Text = "Nenhum";
            // 
            // unmuteTextBox
            // 
            this.unmuteTextBox.Hotkey = null;
            this.unmuteTextBox.Name = "unmuteTextBox";
            this.unmuteTextBox.Text = "Nenhum";
            // 
            // txtMutePath
            // 
            this.txtMutePath.Name = "txtMutePath";
            this.txtMutePath.Visible = false;
            this.txtMutePath.Size = new System.Drawing.Size(0, 20);
            // 
            // txtUnmutePath
            // 
            this.txtUnmutePath.Name = "txtUnmutePath";
            this.txtUnmutePath.Visible = false;
            this.txtUnmutePath.Size = new System.Drawing.Size(0, 20);
            // 
            // panelGeneral
            // 
            this.panelGeneral.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.panelGeneral.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.panelGeneral.BorderWidth = 1;
            this.panelGeneral.Controls.Add(this.lblStartWithWindows);
            this.panelGeneral.Controls.Add(this.chkStartWithWindows);
            this.panelGeneral.Controls.Add(this.btnAbout);
            this.panelGeneral.Controls.Add(this.cbLanguage);
            this.panelGeneral.Location = new System.Drawing.Point(15, 380);
            this.panelGeneral.Name = "panelGeneral";
            this.panelGeneral.Radius = 8;
            this.panelGeneral.Size = new System.Drawing.Size(490, 50);
            this.panelGeneral.TabIndex = 3;
            // 
            // lblStartWithWindows
            // 
            this.lblStartWithWindows.AutoSize = true;
            this.lblStartWithWindows.BackColor = System.Drawing.Color.Transparent;
            this.lblStartWithWindows.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStartWithWindows.ForeColor = System.Drawing.Color.White;
            this.lblStartWithWindows.Location = new System.Drawing.Point(15, 17);
            this.lblStartWithWindows.Name = "lblStartWithWindows";
            this.lblStartWithWindows.Size = new System.Drawing.Size(227, 15);
            this.lblStartWithWindows.TabIndex = 0;
            this.lblStartWithWindows.Text = "Iniciar com Windows";
            // 
            // 
            // chkStartWithWindows
            // 
            this.chkStartWithWindows.Checked = false;
            this.chkStartWithWindows.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chkStartWithWindows.Location = new System.Drawing.Point(245, 12);
            this.chkStartWithWindows.Name = "chkStartWithWindows";
            this.chkStartWithWindows.Size = new System.Drawing.Size(50, 26);
            this.chkStartWithWindows.TabIndex = 1;
            // 
            // btnAbout
            // 
            this.btnAbout.BackColor = System.Drawing.Color.Transparent;
            this.btnAbout.CustomBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnAbout.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnAbout.CustomHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.btnAbout.CustomPressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnAbout.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbout.ForeColor = System.Drawing.Color.White;
            this.btnAbout.Location = new System.Drawing.Point(415, 9);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Radius = 4;
            this.btnAbout.Size = new System.Drawing.Size(60, 32);
            this.btnAbout.TabIndex = 2;
            this.btnAbout.Text = "Sobre";
            this.btnAbout.Click += new System.EventHandler(this.BtnAbout_Click);
            // 
            // cbLanguage
            // 
            this.cbLanguage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.cbLanguage.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLanguage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbLanguage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbLanguage.ForeColor = System.Drawing.Color.White;
            this.cbLanguage.FormattingEnabled = true;
            this.cbLanguage.Items.AddRange(new object[] {
            "PT-BR",
            "EN"});
            this.cbLanguage.Location = new System.Drawing.Point(325, 12);
            this.cbLanguage.Name = "cbLanguage";
            this.cbLanguage.Size = new System.Drawing.Size(80, 24);
            this.cbLanguage.TabIndex = 3;
            this.cbLanguage.SelectedIndexChanged += new System.EventHandler(this.CbLanguage_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(520, 445);
            this.Controls.Add(this.panelGeneral);
            this.Controls.Add(this.panelHotkeys);
            this.Controls.Add(this.panelFeedback);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.txtMutePath);
            this.Controls.Add(this.txtUnmutePath);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MicMute";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.iconContextMenu.ResumeLayout(false);
            this.panelStatus.ResumeLayout(false);
            this.panelStatus.PerformLayout();
            this.panelFeedback.ResumeLayout(false);
            this.panelFeedback.PerformLayout();
            this.panelHotkeys.ResumeLayout(false);
            this.panelHotkeys.PerformLayout();
            this.panelGeneral.ResumeLayout(false);
            this.panelGeneral.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon icon;
        private System.Windows.Forms.ContextMenuStrip iconContextMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem hotkeyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        
        // Panels/Cards
        private MicMute.ModernPanel panelStatus;
        private MicMute.ModernPanel panelFeedback;
        private MicMute.ModernPanel panelHotkeys;
        
        // Status & Device controls
        private MicMute.ModernButton btnToggleMic;
        private System.Windows.Forms.Label lblStatusText;
        private System.Windows.Forms.Label lblDeviceName;
        public MicMute.ModernComboBox cbMics;
        
        // Feedback Card controls
        private System.Windows.Forms.Label lblFeedbackTitle;
        private MicMute.ModernToggleSwitch chkPlayMute;
        private System.Windows.Forms.Label lblPlayMuteText;
        private MicMute.ModernButton btnBrowseMute;
        private System.Windows.Forms.Label lblMuteFile;
        
        private MicMute.ModernToggleSwitch chkPlayUnmute;
        private System.Windows.Forms.Label lblPlayUnmuteText;
        private MicMute.ModernButton btnBrowseUnmute;
        private System.Windows.Forms.Label lblUnmuteFile;
        
        // Hotkey Card controls
        private System.Windows.Forms.Label lblHotkeysTitle;
        private System.Windows.Forms.Label lblToggleLabel;
        private Shortcut.Forms.HotkeyTextBox hotkeyTextBox;
        private MicMute.ModernButton buttonReset;
        private System.Windows.Forms.Label lblMuteLabel;
        private Shortcut.Forms.HotkeyTextBox muteTextBox;
        private MicMute.ModernButton muteReset;
        private System.Windows.Forms.Label lblUnmuteLabel;
        private Shortcut.Forms.HotkeyTextBox unmuteTextBox;
        private MicMute.ModernButton unmuteReset;
        
        // Background logical path variables
        private System.Windows.Forms.TextBox txtMutePath;
        private System.Windows.Forms.TextBox txtUnmutePath;
        
        // General Settings controls
        private MicMute.ModernPanel panelGeneral;
        private System.Windows.Forms.Label lblStartWithWindows;
        public MicMute.ModernToggleSwitch chkStartWithWindows;
        private MicMute.ModernButton btnAbout;
        private MicMute.ModernComboBox cbLanguage;
        
        // Volume controls
        private System.Windows.Forms.Label lblVolumeIcon;
        private System.Windows.Forms.Label lblVolumeValue;
        private System.Windows.Forms.TrackBar trackBarVolume;
    }
}
