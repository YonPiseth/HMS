using System;
using System.Windows.Forms;
using System.Drawing;

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
            this.Size = new System.Drawing.Size(450, 480); // Adjusted size
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White; // Set form background color

            // Main layout panel
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainLayout.RowCount = 6; // Number of rows for controls + buttons
            for (int i = 0; i < 5; i++) // Set height for control rows
            {
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            }
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Buttons row
            mainLayout.Padding = new Padding(15);
            mainLayout.AutoScroll = true;

            // Supplier Name
            Label lblSupplierName = new Label { Text = "Supplier Name:" };
            UIHelper.StyleLabel(lblSupplierName);
            this.txtSupplierName.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(this.txtSupplierName);

            // Contact Person
            Label lblContactPerson = new Label { Text = "Contact Person:" };
            UIHelper.StyleLabel(lblContactPerson);
            this.txtContactPerson.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(this.txtContactPerson);

            // Email
            Label lblEmail = new Label { Text = "Email:" };
            UIHelper.StyleLabel(lblEmail);
            this.txtEmail.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(this.txtEmail);

            // Phone
            Label lblPhone = new Label { Text = "Phone:" };
            UIHelper.StyleLabel(lblPhone);
            this.txtPhone.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(this.txtPhone);

            // Address
            Label lblAddress = new Label { Text = "Address:" };
            UIHelper.StyleLabel(lblAddress);
            this.txtAddress.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(this.txtAddress);
            this.txtAddress.Multiline = true;
            this.txtAddress.Height = 60; // Allow multiple lines
            mainLayout.RowStyles[4] = new RowStyle(SizeType.Absolute, 70); // Adjust row height for multiline textbox

            // Buttons Panel
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Padding = new Padding(0, 5, 0, 0);

            this.btnSave.Text = "Save";
            UIHelper.StyleButton(this.btnSave); // Apply button styling
            this.btnSave.Width = 100; // Adjusted width for form buttons
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            this.btnCancel.Text = "Cancel";
            UIHelper.StyleButton(this.btnCancel); // Apply button styling
            this.btnCancel.Width = 100; // Adjusted width for form buttons
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            buttonPanel.Controls.Add(this.btnCancel);
            buttonPanel.Controls.Add(this.btnSave);

            // Add controls to main layout
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
            mainLayout.SetRowSpan(this.txtAddress, 2); // Span 2 rows for multiline textbox

            mainLayout.Controls.Add(buttonPanel, 0, 5); // Buttons at the bottom
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);
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