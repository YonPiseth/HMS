using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;

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

            // Title with icon
            Panel titlePanel = new Panel();
            titlePanel.Dock = DockStyle.Fill;
            titlePanel.Height = 48;
            titlePanel.Padding = new Padding(0, 0, 0, 0);
            Label lblTitle = new Label();
            lblTitle.Text = "Patients";
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
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);
            searchPanel.Controls.Add(this.txtSearch);

            // DataGridView
            this.dataGridView1.Dock = DockStyle.Fill;

            // Button panel
            this.buttonPanel.Dock = DockStyle.Fill;
            this.buttonPanel.Height = 48;
            this.buttonPanel.Padding = new Padding(0, 8, 0, 0);

            // Style and add buttons to buttonPanel
            this.btnAdd.Text = "Add Patient";
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

            this.dataGridView1.DataBindingComplete += (s, e) => {
                if (this.dataGridView1.Columns.Contains("IsDeleted"))
                    this.dataGridView1.Columns["IsDeleted"].Visible = false;
                if (this.dataGridView1.Columns.Contains("PatientID"))
                    this.dataGridView1.Columns["PatientID"].Visible = false;
            };
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

        private void LoadPatients(string search = "")
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT * FROM tblPatient 
                               WHERE IsDeleted = 0 AND 
                               (FirstName LIKE @search OR 
                                LastName LIKE @search OR 
                                CAST(PatientID AS VARCHAR) LIKE @search)";
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
                    string query = @"INSERT INTO tblPatient (FirstName, LastName, DateOfBirth, Gender, BloodType, ContactNumber, Email, Address, InsuranceNumber, [Patient's Family], Status, ProfilePhoto, IsDeleted) VALUES (@FirstName, @LastName, @DateOfBirth, @Gender, @BloodType, @ContactNumber, @Email, @Address, @InsuranceNumber, @Family, @Status, @ProfilePhoto, 0)";
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
                MessageBox.Show("Please select a patient to update.");
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            var form = new PatientForm();
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
            form.txtFamily.Text = row.Cells["Patient's Family"].Value?.ToString();
            form.cmbStatus.Text = row.Cells["Status"].Value?.ToString();
            // Load image
            if (row.Cells["ProfilePhoto"].Value != DBNull.Value && row.Cells["ProfilePhoto"].Value != null)
                form.picPhoto.Image = ByteArrayToImage((byte[])row.Cells["ProfilePhoto"].Value);
            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE tblPatient SET FirstName=@FirstName, LastName=@LastName, DateOfBirth=@DateOfBirth, Gender=@Gender, BloodType=@BloodType, ContactNumber=@ContactNumber, Email=@Email, Address=@Address, InsuranceNumber=@InsuranceNumber, [Patient's Family]=@Family, Status=@Status, ProfilePhoto=@ProfilePhoto WHERE PatientID=@PatientID";
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
                    cmd.Parameters.AddWithValue("@PatientID", row.Cells["PatientID"].Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadPatients();
                MessageBox.Show("Patient updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a patient to delete.");
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete this patient?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE tblPatient SET IsDeleted=1 WHERE PatientID=@PatientID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@PatientID", row.Cells["PatientID"].Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadPatients();
                MessageBox.Show("Patient deleted (soft delete) successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Log out (to be implemented)
            MessageBox.Show("Log Out clicked");
        }
    }
}
