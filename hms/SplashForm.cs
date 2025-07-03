using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace HMS
{
    public partial class SplashForm : Form
    {
        private System.Windows.Forms.Timer timer;
        private Label lblTitle;
        private Label lblSubtitle;
        private ProgressBar progressBar;
        private Panel mainPanel;
        private Panel contentPanel;
        private int progressValue = 0;
        private PictureBox logoPictureBox;
        private Label lblVersion;
        private int ellipsisState = 0;
        private string baseSubtitle = "Loading";

        public SplashForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.timer = new System.Windows.Forms.Timer();
            this.lblTitle = new Label();
            this.lblSubtitle = new Label();
            this.progressBar = new ProgressBar();
            this.mainPanel = new Panel();
            this.contentPanel = new Panel();
            this.logoPictureBox = new PictureBox();
            this.lblVersion = new Label();

            // Form settings
            this.Text = "Hospital Management System";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Main panel with gradient background
            this.mainPanel.Dock = DockStyle.Fill;
            this.mainPanel.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    this.mainPanel.ClientRectangle,
                    Color.FromArgb(41, 128, 185),  // Modern blue
                    Color.FromArgb(44, 62, 80),    // Dark blue
                    LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, this.mainPanel.ClientRectangle);
                }
            };

            // Content panel with glass effect and drop shadow
            this.contentPanel.Size = new Size(600, 400);
            this.contentPanel.Location = new Point(
                (this.Width - this.contentPanel.Width) / 2,
                (this.Height - this.contentPanel.Height) / 2
            );
            this.contentPanel.BackColor = Color.FromArgb(220, 230, 240, 220);
            this.contentPanel.Paint += (s, e) =>
            {
                // Draw drop shadow
                Rectangle shadowRect = new Rectangle(8, 8, this.contentPanel.Width - 16, this.contentPanel.Height - 16);
                using (GraphicsPath shadowPath = RoundedRect(shadowRect, 25))
                using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(60, 0, 0, 0)))
                {
                    e.Graphics.FillPath(shadowBrush, shadowPath);
                }
                // Draw rounded rectangle
                Rectangle rect = new Rectangle(0, 0, this.contentPanel.Width - 1, this.contentPanel.Height - 1);
                using (GraphicsPath path = RoundedRect(rect, 20))
                using (SolidBrush glassBrush = new SolidBrush(Color.FromArgb(220, 255, 255, 255)))
                {
                    e.Graphics.FillPath(glassBrush, path);
                }
                this.contentPanel.Region = new Region(RoundedRect(rect, 20));
            };

            // Logo PictureBox
            this.logoPictureBox.Size = new Size(120, 120);
            this.logoPictureBox.Location = new Point((this.contentPanel.Width - this.logoPictureBox.Width) / 2, 20);
            this.logoPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            try
            {
                this.logoPictureBox.Image = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Image", "Loading Image.jpg"));
            }
            catch { /* If image not found, leave blank */ }
            this.logoPictureBox.BackColor = Color.Transparent;

            // Title label with modern font
            this.lblTitle.Text = "Hospital Management System";
            this.lblTitle.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(41, 128, 185);
            this.lblTitle.AutoSize = false;
            this.lblTitle.Size = new Size(550, 60);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Location = new Point(25, 150);

            // Subtitle label (animated ellipsis)
            this.lblSubtitle.Text = baseSubtitle + "...";
            this.lblSubtitle.Font = new Font("Segoe UI", 13, FontStyle.Italic);
            this.lblSubtitle.ForeColor = Color.FromArgb(44, 62, 80);
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.Size = new Size(550, 30);
            this.lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblSubtitle.Location = new Point(25, 220);

            // Modern progress bar (thicker, rounded look)
            this.progressBar.Size = new Size(500, 18);
            this.progressBar.Location = new Point(50, 270);
            this.progressBar.Style = ProgressBarStyle.Continuous;
            this.progressBar.ForeColor = Color.FromArgb(41, 128, 185);
            this.progressBar.BackColor = Color.FromArgb(236, 240, 241);
            this.progressBar.Maximum = 100;

            // Version label
            this.lblVersion.Text = "Version 1.0.0";
            this.lblVersion.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            this.lblVersion.ForeColor = Color.Gray;
            this.lblVersion.AutoSize = false;
            this.lblVersion.Size = new Size(550, 20);
            this.lblVersion.TextAlign = ContentAlignment.MiddleCenter;
            this.lblVersion.Location = new Point(25, 350);

            // Add controls to content panel
            this.contentPanel.Controls.Add(this.logoPictureBox);
            this.contentPanel.Controls.Add(this.lblTitle);
            this.contentPanel.Controls.Add(this.lblSubtitle);
            this.contentPanel.Controls.Add(this.progressBar);
            this.contentPanel.Controls.Add(this.lblVersion);

            // Add content panel to main panel
            this.mainPanel.Controls.Add(this.contentPanel);
            this.Controls.Add(this.mainPanel);

            // Timer settings for smooth progress and animation
            this.timer.Interval = 50; // Update every 50ms
            this.timer.Tick += Timer_Tick;
            this.timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            progressValue += 2;
            if (progressValue <= 100)
            {
                progressBar.Value = progressValue;
                // Animate ellipsis
                ellipsisState = (ellipsisState + 1) % 4;
                lblSubtitle.Text = baseSubtitle + new string('.', ellipsisState);
            }
            else
            {
                this.timer.Stop();
                this.DialogResult = DialogResult.OK;
            }
        }

        // Helper for rounded rectangles
        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            // Top left arc
            path.AddArc(arc, 180, 90);
            // Top right arc
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            // Bottom right arc
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            // Bottom left arc
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
} 