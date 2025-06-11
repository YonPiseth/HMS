using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace HMS
{
    public class LoginForm : Form
    {
        public TextBox txtUsername;
        public TextBox txtPassword;
        private Button btnLogin;
        private Button btnCancel;
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";
        public string UserRole { get; private set; }
        public int UserID { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtUsername = new TextBox();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.btnCancel = new Button();

            // Form settings
            this.Text = "Hospital Management System - Login";
            this.Size = new System.Drawing.Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = System.Drawing.Color.White;

            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "Welcome to HMS";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.FromArgb(24, 33, 54);
            lblTitle.Location = new System.Drawing.Point(100, 20);
            lblTitle.Size = new System.Drawing.Size(200, 30);
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // Username
            Label lblUsername = new Label();
            lblUsername.Text = "Username:";
            lblUsername.Location = new System.Drawing.Point(50, 80);
            lblUsername.Size = new System.Drawing.Size(80, 20);
            lblUsername.Font = new System.Drawing.Font("Segoe UI", 10);
            this.txtUsername.Location = new System.Drawing.Point(140, 80);
            this.txtUsername.Size = new System.Drawing.Size(200, 27);
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 10);

            // Password
            Label lblPassword = new Label();
            lblPassword.Text = "Password:";
            lblPassword.Location = new System.Drawing.Point(50, 120);
            lblPassword.Size = new System.Drawing.Size(80, 20);
            lblPassword.Font = new System.Drawing.Font("Segoe UI", 10);
            this.txtPassword.Location = new System.Drawing.Point(140, 120);
            this.txtPassword.Size = new System.Drawing.Size(200, 27);
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 10);

            // Login Button
            this.btnLogin.Text = "Login";
            this.btnLogin.Location = new System.Drawing.Point(140, 170);
            this.btnLogin.Size = new System.Drawing.Size(90, 35);
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);

            // Cancel Button
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(250, 170);
            this.btnCancel.Size = new System.Drawing.Size(90, 35);
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(220, 220, 220);
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(24, 33, 54);
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                lblTitle,
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                btnLogin, btnCancel
            });
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT UserID, Role FROM tblUser WHERE Username = @Username AND Password = @Password AND IsActive = 1";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Username", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@Password", txtPassword.Text);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            UserID = reader.GetInt32(0);
                            UserRole = reader.GetString(1);
                            
                            // Update last login date
                            reader.Close();
                            string updateQuery = "UPDATE tblUser SET LastLoginDate = GETDATE() WHERE UserID = @UserID";
                            SqlCommand updateCmd = new SqlCommand(updateQuery, con);
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