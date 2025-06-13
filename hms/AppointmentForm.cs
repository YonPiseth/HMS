using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace HMS
{
    public partial class AppointmentForm : Form
    {
        public ComboBox cmbPatient;
        public ComboBox cmbDoctor;
        public DateTimePicker dtpDate;
        public DateTimePicker dtpTime;
        public TextBox txtReason;
        public ComboBox cmbStatus;
        private Button btnSave;
        private Button btnCancel;
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";

        public AppointmentForm()
        {
            InitializeComponent();
            LoadPatients();
            LoadDoctors();
        }

        private void InitializeComponent()
        {
            this.cmbPatient = new ComboBox();
            this.cmbDoctor = new ComboBox();
            this.dtpDate = new DateTimePicker();
            this.dtpTime = new DateTimePicker();
            this.txtReason = new TextBox();
            this.cmbStatus = new ComboBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            // Form settings
            this.Text = "Appointment Information";
            this.Size = new System.Drawing.Size(400, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Labels and Controls
            int labelX = 20;
            int controlX = 150;
            int startY = 20;
            int spacing = 40;

            // Patient
            Label lblPatient = new Label();
            lblPatient.Text = "Patient:";
            lblPatient.Location = new System.Drawing.Point(labelX, startY);
            lblPatient.AutoSize = true;
            this.cmbPatient.Location = new System.Drawing.Point(controlX, startY);
            this.cmbPatient.Size = new System.Drawing.Size(200, 23);
            this.cmbPatient.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbPatient.DisplayMember = "FullName";
            this.cmbPatient.ValueMember = "PatientID";

            // Doctor
            Label lblDoctor = new Label();
            lblDoctor.Text = "Doctor:";
            lblDoctor.Location = new System.Drawing.Point(labelX, startY + spacing);
            lblDoctor.AutoSize = true;
            this.cmbDoctor.Location = new System.Drawing.Point(controlX, startY + spacing);
            this.cmbDoctor.Size = new System.Drawing.Size(200, 23);
            this.cmbDoctor.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbDoctor.DisplayMember = "FullName";
            this.cmbDoctor.ValueMember = "DoctorID";

            // Date
            Label lblDate = new Label();
            lblDate.Text = "Date:";
            lblDate.Location = new System.Drawing.Point(labelX, startY + spacing * 2);
            lblDate.AutoSize = true;
            this.dtpDate.Location = new System.Drawing.Point(controlX, startY + spacing * 2);
            this.dtpDate.Size = new System.Drawing.Size(200, 23);
            this.dtpDate.Format = DateTimePickerFormat.Short;

            // Time
            Label lblTime = new Label();
            lblTime.Text = "Time:";
            lblTime.Location = new System.Drawing.Point(labelX, startY + spacing * 3);
            lblTime.AutoSize = true;
            this.dtpTime.Location = new System.Drawing.Point(controlX, startY + spacing * 3);
            this.dtpTime.Size = new System.Drawing.Size(200, 23);
            this.dtpTime.Format = DateTimePickerFormat.Time;
            this.dtpTime.ShowUpDown = true;

            // Reason
            Label lblReason = new Label();
            lblReason.Text = "Reason:";
            lblReason.Location = new System.Drawing.Point(labelX, startY + spacing * 4);
            lblReason.AutoSize = true;
            this.txtReason.Location = new System.Drawing.Point(controlX, startY + spacing * 4);
            this.txtReason.Size = new System.Drawing.Size(200, 23);

            // Status
            Label lblStatus = new Label();
            lblStatus.Text = "Status:";
            lblStatus.Location = new System.Drawing.Point(labelX, startY + spacing * 5);
            lblStatus.AutoSize = true;
            this.cmbStatus.Location = new System.Drawing.Point(controlX, startY + spacing * 5);
            this.cmbStatus.Size = new System.Drawing.Size(200, 23);
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new string[] { "Scheduled", "Completed", "Cancelled", "No Show" });

            // Buttons
            this.btnSave.Text = "Save";
            this.btnSave.Location = new System.Drawing.Point(controlX, startY + spacing * 6);
            this.btnSave.Size = new System.Drawing.Size(90, 30);
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(controlX + 100, startY + spacing * 6);
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                lblPatient, cmbPatient,
                lblDoctor, cmbDoctor,
                lblDate, dtpDate,
                lblTime, dtpTime,
                lblReason, txtReason,
                lblStatus, cmbStatus,
                btnSave, btnCancel
            });
        }

        private void LoadPatients()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT PatientID, FirstName + ' ' + LastName as FullName FROM tblPatient WHERE IsDeleted = 0";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbPatient.DataSource = dt;
                cmbPatient.DisplayMember = "FullName";
                cmbPatient.ValueMember = "PatientID";
            }
        }

        private void LoadDoctors()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT DoctorID, FirstName + ' ' + LastName as FullName FROM tblDoctor WHERE IsDeleted = 0";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
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
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
