using System;
using System.Windows.Forms;
using System.Drawing;

namespace HMS
{
    public partial class PatientRegistrationForm : Form
    {
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private DateTimePicker dtpDOB;
        private ComboBox cmbGender;
        private TextBox txtContactNumber;
        private TextBox txtEmail;
        private TextBox txtAddress;
        private ComboBox cmbBloodType;
        private TextBox txtInsuranceNumber;
        private TextBox txtMedicalHistory;
        private Button btnSave;
        private Button btnCancel;
        private Button btnClear;
        private ErrorProvider errorProvider;
        private ToolTip toolTip;

        private void InitializeComponent()
        {
            this.txtFirstName = new TextBox();
            this.txtLastName = new TextBox();
            this.dtpDOB = new DateTimePicker();
            this.cmbGender = new ComboBox();
            this.txtContactNumber = new TextBox();
            this.txtEmail = new TextBox();
            this.txtAddress = new TextBox();
            this.cmbBloodType = new ComboBox();
            this.txtInsuranceNumber = new TextBox();
            this.txtMedicalHistory = new TextBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.btnClear = new Button();

            // Form settings
            this.Text = isEditMode ? "Edit Patient" : "New Patient Registration";
            this.Size = new Size(800, 650); 
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Create error provider and tooltip
            errorProvider = new ErrorProvider();
            toolTip = new ToolTip();
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 500;
            toolTip.ReshowDelay = 500;
            toolTip.ShowAlways = true;

            // Main TableLayoutPanel
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            mainLayout.RowCount = 11; // Labels + TextBoxes/ComboBoxes + buttons

            // Set row heights for better spacing
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Title row
            for (int i = 1; i <= 9; i++)
            {
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Standard field rows
            }
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Buttons row
            mainLayout.Padding = new Padding(20); // More generous padding
            mainLayout.AutoScroll = true;
            UIHelper.ApplyPanelStyles(mainLayout);

            // Title Label
            Label lblTitle = new Label
            {
                Text = isEditMode ? "Edit Patient Information" : "New Patient Registration",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            UIHelper.StyleModernTitle(lblTitle);
            mainLayout.Controls.Add(lblTitle, 0, 0);
            mainLayout.SetColumnSpan(lblTitle, 2);

            // Helper to add a row of label and control
            Action<string, Control, int> addRow = (labelText, control, row) =>
            {
                Label lbl = new Label { Text = labelText, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left };
                UIHelper.StyleModernLabel(lbl);
                mainLayout.Controls.Add(lbl, 0, row);
                control.Dock = DockStyle.Fill;
                mainLayout.Controls.Add(control, 1, row);
            };

            // First Name
            addRow("First Name:", txtFirstName, 1);
            UIHelper.StyleModernTextBox(this.txtFirstName);

            // Last Name
            addRow("Last Name:", txtLastName, 2);
            UIHelper.StyleModernTextBox(this.txtLastName);

            // Date of Birth
            addRow("Date of Birth:", dtpDOB, 3);
            this.dtpDOB.Format = DateTimePickerFormat.Short;
            this.dtpDOB.Font = new Font("Segoe UI", 10);
            this.dtpDOB.MaxDate = DateTime.Today;

            // Gender
            addRow("Gender:", cmbGender, 4);
            UIHelper.StyleModernComboBox(this.cmbGender);
            this.cmbGender.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbGender.Items.AddRange(new string[] { "Male", "Female", "Other" });

            // Contact Number (Phone)
            addRow("Phone:", txtContactNumber, 5);
            UIHelper.StyleModernTextBox(this.txtContactNumber);

            // Email
            addRow("Email:", txtEmail, 6);
            UIHelper.StyleModernTextBox(this.txtEmail);

            // Address
            addRow("Address:", txtAddress, 7);
            UIHelper.StyleModernTextBox(this.txtAddress);
            this.txtAddress.Multiline = true;
            this.txtAddress.Height = 80; 
            mainLayout.RowStyles[7] = new RowStyle(SizeType.Absolute, 80); // Adjust height for address row
            mainLayout.SetRowSpan(this.txtAddress, 2); // Span two rows for address

            // Blood Type
            addRow("Blood Type:", cmbBloodType, 8); // This row index will be affected by previous row span
            UIHelper.StyleModernComboBox(this.cmbBloodType);
            this.cmbBloodType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbBloodType.Items.AddRange(new string[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" });

            // Insurance Number
            addRow("Insurance Number:", txtInsuranceNumber, 9);
            UIHelper.StyleModernTextBox(this.txtInsuranceNumber);

            // Medical History
            addRow("Medical History:", txtMedicalHistory, 10);
            UIHelper.StyleModernTextBox(this.txtMedicalHistory);
            this.txtMedicalHistory.Multiline = true;
            this.txtMedicalHistory.Height = 100; 
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Add new row style for Medical History
            mainLayout.SetRowSpan(this.txtMedicalHistory, 2); // Span two rows for medical history

            // Button Panel
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Padding = new Padding(0, 10, 0, 0);
            UIHelper.ApplyPanelStyles(buttonPanel);

            this.btnCancel.Text = "Cancel";
            UIHelper.StyleModernButton(this.btnCancel);
            this.btnCancel.Width = 100;
            this.btnCancel.Click += (s, e) => this.Close();
            buttonPanel.Controls.Add(this.btnCancel);

            this.btnClear.Text = "Clear";
            UIHelper.StyleModernButton(this.btnClear);
            this.btnClear.Width = 100;
            this.btnClear.Click += (s, e) => ClearForm();
            buttonPanel.Controls.Add(this.btnClear);

            this.btnSave.Text = "Save";
            UIHelper.StyleModernButton(this.btnSave);
            this.btnSave.Width = 100;
            this.btnSave.Click += BtnSave_Click;
            buttonPanel.Controls.Add(this.btnSave);

            mainLayout.Controls.Add(buttonPanel, 0, mainLayout.RowCount - 1); // Add buttons to the last row
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);
        }
    }
} 