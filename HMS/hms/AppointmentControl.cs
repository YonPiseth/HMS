using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

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
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";
        private Panel buttonPanel;

        public AppointmentControl()
        {
            InitializeComponent();
            LoadAppointments();
            StyleGridAndButtons();
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

            // Use TableLayoutPanel for clean layout
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 4;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48)); // Title
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48)); // Search
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // DataGridView
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Button panel
            layout.Padding = new Padding(16);
            layout.BackColor = System.Drawing.Color.White;

            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "Appointments";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 18, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.FromArgb(24, 33, 54);
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // Search
            Panel searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Fill;
            searchPanel.Height = 40;
            searchPanel.Padding = new Padding(0, 8, 0, 8);
            this.txtSearch.Dock = DockStyle.Left;
            this.txtSearch.Width = 320;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 11);
            this.txtSearch.BorderStyle = BorderStyle.FixedSingle;
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);
            searchPanel.Controls.Add(this.txtSearch);

            // DataGridView
            this.dataGridView1.Dock = DockStyle.Fill;
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            this.dataGridView1.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10);
            this.dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.AliceBlue;
            this.dataGridView1.DataBindingComplete += (s, e) => {
                if (this.dataGridView1.Columns.Contains("IsDeleted"))
                    this.dataGridView1.Columns["IsDeleted"].Visible = false;
            };

            // Button panel
            this.buttonPanel.Dock = DockStyle.Fill;
            this.buttonPanel.Height = 48;
            this.buttonPanel.Padding = new Padding(0, 8, 0, 0);
            this.btnAdd.Text = "Add Appointment";
            this.btnDelete.Text = "Delete";
            this.btnUpdate.Text = "Update";
            this.btnLogout.Text = "Log Out";
            foreach (Button btn in new[] { btnAdd, btnDelete, btnUpdate, btnLogout })
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
                btn.ForeColor = System.Drawing.Color.White;
                btn.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
                btn.Height = 36;
                btn.Width = 120;
                btn.Margin = new Padding(10, 0, 0, 0);
                btn.FlatAppearance.BorderSize = 0;
                this.buttonPanel.Controls.Add(btn);
            }
            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            this.btnUpdate.Click += new EventHandler(this.btnUpdate_Click);
            this.btnLogout.Click += new EventHandler(this.btnLogout_Click);

            // Add controls to layout
            layout.Controls.Add(lblTitle, 0, 0);
            layout.Controls.Add(searchPanel, 0, 1);
            layout.Controls.Add(this.dataGridView1, 0, 2);
            layout.Controls.Add(this.buttonPanel, 0, 3);
            // Clear and add layout to UserControl
            this.Controls.Clear();
            this.Controls.Add(layout);
            this.Size = new System.Drawing.Size(900, 500);
        }

        private void StyleGridAndButtons()
        {
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            dataGridView1.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10);
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.AliceBlue;
            foreach (Button btn in new[] { btnAdd, btnDelete, btnUpdate, btnLogout })
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
                btn.ForeColor = System.Drawing.Color.White;
                btn.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
                btn.Height = 36;
            }
        }

        private void LoadAppointments(string search = "")
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT a.*, p.FirstName + ' ' + p.LastName as PatientName, 
                                d.FirstName + ' ' + d.LastName as DoctorName 
                                FROM tblAppointment a 
                                LEFT JOIN tblPatient p ON a.PatientID = p.PatientID 
                                LEFT JOIN tblDoctor d ON a.DoctorID = d.DoctorID 
                                WHERE a.IsDeleted = 0 
                                AND (CAST(a.AppointmentID AS VARCHAR) LIKE @search 
                                OR p.FirstName + ' ' + p.LastName LIKE @search 
                                OR d.FirstName + ' ' + d.LastName LIKE @search 
                                OR a.Status LIKE @search 
                                OR a.Reason LIKE @search)";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadAppointments(txtSearch.Text);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Check if there are patients and doctors
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM tblPatient WHERE IsDeleted = 0", con);
                SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM tblDoctor WHERE IsDeleted = 0", con);
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
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO tblAppointment (PatientID, DoctorID, AppointmentDate, AppointmentTime, Reason, Status, IsDeleted) 
                                   VALUES (@PatientID, @DoctorID, @AppointmentDate, @AppointmentTime, @Reason, @Status, 0)";
                    SqlCommand cmd = new SqlCommand(query, con);
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
                    catch (SqlException ex)
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
                MessageBox.Show("Please select an appointment to update.");
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            var form = new AppointmentForm();
            
            // Fill form with selected row data
            form.cmbPatient.SelectedValue = row.Cells["PatientID"].Value;
            form.cmbDoctor.SelectedValue = row.Cells["DoctorID"].Value;
            form.dtpDate.Value = Convert.ToDateTime(row.Cells["AppointmentDate"].Value);
            form.dtpTime.Value = DateTime.Today.Add(TimeSpan.Parse(row.Cells["AppointmentTime"].Value.ToString()));
            form.txtReason.Text = row.Cells["Reason"].Value?.ToString();
            form.cmbStatus.Text = row.Cells["Status"].Value?.ToString();

            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE tblAppointment SET PatientID=@PatientID, DoctorID=@DoctorID, 
                                   AppointmentDate=@AppointmentDate, AppointmentTime=@AppointmentTime, 
                                   Reason=@Reason, Status=@Status WHERE AppointmentID=@AppointmentID";
                    SqlCommand cmd = new SqlCommand(query, con);
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
                MessageBox.Show("Please select an appointment to delete.");
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete this appointment?", "Confirm Delete", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE tblAppointment SET IsDeleted=1 WHERE AppointmentID=@AppointmentID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@AppointmentID", row.Cells["AppointmentID"].Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadAppointments();
                MessageBox.Show("Appointment deleted successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Log out (to be implemented)
            MessageBox.Show("Log Out clicked");
        }

        public void LoadPatientAppointments(int patientID)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT a.AppointmentID, p.FirstName + ' ' + p.LastName as PatientName, 
                                d.FirstName + ' ' + d.LastName as DoctorName, 
                                a.AppointmentDate, a.AppointmentTime, a.Reason, a.Status
                                FROM tblAppointment a
                                INNER JOIN tblPatient p ON a.PatientID = p.PatientID
                                INNER JOIN tblDoctor d ON a.DoctorID = d.DoctorID
                                WHERE a.PatientID = @PatientID AND a.IsDeleted = 0
                                ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@PatientID", patientID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;

                // Hide the Add, Update, and Delete buttons for patient view
                btnAdd.Visible = false;
                btnUpdate.Visible = false;
                btnDelete.Visible = false;
            }
        }
    }
}
