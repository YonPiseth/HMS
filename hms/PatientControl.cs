using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

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
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";

        public PatientControl()
        {
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

            // Title with icon
            Panel titlePanel = new Panel();
            UIHelper.ApplyPanelStyles(titlePanel); // Apply panel styling
            titlePanel.Dock = DockStyle.Fill;
            Label lblTitle = new Label();
            lblTitle.Text = "Manage Patients"; // More descriptive title
            UIHelper.StyleLabelTitle(lblTitle); // Apply title label styling
            lblTitle.Location = new Point(0,0);
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

            this.btnAdd.Text = "Add Patient";
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

            this.dataGridView1.DataBindingComplete += (s, e) => {
                if (this.dataGridView1.Columns.Contains("IsDeleted"))
                    this.dataGridView1.Columns["IsDeleted"].Visible = false;
                if (this.dataGridView1.Columns.Contains("PatientID"))
                    this.dataGridView1.Columns["PatientID"].Visible = false;
                if (this.dataGridView1.Columns.Contains("ProfilePhoto"))
                    this.dataGridView1.Columns["ProfilePhoto"].Visible = false;

                // Set specific column widths for better readability
                if (this.dataGridView1.Columns.Contains("FirstName")) this.dataGridView1.Columns["FirstName"].Width = 120;
                if (this.dataGridView1.Columns.Contains("LastName")) this.dataGridView1.Columns["LastName"].Width = 120;
                if (this.dataGridView1.Columns.Contains("ContactNumber")) this.dataGridView1.Columns["ContactNumber"].Width = 120;
                if (this.dataGridView1.Columns.Contains("Email")) this.dataGridView1.Columns["Email"].Width = 180;
                // AutoSizeColumnsMode is Fill, so remaining columns will adjust
            };
        }

        private byte[] ImageToByteArray(System.Drawing.Image image)
        {
            if (image == null) return null;
            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

        private System.Drawing.Image ByteArrayToImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;
            using (var ms = new MemoryStream(bytes))
            {
                return System.Drawing.Image.FromStream(ms);
            }
        }

        private void LoadPatients(string search = "")
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT p.*, r.RoomNumber 
                               FROM tblPatient p 
                               LEFT JOIN tblRoom r ON p.PatientID = r.PatientID
                               WHERE p.IsDeleted = 0 AND 
                               (p.FirstName LIKE @search OR 
                                p.LastName LIKE @search OR 
                                CAST(p.PatientID AS VARCHAR) LIKE @search OR
                                p.ContactNumber LIKE @search OR
                                p.Email LIKE @search OR
                                p.Address LIKE @search)"; // Expanded search fields
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                // Bind DataTable to DataGridView
                dataGridView1.DataSource = dt;
                // Add image column if not present
                if (!dataGridView1.Columns.Contains("ProfilePhotoDisplay"))
                {
                    DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
                    imgCol.Name = "ProfilePhotoDisplay";
                    imgCol.HeaderText = "";
                    imgCol.Width = 60;
                    imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
                    dataGridView1.Columns.Insert(0, imgCol);
                }
                // Fill image column
                foreach (DataGridViewRow dgRow in dataGridView1.Rows)
                {
                    if (dgRow.IsNewRow) continue;
                    var row = ((DataRowView)dgRow.DataBoundItem)?.Row;
                    if (row != null && row["ProfilePhoto"] != DBNull.Value)
                        dgRow.Cells["ProfilePhotoDisplay"].Value = ByteArrayToImage((byte[])row["ProfilePhoto"]);
                    else
                        dgRow.Cells["ProfilePhotoDisplay"].Value = null;
                }
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
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO tblPatient (FirstName, LastName, DateOfBirth, Gender, BloodType, ContactNumber, Email, Address, InsuranceNumber, PatientFamily, Status, ProfilePhoto, IsDeleted) 
                                   VALUES (@FirstName, @LastName, @DateOfBirth, @Gender, @BloodType, @ContactNumber, @Email, @Address, @InsuranceNumber, @Family, @Status, @ProfilePhoto, 0)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@FirstName", form.txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@LastName", form.txtLastName.Text);
                    cmd.Parameters.AddWithValue("@DateOfBirth", form.dtpDateOfBirth.Value);
                    cmd.Parameters.AddWithValue("@Gender", form.cmbGender.Text);
                    cmd.Parameters.AddWithValue("@BloodType", form.cmbBloodType.Text);
                    cmd.Parameters.AddWithValue("@ContactNumber", form.txtContactNumber.Text);
                    cmd.Parameters.AddWithValue("@Email", form.txtEmail.Text);
                    cmd.Parameters.AddWithValue("@Address", form.txtAddress.Text);
                    cmd.Parameters.AddWithValue("@InsuranceNumber", form.txtInsuranceNumber.Text);
                    cmd.Parameters.AddWithValue("@Family", form.txtFamily.Text);
                    cmd.Parameters.AddWithValue("@Status", form.cmbStatus.Text);
                    cmd.Parameters.AddWithValue("@ProfilePhoto", (object)ImageToByteArray(form.picPhoto.Image) ?? DBNull.Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
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
            // Fill form with selected row data
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
            // Load image
            if (row.Cells["ProfilePhoto"].Value != DBNull.Value && row.Cells["ProfilePhoto"].Value != null)
                form.picPhoto.Image = ByteArrayToImage((byte[])row.Cells["ProfilePhoto"].Value);
            // Load Room Number
            if (dataGridView1.Columns.Contains("RoomNumber") && row.Cells["RoomNumber"].Value != DBNull.Value)
            {
                form.txtRoomNumber.Text = row.Cells["RoomNumber"].Value?.ToString();
            }
            else
            {
                form.txtRoomNumber.Text = ""; // Clear if no room is assigned
            }

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
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE tblPatient SET IsDeleted = 1 WHERE PatientID = @PatientID"; // Soft delete
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@PatientID", patientId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadPatients();
                MessageBox.Show("Patient deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
