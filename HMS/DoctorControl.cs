using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Linq;
using HMS;
using HMS.Models;
using HMS.Helpers;
using HMS.UI;
using HMS.Repositories;

namespace HMS
{
    public class DoctorControl : UserControl
    {
        private DataGridView dataGridView1;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnUpdate;
        private Button btnLogout;
        private FlowLayoutPanel buttonPanel;
        private DoctorRepository repository;

        public DoctorControl()
        {
            repository = new DoctorRepository();
            InitializeComponent();
            LoadDoctors();
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);
            this.dataGridView1.DataBindingComplete += (s, e) => {
                if (this.dataGridView1.Columns.Contains("IsDeleted"))
                    this.dataGridView1.Columns["IsDeleted"].Visible = false;
                if (this.dataGridView1.Columns.Contains("DoctorID"))
                    this.dataGridView1.Columns["DoctorID"].Visible = false;
                if (this.dataGridView1.Columns.Contains("ProfilePhoto"))
                    this.dataGridView1.Columns["ProfilePhoto"].Visible = false;

                if (this.dataGridView1.Columns.Contains("ProfilePhotoDisplay")) this.dataGridView1.Columns["ProfilePhotoDisplay"].Width = 60;
                if (this.dataGridView1.Columns.Contains("FirstName")) this.dataGridView1.Columns["FirstName"].Width = 100;
                if (this.dataGridView1.Columns.Contains("LastName")) this.dataGridView1.Columns["LastName"].Width = 100;
                if (this.dataGridView1.Columns.Contains("SpecializationName")) this.dataGridView1.Columns["SpecializationName"].Width = 120;
                if (this.dataGridView1.Columns.Contains("ContactNumber")) this.dataGridView1.Columns["ContactNumber"].Width = 110;
                if (this.dataGridView1.Columns.Contains("Email")) this.dataGridView1.Columns["Email"].Width = 150;
                if (this.dataGridView1.Columns.Contains("YearsOfExperience")) this.dataGridView1.Columns["YearsOfExperience"].Width = 70;
                if (this.dataGridView1.Columns.Contains("Qualification")) this.dataGridView1.Columns["Qualification"].Width = 120;
                if (this.dataGridView1.Columns.Contains("Department")) this.dataGridView1.Columns["Department"].Width = 100;
                if (this.dataGridView1.Columns.Contains("WorkingHours")) this.dataGridView1.Columns["WorkingHours"].Width = 120;
                if (this.dataGridView1.Columns.Contains("IsAvailable")) 
                {
                    this.dataGridView1.Columns["IsAvailable"].Width = 80;
                    this.dataGridView1.Columns["IsAvailable"].Visible = false;
                }
            };
        }

        private void InitializeComponent()
        {
            this.dataGridView1 = new DataGridView();
            this.txtSearch = new TextBox();
            this.btnAdd = new Button();
            this.btnDelete = new Button();
            this.btnUpdate = new Button();
            this.btnLogout = new Button();
            this.buttonPanel = new FlowLayoutPanel();

            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 4;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            layout.Padding = new Padding(15);
            layout.BackColor = Color.White;

            Panel titlePanel = new Panel();
            titlePanel.Dock = DockStyle.Fill;
            titlePanel.BackColor = Color.Transparent;
            Label lblTitle = new Label();
            lblTitle.Text = "Manage Doctors";
            UIHelper.StyleHeading(lblTitle, 3);
            lblTitle.Location = new Point(0, 0);
            titlePanel.Controls.Add(lblTitle);
            lblTitle.BringToFront();

            FlowLayoutPanel searchFlowPanel = new FlowLayoutPanel();
            searchFlowPanel.Dock = DockStyle.Fill;
            searchFlowPanel.Padding = new Padding(0, UITheme.SpacingSM, 0, 0);
            searchFlowPanel.FlowDirection = FlowDirection.LeftToRight;
            searchFlowPanel.BackColor = Color.Transparent;
            
            Label lblSearch = new Label 
            { 
                Text = "Search:", 
                AutoSize = true, 
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 8, UITheme.SpacingSM, 0)
            };
            UIHelper.StyleModernLabel(lblSearch);
            searchFlowPanel.Controls.Add(lblSearch);

            this.txtSearch.Width = 250;
            this.txtSearch.Height = 30;
            UIHelper.StyleModernTextBox(this.txtSearch);
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);
            searchFlowPanel.Controls.Add(this.txtSearch);

            UIHelper.StyleDataGridView(this.dataGridView1);
            Panel gridContainer = UIHelper.WrapDataGridViewInRoundedPanel(this.dataGridView1);

            this.buttonPanel.Dock = DockStyle.Fill;
            this.buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            this.buttonPanel.Padding = new Padding(0, UITheme.SpacingSM, UITheme.SpacingMD, UITheme.SpacingSM);
            this.buttonPanel.Height = 60;
            this.buttonPanel.AutoSize = false;
            this.buttonPanel.WrapContents = false;

            this.btnAdd.Text = "Add Doctor";
            this.btnUpdate.Text = "Update";
            this.btnDelete.Text = "Delete";
            this.btnLogout.Text = "Log Out";
            
            foreach (Button btn in new[] { btnAdd, btnUpdate, btnDelete, btnLogout })
            {
                UIHelper.StyleModernButton(btn);
                btn.Width = 120;
                btn.Height = 40;
                btn.Margin = new Padding(UITheme.SpacingSM, 0, 0, 0);
            }
            
            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
            this.btnUpdate.Click += new EventHandler(this.btnUpdate_Click);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            this.btnLogout.Click += new EventHandler(this.btnLogout_Click);
            
            this.buttonPanel.Controls.Add(this.btnLogout);
            this.buttonPanel.Controls.Add(this.btnDelete);
            this.buttonPanel.Controls.Add(this.btnUpdate);
            this.buttonPanel.Controls.Add(this.btnAdd);

            layout.Controls.Add(titlePanel, 0, 0);
            layout.Controls.Add(searchFlowPanel, 0, 1);
            layout.Controls.Add(gridContainer, 0, 2);
            layout.Controls.Add(this.buttonPanel, 0, 3);

            this.Controls.Clear();
            this.Controls.Add(layout);
            this.Size = new Size(950, 550);
        }


        private void LoadDoctors(string search = "")
        {
            var doctors = string.IsNullOrWhiteSpace(search)
                ? repository.GetAllWithSpecialization()
                : repository.SearchWithSpecialization(search);

            DataTable dt = new DataTable();
            dt.Columns.Add("DoctorID", typeof(int));
            dt.Columns.Add("FirstName", typeof(string));
            dt.Columns.Add("LastName", typeof(string));
            dt.Columns.Add("SpecializationName", typeof(string));
            dt.Columns.Add("ContactNumber", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("YearsOfExperience", typeof(int));
            dt.Columns.Add("Qualification", typeof(string));
            dt.Columns.Add("Department", typeof(string));
            dt.Columns.Add("WorkingHours", typeof(string));
            dt.Columns.Add("IsAvailable", typeof(bool));
            dt.Columns.Add("ProfilePhoto", typeof(byte[]));

            foreach (var doctor in doctors)
            {
                dt.Rows.Add(
                    doctor.DoctorID,
                    doctor.FirstName,
                    doctor.LastName,
                    doctor.SpecializationName ?? string.Empty,
                    doctor.Phone,
                    doctor.Email,
                    doctor.YearsOfExperience,
                    doctor.Qualification ?? string.Empty,
                    doctor.Department ?? string.Empty,
                    doctor.WorkingHours ?? string.Empty,
                    doctor.IsAvailable,
                    doctor.ProfilePhoto
                );
            }

            dataGridView1.DataSource = dt;

            if (!dataGridView1.Columns.Contains("ProfilePhotoDisplay"))
            {
                DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
                imgCol.Name = "ProfilePhotoDisplay";
                imgCol.HeaderText = "";
                imgCol.Width = 60;
                imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                dataGridView1.Columns.Insert(0, imgCol);
            }

            foreach (DataGridViewRow dgRow in dataGridView1.Rows)
            {
                if (dgRow.IsNewRow) continue;
                var row = ((DataRowView)dgRow.DataBoundItem)?.Row;
                if (row != null && row["ProfilePhoto"] != DBNull.Value)
                    dgRow.Cells["ProfilePhotoDisplay"].Value = ImageHelper.ByteArrayToImage((byte[])row["ProfilePhoto"]);
                else
                    dgRow.Cells["ProfilePhotoDisplay"].Value = null;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadDoctors(txtSearch.Text);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new DoctorForm();
            form.Load += form.DoctorForm_Load;
            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = DatabaseHelper.GetConnection())
                {
                    string query = @"INSERT INTO tblDoctor (FirstName, LastName, SpecializationID, ContactNumber, Email, Address, 
                                   YearsOfExperience, Qualification, Department, WorkingHours, IsAvailable, ProfilePhoto, IsDeleted)
                                   VALUES (@FirstName, @LastName, @SpecializationID, @ContactNumber, @Email, @Address, 
                                   @YearsOfExperience, @Qualification, @Department, @WorkingHours, @IsAvailable, @ProfilePhoto, 0)";
                    Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@FirstName", form.txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@LastName", form.txtLastName.Text);
                    cmd.Parameters.AddWithValue("@SpecializationID", form.cmbSpecialization.SelectedValue);
                    cmd.Parameters.AddWithValue("@ContactNumber", form.txtContactNumber.Text);
                    cmd.Parameters.AddWithValue("@Email", form.txtEmail.Text);
                    cmd.Parameters.AddWithValue("@Address", form.txtAddress?.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@YearsOfExperience", form.numYearsOfExperience.Value);
                    cmd.Parameters.AddWithValue("@Qualification", form.txtQualification?.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Department", form.txtDepartment?.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@WorkingHours", form.txtWorkingHours?.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@IsAvailable", form.chkIsAvailable?.Checked ?? true);

                    byte[] photoBytes = ImageHelper.ImageToByteArray(form.picPhoto.Image);
                    object profilePhotoParam = (object)photoBytes ?? DBNull.Value;

                    cmd.Parameters.Add("@ProfilePhoto", SqlDbType.VarBinary).Value = profilePhotoParam;
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadDoctors();
                MessageBox.Show("Doctor added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a doctor to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            int doctorId = Convert.ToInt32(row.Cells["DoctorID"].Value);
            Doctor doctor = LoadDoctorById(doctorId);
            var form = new DoctorForm();
            
            form.Load += (s, e) =>
            {
                form.DoctorForm_Load(s, e);
                
                if (doctor != null)
                {
                    form.txtFirstName.Text = doctor.FirstName;
                    form.txtLastName.Text = doctor.LastName;
                    form.txtEmail.Text = doctor.Email;
                    form.txtContactNumber.Text = doctor.Phone;
                    form.txtAddress.Text = doctor.Address;
                    form.numYearsOfExperience.Value = doctor.YearsOfExperience;
                    form.txtQualification.Text = doctor.Qualification;
                    form.txtDepartment.Text = doctor.Department;
                    form.txtWorkingHours.Text = doctor.WorkingHours;
                    form.chkIsAvailable.Checked = doctor.IsAvailable;
                    
                    if (doctor.SpecializationID > 0 && form.cmbSpecialization.Items.Count > 0)
                        form.cmbSpecialization.SelectedValue = doctor.SpecializationID;
                    
                    if (doctor.ProfilePhoto != null && doctor.ProfilePhoto.Length > 0)
                    {
                        try
                        {
                            form.picPhoto.Image = ImageHelper.ByteArrayToImage(doctor.ProfilePhoto);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error loading profile photo for doctor: {ex.Message}", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            form.picPhoto.Image = null;
                        }
                    }
                    else
                    {
                        form.picPhoto.Image = null;
                    }
                }
            };

            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = DatabaseHelper.GetConnection())
                {
                    string query = @"UPDATE tblDoctor SET FirstName=@FirstName, LastName=@LastName, SpecializationID=@SpecializationID, 
                                   ContactNumber=@ContactNumber, Email=@Email, Address=@Address, YearsOfExperience=@YearsOfExperience, 
                                   Qualification=@Qualification, Department=@Department, WorkingHours=@WorkingHours, 
                                   IsAvailable=@IsAvailable, ProfilePhoto=@ProfilePhoto WHERE DoctorID=@DoctorID";
                    Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@FirstName", form.txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@LastName", form.txtLastName.Text);
                    cmd.Parameters.AddWithValue("@SpecializationID", form.cmbSpecialization.SelectedValue);
                    cmd.Parameters.AddWithValue("@ContactNumber", form.txtContactNumber.Text);
                    cmd.Parameters.AddWithValue("@Email", form.txtEmail.Text);
                    cmd.Parameters.AddWithValue("@Address", form.txtAddress?.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@YearsOfExperience", form.numYearsOfExperience.Value);
                    cmd.Parameters.AddWithValue("@Qualification", form.txtQualification?.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@Department", form.txtDepartment?.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@WorkingHours", form.txtWorkingHours?.Text ?? string.Empty);
                    cmd.Parameters.AddWithValue("@IsAvailable", form.chkIsAvailable?.Checked ?? true);
                    cmd.Parameters.Add("@ProfilePhoto", SqlDbType.VarBinary).Value = (object)ImageHelper.ImageToByteArray(form.picPhoto.Image) ?? DBNull.Value;
                    cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadDoctors();
                MessageBox.Show("Doctor updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private Doctor LoadDoctorById(int doctorId)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT d.DoctorID, d.FirstName, d.LastName, d.SpecializationID, d.ContactNumber, d.Email, d.Address,
                                        d.YearsOfExperience, d.Qualification, d.Department, d.WorkingHours, d.IsAvailable, d.ProfilePhoto
                                 FROM tblDoctor d
                                 WHERE d.DoctorID = @DoctorID AND d.IsDeleted = 0";
                
                Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                con.Open();
                
                using (Microsoft.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Doctor doctor = new Doctor
                        {
                            DoctorID = reader.GetInt32("DoctorID"),
                            FirstName = reader["FirstName"]?.ToString() ?? string.Empty,
                            LastName = reader["LastName"]?.ToString() ?? string.Empty,
                            SpecializationID = reader["SpecializationID"] != DBNull.Value ? reader.GetInt32("SpecializationID") : 0,
                            Phone = reader["ContactNumber"]?.ToString() ?? string.Empty,
                            Email = reader["Email"]?.ToString() ?? string.Empty,
                            Address = reader["Address"]?.ToString() ?? string.Empty,
                            YearsOfExperience = reader["YearsOfExperience"] != DBNull.Value ? reader.GetInt32("YearsOfExperience") : 0,
                            Qualification = reader["Qualification"]?.ToString() ?? string.Empty,
                            Department = reader["Department"]?.ToString() ?? string.Empty,
                            WorkingHours = reader["WorkingHours"]?.ToString() ?? string.Empty,
                            IsAvailable = reader["IsAvailable"] != DBNull.Value ? reader.GetBoolean("IsAvailable") : true,
                            ProfilePhoto = reader["ProfilePhoto"] != DBNull.Value ? (byte[])reader["ProfilePhoto"] : null
                        };
                        return doctor;
                    }
                }
            }
            return null;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a doctor to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this doctor?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                int doctorId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["DoctorID"].Value);
                if (repository.Delete(doctorId))
                {
                    LoadDoctors();
                    MessageBox.Show("Doctor deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to delete doctor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (this.ParentForm is MainForm mainForm)
            {
                mainForm.Logout();
            }
        }
    }
}
