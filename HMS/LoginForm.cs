using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;
using System.Configuration;
using HMS;
using HMS.UI;

namespace HMS
{
    public partial class LoginForm : Form
    {
        public string UserRole { get; private set; }
        public int UserID { get; private set; }

        public LoginForm()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing login form: {ex.Message}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            
            this.Load += LoginForm_Load;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            try
            {
                this.BackColor = UITheme.BackgroundWhite;
                this.FormBorderStyle = FormBorderStyle.None;
                this.Size = new Size(450, 550);
                
                // Add a custom title bar panel
                Panel titleBar = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = UITheme.PrimarySlate };
                Label lblClose = new Label { Text = "×", ForeColor = Color.White, Font = new Font("Segoe UI", 18), 
                                           AutoSize = true, Location = new Point(415, 5), Cursor = Cursors.Hand };
                lblClose.Click += (s, ev) => this.Close();
                titleBar.Controls.Add(lblClose);
                this.Controls.Add(titleBar);

                // Add Premium Card effect
                Panel card = UIHelper.CreateModernCard(UITheme.RadiusLG, true);
                card.Width = 380;
                card.Height = 420;
                card.Location = new Point((this.Width - card.Width) / 2, (this.Height - card.Height) / 2 + 20);
                this.Controls.Add(card);
                card.BringToFront();

                TableLayoutPanel cardLayout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 6, Padding = new Padding(20) };
                cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Logo
                cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Title
                cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70)); // Username
                cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70)); // Password
                cardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Login Button
                cardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Spacer
                card.Controls.Add(cardLayout);

                Label lblLogo = new Label { Text = "HMS PRO", Font = new Font("Segoe UI", 24, FontStyle.Bold), 
                                         ForeColor = UITheme.BrandPrimary, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
                Label lblWelcome = new Label { Text = "Sign in to continue", Font = UITheme.FontBodyLarge, 
                                            ForeColor = UITheme.TextSecondary, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
                
                cardLayout.Controls.Add(lblLogo, 0, 0);
                cardLayout.Controls.Add(lblWelcome, 0, 1);

                // Re-parent existing controls to the new modern layout
                Panel userPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 10) };
                Label lUser = new Label { Text = "Username", Font = UITheme.FontBodySmall, ForeColor = UITheme.TextSecondary, Dock = DockStyle.Top };
                txtUsername.Dock = DockStyle.Bottom;
                UIHelper.StyleModernTextBox(txtUsername);
                userPanel.Controls.Add(lUser);
                userPanel.Controls.Add(txtUsername);
                cardLayout.Controls.Add(userPanel, 0, 2);

                Panel passPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 10, 0, 10) };
                Label lPass = new Label { Text = "Password", Font = UITheme.FontBodySmall, ForeColor = UITheme.TextSecondary, Dock = DockStyle.Top };
                txtPassword.Dock = DockStyle.Bottom;
                UIHelper.StyleModernTextBox(txtPassword);
                passPanel.Controls.Add(lPass);
                passPanel.Controls.Add(txtPassword);
                cardLayout.Controls.Add(passPanel, 0, 3);

                btnLogin.Dock = DockStyle.Fill;
                UIHelper.StyleModernButton(btnLogin);
                btnLogin.Text = "LOG IN";
                cardLayout.Controls.Add(btnLogin, 0, 4);
                
                // Remove the old layout
                if (mainLayout != null) this.Controls.Remove(mainLayout);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Login Theme error: " + ex.Message);
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                try
                {
                    con.Open();
                    string query = "SELECT UserID, Role FROM tblUser WHERE Username = @Username AND Password = @Password AND IsActive = 1";
                    Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Text);

                    using (Microsoft.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            UserID = reader.GetInt32(0);
                            UserRole = reader.GetString(1);
                            
                            reader.Close();
                            string updateQuery = "UPDATE tblUser SET LastLoginDate = GETDATE() WHERE UserID = @UserID";
                            Microsoft.Data.SqlClient.SqlCommand updateCmd = new Microsoft.Data.SqlClient.SqlCommand(updateQuery, con);
                            updateCmd.Parameters.AddWithValue("@UserID", UserID);
                            updateCmd.ExecuteNonQuery();

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error during login: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
