using System;
using System.Windows.Forms;
using System.Drawing;

namespace HMS
{
    public partial class SplashForm : Form
    {
        private System.Windows.Forms.Timer timer;
        private Label lblTitle;
        private Label lblSubtitle;
        private ProgressBar progressBar;

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

            // Form settings
            this.Text = "Hospital Management System";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;

            // Title label
            this.lblTitle.Text = "Hospital Management System";
            this.lblTitle.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(24, 33, 54);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTitle.Dock = DockStyle.Top;
            this.lblTitle.Height = 100;

            // Subtitle label
            this.lblSubtitle.Text = "Loading...";
            this.lblSubtitle.Font = new Font("Segoe UI", 12);
            this.lblSubtitle.ForeColor = Color.FromArgb(24, 33, 54);
            this.lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblSubtitle.Dock = DockStyle.Top;
            this.lblSubtitle.Height = 50;

            // Progress bar
            this.progressBar.Style = ProgressBarStyle.Marquee;
            this.progressBar.MarqueeAnimationSpeed = 30;
            this.progressBar.Dock = DockStyle.Bottom;
            this.progressBar.Height = 20;

            // Add controls to form
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.progressBar);

            // Timer settings
            this.timer.Interval = 5000; // 5 seconds
            this.timer.Tick += Timer_Tick;
            this.timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.timer.Stop();
            this.Close();
        }
    }
} 