using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
using HMS.Models;
using HMS.Helpers;
using HMS.UI;
using HMS.Repositories;
using HMS.Services;
using HMS.Controls;

namespace HMS
{
    public class DoctorControl : BaseEntityControl<Doctor, IDoctorService>
    {
        public DoctorControl(IDoctorService doctorService) : base(doctorService)
        {
        }

        protected override string GetTitle() => "Manage Doctors";
        protected override string GetAddButtonText() => "Add Doctor";
        protected override string GetEntityName() => "Doctor";

        protected override async void LoadData(string search = "")
        {
            var doctors = string.IsNullOrWhiteSpace(search)
                ? await repository.GetAllDoctorsAsync()
                : await repository.SearchDoctorsAsync(search);

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

        protected override void ConfigureDataGridViewColumns()
        {
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
            if (this.dataGridView1.Columns.Contains("IsAvailable")) this.dataGridView1.Columns["IsAvailable"].Visible = false;
        }

        protected override Form CreateEntityForm(int? entityId = null)
        {
            var form = new DoctorForm();
            form.Load += async (s, e) =>
            {
                form.DoctorForm_Load(s, e);
                if (entityId.HasValue)
                {
                    var doctor = await repository.GetDoctorByIdAsync(entityId.Value);
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
                        if (doctor.SpecializationID > 0)
                            form.cmbSpecialization.SelectedValue = doctor.SpecializationID;
                        if (doctor.ProfilePhoto != null)
                            form.picPhoto.Image = ImageHelper.ByteArrayToImage(doctor.ProfilePhoto);
                    }
                }
            };

            form.FormClosing += async (s, e) =>
            {
                if (form.DialogResult == DialogResult.OK)
                {
                    var doctor = new Doctor
                    {
                        DoctorID = entityId ?? 0,
                        FirstName = form.txtFirstName.Text,
                        LastName = form.txtLastName.Text,
                        SpecializationID = Convert.ToInt32(form.cmbSpecialization.SelectedValue),
                        Phone = form.txtContactNumber.Text,
                        Email = form.txtEmail.Text,
                        Address = form.txtAddress.Text,
                        YearsOfExperience = (int)form.numYearsOfExperience.Value,
                        Qualification = form.txtQualification.Text,
                        Department = form.txtDepartment.Text,
                        WorkingHours = form.txtWorkingHours.Text,
                        IsAvailable = form.chkIsAvailable.Checked,
                        ProfilePhoto = ImageHelper.ImageToByteArray(form.picPhoto.Image)
                    };

                    bool success = entityId.HasValue 
                        ? await repository.UpdateDoctorAsync(doctor)
                        : await repository.AddDoctorAsync(doctor);

                    if (!success)
                    {
                        MessageBox.Show("Failed to save doctor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
            };

            return form;
        }

        protected override int GetSelectedEntityId()
        {
            if (dataGridView1.SelectedRows.Count > 0)
                return Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["DoctorID"].Value);
            return 0;
        }
    }
}
