using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Configuration;

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
        private string connectionString;
        private bool isEditMode = false;
        private int patientId = 0;

        public PatientRegistrationForm(int patientId = 0)
        {
            this.patientId = patientId;
            this.isEditMode = patientId > 0;

            // Retrieve connection string from App.config
            connectionString = ConfigurationManager.ConnectionStrings["HMSConnection"].ConnectionString;

            InitializeComponent();
            SetupValidation();
            SetupToolTips();
            if (isEditMode)
            {
                LoadPatientData();
            }
        }

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
            UIHelper.StyleLabelTitle(lblTitle);
            mainLayout.Controls.Add(lblTitle, 0, 0);
            mainLayout.SetColumnSpan(lblTitle, 2);

            // Helper to add a row of label and control
            Action<string, Control, int> addRow = (labelText, control, row) =>
            {
                Label lbl = new Label { Text = labelText, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left };
                UIHelper.StyleLabel(lbl);
                mainLayout.Controls.Add(lbl, 0, row);
                control.Dock = DockStyle.Fill;
                mainLayout.Controls.Add(control, 1, row);
            };

            // First Name
            addRow("First Name:", txtFirstName, 1);
            UIHelper.StyleTextBox(this.txtFirstName);

            // Last Name
            addRow("Last Name:", txtLastName, 2);
            UIHelper.StyleTextBox(this.txtLastName);

            // Date of Birth
            addRow("Date of Birth:", dtpDOB, 3);
            this.dtpDOB.Format = DateTimePickerFormat.Short;
            this.dtpDOB.Font = new Font("Segoe UI", 10);
            this.dtpDOB.MaxDate = DateTime.Today;

            // Gender
            addRow("Gender:", cmbGender, 4);
            UIHelper.StyleComboBox(this.cmbGender);
            this.cmbGender.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbGender.Items.AddRange(new string[] { "Male", "Female", "Other" });

            // Contact Number (Phone)
            addRow("Phone:", txtContactNumber, 5);
            UIHelper.StyleTextBox(this.txtContactNumber);

            // Email
            addRow("Email:", txtEmail, 6);
            UIHelper.StyleTextBox(this.txtEmail);

            // Address
            addRow("Address:", txtAddress, 7);
            UIHelper.StyleTextBox(this.txtAddress);
            this.txtAddress.Multiline = true;
            this.txtAddress.Height = 80; 
            mainLayout.RowStyles[7] = new RowStyle(SizeType.Absolute, 80); // Adjust height for address row
            mainLayout.SetRowSpan(this.txtAddress, 2); // Span two rows for address

            // Blood Type
            addRow("Blood Type:", cmbBloodType, 8); // This row index will be affected by previous row span
            UIHelper.StyleComboBox(this.cmbBloodType);
            this.cmbBloodType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbBloodType.Items.AddRange(new string[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" });

            // Insurance Number
            addRow("Insurance Number:", txtInsuranceNumber, 9);
            UIHelper.StyleTextBox(this.txtInsuranceNumber);

            // Medical History
            addRow("Medical History:", txtMedicalHistory, 10);
            UIHelper.StyleTextBox(this.txtMedicalHistory);
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
            UIHelper.StyleButton(this.btnCancel);
            this.btnCancel.Width = 100;
            this.btnCancel.Click += (s, e) => this.Close();
            buttonPanel.Controls.Add(this.btnCancel);

            this.btnClear.Text = "Clear";
            UIHelper.StyleButton(this.btnClear);
            this.btnClear.Width = 100;
            this.btnClear.Click += (s, e) => ClearForm();
            buttonPanel.Controls.Add(this.btnClear);

            this.btnSave.Text = "Save";
            UIHelper.StyleButton(this.btnSave);
            this.btnSave.Width = 100;
            this.btnSave.Click += BtnSave_Click;
            buttonPanel.Controls.Add(this.btnSave);

            mainLayout.Controls.Add(buttonPanel, 0, mainLayout.RowCount - 1); // Add buttons to the last row
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);
        }

        private void SetupToolTips()
        {
            toolTip.SetToolTip(txtFirstName, "Enter patient's first name");
            toolTip.SetToolTip(txtLastName, "Enter patient's last name");
            toolTip.SetToolTip(dtpDOB, "Select patient's date of birth");
            toolTip.SetToolTip(cmbGender, "Select patient's gender");
            toolTip.SetToolTip(txtContactNumber, "Enter patient's contact number (e.g., 123-456-7890)");
            toolTip.SetToolTip(txtEmail, "Enter patient's email address (e.g., example@domain.com)");
            toolTip.SetToolTip(txtAddress, "Enter patient's full address");
            toolTip.SetToolTip(cmbBloodType, "Select patient's blood type");
            toolTip.SetToolTip(txtInsuranceNumber, "Enter patient's insurance number");
            toolTip.SetToolTip(txtMedicalHistory, "Enter patient's medical history details");
        }

        private void SetupValidation()
        {
            // Add validation for required fields
            txtFirstName.Validating += (sender, e) => ValidateTextBox(txtFirstName, "First Name is required.", e);
            txtLastName.Validating += (sender, e) => ValidateTextBox(txtLastName, "Last Name is required.", e);
            txtContactNumber.Validating += (sender, e) => ValidatePhone(txtContactNumber, e);
            txtEmail.Validating += (sender, e) => ValidateEmail(txtEmail, e);
            txtAddress.Validating += (sender, e) => ValidateTextBox(txtAddress, "Address is required.", e);
            cmbGender.Validating += (sender, e) => ValidateComboBox(cmbGender, "Gender is required.", e);
            cmbBloodType.Validating += (sender, e) => ValidateComboBox(cmbBloodType, "Blood Type is required.", e);
        }

        private void ValidateTextBox(TextBox textBox, string errorMessage, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                errorProvider.SetError(textBox, errorMessage);
                    e.Cancel = true;
                }
                else
                {
                errorProvider.SetError(textBox, string.Empty);
                }
        }

        private void ValidateComboBox(ComboBox comboBox, string errorMessage, System.ComponentModel.CancelEventArgs e)
        {
            if (comboBox.SelectedIndex == -1)
                {
                errorProvider.SetError(comboBox, errorMessage);
                    e.Cancel = true;
                }
                else
                {
                errorProvider.SetError(comboBox, string.Empty);
            }
                }

        private void ValidatePhone(TextBox textBox, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                errorProvider.SetError(textBox, "Contact Number is required.");
                e.Cancel = true;
            }
            else if (!Regex.IsMatch(textBox.Text, @"^\d{3}-\d{3}-\d{4}$"))
                {
                errorProvider.SetError(textBox, "Invalid phone number format (e.g., 123-456-7890).");
                        e.Cancel = true;
                    }
                    else
                    {
                errorProvider.SetError(textBox, string.Empty);
                }
        }

        private void ValidateEmail(TextBox textBox, System.ComponentModel.CancelEventArgs e)
            {
            if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                errorProvider.SetError(textBox, "Email is required.");
                e.Cancel = true;
            }
            else if (!Regex.IsMatch(textBox.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                errorProvider.SetError(textBox, "Invalid email address format.");
                        e.Cancel = true;
                    }
                    else
                    {
                errorProvider.SetError(textBox, string.Empty);
                }
        }

        private void LoadPatientData()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM tblPatient WHERE PatientID = @PatientID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PatientID", patientId);
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            txtFirstName.Text = reader["FirstName"].ToString();
                            txtLastName.Text = reader["LastName"].ToString();
                            dtpDOB.Value = Convert.ToDateTime(reader["DateOfBirth"]);
                        cmbGender.SelectedItem = reader["Gender"].ToString();
                            txtContactNumber.Text = reader["ContactNumber"].ToString();
                            txtEmail.Text = reader["Email"].ToString();
                            txtAddress.Text = reader["Address"].ToString();
                        cmbBloodType.SelectedItem = reader["BloodType"].ToString();
                            txtInsuranceNumber.Text = reader["InsuranceNumber"].ToString();
                            txtMedicalHistory.Text = reader["MedicalHistory"].ToString();
                        }
                    }
            }
        }

        private void SavePatientData()
        {
            if (!ValidateChildren())
            {
                MessageBox.Show("Please correct the errors before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                con.Open();
                string query;
                if (isEditMode)
                {
                    query = "UPDATE tblPatient SET FirstName=@FirstName, LastName=@LastName, DateOfBirth=@DateOfBirth, Gender=@Gender, ContactNumber=@ContactNumber, Email=@Email, Address=@Address, BloodType=@BloodType, InsuranceNumber=@InsuranceNumber, MedicalHistory=@MedicalHistory, LastModifiedDate=@LastModifiedDate WHERE PatientID=@PatientID";
                }
                else
                {
                    query = "INSERT INTO tblPatient (FirstName, LastName, DateOfBirth, Gender, ContactNumber, Email, Address, BloodType, InsuranceNumber, MedicalHistory, CreatedDate, LastModifiedDate) VALUES (@FirstName, @LastName, @DateOfBirth, @Gender, @ContactNumber, @Email, @Address, @BloodType, @InsuranceNumber, @MedicalHistory, @CreatedDate, @LastModifiedDate)";
                }

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                    cmd.Parameters.AddWithValue("@DateOfBirth", dtpDOB.Value);
                    cmd.Parameters.AddWithValue("@Gender", cmbGender.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@ContactNumber", txtContactNumber.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@BloodType", cmbBloodType.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@InsuranceNumber", txtInsuranceNumber.Text);
                    cmd.Parameters.AddWithValue("@MedicalHistory", txtMedicalHistory.Text);
                    cmd.Parameters.AddWithValue("@LastModifiedDate", DateTime.Now);

                    if (!isEditMode)
                    {
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@PatientID", patientId);
                    }
                    cmd.ExecuteNonQuery();
                    MessageBox.Show(isEditMode ? "Patient data updated successfully!" : "Patient registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            }
        }

        private void ClearForm()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            dtpDOB.Value = DateTime.Today;
            cmbGender.SelectedIndex = -1;
            txtContactNumber.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            cmbBloodType.SelectedIndex = -1;
            txtInsuranceNumber.Clear();
            txtMedicalHistory.Clear();
            errorProvider.Clear(); // Clear any existing errors
        }

        private void BtnSave_Click(object sender, EventArgs e)
            {
                SavePatientData();
        }
    }
} 