using System;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.IO;
using HMS;
using HMS.Models;
using HMS.Helpers;

namespace HMS
{
    public partial class DoctorForm : Form
    {
        private int _doctorId;
        private Doctor _doctor;

        public DoctorForm(int doctorId = 0)
        {
            _doctorId = doctorId;
            _doctor = new Doctor();
            InitializeComponent();
            SetupUI();
            UIHelper.ApplyModernTheme(this);
            picPhoto.Image = null;
            this.Load += DoctorForm_Load;
        }

        private void SetupUI()
        {
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainLayout.RowCount = 12;
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
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            mainLayout.Padding = new Padding(20);
            mainLayout.AutoScroll = true;
            UIHelper.ApplyPanelStyles(mainLayout);

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
            UIHelper.StyleButton(this.btnUploadPhoto);
            this.btnUploadPhoto.Width = 100;
            this.btnUploadPhoto.Height = 30;
            this.btnUploadPhoto.Click += new EventHandler(btnUploadPhoto_Click);
            
            mainLayout.Controls.Add(photoSectionPanel, 0, 0);
            mainLayout.SetColumnSpan(photoSectionPanel, 2);
            photoSectionPanel.Anchor = AnchorStyles.None;

            Action<string, Control, int> addRow = (labelText, control, row) =>
            {
                Label lbl = new Label { Text = labelText, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left };
                UIHelper.StyleLabel(lbl);
                mainLayout.Controls.Add(lbl, 0, row);
                control.Dock = DockStyle.Fill;
                mainLayout.Controls.Add(control, 1, row);
            };

            addRow("First Name:", txtFirstName, 1);
            UIHelper.StyleTextBox(this.txtFirstName);

            addRow("Last Name:", txtLastName, 2);
            UIHelper.StyleTextBox(this.txtLastName);

            addRow("Specialization:", cmbSpecialization, 3);
            UIHelper.StyleComboBox(this.cmbSpecialization);
            this.cmbSpecialization.DropDownStyle = ComboBoxStyle.DropDownList;

            addRow("Contact Number:", txtContactNumber, 4);
            UIHelper.StyleTextBox(this.txtContactNumber);

            addRow("Email:", txtEmail, 5);
            UIHelper.StyleTextBox(this.txtEmail);

            // Address
            addRow("Address:", txtAddress, 6);
            UIHelper.StyleTextBox(this.txtAddress);

            // Years of Experience
            addRow("Years of Experience:", numYearsOfExperience, 7);
            this.numYearsOfExperience.Dock = DockStyle.Fill;
            this.numYearsOfExperience.Font = new Font("Segoe UI", 10);
            this.numYearsOfExperience.Minimum = 0;
            this.numYearsOfExperience.Maximum = 100;

            // Qualification
            addRow("Qualification:", txtQualification, 8);
            UIHelper.StyleTextBox(this.txtQualification);

            // Department
            addRow("Department:", txtDepartment, 9);
            UIHelper.StyleTextBox(this.txtDepartment);

            // Working Hours
            addRow("Working Hours:", txtWorkingHours, 10);
            UIHelper.StyleTextBox(this.txtWorkingHours);
            this.txtWorkingHours.PlaceholderText = "e.g., 9:00 AM - 5:00 PM";

            // Is Available (Checkbox)
            Label lblIsAvailable = new Label { Text = "Is Available:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left };
            UIHelper.StyleLabel(lblIsAvailable);
            this.chkIsAvailable.Checked = true;
            this.chkIsAvailable.Text = "Available for appointments";
            this.chkIsAvailable.AutoSize = true;
            mainLayout.Controls.Add(lblIsAvailable, 0, 11);
            mainLayout.Controls.Add(this.chkIsAvailable, 1, 11);

            // Buttons Panel
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Padding = new Padding(0, 10, 0, 0);
            buttonPanel.BackColor = Color.Transparent;

            this.btnCancel.Text = "Cancel";
            UIHelper.StyleButton(this.btnCancel);
            this.btnCancel.Width = 100;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            buttonPanel.Controls.Add(this.btnCancel);

            this.btnSave.Text = "Save";
            UIHelper.StyleButton(this.btnSave);
            this.btnSave.Width = 100;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            buttonPanel.Controls.Add(this.btnSave);
            
            mainLayout.Controls.Add(buttonPanel, 0, 12);
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);
        }

        public void DoctorForm_Load(object sender, EventArgs e)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = "SELECT SpecializationID, SpecializationName FROM tblDoctorType";
                Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbSpecialization.DataSource = dt;
                cmbSpecialization.DisplayMember = "SpecializationName";
                cmbSpecialization.ValueMember = "SpecializationID";
            }
        }

        // Populate Doctor object from form fields
        private void PopulateDoctorFromForm()
        {
            _doctor.FirstName = txtFirstName.Text;
            _doctor.LastName = txtLastName.Text;
            _doctor.Email = txtEmail.Text;
            _doctor.Phone = txtContactNumber.Text;
            _doctor.Address = txtAddress.Text;
            _doctor.SpecializationID = cmbSpecialization.SelectedValue != null ? (int)cmbSpecialization.SelectedValue : 0;
            _doctor.YearsOfExperience = (int)numYearsOfExperience.Value;
            _doctor.Qualification = txtQualification.Text;
            _doctor.Department = txtDepartment.Text;
            _doctor.WorkingHours = txtWorkingHours.Text;
            _doctor.IsAvailable = chkIsAvailable.Checked;
            _doctor.ProfilePhoto = ImageHelper.ImageToByteArray(picPhoto.Image);
            _doctor.DoctorID = _doctorId;
        }

        // Populate form fields from Doctor object
        private void PopulateFormFromDoctor(Doctor doctor)
        {
            if (doctor == null) return;

            txtFirstName.Text = doctor.FirstName;
            txtLastName.Text = doctor.LastName;
            txtEmail.Text = doctor.Email;
            txtContactNumber.Text = doctor.Phone;
            txtAddress.Text = doctor.Address;
            numYearsOfExperience.Value = doctor.YearsOfExperience;
            txtQualification.Text = doctor.Qualification;
            txtDepartment.Text = doctor.Department;
            txtWorkingHours.Text = doctor.WorkingHours;
            chkIsAvailable.Checked = doctor.IsAvailable;
            
            if (doctor.SpecializationID > 0 && cmbSpecialization.Items.Count > 0)
            {
                cmbSpecialization.SelectedValue = doctor.SpecializationID;
            }
            
            if (doctor.ProfilePhoto != null && doctor.ProfilePhoto.Length > 0)
            {
                picPhoto.Image = ImageHelper.ByteArrayToImage(doctor.ProfilePhoto);
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            // Populate Doctor object from form
            PopulateDoctorFromForm();

            // Validate using Doctor class validation
            if (!_doctor.Validate())
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
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
                        MessageBox.Show("Error loading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        picPhoto.Image = null; // Explicitly set to null on error
                    }
                }
            }
        }
    }
} 