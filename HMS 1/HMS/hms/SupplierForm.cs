using System;
using System.Windows.Forms;

namespace HMS
{
    public partial class SupplierForm : Form
    {
        public TextBox txtSupplierName;
        public TextBox txtContactPerson;
        public TextBox txtEmail;
        public TextBox txtPhone;
        public TextBox txtAddress;
        private Button btnSave;
        private Button btnCancel;

        public SupplierForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtSupplierName = new TextBox();
            this.txtContactPerson = new TextBox();
            this.txtEmail = new TextBox();
            this.txtPhone = new TextBox();
            this.txtAddress = new TextBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            // Form settings
            this.Text = "Supplier Information";
            this.Size = new System.Drawing.Size(400, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Supplier Name
            Label lblSupplierName = new Label();
            lblSupplierName.Text = "Supplier Name:";
            lblSupplierName.Location = new System.Drawing.Point(20, 20);
            lblSupplierName.Size = new System.Drawing.Size(120, 20);

            this.txtSupplierName.Location = new System.Drawing.Point(150, 20);
            this.txtSupplierName.Size = new System.Drawing.Size(200, 27);

            // Contact Person
            Label lblContactPerson = new Label();
            lblContactPerson.Text = "Contact Person:";
            lblContactPerson.Location = new System.Drawing.Point(20, 60);
            lblContactPerson.Size = new System.Drawing.Size(120, 20);

            this.txtContactPerson.Location = new System.Drawing.Point(150, 60);
            this.txtContactPerson.Size = new System.Drawing.Size(200, 27);

            // Email
            Label lblEmail = new Label();
            lblEmail.Text = "Email:";
            lblEmail.Location = new System.Drawing.Point(20, 100);
            lblEmail.Size = new System.Drawing.Size(120, 20);

            this.txtEmail.Location = new System.Drawing.Point(150, 100);
            this.txtEmail.Size = new System.Drawing.Size(200, 27);

            // Phone
            Label lblPhone = new Label();
            lblPhone.Text = "Phone:";
            lblPhone.Location = new System.Drawing.Point(20, 140);
            lblPhone.Size = new System.Drawing.Size(120, 20);

            this.txtPhone.Location = new System.Drawing.Point(150, 140);
            this.txtPhone.Size = new System.Drawing.Size(200, 27);

            // Address
            Label lblAddress = new Label();
            lblAddress.Text = "Address:";
            lblAddress.Location = new System.Drawing.Point(20, 180);
            lblAddress.Size = new System.Drawing.Size(120, 20);

            this.txtAddress.Location = new System.Drawing.Point(150, 180);
            this.txtAddress.Size = new System.Drawing.Size(200, 60);
            this.txtAddress.Multiline = true;

            // Save Button
            this.btnSave.Text = "Save";
            this.btnSave.Location = new System.Drawing.Point(150, 280);
            this.btnSave.Size = new System.Drawing.Size(90, 35);
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            // Cancel Button
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(260, 280);
            this.btnCancel.Size = new System.Drawing.Size(90, 35);
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(220, 220, 220);
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // Add controls
            this.Controls.Add(lblSupplierName);
            this.Controls.Add(this.txtSupplierName);
            this.Controls.Add(lblContactPerson);
            this.Controls.Add(this.txtContactPerson);
            this.Controls.Add(lblEmail);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(lblPhone);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(lblAddress);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSupplierName.Text) || 
                string.IsNullOrWhiteSpace(txtContactPerson.Text) || 
                string.IsNullOrWhiteSpace(txtEmail.Text) || 
                string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsValidPhone(txtPhone.Text))
            {
                MessageBox.Show("Please enter a valid phone number.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^[0-9\-\+\(\)\s]{10,15}$");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
} 