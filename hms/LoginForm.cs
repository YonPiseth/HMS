using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing; // Added for Color and Padding

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
            this.Size = new System.Drawing.Size(450, 350); // Adjusted size for better layout
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White; // Use Color from System.Drawing

            // Main layout panel
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainLayout.RowCount = 4; // Title, Username, Password, Buttons
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Title row
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Username row
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Password row
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Buttons row
            mainLayout.Padding = new Padding(30); // Add some padding around the content
            mainLayout.AutoScroll = true; // Enable scrolling if content overflows

            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "Welcome to HMS";
            UIHelper.StyleLabelTitle(lblTitle); // Apply title style from UIHelper
            mainLayout.Controls.Add(lblTitle, 0, 0);
            mainLayout.SetColumnSpan(lblTitle, 2);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter; // Center text in the title area

            // Username
            Label lblUsername = new Label { Text = "Username:" };
            UIHelper.StyleLabel(lblUsername); // Apply label style
            this.txtUsername.Dock = DockStyle.Fill; // Fill the cell
            UIHelper.StyleTextBox(this.txtUsername); // Apply textbox style
            mainLayout.Controls.Add(lblUsername, 0, 1);
            mainLayout.Controls.Add(this.txtUsername, 1, 1);

            // Password
            Label lblPassword = new Label { Text = "Password:" };
            UIHelper.StyleLabel(lblPassword); // Apply label style
            this.txtPassword.Dock = DockStyle.Fill; // Fill the cell
            UIHelper.StyleTextBox(this.txtPassword); // Apply textbox style
            this.txtPassword.PasswordChar = '*'; // Keep password masking
            mainLayout.Controls.Add(lblPassword, 0, 2);
            mainLayout.Controls.Add(this.txtPassword, 1, 2);

            // Buttons Panel
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft; // Align buttons to the right
            buttonPanel.Padding = new Padding(0, 10, 0, 0); // Padding from top

            this.btnCancel.Text = "Cancel";
            UIHelper.StyleButton(this.btnCancel); // Apply button style
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            buttonPanel.Controls.Add(this.btnCancel);

            this.btnLogin.Text = "Login";
            UIHelper.StyleButton(this.btnLogin); // Apply button style
            this.btnLogin.Click += new EventHandler(this.btnLogin_Click);
            buttonPanel.Controls.Add(this.btnLogin);
            
            mainLayout.Controls.Add(buttonPanel, 0, 3);
            mainLayout.SetColumnSpan(buttonPanel, 2);

            // Add main layout to form
            this.Controls.Add(mainLayout);
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