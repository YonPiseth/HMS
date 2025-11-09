using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using HMS.UI;

namespace HMS
{
    public partial class SplashForm : Form
    {
        private System.Windows.Forms.Timer timer;
        private Label lblTitle;
        private Label lblSubtitle;
        private Panel progressBarContainer;
        private Panel progressBarFill;
        private Panel mainPanel;
        private Panel contentPanel;
        private int progressValue = 0;
        private PictureBox logoPictureBox;
        private Label lblVersion;
        private int ellipsisState = 0;
        private string baseSubtitle = "Loading";
        private System.Windows.Forms.Timer animationTimer;

        public SplashForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.timer = new System.Windows.Forms.Timer();
            this.animationTimer = new System.Windows.Forms.Timer();
            this.lblTitle = new Label();
            this.lblSubtitle = new Label();
            this.progressBarContainer = new Panel();
            this.progressBarFill = new Panel();
            this.mainPanel = new Panel();
            this.contentPanel = new Panel();
            this.logoPictureBox = new PictureBox();
            this.lblVersion = new Label();

            this.Text = "Hospital Management System";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = UITheme.BackgroundLight;
            this.DoubleBuffered = true;

            this.mainPanel.Dock = DockStyle.Fill;
            this.mainPanel.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    this.mainPanel.ClientRectangle,
                    UITheme.PrimaryBlue,
                    Color.FromArgb(245, 248, 250),
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, this.mainPanel.ClientRectangle);
                }
            };

            this.contentPanel.Size = new Size(450, 350);
            this.contentPanel.Location = new Point(
                (this.Width - this.contentPanel.Width) / 2,
                (this.Height - this.contentPanel.Height) / 2
            );
            this.contentPanel.BackColor = Color.Transparent;
            this.contentPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                
                Rectangle shadowRect = new Rectangle(6, 6, this.contentPanel.Width - 12, this.contentPanel.Height - 12);
                using (GraphicsPath shadowPath = UITheme.CreateRoundedRectangle(shadowRect, UITheme.RadiusLG))
                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }
                
                Rectangle rect = new Rectangle(0, 0, this.contentPanel.Width - 1, this.contentPanel.Height - 1);
                using (GraphicsPath path = UITheme.CreateRoundedRectangle(rect, UITheme.RadiusLG))
                using (SolidBrush cardBrush = new SolidBrush(UITheme.BackgroundWhite))
                {
                    e.Graphics.FillPath(cardBrush, path);
                }
                
                using (GraphicsPath borderPath = UITheme.CreateRoundedRectangle(rect, UITheme.RadiusLG))
                using (Pen borderPen = new Pen(UITheme.BorderLight, 1))
                {
                    e.Graphics.DrawPath(borderPen, borderPath);
                }
                
                this.contentPanel.Region = new Region(UITheme.CreateRoundedRectangle(rect, UITheme.RadiusLG));
            };

            this.logoPictureBox.Size = new Size(100, 100);
            this.logoPictureBox.Location = new Point((this.contentPanel.Width - this.logoPictureBox.Width) / 2, 30);
            this.logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            this.logoPictureBox.BackColor = Color.Transparent;
            try
            {
                string imageDir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Image");
                string imagePath = Path.Combine(imageDir, "Loading Image.png");
                
                if (!File.Exists(imagePath))
                    imagePath = Path.Combine(imageDir, "Loading Image.jpg");
                
                if (File.Exists(imagePath))
                    this.logoPictureBox.Image = Image.FromFile(imagePath);
            }
            catch { }

            this.lblTitle.Text = "Hospital Management System";
            this.lblTitle.Font = UITheme.FontHeading2;
            this.lblTitle.ForeColor = UITheme.PrimaryBlue;
            this.lblTitle.AutoSize = false;
            this.lblTitle.Size = new Size(420, 50);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Location = new Point(15, 140);
            this.lblTitle.BackColor = Color.Transparent;

            this.lblSubtitle.Text = baseSubtitle + "...";
            this.lblSubtitle.Font = UITheme.FontBody;
            this.lblSubtitle.ForeColor = UITheme.TextSecondary;
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.Size = new Size(420, 25);
            this.lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblSubtitle.Location = new Point(15, 195);
            this.lblSubtitle.BackColor = Color.Transparent;

            this.progressBarContainer.Size = new Size(380, 8);
            this.progressBarContainer.Location = new Point(35, 240);
            this.progressBarContainer.BackColor = UITheme.BackgroundGray;
            this.progressBarContainer.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, this.progressBarContainer.Width - 1, this.progressBarContainer.Height - 1);
                using (GraphicsPath path = UITheme.CreateRoundedRectangle(rect, 4))
                using (SolidBrush brush = new SolidBrush(UITheme.BackgroundGray))
                {
                    e.Graphics.FillPath(brush, path);
                }
                this.progressBarContainer.Region = new Region(UITheme.CreateRoundedRectangle(rect, 4));
            };

            this.progressBarFill.Size = new Size(0, 8);
            this.progressBarFill.Location = new Point(0, 0);
            this.progressBarFill.BackColor = Color.Transparent;
            this.progressBarFill.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                if (this.progressBarFill.Width > 0)
                {
                    Rectangle rect = new Rectangle(0, 0, this.progressBarFill.Width - 1, this.progressBarFill.Height - 1);
                    using (GraphicsPath path = UITheme.CreateRoundedRectangle(rect, 4))
                    using (LinearGradientBrush brush = new LinearGradientBrush(
                        rect,
                        UITheme.PrimaryBlue,
                        UITheme.HoverBlue,
                        LinearGradientMode.Horizontal))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                    this.progressBarFill.Region = new Region(UITheme.CreateRoundedRectangle(rect, 4));
                }
            };

            this.progressBarContainer.Controls.Add(this.progressBarFill);

            this.lblVersion.Text = "Version 1.0.0";
            this.lblVersion.Font = UITheme.FontCaption;
            this.lblVersion.ForeColor = UITheme.TextSecondary;
            this.lblVersion.AutoSize = false;
            this.lblVersion.Size = new Size(420, 20);
            this.lblVersion.TextAlign = ContentAlignment.MiddleCenter;
            this.lblVersion.Location = new Point(15, 280);
            this.lblVersion.BackColor = Color.Transparent;

            this.contentPanel.Controls.Add(this.logoPictureBox);
            this.contentPanel.Controls.Add(this.lblTitle);
            this.contentPanel.Controls.Add(this.lblSubtitle);
            this.contentPanel.Controls.Add(this.progressBarContainer);
            this.contentPanel.Controls.Add(this.lblVersion);

            this.mainPanel.Controls.Add(this.contentPanel);
            this.Controls.Add(this.mainPanel);

            this.timer.Interval = 30;
            this.timer.Tick += Timer_Tick;
            
            this.animationTimer.Interval = 500;
            this.animationTimer.Tick += AnimationTimer_Tick;
            this.animationTimer.Start();
            
            this.timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            progressValue += 1;
            if (progressValue <= 100)
            {
                int targetWidth = (int)((progressValue / 100.0) * progressBarContainer.Width);
                if (progressBarFill.Width < targetWidth)
                {
                    progressBarFill.Width = Math.Min(progressBarFill.Width + 2, targetWidth);
                    progressBarFill.Invalidate();
                }
            }
            else
            {
                this.timer.Stop();
                this.animationTimer.Stop();
                System.Threading.Thread.Sleep(200);
                this.DialogResult = DialogResult.OK;
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            ellipsisState = (ellipsisState + 1) % 4;
            lblSubtitle.Text = baseSubtitle + new string('.', ellipsisState);
        }
    }
}
