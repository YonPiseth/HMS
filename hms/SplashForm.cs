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

            // Form settings
            this.Text = "Hospital Management System";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Main panel with gradient background
            this.mainPanel.Dock = DockStyle.Fill;
            UIHelper.ApplyPanelStyles(this.mainPanel);
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

            // Content panel with glass effect
            this.contentPanel.Size = new Size(600, 400);
            this.contentPanel.Location = new Point(
                (this.Width - this.contentPanel.Width) / 2,
                (this.Height - this.contentPanel.Height) / 2
            );
            UIHelper.ApplyPanelStyles(this.contentPanel);
            this.contentPanel.Paint += (s, e) =>
            {
                // Draw rounded rectangle with shadow
                using (GraphicsPath path = new GraphicsPath())
                {
                    int radius = 20;
                    Rectangle rect = new Rectangle(0, 0, this.contentPanel.Width - 1, this.contentPanel.Height - 1);
                    path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
                    path.AddArc(rect.Right - radius, rect.Y, radius, radius, 270, 90);
                    path.AddArc(rect.Right - radius, rect.Bottom - radius, radius, radius, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - radius, radius, radius, 90, 90);
                    path.CloseAllFigures();

                    this.contentPanel.Region = new Region(path);
                }
            };

            // Title label with modern font
            this.lblTitle.Text = "Hospital Management System";
            UIHelper.StyleLabelTitle(this.lblTitle);
            this.lblTitle.AutoSize = false;
            this.lblTitle.Size = new Size(550, 60);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Location = new Point(25, 40);

            // Subtitle label
            this.lblSubtitle.Text = "Loading...";
            UIHelper.StyleLabel(this.lblSubtitle);
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.Size = new Size(550, 30);
            this.lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblSubtitle.Location = new Point(25, 120);

            // Modern progress bar
            this.progressBar.Size = new Size(500, 6);
            this.progressBar.Location = new Point(50, 200);
            this.progressBar.Style = ProgressBarStyle.Continuous;
            this.progressBar.ForeColor = Color.FromArgb(41, 128, 185);
            this.progressBar.BackColor = Color.FromArgb(236, 240, 241);
            this.progressBar.Maximum = 100;

            // Add controls to content panel
            this.contentPanel.Controls.Add(this.lblTitle);
            this.contentPanel.Controls.Add(this.lblSubtitle);
            this.contentPanel.Controls.Add(this.progressBar);

            // Add content panel to main panel
            this.mainPanel.Controls.Add(this.contentPanel);
            this.Controls.Add(this.mainPanel);

            // Timer settings for smooth progress
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
                lblSubtitle.Text = $"Loading... {progressValue}%";
            }
            else
        {
            this.timer.Stop();
                this.DialogResult = DialogResult.OK;
            }
        }
    }
} 