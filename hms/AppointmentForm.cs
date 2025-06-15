using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing; // Added for Color and Padding

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
            this.Size = new Size(450, 500); // Adjusted size for better field visibility
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
            mainLayout.RowCount = 7; // 6 fields + Buttons
            // Adjust row heights for better spacing
            for (int i = 0; i < 6; i++) 
            {
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Standard field rows
            }
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Buttons row
            mainLayout.Padding = new Padding(20); // More generous padding
            mainLayout.AutoScroll = true;
            UIHelper.ApplyPanelStyles(mainLayout); // Apply panel style to main layout

            // Helper to add a row of label and control
            Action<string, Control, int> addRow = (labelText, control, row) =>
            {
                Label lbl = new Label { Text = labelText, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left };
                UIHelper.StyleLabel(lbl);
                mainLayout.Controls.Add(lbl, 0, row);
                control.Dock = DockStyle.Fill;
                mainLayout.Controls.Add(control, 1, row);
            };

            // Patient
            addRow("Patient:", cmbPatient, 0);
            UIHelper.StyleComboBox(this.cmbPatient);
            this.cmbPatient.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbPatient.DisplayMember = "FullName";
            this.cmbPatient.ValueMember = "PatientID";

            // Doctor
            addRow("Doctor:", cmbDoctor, 1);
            UIHelper.StyleComboBox(this.cmbDoctor);
            this.cmbDoctor.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbDoctor.DisplayMember = "FullName";
            this.cmbDoctor.ValueMember = "DoctorID";

            // Date
            addRow("Date:", dtpDate, 2);
            this.dtpDate.Format = DateTimePickerFormat.Short;
            this.dtpDate.Font = new Font("Segoe UI", 10); // Manual style for DateTimePicker

            // Time
            addRow("Time:", dtpTime, 3);
            this.dtpTime.Format = DateTimePickerFormat.Time;
            this.dtpTime.ShowUpDown = true;
            this.dtpTime.Font = new Font("Segoe UI", 10); // Manual style for DateTimePicker

            // Reason
            addRow("Reason:", txtReason, 4);
            UIHelper.StyleTextBox(this.txtReason);
            this.txtReason.Multiline = true; // Allow multiline for reason
            this.txtReason.Height = 80; // Larger height for multiline
            mainLayout.SetRowSpan(this.txtReason, 2); // Span two rows for reason
            mainLayout.RowStyles[4] = new RowStyle(SizeType.Absolute, 80); // Adjust height for reason row

            // Status
            addRow("Status:", cmbStatus, 5); // This row index will be affected by previous row span
            UIHelper.StyleComboBox(this.cmbStatus);
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new string[] { "Scheduled", "Completed", "Cancelled", "No Show" });

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
            
            mainLayout.Controls.Add(buttonPanel, 0, 6); // Buttons at the bottom
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);
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
