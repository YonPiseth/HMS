using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;
using System.Configuration;
using HMS;

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
                UIHelper.ApplyModernTheme(this);
                
                if (btnLogin != null) 
                    UIHelper.StyleModernButton(btnLogin);
                
                if (btnCancel != null)
                {
                    btnCancel.FlatStyle = FlatStyle.Flat;
                    btnCancel.BackColor = UI.UITheme.BackgroundGray;
                    btnCancel.ForeColor = UI.UITheme.TextPrimary;
                    btnCancel.Font = UI.UITheme.FontButton;
                    btnCancel.FlatAppearance.BorderSize = 0;
                    btnCancel.Cursor = Cursors.Hand;
                    btnCancel.Height = 40;
                }
                
                if (lblTitle != null)
                {
                    UIHelper.StyleHeading(lblTitle, 2);
                    lblTitle.ForeColor = UI.UITheme.PrimaryBlue;
                }
                if (lblUsername != null) UIHelper.StyleModernLabel(lblUsername);
                if (lblPassword != null) UIHelper.StyleModernLabel(lblPassword);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Theme error: " + ex.Message);
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
