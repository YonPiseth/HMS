using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing; // Added for Color and Point
using HMS;
using HMS.UI;

namespace HMS
{
    public partial class AppointmentControl : UserControl
    {
        private DataGridView dataGridView1;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnUpdate;
        private Button btnLogout;
        private FlowLayoutPanel buttonPanel;

        public AppointmentControl()
        {
            InitializeComponent();
            LoadAppointments();
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
            lblTitle.Text = "Manage Appointments";
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
            this.dataGridView1.DataBindingComplete += (s, e) => {
                if (this.dataGridView1.Columns.Contains("IsDeleted"))
                    this.dataGridView1.Columns["IsDeleted"].Visible = false;

                if (this.dataGridView1.Columns.Contains("PatientName")) this.dataGridView1.Columns["PatientName"].Width = 150;
                if (this.dataGridView1.Columns.Contains("DoctorName")) this.dataGridView1.Columns["DoctorName"].Width = 150;
                if (this.dataGridView1.Columns.Contains("AppointmentDate")) this.dataGridView1.Columns["AppointmentDate"].Width = 100;
                if (this.dataGridView1.Columns.Contains("AppointmentTime")) this.dataGridView1.Columns["AppointmentTime"].Width = 80;
                if (this.dataGridView1.Columns.Contains("Status")) this.dataGridView1.Columns["Status"].Width = 90;
            };

            this.buttonPanel.Dock = DockStyle.Fill;
            this.buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            this.buttonPanel.Padding = new Padding(0, UITheme.SpacingSM, UITheme.SpacingMD, UITheme.SpacingSM);
            this.buttonPanel.Height = 60;
            this.buttonPanel.AutoSize = false;
            this.buttonPanel.WrapContents = false;

            this.btnAdd.Text = "Add Appointment";
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

        private void LoadAppointments(string search = "")
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT a.AppointmentID, 
                                p.FirstName + ' ' + p.LastName as PatientName,
                                d.FirstName + ' ' + d.LastName as DoctorName,
                                a.AppointmentDate, a.AppointmentTime, a.Reason, a.Status 
                                FROM tblAppointment a 
                                LEFT JOIN tblPatient p ON a.PatientID = p.PatientID 
                                LEFT JOIN tblDoctor d ON a.DoctorID = d.DoctorID 
                                WHERE a.IsDeleted = 0 
                                AND (CAST(a.AppointmentID AS VARCHAR) LIKE @search 
                                OR p.FirstName + ' ' + p.LastName LIKE @search 
                                OR d.FirstName + ' ' + d.LastName LIKE @search 
                                OR a.Status LIKE @search 
                                OR a.Reason LIKE @search)";
                Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        // Overload to load appointments for a specific patient
        public void LoadPatientAppointments(int patientUserID)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT a.AppointmentID, 
                                p.FirstName + ' ' + p.LastName as PatientName,
                                d.FirstName + ' ' + d.LastName as DoctorName,
                                a.AppointmentDate, a.AppointmentTime, a.Reason, a.Status 
                                FROM tblAppointment a 
                                INNER JOIN tblPatient p ON a.PatientID = p.PatientID 
                                LEFT JOIN tblDoctor d ON a.DoctorID = d.DoctorID 
                                WHERE a.IsDeleted = 0 AND p.UserID = @PatientUserID 
                                ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC";
                Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@PatientUserID", patientUserID);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;

                // Hide action buttons for patient view
                btnAdd.Visible = false;
                btnUpdate.Visible = false;
                btnDelete.Visible = false;
                btnLogout.Visible = false; // Also hide logout as it's handled by MainForm

                // Make the search bar read-only for patient view if desired
                txtSearch.ReadOnly = true;
                txtSearch.BackColor = System.Drawing.SystemColors.Control;

                // Adjust grid to fill more space if buttons are hidden
                TableLayoutPanel parentLayout = this.Parent as TableLayoutPanel;
                if (parentLayout != null) {
                    parentLayout.RowStyles[3].Height = 0; // Collapse button row
                }
            }
            
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadAppointments(txtSearch.Text);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Check if there are patients and doctors
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                con.Open();
                Microsoft.Data.SqlClient.SqlCommand cmd1 = new Microsoft.Data.SqlClient.SqlCommand("SELECT COUNT(*) FROM tblPatient WHERE IsDeleted = 0", con);
                Microsoft.Data.SqlClient.SqlCommand cmd2 = new Microsoft.Data.SqlClient.SqlCommand("SELECT COUNT(*) FROM tblDoctor WHERE IsDeleted = 0", con);
                int patientCount = (int)cmd1.ExecuteScalar();
                int doctorCount = (int)cmd2.ExecuteScalar();
                if (patientCount == 0 || doctorCount == 0)
                {
                    MessageBox.Show("Please add at least one patient and one doctor before creating an appointment.", "Data Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            var form = new AppointmentForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = DatabaseHelper.GetConnection())
                {
                    string query = @"INSERT INTO tblAppointment (PatientID, DoctorID, AppointmentDate, AppointmentTime, Reason, Status, IsDeleted) 
                                   VALUES (@PatientID, @DoctorID, @AppointmentDate, @AppointmentTime, @Reason, @Status, 0)";
                    Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@PatientID", form.cmbPatient.SelectedValue);
                    cmd.Parameters.AddWithValue("@DoctorID", form.cmbDoctor.SelectedValue);
                    cmd.Parameters.AddWithValue("@AppointmentDate", form.dtpDate.Value.Date);
                    cmd.Parameters.AddWithValue("@AppointmentTime", form.dtpTime.Value.TimeOfDay);
                    cmd.Parameters.AddWithValue("@Reason", form.txtReason.Text);
                    cmd.Parameters.AddWithValue("@Status", form.cmbStatus.Text);
                    con.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Microsoft.Data.SqlClient.SqlException ex)
                    {
                        MessageBox.Show("Error adding appointment: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                LoadAppointments();
                MessageBox.Show("Appointment added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an appointment to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            var form = new AppointmentForm();
            
            // Fill form with selected row data
            // Ensure PatientID and DoctorID are correctly mapped to SelectedValue
            form.cmbPatient.SelectedValue = row.Cells["PatientID"].Value;
            form.cmbDoctor.SelectedValue = row.Cells["DoctorID"].Value;
            form.dtpDate.Value = Convert.ToDateTime(row.Cells["AppointmentDate"].Value);
            form.dtpTime.Value = DateTime.Today.Add(TimeSpan.Parse(row.Cells["AppointmentTime"].Value.ToString()));
            form.txtReason.Text = row.Cells["Reason"].Value?.ToString();
            form.cmbStatus.Text = row.Cells["Status"].Value?.ToString();

            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = DatabaseHelper.GetConnection())
                {
                    string query = @"UPDATE tblAppointment SET PatientID=@PatientID, DoctorID=@DoctorID, 
                                   AppointmentDate=@AppointmentDate, AppointmentTime=@AppointmentTime, 
                                   Reason=@Reason, Status=@Status WHERE AppointmentID=@AppointmentID";
                    Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@PatientID", form.cmbPatient.SelectedValue);
                    cmd.Parameters.AddWithValue("@DoctorID", form.cmbDoctor.SelectedValue);
                    cmd.Parameters.AddWithValue("@AppointmentDate", form.dtpDate.Value.Date);
                    cmd.Parameters.AddWithValue("@AppointmentTime", form.dtpTime.Value.TimeOfDay);
                    cmd.Parameters.AddWithValue("@Reason", form.txtReason.Text);
                    cmd.Parameters.AddWithValue("@Status", form.cmbStatus.Text);
                    cmd.Parameters.AddWithValue("@AppointmentID", row.Cells["AppointmentID"].Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadAppointments();
                MessageBox.Show("Appointment updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an appointment to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this appointment?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                int appointmentId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["AppointmentID"].Value);
                using (SqlConnection con = DatabaseHelper.GetConnection())
                {
                    string query = "UPDATE tblAppointment SET IsDeleted = 1 WHERE AppointmentID = @AppointmentID"; // Soft delete
                    Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadAppointments();
                MessageBox.Show("Appointment deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
