using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.IO;

namespace HMS
{
    public partial class PatientForm : Form
    {
        public TextBox txtFirstName;
        public TextBox txtLastName;
        public DateTimePicker dtpDateOfBirth;
        public ComboBox cmbGender;
        public ComboBox cmbBloodType;
        public TextBox txtContactNumber;
        public TextBox txtEmail;
        public TextBox txtAddress;
        public TextBox txtInsuranceNumber;
        public TextBox txtFamily;
        public ComboBox cmbStatus;
        public PictureBox picPhoto;
        public Button btnUploadPhoto;
        public TextBox txtRoomNumber;
        private Button btnSave;
        private Button btnCancel;
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";
        private int _patientId; // To store patient ID for updates, 0 for new patient

        public PatientForm(int patientId = 0)
        {
            _patientId = patientId;
            InitializeComponent();
            this.Load += PatientForm_Load;
        }

        private void PatientForm_Load(object sender, EventArgs e)
        {
            // Only load room number if it's an existing patient being updated
            if (_patientId > 0)
            {
                LoadRoomNumberForPatient(_patientId);
            }
        }

        private void LoadRoomNumberForPatient(int patientId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT RoomNumber FROM tblRoom WHERE PatientID = @PatientID";
                using (SqlCommand cmd = new SqlCommand(query, con))
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

        private void InitializeComponent()
        {
            this.txtFirstName = new TextBox();
            this.txtLastName = new TextBox();
            this.dtpDateOfBirth = new DateTimePicker();
            this.cmbGender = new ComboBox();
            this.cmbBloodType = new ComboBox();
            this.txtContactNumber = new TextBox();
            this.txtEmail = new TextBox();
            this.txtAddress = new TextBox();
            this.txtInsuranceNumber = new TextBox();
            this.txtFamily = new TextBox();
            this.cmbStatus = new ComboBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.picPhoto = new PictureBox();
            this.btnUploadPhoto = new Button();
            this.txtRoomNumber = new TextBox();

            // Form settings
            this.Text = "Patient Information";
            this.Size = new Size(500, 780); // Adjusted size for better field visibility
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Main layout panel
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainLayout.RowCount = 14; // Photo + 12 fields + Buttons
            // Adjust row heights for better spacing
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120)); // Photo row
            for (int i = 1; i <= 12; i++) 
            {
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Standard field rows
            }
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Buttons row
            mainLayout.Padding = new Padding(20); // More generous padding
            mainLayout.AutoScroll = true;
            UIHelper.ApplyPanelStyles(mainLayout); // Apply panel style to main layout

            // Profile photo section
            FlowLayoutPanel photoSectionPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, Padding = new Padding(0), Anchor = AnchorStyles.None };
            photoSectionPanel.Controls.Add(this.picPhoto);
            photoSectionPanel.Controls.Add(this.btnUploadPhoto);
            photoSectionPanel.BackColor = Color.Transparent; // Ensure background transparency

            this.picPhoto.Width = 100;
            this.picPhoto.Height = 100;
            this.picPhoto.SizeMode = PictureBoxSizeMode.Zoom;
            this.picPhoto.BorderStyle = BorderStyle.FixedSingle;
            this.picPhoto.Margin = new Padding(0, 0, 0, 5); // Margin below photo

            this.btnUploadPhoto.Text = "Upload Photo";
            UIHelper.StyleButton(this.btnUploadPhoto); // Apply button styling
            this.btnUploadPhoto.Width = 100; // Consistent width
            this.btnUploadPhoto.Height = 30; // Consistent height
            this.btnUploadPhoto.Click += new EventHandler(btnUploadPhoto_Click);
            
            mainLayout.Controls.Add(photoSectionPanel, 0, 0);
            mainLayout.SetColumnSpan(photoSectionPanel, 2);
            photoSectionPanel.Anchor = AnchorStyles.None; // Center the entire photo section

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
            addRow("Date of Birth:", dtpDateOfBirth, 3);
            this.dtpDateOfBirth.Format = DateTimePickerFormat.Short;
            this.dtpDateOfBirth.Font = new Font("Segoe UI", 10); // Manual style for DateTimePicker

            // Gender
            addRow("Gender:", cmbGender, 4);
            UIHelper.StyleComboBox(this.cmbGender);
            this.cmbGender.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbGender.Items.AddRange(new object[] { "Male", "Female", "Other" });

            // Blood Type
            addRow("Blood Type:", cmbBloodType, 5);
            UIHelper.StyleComboBox(this.cmbBloodType);
            this.cmbBloodType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbBloodType.Items.AddRange(new object[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" });

            // Contact Number
            addRow("Contact Number:", txtContactNumber, 6);
            UIHelper.StyleTextBox(this.txtContactNumber);

            // Email
            addRow("Email:", txtEmail, 7);
            UIHelper.StyleTextBox(this.txtEmail);

            // Address
            addRow("Address:", txtAddress, 8);
            UIHelper.StyleTextBox(this.txtAddress);

            // Insurance Number
            addRow("Insurance Number:", txtInsuranceNumber, 9);
            UIHelper.StyleTextBox(this.txtInsuranceNumber);

            // Patient's Family
            addRow("Patient's Family:", txtFamily, 10);
            UIHelper.StyleTextBox(this.txtFamily);

            // Status
            addRow("Status:", cmbStatus, 11);
            UIHelper.StyleComboBox(this.cmbStatus);
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new object[] { "Active", "Inactive", "Discharged" });

            // Room Number
            addRow("Room Number:", txtRoomNumber, 12);
            UIHelper.StyleTextBox(this.txtRoomNumber);

            // Buttons Panel
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft; // Buttons align to the right
            buttonPanel.Padding = new Padding(0, 10, 0, 0); // Padding from top
            buttonPanel.BackColor = Color.Transparent; // Ensure background transparency

            this.btnCancel.Text = "Cancel";
            UIHelper.StyleButton(this.btnCancel); // Apply button styling
            this.btnCancel.Width = 100; 
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            buttonPanel.Controls.Add(this.btnCancel);

            this.btnSave.Text = "Save";
            UIHelper.StyleButton(this.btnSave); // Apply button styling
            this.btnSave.Width = 100; 
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            buttonPanel.Controls.Add(this.btnSave);
            
            mainLayout.Controls.Add(buttonPanel, 0, 13); // Buttons at the bottom
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                cmbGender.SelectedIndex == -1 ||
                cmbBloodType.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtContactNumber.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text) ||
                cmbStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                try
                {
                    // 1. Update/Insert into tblPatient
                    string patientQuery;
                    SqlCommand patientCmd;

                    if (_patientId == 0) // New patient
                    {
                        patientQuery = @"INSERT INTO tblPatient (FirstName, LastName, DateOfBirth, Gender, BloodType, ContactNumber, Email, Address, InsuranceNumber, PatientFamily, Status, ProfilePhoto, IsDeleted) 
                                       VALUES (@FirstName, @LastName, @DateOfBirth, @Gender, @BloodType, @ContactNumber, @Email, @Address, @InsuranceNumber, @Family, @Status, @ProfilePhoto, 0);
                                       SELECT SCOPE_IDENTITY();";
                        patientCmd = new SqlCommand(patientQuery, con, transaction);
                    }
                    else // Existing patient
                    {
                        patientQuery = @"UPDATE tblPatient SET FirstName=@FirstName, LastName=@LastName, DateOfBirth=@DateOfBirth, Gender=@Gender, BloodType=@BloodType, ContactNumber=@ContactNumber, Email=@Email, Address=@Address, InsuranceNumber=@InsuranceNumber, PatientFamily=@Family, Status=@Status, ProfilePhoto=@ProfilePhoto WHERE PatientID=@PatientID";
                        patientCmd = new SqlCommand(patientQuery, con, transaction);
                        patientCmd.Parameters.AddWithValue("@PatientID", _patientId);
                    }

                    patientCmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                    patientCmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                    patientCmd.Parameters.AddWithValue("@DateOfBirth", dtpDateOfBirth.Value);
                    patientCmd.Parameters.AddWithValue("@Gender", cmbGender.Text);
                    patientCmd.Parameters.AddWithValue("@BloodType", cmbBloodType.Text);
                    patientCmd.Parameters.AddWithValue("@ContactNumber", txtContactNumber.Text);
                    patientCmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    patientCmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                    patientCmd.Parameters.AddWithValue("@InsuranceNumber", txtInsuranceNumber.Text);
                    patientCmd.Parameters.AddWithValue("@Family", txtFamily.Text);
                    patientCmd.Parameters.AddWithValue("@Status", cmbStatus.Text);
                    patientCmd.Parameters.AddWithValue("@ProfilePhoto", (object)ImageToByteArray(picPhoto.Image) ?? DBNull.Value);

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
                    SqlCommand unassignOldRoomCmd = new SqlCommand("UPDATE tblRoom SET PatientID = NULL WHERE PatientID = @PatientID", con, transaction);
                    unassignOldRoomCmd.Parameters.AddWithValue("@PatientID", _patientId);
                    unassignOldRoomCmd.ExecuteNonQuery();

                    if (!string.IsNullOrEmpty(newRoomNumber))
                    {
                        // Check if the new room number exists
                        SqlCommand checkRoomCmd = new SqlCommand("SELECT RoomID, PatientID FROM tblRoom WHERE RoomNumber = @RoomNumber", con, transaction);
                        checkRoomCmd.Parameters.AddWithValue("@RoomNumber", newRoomNumber);
                        SqlDataReader reader = checkRoomCmd.ExecuteReader();

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
                                SqlCommand assignRoomCmd = new SqlCommand("UPDATE tblRoom SET PatientID = @PatientID WHERE RoomID = @RoomID", con, transaction);
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
                            SqlCommand createRoomCmd = new SqlCommand("INSERT INTO tblRoom (RoomNumber, RoomType, Status, PatientID) VALUES (@RoomNumber, 'Standard', 'Occupied', @PatientID); SELECT SCOPE_IDENTITY();", con, transaction);
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

        private byte[] ImageToByteArray(System.Drawing.Image image)
        {
            if (image == null) return null;
            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private System.Drawing.Image ByteArrayToImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;
            using (var ms = new MemoryStream(bytes))
            {
                return System.Drawing.Image.FromStream(ms);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
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
                        MessageBox.Show("Error loading image: " + ex.Message, "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
} 