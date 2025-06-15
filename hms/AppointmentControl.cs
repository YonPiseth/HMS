using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing; // Added for Color and Point

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

            // Use TableLayoutPanel for clean layout
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 4;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55)); // Title (slightly taller)
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55)); // Search area (slightly taller)
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // DataGridView
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Button panel (taller for spacing)
            layout.Padding = new Padding(15); // Consistent padding
            layout.BackColor = Color.White;

            // Title
            Panel titlePanel = new Panel();
            UIHelper.ApplyPanelStyles(titlePanel); // Apply panel styling
            titlePanel.Dock = DockStyle.Fill;
            Label lblTitle = new Label();
            lblTitle.Text = "Manage Appointments"; // More descriptive title
            UIHelper.StyleLabelTitle(lblTitle); // Apply title label styling
            lblTitle.Location = new Point(0, 0); // Position within panel
            titlePanel.Controls.Add(lblTitle);
            lblTitle.BringToFront(); // Ensure label is visible on panel

            // Search Panel
            FlowLayoutPanel searchFlowPanel = new FlowLayoutPanel();
            searchFlowPanel.Dock = DockStyle.Fill;
            searchFlowPanel.Padding = new Padding(0, 5, 0, 0); // Padding top
            searchFlowPanel.FlowDirection = FlowDirection.LeftToRight;
            searchFlowPanel.Controls.Add(new Label { Text = "Search: ", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft });
            UIHelper.StyleLabel((Label)searchFlowPanel.Controls[0]); // Style the new label

            this.txtSearch.Width = 250; // Adjusted width
            this.txtSearch.Height = 30; // Standard height
            UIHelper.StyleTextBox(this.txtSearch); // Apply text box styling
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);
            searchFlowPanel.Controls.Add(this.txtSearch);

            // DataGridView
            this.dataGridView1.Dock = DockStyle.Fill;
            UIHelper.StyleDataGridView(this.dataGridView1); // Apply DataGridView styling
            this.dataGridView1.DataBindingComplete += (s, e) => {
                if (this.dataGridView1.Columns.Contains("IsDeleted"))
                    this.dataGridView1.Columns["IsDeleted"].Visible = false;

                // Set specific column widths for better readability
                if (this.dataGridView1.Columns.Contains("PatientName")) this.dataGridView1.Columns["PatientName"].Width = 150;
                if (this.dataGridView1.Columns.Contains("DoctorName")) this.dataGridView1.Columns["DoctorName"].Width = 150;
                if (this.dataGridView1.Columns.Contains("AppointmentDate")) this.dataGridView1.Columns["AppointmentDate"].Width = 100;
                if (this.dataGridView1.Columns.Contains("AppointmentTime")) this.dataGridView1.Columns["AppointmentTime"].Width = 80;
                if (this.dataGridView1.Columns.Contains("Status")) this.dataGridView1.Columns["Status"].Width = 90;
            };

            // Button panel (action bar at bottom right)
            this.buttonPanel.Dock = DockStyle.Fill;
            this.buttonPanel.FlowDirection = FlowDirection.RightToLeft; // Buttons align to the right
            this.buttonPanel.Padding = new Padding(0, 10, 0, 0); // Padding from top

            // Style and add buttons to buttonPanel (in reverse order for RightToLeft flow)
            this.btnLogout.Text = "Log Out";
            UIHelper.StyleButton(this.btnLogout);
            this.btnLogout.Click += new EventHandler(this.btnLogout_Click);
            this.buttonPanel.Controls.Add(this.btnLogout);

            this.btnDelete.Text = "Delete";
            UIHelper.StyleButton(this.btnDelete);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            this.buttonPanel.Controls.Add(this.btnDelete);

            this.btnUpdate.Text = "Update";
            UIHelper.StyleButton(this.btnUpdate);
            this.btnUpdate.Click += new EventHandler(this.btnUpdate_Click);
            this.buttonPanel.Controls.Add(this.btnUpdate);

            this.btnAdd.Text = "Add Appointment";
            UIHelper.StyleButton(this.btnAdd);
            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
            this.buttonPanel.Controls.Add(this.btnAdd);

            // Add controls to layout
            layout.Controls.Add(titlePanel, 0, 0);
            layout.Controls.Add(searchFlowPanel, 0, 1);
            layout.Controls.Add(this.dataGridView1, 0, 2);
            layout.Controls.Add(this.buttonPanel, 0, 3);

            // Clear and add layout to UserControl
            this.Controls.Clear();
            this.Controls.Add(layout);
            this.Size = new Size(950, 550); // Adjusted default size for the control
        }

        private void LoadAppointments(string search = "")
        {
            using (SqlConnection con = new SqlConnection(connectionString))
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
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        // Overload to load appointments for a specific patient
        public void LoadPatientAppointments(int patientUserID)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
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
                SqlDataAdapter da = new SqlDataAdapter(query, con);
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
                MessageBox.Show("Please select an appointment to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this appointment?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                int appointmentId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["AppointmentID"].Value);
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE tblAppointment SET IsDeleted = 1 WHERE AppointmentID = @AppointmentID"; // Soft delete
                    SqlCommand cmd = new SqlCommand(query, con);
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
            // This button might be redundant if the logout is handled by the MainForm
            // For now, it could close the current control or trigger a main form event
            MessageBox.Show("Logout functionality would be implemented here.", "Logout", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
