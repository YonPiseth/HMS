using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing; // Added for Color and Padding

namespace HMS
{
    public partial class LoginForm : Form
    {
        private Button btnLogin;
        private Button btnCancel;
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";
        public string UserRole { get; private set; }
        public int UserID { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
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