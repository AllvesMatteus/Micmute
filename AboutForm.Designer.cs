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
            this.lblAuthorLabel = new System.Windows.Forms.Label();
            this.lblAuthorName = new System.Windows.Forms.Label();
            this.btnGithub = new MicMute.ModernButton();
            this.btnLinkedin = new MicMute.ModernButton();
            this.panelCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelCard
            // 
            this.panelCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.panelCard.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(58)))), ((int)(((byte)(58)))));
            this.panelCard.BorderWidth = 1;
            this.panelCard.Controls.Add(this.lblTitle);
            this.panelCard.Controls.Add(this.lblAuthorLabel);
            this.panelCard.Controls.Add(this.lblAuthorName);
            this.panelCard.Controls.Add(this.btnGithub);
            this.panelCard.Controls.Add(this.btnLinkedin);
            this.panelCard.Location = new System.Drawing.Point(15, 15);
            this.panelCard.Name = "panelCard";
            this.panelCard.Radius = 8;
            this.panelCard.Size = new System.Drawing.Size(250, 170);
            this.panelCard.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(15, 15);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(147, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "MicMute v0.1.8.4";
            // 
            // lblAuthorLabel
            // 
            this.lblAuthorLabel.AutoSize = true;
            this.lblAuthorLabel.BackColor = System.Drawing.Color.Transparent;
            this.lblAuthorLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAuthorLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblAuthorLabel.Location = new System.Drawing.Point(15, 45);
            this.lblAuthorLabel.Name = "lblAuthorLabel";
            this.lblAuthorLabel.Size = new System.Drawing.Size(97, 15);
            this.lblAuthorLabel.TabIndex = 1;
            this.lblAuthorLabel.Text = "Desenvolvido por:";
            // 
            // lblAuthorName
            // 
            this.lblAuthorName.AutoSize = true;
            this.lblAuthorName.BackColor = System.Drawing.Color.Transparent;
            this.lblAuthorName.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAuthorName.ForeColor = System.Drawing.Color.White;
            this.lblAuthorName.Location = new System.Drawing.Point(15, 65);
            this.lblAuthorName.Name = "lblAuthorName";
            this.lblAuthorName.Size = new System.Drawing.Size(91, 19);
            this.lblAuthorName.TabIndex = 2;
            this.lblAuthorName.Text = "Mateus Alves";
            // 
            // btnGithub
            // 
            this.btnGithub.BackColor = System.Drawing.Color.Transparent;
            this.btnGithub.CustomBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnGithub.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnGithub.CustomHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.btnGithub.CustomPressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnGithub.Location = new System.Drawing.Point(15, 105);
            this.btnGithub.Name = "btnGithub";
            this.btnGithub.Radius = 6;
            this.btnGithub.Size = new System.Drawing.Size(48, 48);
            this.btnGithub.TabIndex = 3;
            this.btnGithub.Click += new System.EventHandler(this.BtnGithub_Click);
            // 
            // btnLinkedin
            // 
            this.btnLinkedin.BackColor = System.Drawing.Color.Transparent;
            this.btnLinkedin.CustomBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnLinkedin.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.btnLinkedin.CustomHoverColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(62)))));
            this.btnLinkedin.CustomPressedColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.btnLinkedin.Location = new System.Drawing.Point(75, 105);
            this.btnLinkedin.Name = "btnLinkedin";
            this.btnLinkedin.Radius = 6;
            this.btnLinkedin.Size = new System.Drawing.Size(48, 48);
            this.btnLinkedin.TabIndex = 4;
            this.btnLinkedin.Click += new System.EventHandler(this.BtnLinkedin_Click);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(280, 200);
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
        private System.Windows.Forms.Label lblAuthorLabel;
        private System.Windows.Forms.Label lblAuthorName;
        private MicMute.ModernButton btnGithub;
        private MicMute.ModernButton btnLinkedin;
    }
}
