using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Drawing; // Added for Color and Point

namespace HMS
{
    public class DoctorControl : UserControl
    {
        private DataGridView dataGridView1;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnUpdate;
        private Button btnLogout;
        private FlowLayoutPanel buttonPanel;
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";

        public DoctorControl()
        {
            InitializeComponent();
            LoadDoctors();
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);
            this.dataGridView1.DataBindingComplete += (s, e) => {
                if (this.dataGridView1.Columns.Contains("IsDeleted"))
                    this.dataGridView1.Columns["IsDeleted"].Visible = false;
                if (this.dataGridView1.Columns.Contains("DoctorID"))
                    this.dataGridView1.Columns["DoctorID"].Visible = false;
                if (this.dataGridView1.Columns.Contains("ProfilePhoto"))
                    this.dataGridView1.Columns["ProfilePhoto"].Visible = false;

                // Set specific column widths for better readability
                if (this.dataGridView1.Columns.Contains("ProfilePhotoDisplay")) this.dataGridView1.Columns["ProfilePhotoDisplay"].Width = 60;
                if (this.dataGridView1.Columns.Contains("FirstName")) this.dataGridView1.Columns["FirstName"].Width = 120;
                if (this.dataGridView1.Columns.Contains("LastName")) this.dataGridView1.Columns["LastName"].Width = 120;
                if (this.dataGridView1.Columns.Contains("SpecializationName")) this.dataGridView1.Columns["SpecializationName"].Width = 150;
                if (this.dataGridView1.Columns.Contains("ContactNumber")) this.dataGridView1.Columns["ContactNumber"].Width = 120;
                if (this.dataGridView1.Columns.Contains("Email")) this.dataGridView1.Columns["Email"].Width = 180;
                if (this.dataGridView1.Columns.Contains("YearsOfExperience")) this.dataGridView1.Columns["YearsOfExperience"].Width = 80; // Smaller width for experience
            };
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
            layout.BackColor = Color.White; // Use Color from System.Drawing

            // Title with icon
            Panel titlePanel = new Panel();
            UIHelper.ApplyPanelStyles(titlePanel); // Apply panel styling
            titlePanel.Dock = DockStyle.Fill;
            Label lblTitle = new Label();
            lblTitle.Text = "Manage Doctors"; // More descriptive title
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

            this.btnAdd.Text = "Add Doctor";
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

        private byte[] ImageToByteArray(System.Drawing.Image image)
        {
            if (image == null) return null; // This is the crucial line to add or ensure is present
            using (var ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private System.Drawing.Image ByteArrayToImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;
            using (var ms = new MemoryStream(bytes))
            {
                // Create a new Bitmap from the stream to ensure it's independent
                return new System.Drawing.Bitmap(System.Drawing.Image.FromStream(ms));
            }
        }

        private void LoadDoctors(string search = "")
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT d.DoctorID, d.FirstName, d.LastName, t.SpecializationName, d.ContactNumber, d.Email, d.YearsOfExperience, d.ProfilePhoto
                                 FROM tblDoctor d
                                 LEFT JOIN tblDoctorType t ON d.SpecializationID = t.SpecializationID
                                 WHERE d.IsDeleted = 0 AND (
                                    CAST(d.DoctorID AS VARCHAR) LIKE @search OR
                                    d.FirstName LIKE @search OR
                                    d.LastName LIKE @search OR
                                    t.SpecializationName LIKE @search OR
                                    d.ContactNumber LIKE @search OR
                                    d.Email LIKE @search)"; // Expanded search fields
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
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
            LoadDoctors(txtSearch.Text);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new DoctorForm();
            form.Load += form.DoctorForm_Load;
            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO tblDoctor (FirstName, LastName, SpecializationID, ContactNumber, Email, YearsOfExperience, ProfilePhoto, IsDeleted)
                                   VALUES (@FirstName, @LastName, @SpecializationID, @ContactNumber, @Email, @YearsOfExperience, @ProfilePhoto, 0)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@FirstName", form.txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@LastName", form.txtLastName.Text);
                    cmd.Parameters.AddWithValue("@SpecializationID", form.cmbSpecialization.SelectedValue);
                    cmd.Parameters.AddWithValue("@ContactNumber", form.txtContactNumber.Text);
                    cmd.Parameters.AddWithValue("@Email", form.txtEmail.Text);
                    cmd.Parameters.AddWithValue("@YearsOfExperience", form.numYearsOfExperience.Value);
                    cmd.Parameters.AddWithValue("@ProfilePhoto", (object)ImageToByteArray(form.picPhoto.Image) ?? DBNull.Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadDoctors();
                MessageBox.Show("Doctor added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a doctor to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            var form = new DoctorForm();
            form.Load += form.DoctorForm_Load;
            // Fill form with selected row data
            form.txtFirstName.Text = row.Cells["FirstName"].Value?.ToString();
            form.txtLastName.Text = row.Cells["LastName"].Value?.ToString();
            form.cmbSpecialization.Text = row.Cells["SpecializationName"].Value?.ToString();
            form.txtContactNumber.Text = row.Cells["ContactNumber"].Value?.ToString();
            form.txtEmail.Text = row.Cells["Email"].Value?.ToString();
            form.numYearsOfExperience.Value = Convert.ToDecimal(row.Cells["YearsOfExperience"].Value);
            // Load image
            if (row.Cells["ProfilePhoto"].Value != DBNull.Value && row.Cells["ProfilePhoto"].Value != null)
            {
                try
                {
                    form.picPhoto.Image = ByteArrayToImage((byte[])row.Cells["ProfilePhoto"].Value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading profile photo for doctor: {ex.Message}", "Image Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    form.picPhoto.Image = null; // Set to null if image is corrupted
                }
            }
            else
            {
                form.picPhoto.Image = null; // Ensure it's null if no photo exists
            }

            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE tblDoctor SET FirstName=@FirstName, LastName=@LastName, SpecializationID=@SpecializationID, ContactNumber=@ContactNumber, Email=@Email, YearsOfExperience=@YearsOfExperience, ProfilePhoto=@ProfilePhoto WHERE DoctorID=@DoctorID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@FirstName", form.txtFirstName.Text);
                    cmd.Parameters.AddWithValue("@LastName", form.txtLastName.Text);
                    cmd.Parameters.AddWithValue("@SpecializationID", form.cmbSpecialization.SelectedValue);
                    cmd.Parameters.AddWithValue("@ContactNumber", form.txtContactNumber.Text);
                    cmd.Parameters.AddWithValue("@Email", form.txtEmail.Text);
                    cmd.Parameters.AddWithValue("@YearsOfExperience", form.numYearsOfExperience.Value);
                    cmd.Parameters.AddWithValue("@ProfilePhoto", (object)ImageToByteArray(form.picPhoto.Image) ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DoctorID", row.Cells["DoctorID"].Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadDoctors();
                MessageBox.Show("Doctor updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a doctor to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this doctor?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                int doctorId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["DoctorID"].Value);
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE tblDoctor SET IsDeleted = 1 WHERE DoctorID = @DoctorID"; // Soft delete
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadDoctors();
                MessageBox.Show("Doctor deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
