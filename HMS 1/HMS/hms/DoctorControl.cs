using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;

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
            StyleGridAndButtons();
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);
            this.dataGridView1.DataBindingComplete += (s, e) => {
                if (this.dataGridView1.Columns.Contains("IsDeleted"))
                    this.dataGridView1.Columns["IsDeleted"].Visible = false;
                if (this.dataGridView1.Columns.Contains("DoctorID"))
                    this.dataGridView1.Columns["DoctorID"].Visible = false;
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
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48)); // Title
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48)); // Search
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // DataGridView
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Button panel
            layout.Padding = new Padding(16);
            layout.BackColor = System.Drawing.Color.White;

            // Title with icon
            Panel titlePanel = new Panel();
            titlePanel.Dock = DockStyle.Fill;
            titlePanel.Height = 48;
            titlePanel.Padding = new Padding(0, 0, 0, 0);
            Label lblTitle = new Label();
            lblTitle.Text = "Doctors";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 18, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.FromArgb(24, 33, 54);
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            titlePanel.Controls.Add(lblTitle);
            lblTitle.Location = new System.Drawing.Point(0, 0);
            lblTitle.Width = 300;

            // Search
            Panel searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Fill;
            searchPanel.Height = 40;
            searchPanel.Padding = new Padding(0, 8, 0, 8);
            this.txtSearch.Dock = DockStyle.Left;
            this.txtSearch.Width = 320;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 11);
            this.txtSearch.BorderStyle = BorderStyle.FixedSingle;
            searchPanel.Controls.Add(this.txtSearch);

            // DataGridView
            this.dataGridView1.Dock = DockStyle.Fill;
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Button panel
            this.buttonPanel.Dock = DockStyle.Fill;
            this.buttonPanel.Height = 48;
            this.buttonPanel.Padding = new Padding(0, 8, 0, 0);
            this.btnAdd.Text = "Add Doctor";
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
            layout.Controls.Add(titlePanel, 0, 0);
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

        private void LoadDoctors(string search = "")
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT d.DoctorID, d.FirstName, d.LastName, t.SpecializationName, d.ContactNumber, d.Email, d.YearsOfExperience, d.ProfilePhoto
                                 FROM tblDoctor d
                                 LEFT JOIN tblDoctorType t ON d.SpecializationID = t.SpecializationID
                                 WHERE d.IsDeleted = 0 AND (CAST(d.DoctorID AS VARCHAR) LIKE @search OR d.FirstName LIKE @search OR d.LastName LIKE @search OR t.SpecializationName LIKE @search)";
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
                MessageBox.Show("Please select a doctor to update.");
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
                form.picPhoto.Image = ByteArrayToImage((byte[])row.Cells["ProfilePhoto"].Value);
            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE tblDoctor SET FirstName=@FirstName, LastName=@LastName, SpecializationID=@SpecializationID, ContactNumber=@ContactNumber, Email=@Email, YearsOfExperience=@YearsOfExperience, ProfilePhoto=@ProfilePhoto WHERE DoctorID=@DoctorID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DoctorID", row.Cells["DoctorID"].Value);
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
                MessageBox.Show("Doctor updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a doctor to delete.");
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete this doctor?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE tblDoctor SET IsDeleted=1 WHERE DoctorID=@DoctorID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DoctorID", row.Cells["DoctorID"].Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadDoctors();
                MessageBox.Show("Doctor deleted (soft delete) successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Log out (to be implemented)
            MessageBox.Show("Log Out clicked");
        }
    }
} 