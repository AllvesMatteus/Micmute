using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MicMute
{
    public static class FluentTheme
    {
        public static readonly Color BackColor = Color.FromArgb(32, 32, 32);       // Dark Theme Background
        public static readonly Color CardColor = Color.FromArgb(45, 45, 45);       // Fluent Card Background
        public static readonly Color CardBorder = Color.FromArgb(58, 58, 58);      // Border for Cards
        
        public static readonly Color TextPrimary = Color.FromArgb(255, 255, 255);  // White text
        public static readonly Color TextSecondary = Color.FromArgb(170, 170, 170);// Muted gray text
        
        public static readonly Color AccentColor = Color.FromArgb(0, 120, 212);    // Win11 Accent Blue
        public static readonly Color AccentHover = Color.FromArgb(25, 142, 230);
        public static readonly Color AccentPressed = Color.FromArgb(0, 102, 180);
        
        public static readonly Color ButtonColor = Color.FromArgb(50, 50, 50);
        public static readonly Color ButtonHover = Color.FromArgb(62, 62, 62);
        public static readonly Color ButtonPressed = Color.FromArgb(40, 40, 40);
        public static readonly Color ButtonBorder = Color.FromArgb(68, 68, 68);

        public static readonly Color MuteRed = Color.FromArgb(232, 17, 35);        // Mic Muted (Red)
        public static readonly Color UnmuteGreen = Color.FromArgb(16, 124, 65);     // Mic Active (Green)
        public static readonly Color ErrorOrange = Color.FromArgb(247, 99, 12);    // Warning/Error

        public static GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float r = radius * 2F;
            if (r <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            
            if (r > rect.Width) r = rect.Width;
            if (r > rect.Height) r = rect.Height;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, r, r, 180, 90);
            path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
            path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    public class ModernPanel : Panel
    {
        public int Radius { get; set; } = 8;
        public Color BorderColor { get; set; } = FluentTheme.CardBorder;
        public int BorderWidth { get; set; } = 1;

        public ModernPanel()
        {
            DoubleBuffered = true;
            BackColor = FluentTheme.CardColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = FluentTheme.GetRoundedPath(rect, Radius))
            {
                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    g.FillPath(brush, path);
                }

                if (BorderWidth > 0)
                {
                    using (Pen pen = new Pen(BorderColor, BorderWidth))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }
        }
    }

    public class ModernButton : Control
    {
        private bool isHovered = false;
        private bool isPressed = false;

        public int Radius { get; set; } = 6;
        public Color CustomBackColor { get; set; } = FluentTheme.ButtonColor;
        public Color CustomHoverColor { get; set; } = FluentTheme.ButtonHover;
        public Color CustomPressedColor { get; set; } = FluentTheme.ButtonPressed;
        public Color CustomBorderColor { get; set; } = FluentTheme.ButtonBorder;

        private Image buttonImage = null;
        public Image Image
        {
            get => buttonImage;
            set { buttonImage = value; Invalidate(); }
        }

        public string IconGlyph { get; set; } = "";
        public float IconSize { get; set; } = 11f;

        // Dummy properties for WinForms designer compatibility
        public object FlatAppearance => null;
        public object FlatStyle { get; set; }
        public bool UseVisualStyleBackColor { get; set; }

        public ModernButton()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            ForeColor = FluentTheme.TextPrimary;
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            base.OnMouseEnter(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isHovered = false;
            isPressed = false;
            base.OnMouseLeave(e);
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }
            base.OnMouseDown(mevent);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            isPressed = false;
            Invalidate();
            base.OnMouseUp(mevent);
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (Parent != null)
            {
                using (SolidBrush parentBrush = new SolidBrush(Parent.BackColor))
                {
                    g.FillRectangle(parentBrush, ClientRectangle);
                }
            }

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            
            Color bg = CustomBackColor;
            if (Enabled)
            {
                if (isPressed) bg = CustomPressedColor;
                else if (isHovered) bg = CustomHoverColor;
            }
            else
            {
                bg = Color.FromArgb(40, 40, 40);
            }

            using (GraphicsPath path = FluentTheme.GetRoundedPath(rect, Radius))
            {
                using (SolidBrush brush = new SolidBrush(bg))
                {
                    g.FillPath(brush, path);
                }

                if (Enabled)
                {
                    using (Pen pen = new Pen(CustomBorderColor, 1))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            int textXOffset = 0;
            if (Image != null)
            {
                int targetSize = (int)(Math.Min(Width, Height) * 0.5f);
                if (targetSize < 16) targetSize = 16;
                float ix = (Width - targetSize) / 2f;
                float iy = (Height - targetSize) / 2f;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(Image, ix, iy, targetSize, targetSize);
            }
            else if (!string.IsNullOrEmpty(IconGlyph))
            {
                using (Font iconFont = new Font("Segoe MDL2 Assets", IconSize, FontStyle.Regular))
                using (Brush iconBrush = new SolidBrush(Enabled ? ForeColor : FluentTheme.TextSecondary))
                {
                    SizeF iconSize = g.MeasureString(IconGlyph, iconFont);
                    float iy = (Height - iconSize.Height) / 2f;
                    float ix = Text == "" ? (Width - iconSize.Width) / 2f : 12f;
                    g.DrawString(IconGlyph, iconFont, iconBrush, ix, iy);
                    textXOffset = (int)(ix + iconSize.Width - 4);
                }
            }

            string text = Text;
            if (!string.IsNullOrEmpty(text))
            {
                using (Brush textBrush = new SolidBrush(Enabled ? ForeColor : FluentTheme.TextSecondary))
                {
                    SizeF textSize = g.MeasureString(text, Font);
                    float tx = textXOffset > 0 ? textXOffset + 8 : (Width - textSize.Width) / 2f;
                    float ty = (Height - textSize.Height) / 2f;
                    g.DrawString(text, Font, textBrush, tx, ty);
                }
            }
        }
    }

    public class ModernToggleSwitch : Control
    {
        private bool isChecked = false;
        public bool Checked
        {
            get => isChecked;
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    CheckedChanged?.Invoke(this, EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        public event EventHandler CheckedChanged;

        private bool isHovered = false;

        public ModernToggleSwitch()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Size = new Size(50, 26);
            Cursor = Cursors.Hand;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            base.OnMouseEnter(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isHovered = false;
            base.OnMouseLeave(e);
            Invalidate();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Checked = !Checked;
            }
            base.OnMouseClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (Parent != null)
            {
                using (SolidBrush parentBrush = new SolidBrush(Parent.BackColor))
                {
                    g.FillRectangle(parentBrush, ClientRectangle);
                }
            }

            int trackWidth = 44;
            int trackHeight = 20;
            int trackX = 2;
            int trackY = (Height - trackHeight) / 2;
            Rectangle trackRect = new Rectangle(trackX, trackY, trackWidth, trackHeight);

            Color trackColor = Checked ? FluentTheme.AccentColor : Color.FromArgb(80, 80, 80);
            if (isHovered)
            {
                trackColor = Checked ? FluentTheme.AccentHover : Color.FromArgb(100, 100, 100);
            }

            using (GraphicsPath trackPath = FluentTheme.GetRoundedPath(trackRect, trackHeight / 2 - 1))
            {
                using (SolidBrush brush = new SolidBrush(trackColor))
                {
                    g.FillPath(brush, trackPath);
                }
                
                if (!Checked)
                {
                    using (Pen pen = new Pen(Color.FromArgb(120, 120, 120), 1))
                    {
                        g.DrawPath(pen, trackPath);
                    }
                }
            }

            int thumbSize = 14;
            int thumbY = trackY + (trackHeight - thumbSize) / 2;
            int thumbX = Checked ? (trackRect.Right - thumbSize - 3) : (trackRect.Left + 3);

            Rectangle thumbRect = new Rectangle(thumbX, thumbY, thumbSize, thumbSize);
            Color thumbColor = Checked ? Color.White : Color.FromArgb(200, 200, 200);

            using (SolidBrush brush = new SolidBrush(thumbColor))
            {
                g.FillEllipse(brush, thumbRect);
            }
        }
    }

    public class ModernComboBox : ComboBox
    {
        public ModernComboBox()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            FlatStyle = FlatStyle.Flat;
            BackColor = FluentTheme.CardColor;
            ForeColor = FluentTheme.TextPrimary;
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnGotFocus(EventArgs e) { base.OnGotFocus(e); Invalidate(); }
        protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); Invalidate(); }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color bg = isSelected ? FluentTheme.ButtonHover : FluentTheme.CardColor;
            Color fg = FluentTheme.TextPrimary;

            using (SolidBrush brush = new SolidBrush(bg))
            {
                g.FillRectangle(brush, e.Bounds);
            }

            string text = Items[e.Index].ToString();
            using (Brush textBrush = new SolidBrush(fg))
            {
                SizeF size = g.MeasureString(text, Font);
                float tx = e.Bounds.X + 8;
                float ty = e.Bounds.Y + (e.Bounds.Height - size.Height) / 2f;
                g.DrawString(text, Font, textBrush, tx, ty);
            }

            e.DrawFocusRectangle();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = FluentTheme.GetRoundedPath(rect, 4))
            {
                using (SolidBrush brush = new SolidBrush(FluentTheme.CardColor))
                {
                    g.FillPath(brush, path);
                }
                
                Color borderColor = Focused ? FluentTheme.AccentColor : FluentTheme.CardBorder;
                using (Pen pen = new Pen(borderColor, Focused ? 2 : 1))
                {
                    g.DrawPath(pen, path);
                }
            }

            using (Font iconFont = new Font("Segoe MDL2 Assets", 8F, FontStyle.Regular))
            using (Brush iconBrush = new SolidBrush(FluentTheme.TextSecondary))
            {
                string chevronGlyph = "\uE70D";
                SizeF iconSize = g.MeasureString(chevronGlyph, iconFont);
                float ix = Width - iconSize.Width - 12;
                float iy = (Height - iconSize.Height) / 2f;
                g.DrawString(chevronGlyph, iconFont, iconBrush, ix, iy);
            }

            string text = SelectedItem != null ? SelectedItem.ToString() : "";
            using (Brush textBrush = new SolidBrush(FluentTheme.TextPrimary))
            {
                SizeF size = g.MeasureString(text, Font);
                float tx = 10;
                float ty = (Height - size.Height) / 2f;
                
                RectangleF layoutRect = new RectangleF(tx, ty, Width - 32, size.Height);
                StringFormat format = new StringFormat();
                format.Trimming = StringTrimming.EllipsisCharacter;
                format.FormatFlags = StringFormatFlags.NoWrap;
                g.DrawString(text, Font, textBrush, layoutRect, format);
            }
        }
    }

    public class ModernTextBoxContainer : Panel
    {
        private Control innerControl;
        private bool isFocused = false;
        
        public ModernTextBoxContainer(Control inner)
        {
            innerControl = inner;
            DoubleBuffered = true;
            BackColor = FluentTheme.CardColor;
            Padding = new Padding(8, 6, 8, 6);
            Controls.Add(inner);
            
            inner.Location = new Point(8, 6);
            inner.Width = Width - 16;
            inner.BackColor = FluentTheme.CardColor;
            inner.ForeColor = FluentTheme.TextPrimary;
            inner.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            
            if (inner is TextBox tb)
            {
                tb.BorderStyle = BorderStyle.None;
            }

            // Track focus state for visual feedback
            inner.GotFocus += (s, e) => { isFocused = true; Invalidate(); };
            inner.LostFocus += (s, e) => { isFocused = false; Invalidate(); };

            SizeChanged += (s, e) =>
            {
                inner.Width = Width - 16;
            };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = FluentTheme.GetRoundedPath(rect, 4))
            {
                using (SolidBrush brush = new SolidBrush(FluentTheme.CardColor))
                {
                    g.FillPath(brush, path);
                }
                
                // Draw full border: accent blue when focused, subtle gray otherwise
                Color borderColor = isFocused ? FluentTheme.AccentColor : FluentTheme.CardBorder;
                float borderWidth = isFocused ? 2f : 1f;
                using (Pen pen = new Pen(borderColor, borderWidth))
                {
                    g.DrawPath(pen, path);
                }

                // Draw accent bottom line (thicker when focused)
                using (Pen accentPen = new Pen(isFocused ? FluentTheme.AccentColor : FluentTheme.CardBorder, isFocused ? 2 : 1))
                {
                    g.DrawLine(accentPen, 4, Height - 1, Width - 5, Height - 1);
                }
            }
        }
    }
}
