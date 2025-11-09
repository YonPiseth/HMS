using System;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.IO;
using HMS.Models;
using HMS.Helpers;

namespace HMS
{
    public partial class PatientForm : Form
    {
        private int _patientId;
        private Patient _patient;

        public PatientForm(int patientId = 0)
        {
            _patientId = patientId;
            _patient = new Patient();
            InitializeComponent();
            SetupUI();
            UIHelper.ApplyModernTheme(this);
            picPhoto.Image = null;
            this.Load += PatientForm_Load;
        }

        private void SetupUI()
        {
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainLayout.RowCount = 14;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            mainLayout.Padding = new Padding(20);
            mainLayout.AutoScroll = true;

            FlowLayoutPanel photoSectionPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, Padding = new Padding(0), Anchor = AnchorStyles.None };
            photoSectionPanel.Controls.Add(this.picPhoto);
            photoSectionPanel.Controls.Add(this.btnUploadPhoto);
            photoSectionPanel.BackColor = Color.Transparent;

            this.picPhoto.Width = 100;
            this.picPhoto.Height = 100;
            this.picPhoto.SizeMode = PictureBoxSizeMode.Zoom;
            this.picPhoto.BorderStyle = BorderStyle.FixedSingle;
            this.picPhoto.Margin = new Padding(0, 0, 0, 5);

            this.btnUploadPhoto.Text = "Upload Photo";
            this.btnUploadPhoto.Width = 100;
            this.btnUploadPhoto.Height = 30;
            this.btnUploadPhoto.Click += new EventHandler(btnUploadPhoto_Click);

            mainLayout.Controls.Add(photoSectionPanel, 0, 0);
            mainLayout.SetColumnSpan(photoSectionPanel, 2);
            photoSectionPanel.Anchor = AnchorStyles.None;

            Action<string, Control, int> addRow = (labelText, control, row) =>
            {
                Label lbl = new Label { Text = labelText, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left };
                UIHelper.StyleModernLabel(lbl);
                mainLayout.Controls.Add(lbl, 0, row);
                control.Dock = DockStyle.Fill;
                mainLayout.Controls.Add(control, 1, row);
            };

            addRow("First Name:", txtFirstName, 1);
            addRow("Last Name:", txtLastName, 2);
            addRow("Date of Birth:", dtpDateOfBirth, 3);
            this.dtpDateOfBirth.Format = DateTimePickerFormat.Short;
            addRow("Gender:", cmbGender, 4);
            this.cmbGender.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbGender.Items.AddRange(new object[] { "Male", "Female", "Other" });
            addRow("Blood Type:", cmbBloodType, 5);
            this.cmbBloodType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbBloodType.Items.AddRange(new object[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" });
            addRow("Contact Number:", txtContactNumber, 6);
            addRow("Email:", txtEmail, 7);
            addRow("Address:", txtAddress, 8);
            addRow("Insurance Number:", txtInsuranceNumber, 9);
            addRow("Patient's Family:", txtFamily, 10);
            addRow("Status:", cmbStatus, 11);
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new object[] { "Active", "Inactive", "Discharged" });
            addRow("Room Number:", txtRoomNumber, 12);

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Padding = new Padding(0, 10, 0, 0);
            buttonPanel.BackColor = Color.Transparent;

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Width = 100;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            buttonPanel.Controls.Add(this.btnCancel);

            this.btnSave.Text = "Save";
            this.btnSave.Width = 100;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            buttonPanel.Controls.Add(this.btnSave);

            mainLayout.Controls.Add(buttonPanel, 0, 13);
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);

            UIHelper.StyleModernButton(btnCancel);
            UIHelper.StyleModernButton(btnSave);
            UIHelper.StyleModernTextBox(txtFirstName);
            UIHelper.StyleModernTextBox(txtLastName);
            UIHelper.StyleModernTextBox(txtContactNumber);
            UIHelper.StyleModernTextBox(txtEmail);
            UIHelper.StyleModernTextBox(txtAddress);
            UIHelper.StyleModernTextBox(txtInsuranceNumber);
            UIHelper.StyleModernTextBox(txtFamily);
            UIHelper.StyleModernTextBox(txtRoomNumber);
            UIHelper.StyleModernPanel(mainLayout);
            UIHelper.StyleModernPanel(photoSectionPanel);
            UIHelper.StyleModernPanel(buttonPanel);
        }

        private void PatientForm_Load(object sender, EventArgs e)
        {
            if (_patientId > 0)
            {
                LoadRoomNumberForPatient(_patientId);
            }
        }

        private void LoadRoomNumberForPatient(int patientId)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = "SELECT RoomNumber FROM tblRoom WHERE PatientID = @PatientID";
                using (Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PatientID", patientId);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        txtRoomNumber.Text = result.ToString();
                    }
                    else
                    {
                        txtRoomNumber.Text = "";
                    }
                }
            }
        }

        // Populate Patient object from form fields
        private void PopulatePatientFromForm()
        {
            _patient.FirstName = txtFirstName.Text;
            _patient.LastName = txtLastName.Text;
            _patient.Email = txtEmail.Text;
            _patient.Phone = txtContactNumber.Text;
            _patient.Address = txtAddress.Text;
            _patient.DateOfBirth = dtpDateOfBirth.Value;
            _patient.Gender = cmbGender.Text;
            _patient.BloodType = cmbBloodType.Text;
            _patient.InsuranceNumber = txtInsuranceNumber.Text;
            _patient.PatientFamily = txtFamily.Text;
            _patient.Status = cmbStatus.Text;
            _patient.ProfilePhoto = ImageHelper.ImageToByteArray(picPhoto.Image);
            _patient.PatientID = _patientId;
        }

        // Populate form fields from Patient object
        private void PopulateFormFromPatient(Patient patient)
        {
            if (patient == null) return;

            txtFirstName.Text = patient.FirstName;
            txtLastName.Text = patient.LastName;
            txtEmail.Text = patient.Email;
            txtContactNumber.Text = patient.Phone;
            txtAddress.Text = patient.Address;
            dtpDateOfBirth.Value = patient.DateOfBirth;
            cmbGender.Text = patient.Gender;
            cmbBloodType.Text = patient.BloodType;
            txtInsuranceNumber.Text = patient.InsuranceNumber;
            txtFamily.Text = patient.PatientFamily;
            cmbStatus.Text = patient.Status;
            
            if (patient.ProfilePhoto != null && patient.ProfilePhoto.Length > 0)
            {
                picPhoto.Image = ImageHelper.ByteArrayToImage(patient.ProfilePhoto);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Populate Patient object from form
            PopulatePatientFromForm();

            // Validate using Patient class validation
            if (!_patient.Validate())
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                con.Open();
                Microsoft.Data.SqlClient.SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    // 1. Update/Insert into tblPatient
                    string patientQuery;
                    Microsoft.Data.SqlClient.SqlCommand patientCmd;

                    if (_patientId == 0) // New patient
                    {
                        patientQuery = @"INSERT INTO tblPatient (FirstName, LastName, DateOfBirth, Gender, BloodType, ContactNumber, 
                                    Email, Address, InsuranceNumber, PatientFamily, Status, ProfilePhoto, IsDeleted) 
                                       VALUES (@FirstName, @LastName, @DateOfBirth, @Gender, @BloodType, @ContactNumber, @Email, @Address, 
                                       @InsuranceNumber, @Family, @Status, @ProfilePhoto, 0);
                                       SELECT SCOPE_IDENTITY();";
                        patientCmd = new Microsoft.Data.SqlClient.SqlCommand(patientQuery, con, transaction);
                    }
                    else // Existing patient
                    {
                        patientQuery = @"UPDATE tblPatient SET FirstName=@FirstName, LastName=@LastName, DateOfBirth=@DateOfBirth, Gender=@Gender,
                         BloodType=@BloodType, ContactNumber=@ContactNumber, Email=@Email, Address=@Address, InsuranceNumber=@InsuranceNumber, PatientFamily=@Family,
                          Status=@Status, ProfilePhoto=@ProfilePhoto WHERE PatientID=@PatientID";
                        patientCmd = new Microsoft.Data.SqlClient.SqlCommand(patientQuery, con, transaction);
                        patientCmd.Parameters.AddWithValue("@PatientID", _patientId);
                    }

                    // Use Patient object properties
                    patientCmd.Parameters.AddWithValue("@FirstName", _patient.FirstName);
                    patientCmd.Parameters.AddWithValue("@LastName", _patient.LastName);
                    patientCmd.Parameters.AddWithValue("@DateOfBirth", _patient.DateOfBirth);
                    patientCmd.Parameters.AddWithValue("@Gender", _patient.Gender);
                    patientCmd.Parameters.AddWithValue("@BloodType", _patient.BloodType);
                    patientCmd.Parameters.AddWithValue("@ContactNumber", _patient.Phone);
                    patientCmd.Parameters.AddWithValue("@Email", _patient.Email);
                    patientCmd.Parameters.AddWithValue("@Address", _patient.Address);
                    patientCmd.Parameters.AddWithValue("@InsuranceNumber", _patient.InsuranceNumber);
                    patientCmd.Parameters.AddWithValue("@Family", _patient.PatientFamily);
                    patientCmd.Parameters.AddWithValue("@Status", _patient.Status);
                    patientCmd.Parameters.Add("@ProfilePhoto", SqlDbType.VarBinary).Value = (object)_patient.ProfilePhoto ?? DBNull.Value;

                    if (_patientId == 0)
                    {
                        _patientId = Convert.ToInt32(patientCmd.ExecuteScalar()); // Get new PatientID
                    }
                    else
                    {
                        patientCmd.ExecuteNonQuery();
                    }

                    // 2. Handle Room Number update (tblRoom)
                    string newRoomNumber = txtRoomNumber.Text.Trim();

                    // First, unassign the patient from their current room if they have one
                    // This is crucial to prevent a patient from being assigned to multiple rooms conceptually,
                    // or to allow a room to become free if the patient moves/leaves.
                    Microsoft.Data.SqlClient.SqlCommand unassignOldRoomCmd = new Microsoft.Data.SqlClient.SqlCommand("UPDATE tblRoom SET PatientID = NULL WHERE PatientID = @PatientID", con, transaction);
                    unassignOldRoomCmd.Parameters.AddWithValue("@PatientID", _patientId);
                    unassignOldRoomCmd.ExecuteNonQuery();

                    if (!string.IsNullOrEmpty(newRoomNumber))
                    {
                        // Check if the new room number exists
                        Microsoft.Data.SqlClient.SqlCommand checkRoomCmd = new Microsoft.Data.SqlClient.SqlCommand("SELECT RoomID, PatientID FROM tblRoom WHERE RoomNumber = @RoomNumber", con, transaction);
                        checkRoomCmd.Parameters.AddWithValue("@RoomNumber", newRoomNumber);
                        Microsoft.Data.SqlClient.SqlDataReader reader = checkRoomCmd.ExecuteReader();

                        int targetRoomId = -1;
                        int? existingPatientIdInRoom = null;

                        if (reader.Read())
                        {
                            targetRoomId = reader.GetInt32(0);
                            if (!reader.IsDBNull(1))
                            {
                                existingPatientIdInRoom = reader.GetInt32(1);
                            }
                        }
                        reader.Close();

                        if (targetRoomId != -1) // Room exists
                        {
                            if (existingPatientIdInRoom == null || existingPatientIdInRoom == _patientId) // Room is free or already assigned to this patient
                            {
                                Microsoft.Data.SqlClient.SqlCommand assignRoomCmd = new Microsoft.Data.SqlClient.SqlCommand("UPDATE tblRoom SET PatientID = @PatientID WHERE RoomID = @RoomID", con, transaction);
                                assignRoomCmd.Parameters.AddWithValue("@PatientID", _patientId);
                                assignRoomCmd.Parameters.AddWithValue("@RoomID", targetRoomId);
                                assignRoomCmd.ExecuteNonQuery();
                            }
                            else // Room is occupied by another patient
                            {
                                throw new InvalidOperationException($"Room {newRoomNumber} is already occupied by another patient.");
                            }
                        }
                        else // Room does not exist, create it
                        {
                            Microsoft.Data.SqlClient.SqlCommand createRoomCmd = new Microsoft.Data.SqlClient.SqlCommand("INSERT INTO tblRoom (RoomNumber, RoomType, Status, PatientID) VALUES (@RoomNumber, 'Standard', 'Occupied', @PatientID); SELECT SCOPE_IDENTITY();", con, transaction);
                            createRoomCmd.Parameters.AddWithValue("@RoomNumber", newRoomNumber);
                            createRoomCmd.Parameters.AddWithValue("@PatientID", _patientId);
                            createRoomCmd.ExecuteNonQuery(); // Execute, no need for RoomID back now
                        }
                    }

                    transaction.Commit();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (InvalidOperationException ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error assigning room: " + ex.Message, "Room Assignment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error saving patient data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnUploadPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        picPhoto.Image = Image.FromFile(ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        picPhoto.Image = null;
                    }
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