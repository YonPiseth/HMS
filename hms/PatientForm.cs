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
    }
} 