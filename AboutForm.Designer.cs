namespace MicMute
{
    partial class AboutForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelCard = new MicMute.ModernPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblVersionStatus = new System.Windows.Forms.Label();
            this.lblReleaseDate = new System.Windows.Forms.Label();
            this.lblBuildInfo = new System.Windows.Forms.Label();
            this.lblNotesTitle = new System.Windows.Forms.Label();
            this.lblNotesBody = new System.Windows.Forms.Label();
            this.lblAuthorLabel = new System.Windows.Forms.Label();
            this.lblAuthorName = new System.Windows.Forms.Label();
            this.btnGithub = new MicMute.ModernButton();
            this.btnLinkedin = new MicMute.ModernButton();
            this.btnReleasePage = new MicMute.ModernButton();
            this.panelCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelCard
            // 
            this.panelCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.panelCard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.panelCard.BorderWidth = 1;
            this.panelCard.Controls.Add(this.lblTitle);
            this.panelCard.Controls.Add(this.lblVersionStatus);
            this.panelCard.Controls.Add(this.lblReleaseDate);
            this.panelCard.Controls.Add(this.lblBuildInfo);
            this.panelCard.Controls.Add(this.lblNotesTitle);
            this.panelCard.Controls.Add(this.lblNotesBody);
            this.panelCard.Controls.Add(this.lblAuthorLabel);
            this.panelCard.Controls.Add(this.lblAuthorName);
            this.panelCard.Controls.Add(this.btnGithub);
            this.panelCard.Controls.Add(this.btnLinkedin);
            this.panelCard.Controls.Add(this.btnReleasePage);
            this.panelCard.Location = new System.Drawing.Point(15, 15);
            this.panelCard.Name = "panelCard";
            this.panelCard.Radius = 8;
            this.panelCard.Size = new System.Drawing.Size(330, 310);
            this.panelCard.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(15, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(147, 23);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "MicMute v1.1.0";
            // 
            // lblVersionStatus
            // 
            this.lblVersionStatus.AutoSize = true;
            this.lblVersionStatus.BackColor = System.Drawing.Color.Transparent;
            this.lblVersionStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersionStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(100)))));
            this.lblVersionStatus.Location = new System.Drawing.Point(16, 40);
            this.lblVersionStatus.Name = "lblVersionStatus";
            this.lblVersionStatus.Size = new System.Drawing.Size(180, 15);
            this.lblVersionStatus.TabIndex = 1;
            this.lblVersionStatus.Text = "✓ Versão mais recente instalada";
            // 
            // lblReleaseDate
            // 
            this.lblReleaseDate.AutoSize = true;
            this.lblReleaseDate.BackColor = System.Drawing.Color.Transparent;
            this.lblReleaseDate.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReleaseDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(190)))), ((int)(((byte)(190)))));
            this.lblReleaseDate.Location = new System.Drawing.Point(16, 62);
            this.lblReleaseDate.Name = "lblReleaseDate";
            this.lblReleaseDate.Size = new System.Drawing.Size(180, 15);
            this.lblReleaseDate.TabIndex = 2;
            this.lblReleaseDate.Text = "Lançado em: --";
            // 
            // lblBuildInfo
            // 
            this.lblBuildInfo.AutoSize = true;
            this.lblBuildInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblBuildInfo.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBuildInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(145)))), ((int)(((byte)(145)))), ((int)(((byte)(145)))));
            this.lblBuildInfo.Location = new System.Drawing.Point(16, 80);
            this.lblBuildInfo.Name = "lblBuildInfo";
            this.lblBuildInfo.Size = new System.Drawing.Size(220, 13);
            this.lblBuildInfo.TabIndex = 3;
            this.lblBuildInfo.Text = "Build 1.1.0.0 • .NET Framework 4.8";
            // 
            // lblNotesTitle
            // 
            this.lblNotesTitle.AutoSize = true;
            this.lblNotesTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblNotesTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotesTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblNotesTitle.Location = new System.Drawing.Point(16, 104);
            this.lblNotesTitle.Name = "lblNotesTitle";
            this.lblNotesTitle.Size = new System.Drawing.Size(120, 15);
            this.lblNotesTitle.TabIndex = 4;
            this.lblNotesTitle.Text = "Detalhes da versão:";
            // 
            // lblNotesBody
            // 
            this.lblNotesBody.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.lblNotesBody.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotesBody.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblNotesBody.Location = new System.Drawing.Point(16, 123);
            this.lblNotesBody.Name = "lblNotesBody";
            this.lblNotesBody.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.lblNotesBody.Size = new System.Drawing.Size(298, 72);
            this.lblNotesBody.TabIndex = 5;
            this.lblNotesBody.Text = "";
            this.lblNotesBody.AutoEllipsis = true;
            // 
            // lblAuthorLabel
            // 
            this.lblAuthorLabel.AutoSize = true;
            this.lblAuthorLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblAuthorLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAuthorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblAuthorLabel.Location = new System.Drawing.Point(16, 205);
            this.lblAuthorLabel.Name = "lblAuthorLabel";
            this.lblAuthorLabel.Size = new System.Drawing.Size(97, 15);
            this.lblAuthorLabel.TabIndex = 6;
            this.lblAuthorLabel.Text = "Desenvolvido por:";
            // 
            // lblAuthorName
            // 
            this.lblAuthorName.AutoSize = true;
            this.lblAuthorName.BackColor = System.Drawing.Color.Transparent;
            this.lblAuthorName.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAuthorName.ForeColor = System.Drawing.Color.White;
            this.lblAuthorName.Location = new System.Drawing.Point(16, 223);
            this.lblAuthorName.Name = "lblAuthorName";
            this.lblAuthorName.Size = new System.Drawing.Size(102, 19);
            this.lblAuthorName.TabIndex = 7;
            this.lblAuthorName.Text = "AllvesMatteus";
            // 
            // btnGithub
            // 
            this.btnGithub.BackColor = System.Drawing.Color.Transparent;
            this.btnGithub.CustomBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnGithub.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnGithub.CustomHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.btnGithub.CustomPressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnGithub.Location = new System.Drawing.Point(16, 250);
            this.btnGithub.Name = "btnGithub";
            this.btnGithub.Radius = 6;
            this.btnGithub.Size = new System.Drawing.Size(46, 46);
            this.btnGithub.TabIndex = 8;
            this.btnGithub.Click += new System.EventHandler(this.BtnGithub_Click);
            // 
            // btnLinkedin
            // 
            this.btnLinkedin.BackColor = System.Drawing.Color.Transparent;
            this.btnLinkedin.CustomBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnLinkedin.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnLinkedin.CustomHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.btnLinkedin.CustomPressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnLinkedin.Location = new System.Drawing.Point(72, 250);
            this.btnLinkedin.Name = "btnLinkedin";
            this.btnLinkedin.Radius = 6;
            this.btnLinkedin.Size = new System.Drawing.Size(46, 46);
            this.btnLinkedin.TabIndex = 9;
            this.btnLinkedin.Click += new System.EventHandler(this.BtnLinkedin_Click);
            // 
            // btnReleasePage
            // 
            this.btnReleasePage.BackColor = System.Drawing.Color.Transparent;
            this.btnReleasePage.CustomBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnReleasePage.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnReleasePage.CustomHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.btnReleasePage.CustomPressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnReleasePage.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReleasePage.ForeColor = System.Drawing.Color.White;
            this.btnReleasePage.Location = new System.Drawing.Point(194, 258);
            this.btnReleasePage.Name = "btnReleasePage";
            this.btnReleasePage.Radius = 6;
            this.btnReleasePage.Size = new System.Drawing.Size(120, 32);
            this.btnReleasePage.TabIndex = 10;
            this.btnReleasePage.Text = "Repositório";
            this.btnReleasePage.Click += new System.EventHandler(this.BtnReleasePage_Click);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(360, 340);
            this.Controls.Add(this.panelCard);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sobre";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            this.panelCard.ResumeLayout(false);
            this.panelCard.PerformLayout();
            this.ResumeLayout(false);
        }

        private MicMute.ModernPanel panelCard;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblVersionStatus;
        private System.Windows.Forms.Label lblReleaseDate;
        private System.Windows.Forms.Label lblBuildInfo;
        private System.Windows.Forms.Label lblNotesTitle;
        private System.Windows.Forms.Label lblNotesBody;
        private System.Windows.Forms.Label lblAuthorLabel;
        private System.Windows.Forms.Label lblAuthorName;
        private MicMute.ModernButton btnGithub;
        private MicMute.ModernButton btnLinkedin;
        private MicMute.ModernButton btnReleasePage;
    }
}
