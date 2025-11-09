using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using HMS;

namespace HMS
{
    public partial class PatientRegistrationForm : Form
    {
        private bool isEditMode = false;
        private int patientId = 0;

        public PatientRegistrationForm(int patientId = 0)
        {
            this.patientId = patientId;
            this.isEditMode = patientId > 0;

            InitializeComponent();
            SetupValidation();
            SetupToolTips();
            if (isEditMode)
            {
                LoadPatientData();
            }
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
            using (Microsoft.Data.SqlClient.SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM tblPatient WHERE PatientID = @PatientID";
                using (Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PatientID", patientId);
                    con.Open();
                    Microsoft.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader();
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

                using (Microsoft.Data.SqlClient.SqlConnection con = DatabaseHelper.GetConnection())
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

                using (Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con))
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