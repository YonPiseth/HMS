using System;
using System.Windows.Forms;
using System.Drawing;
using HMS.Models;

namespace HMS
{
    public partial class SupplierForm : Form
    {
        private Supplier _supplier;

        public SupplierForm()
        {
            _supplier = new Supplier();
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainLayout.RowCount = 6;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            mainLayout.Padding = new Padding(15);
            mainLayout.AutoScroll = true;

            Label lblSupplierName = new Label { Text = "Supplier Name:" };
            UIHelper.StyleLabel(lblSupplierName);
            this.txtSupplierName.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(this.txtSupplierName);

            Label lblContactPerson = new Label { Text = "Contact Person:" };
            UIHelper.StyleLabel(lblContactPerson);
            this.txtContactPerson.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(this.txtContactPerson);

            Label lblEmail = new Label { Text = "Email:" };
            UIHelper.StyleLabel(lblEmail);
            this.txtEmail.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(this.txtEmail);

            Label lblPhone = new Label { Text = "Phone:" };
            UIHelper.StyleLabel(lblPhone);
            this.txtPhone.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(this.txtPhone);

            Label lblAddress = new Label { Text = "Address:" };
            UIHelper.StyleLabel(lblAddress);
            this.txtAddress.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(this.txtAddress);
            this.txtAddress.Multiline = true;
            this.txtAddress.Height = 60;

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Padding = new Padding(0, 5, 0, 0);

            this.btnSave.Text = "Save";
            UIHelper.StyleButton(this.btnSave);
            this.btnSave.Width = 100;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            this.btnCancel.Text = "Cancel";
            UIHelper.StyleButton(this.btnCancel);
            this.btnCancel.Width = 100;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            buttonPanel.Controls.Add(this.btnCancel);
            buttonPanel.Controls.Add(this.btnSave);

            mainLayout.Controls.Add(lblSupplierName, 0, 0);
            mainLayout.Controls.Add(this.txtSupplierName, 1, 0);

            mainLayout.Controls.Add(lblContactPerson, 0, 1);
            mainLayout.Controls.Add(this.txtContactPerson, 1, 1);

            mainLayout.Controls.Add(lblEmail, 0, 2);
            mainLayout.Controls.Add(this.txtEmail, 1, 2);

            mainLayout.Controls.Add(lblPhone, 0, 3);
            mainLayout.Controls.Add(this.txtPhone, 1, 3);

            mainLayout.Controls.Add(lblAddress, 0, 4);
            mainLayout.Controls.Add(this.txtAddress, 1, 4);
            mainLayout.SetRowSpan(this.txtAddress, 2);

            mainLayout.Controls.Add(buttonPanel, 0, 5);
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);
        }

        private void PopulateSupplierFromForm()
        {
            _supplier.SupplierName = txtSupplierName.Text;
            _supplier.ContactPerson = txtContactPerson.Text;
            _supplier.Email = txtEmail.Text;
            _supplier.Phone = txtPhone.Text;
            _supplier.Address = txtAddress.Text;
        }

        private void PopulateFormFromSupplier(Supplier supplier)
        {
            if (supplier == null) return;

            txtSupplierName.Text = supplier.SupplierName;
            txtContactPerson.Text = supplier.ContactPerson;
            txtEmail.Text = supplier.Email;
            txtPhone.Text = supplier.Phone;
            txtAddress.Text = supplier.Address;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Populate Supplier object from form
            PopulateSupplierFromForm();

            // Validate using Supplier class validation
            if (!_supplier.Validate())
            {
                MessageBox.Show("Please fill in all required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Additional validation for email format
            if (!IsValidEmail(_supplier.Email))
            {
                MessageBox.Show("Please enter a valid email address.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Additional validation for phone format
            if (!IsValidPhone(_supplier.Phone))
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