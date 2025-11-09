using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Linq;
using HMS.Helpers;
using HMS.UI;
using HMS.Repositories;
using HMS.Models;

namespace HMS
{
    public partial class PatientControl : UserControl
    {
        private DataGridView dataGridView1;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnUpdate;
        private Button btnLogout;
        private FlowLayoutPanel buttonPanel;
        private PatientRepository repository;

        public PatientControl()
        {
            repository = new PatientRepository();
            InitializeComponent();
            LoadPatients();
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
            lblTitle.Text = "Manage Patients";
            UIHelper.StyleHeading(lblTitle, 3);
            lblTitle.Location = new Point(0,0);
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

            this.btnAdd.Text = "Add Patient";
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

            this.dataGridView1.DataBindingComplete += (s, e) => {
                if (this.dataGridView1.Columns.Contains("IsDeleted"))
                    this.dataGridView1.Columns["IsDeleted"].Visible = false;
                if (this.dataGridView1.Columns.Contains("PatientID"))
                    this.dataGridView1.Columns["PatientID"].Visible = false;
                if (this.dataGridView1.Columns.Contains("ProfilePhoto"))
                    this.dataGridView1.Columns["ProfilePhoto"].Visible = false;

                if (this.dataGridView1.Columns.Contains("FirstName")) this.dataGridView1.Columns["FirstName"].Width = 120;
                if (this.dataGridView1.Columns.Contains("LastName")) this.dataGridView1.Columns["LastName"].Width = 120;
                if (this.dataGridView1.Columns.Contains("ContactNumber")) this.dataGridView1.Columns["ContactNumber"].Width = 120;
                if (this.dataGridView1.Columns.Contains("Email")) this.dataGridView1.Columns["Email"].Width = 180;
            };
        }


        private void LoadPatients(string search = "")
        {
            var patients = string.IsNullOrWhiteSpace(search) 
                ? repository.GetAllWithRooms() 
                : repository.GetAllWithRooms().Where(p => 
                    p.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.LastName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.PatientID.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Phone.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Email.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Address.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

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

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadPatients(txtSearch.Text);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new PatientForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadPatients();
                MessageBox.Show("Patient added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a patient to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            int patientId = Convert.ToInt32(row.Cells["PatientID"].Value);
            var form = new PatientForm(patientId);
            
            form.txtFirstName.Text = row.Cells["FirstName"].Value?.ToString();
            form.txtLastName.Text = row.Cells["LastName"].Value?.ToString();
            form.dtpDateOfBirth.Value = Convert.ToDateTime(row.Cells["DateOfBirth"].Value);
            form.cmbGender.Text = row.Cells["Gender"].Value?.ToString();
            form.cmbBloodType.Text = row.Cells["BloodType"].Value?.ToString();
            form.txtContactNumber.Text = row.Cells["ContactNumber"].Value?.ToString();
            form.txtEmail.Text = row.Cells["Email"].Value?.ToString();
            form.txtAddress.Text = row.Cells["Address"].Value?.ToString();
            form.txtInsuranceNumber.Text = row.Cells["InsuranceNumber"].Value?.ToString();
            form.txtFamily.Text = row.Cells["PatientFamily"].Value?.ToString();
            form.cmbStatus.Text = row.Cells["Status"].Value?.ToString();
            
            if (row.Cells["ProfilePhoto"].Value != DBNull.Value && row.Cells["ProfilePhoto"].Value != null)
                form.picPhoto.Image = ImageHelper.ByteArrayToImage((byte[])row.Cells["ProfilePhoto"].Value);
            
            if (dataGridView1.Columns.Contains("RoomNumber") && row.Cells["RoomNumber"].Value != DBNull.Value)
                form.txtRoomNumber.Text = row.Cells["RoomNumber"].Value?.ToString();
            else
                form.txtRoomNumber.Text = "";

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadPatients();
                MessageBox.Show("Patient updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a patient to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this patient?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                int patientId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["PatientID"].Value);
                if (repository.Delete(patientId))
                {
                    LoadPatients();
                    MessageBox.Show("Patient deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to delete patient.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
