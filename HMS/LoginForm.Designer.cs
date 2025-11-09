using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace HMS
{
    public partial class LoginForm : Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnCancel;

        private void InitializeComponent()
        {
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            btnLogin = new Button();
            btnCancel = new Button();
            mainLayout = new TableLayoutPanel();
            logoBox = new PictureBox();
            lblTitle = new Label();
            lblUsername = new Label();
            lblPassword = new Label();
            buttonPanel = new FlowLayoutPanel();
            mainLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)logoBox).BeginInit();
            buttonPanel.SuspendLayout();
            SuspendLayout();
            txtUsername.Dock = DockStyle.Fill;
            txtUsername.Location = new Point(163, 93);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(236, 27);
            txtUsername.TabIndex = 3;

            txtPassword.Dock = DockStyle.Fill;
            txtPassword.Location = new Point(163, 133);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(236, 27);
            txtPassword.TabIndex = 5;

            btnLogin.Location = new Point(207, 13);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(100, 40);
            btnLogin.TabIndex = 1;
            btnLogin.Text = "Login";
            btnLogin.Click += btnLogin_Click;

            btnCancel.Location = new Point(100, 13);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 40);
            btnCancel.TabIndex = 0;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;

            mainLayout.AutoScroll = true;
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainLayout.Controls.Add(logoBox, 0, 0);
            mainLayout.Controls.Add(lblTitle, 1, 0);
            mainLayout.Controls.Add(lblUsername, 0, 1);
            mainLayout.Controls.Add(txtUsername, 1, 1);
            mainLayout.Controls.Add(lblPassword, 0, 2);
            mainLayout.Controls.Add(txtPassword, 1, 2);
            mainLayout.Controls.Add(buttonPanel, 0, 3);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(0, 0);
            mainLayout.Name = "mainLayout";
            mainLayout.Padding = new Padding(30);
            mainLayout.RowCount = 4;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            mainLayout.Size = new Size(432, 303);
            mainLayout.TabIndex = 0;

            logoBox.Dock = DockStyle.Fill;
            logoBox.Location = new Point(30, 30);
            logoBox.Margin = new Padding(0, 0, 10, 0);
            logoBox.Name = "logoBox";
            logoBox.Size = new Size(120, 60);
            logoBox.SizeMode = PictureBoxSizeMode.Zoom;
            logoBox.TabIndex = 0;
            logoBox.TabStop = false;

            lblTitle.Location = new Point(163, 30);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(250, 40);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "Welcome to HMS";
            lblTitle.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(33, 150, 243);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;

            lblUsername.Location = new Point(33, 90);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(120, 23);
            lblUsername.TabIndex = 2;
            lblUsername.Text = "Username:";

            lblPassword.Location = new Point(33, 130);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(120, 23);
            lblPassword.TabIndex = 4;
            lblPassword.Text = "Password:";

            mainLayout.SetColumnSpan(buttonPanel, 2);
            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(btnLogin);
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Location = new Point(33, 173);
            buttonPanel.Name = "buttonPanel";
            buttonPanel.Padding = new Padding(0, 10, 0, 0);
            buttonPanel.Size = new Size(366, 97);
            buttonPanel.TabIndex = 6;

            BackColor = Color.FromArgb(250, 250, 250);
            ClientSize = new Size(500, 400);
            Controls.Add(mainLayout);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Hospital Management System - Login";
            Padding = new Padding(24);
            mainLayout.ResumeLayout(false);
            mainLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)logoBox).EndInit();
            buttonPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
        private TableLayoutPanel mainLayout;
        private PictureBox logoBox;
        private Label lblTitle;
        private Label lblUsername;
        private Label lblPassword;
        private FlowLayoutPanel buttonPanel;
    }
}
