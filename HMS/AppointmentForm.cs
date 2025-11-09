using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing; // Added for Color and Padding
using HMS;

namespace HMS
{
    public partial class AppointmentForm : Form
    {
        public AppointmentForm()
        {
            InitializeComponent();
            SetupUI();
            LoadPatients();
            LoadDoctors();
        }

        private void SetupUI()
        {
            // Main layout panel
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainLayout.RowCount = 7;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            mainLayout.Padding = new Padding(20);
            mainLayout.AutoScroll = true;
            UIHelper.ApplyPanelStyles(mainLayout);

            // Patient
            Label lblPatient = new Label { Text = "Patient:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left };
            UIHelper.StyleLabel(lblPatient);
            mainLayout.Controls.Add(lblPatient, 0, 0);
            this.cmbPatient.Dock = DockStyle.Fill;
            UIHelper.StyleComboBox(this.cmbPatient);
            this.cmbPatient.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbPatient.DisplayMember = "FullName";
            this.cmbPatient.ValueMember = "PatientID";
            mainLayout.Controls.Add(this.cmbPatient, 1, 0);

            // Doctor
            Label lblDoctor = new Label { Text = "Doctor:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left };
            UIHelper.StyleLabel(lblDoctor);
            mainLayout.Controls.Add(lblDoctor, 0, 1);
            this.cmbDoctor.Dock = DockStyle.Fill;
            UIHelper.StyleComboBox(this.cmbDoctor);
            this.cmbDoctor.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbDoctor.DisplayMember = "FullName";
            this.cmbDoctor.ValueMember = "DoctorID";
            mainLayout.Controls.Add(this.cmbDoctor, 1, 1);

            // Date
            Label lblDate = new Label { Text = "Date:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left };
            UIHelper.StyleLabel(lblDate);
            mainLayout.Controls.Add(lblDate, 0, 2);
            this.dtpDate.Dock = DockStyle.Fill;
            this.dtpDate.Format = DateTimePickerFormat.Short;
            this.dtpDate.Font = new Font("Segoe UI", 10);
            mainLayout.Controls.Add(this.dtpDate, 1, 2);

            // Time
            Label lblTime = new Label { Text = "Time:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left };
            UIHelper.StyleLabel(lblTime);
            mainLayout.Controls.Add(lblTime, 0, 3);
            this.dtpTime.Dock = DockStyle.Fill;
            this.dtpTime.Format = DateTimePickerFormat.Time;
            this.dtpTime.ShowUpDown = true;
            this.dtpTime.Font = new Font("Segoe UI", 10);
            mainLayout.Controls.Add(this.dtpTime, 1, 3);

            // Reason
            Label lblReason = new Label { Text = "Reason:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left };
            UIHelper.StyleLabel(lblReason);
            mainLayout.Controls.Add(lblReason, 0, 4);
            this.txtReason.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(this.txtReason);
            this.txtReason.Multiline = true;
            this.txtReason.Height = 80;
            mainLayout.SetRowSpan(this.txtReason, 2);
            mainLayout.Controls.Add(this.txtReason, 1, 4);

            // Status
            Label lblStatus = new Label { Text = "Status:", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left };
            UIHelper.StyleLabel(lblStatus);
            mainLayout.Controls.Add(lblStatus, 0, 5);
            this.cmbStatus.Dock = DockStyle.Fill;
            UIHelper.StyleComboBox(this.cmbStatus);
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new string[] { "Scheduled", "Completed", "Cancelled", "No Show" });
            mainLayout.Controls.Add(this.cmbStatus, 1, 5);

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
            
            mainLayout.Controls.Add(buttonPanel, 0, 6);
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);
        }

        private void LoadPatients()
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = "SELECT PatientID, FirstName + ' ' + LastName as FullName FROM tblPatient WHERE IsDeleted = 0";
                Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbPatient.DataSource = dt;
                cmbPatient.DisplayMember = "FullName";
                cmbPatient.ValueMember = "PatientID";
            }
        }

        private void LoadDoctors()
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = "SELECT DoctorID, FirstName + ' ' + LastName as FullName FROM tblDoctor WHERE IsDeleted = 0";
                Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbDoctor.DataSource = dt;
                cmbDoctor.DisplayMember = "FullName";
                cmbDoctor.ValueMember = "DoctorID";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbPatient.SelectedIndex == -1 ||
                cmbDoctor.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtReason.Text) ||
                cmbStatus.SelectedIndex == -1)
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
    }
}
