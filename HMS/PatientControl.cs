using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
using HMS.Models;
using HMS.Helpers;
using HMS.UI;
using HMS.Repositories;
using HMS.Services;
using HMS.Controls;

namespace HMS
{
    public partial class PatientControl : BaseEntityControl<Patient, IPatientService>
    {
        public PatientControl(IPatientService patientService) : base(patientService)
        {
        }

        protected override string GetTitle() => "Manage Patients";
        protected override string GetAddButtonText() => "Add Patient";
        protected override string GetEntityName() => "Patient";

        protected override async void LoadData(string search = "")
        {
            var patients = string.IsNullOrWhiteSpace(search)
                ? await repository.GetAllPatientsAsync()
                : await repository.SearchPatientsAsync(search);

            DataTable dt = new DataTable();
            dt.Columns.Add("PatientID", typeof(int));
            dt.Columns.Add("FirstName", typeof(string));
            dt.Columns.Add("LastName", typeof(string));
            dt.Columns.Add("DateOfBirth", typeof(DateTime));
            dt.Columns.Add("Gender", typeof(string));
            dt.Columns.Add("BloodType", typeof(string));
            dt.Columns.Add("ContactNumber", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("Address", typeof(string));
            dt.Columns.Add("InsuranceNumber", typeof(string));
            dt.Columns.Add("PatientFamily", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("ProfilePhoto", typeof(byte[]));
            dt.Columns.Add("RoomNumber", typeof(string));

            foreach (var patient in patients)
            {
                dt.Rows.Add(
                    patient.PatientID,
                    patient.FirstName,
                    patient.LastName,
                    patient.DateOfBirth,
                    patient.Gender,
                    patient.BloodType,
                    patient.Phone,
                    patient.Email,
                    patient.Address,
                    patient.InsuranceNumber,
                    patient.PatientFamily,
                    patient.Status,
                    patient.ProfilePhoto,
                    patient.RoomNumber ?? string.Empty
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
            if (this.dataGridView1.Columns.Contains("PatientID"))
                this.dataGridView1.Columns["PatientID"].Visible = false;
            if (this.dataGridView1.Columns.Contains("ProfilePhoto"))
                this.dataGridView1.Columns["ProfilePhoto"].Visible = false;

            if (this.dataGridView1.Columns.Contains("FirstName")) this.dataGridView1.Columns["FirstName"].Width = 120;
            if (this.dataGridView1.Columns.Contains("LastName")) this.dataGridView1.Columns["LastName"].Width = 120;
            if (this.dataGridView1.Columns.Contains("ContactNumber")) this.dataGridView1.Columns["ContactNumber"].Width = 120;
            if (this.dataGridView1.Columns.Contains("Email")) this.dataGridView1.Columns["Email"].Width = 180;
        }

        protected override Form CreateEntityForm(int? entityId = null)
        {
            var form = new PatientForm(entityId ?? -1);
            if (entityId.HasValue)
            {
                // Load existing data into form
                // This logic was previously in btnUpdate_Click but should be encapsulation in PatientForm
                // For simplicity here, we'll keep the existing form behavior
            }
            return form;
        }

        protected override int GetSelectedEntityId()
        {
            if (dataGridView1.SelectedRows.Count > 0)
                return Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["PatientID"].Value);
            return 0;
        }
    }
}
