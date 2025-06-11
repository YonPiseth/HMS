using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace HMS
{
    public partial class RoomControl : UserControl
    {
        private DataGridView dataGridView1;
        private TextBox txtSearch;
        private ComboBox cmbStatus;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnUpdate;
        private Button btnLogout;
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";
        private FlowLayoutPanel buttonPanel;

        public RoomControl()
        {
            InitializeComponent();
            LoadRooms();
            StyleGridAndButtons();
        }

        private void InitializeComponent()
        {
            this.dataGridView1 = new DataGridView();
            this.txtSearch = new TextBox();
            this.cmbStatus = new ComboBox();
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
            lblTitle.Text = "Rooms";
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
            this.cmbStatus.Dock = DockStyle.Left;
            this.cmbStatus.Width = 120;
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new string[] { "All", "Available", "Occupied", "Maintenance" });
            this.cmbStatus.SelectedIndex = 0;
            this.cmbStatus.SelectedIndexChanged += new EventHandler(this.cmbStatus_SelectedIndexChanged);
            searchPanel.Controls.Add(this.cmbStatus);
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
            this.btnAdd.Text = "Add Room";
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

        private void LoadRooms(string search = "", string status = "All")
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT * FROM tblRoom WHERE IsDeleted = 0 AND (CAST(RoomID AS VARCHAR) LIKE @search OR RoomNumber LIKE @search OR RoomType LIKE @search OR CAST(Floor AS VARCHAR) LIKE @search OR CAST(Capacity AS VARCHAR) LIKE @search OR CAST(RatePerDay AS VARCHAR) LIKE @search OR Status LIKE @search)";
                if (status != "All")
                {
                    query += " AND Status = @status";
                }
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");
                if (status != "All")
                {
                    da.SelectCommand.Parameters.AddWithValue("@status", status);
                }
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadRooms(txtSearch.Text, cmbStatus.Text);
        }

        private void cmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRooms(txtSearch.Text, cmbStatus.Text);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new RoomForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO tblRoom (RoomNumber, RoomType, Floor, Capacity, 
                                   RatePerDay, Status, IsDeleted) 
                                   VALUES (@RoomNumber, @RoomType, @Floor, @Capacity, 
                                   @RatePerDay, @Status, 0)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@RoomNumber", form.txtRoomNumber.Text);
                    cmd.Parameters.AddWithValue("@RoomType", form.cmbRoomType.Text);
                    cmd.Parameters.AddWithValue("@Floor", form.numFloor.Value);
                    cmd.Parameters.AddWithValue("@Capacity", form.numCapacity.Value);
                    cmd.Parameters.AddWithValue("@RatePerDay", form.numRatePerDay.Value);
                    cmd.Parameters.AddWithValue("@Status", "Available");
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadRooms();
                MessageBox.Show("Room added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a room to delete.");
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete this room?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE tblRoom SET IsDeleted = 1 WHERE RoomNumber = @RoomNumber";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@RoomNumber", row.Cells["RoomNumber"].Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadRooms();
                MessageBox.Show("Room deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a room to update.");
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            var form = new RoomForm();
            
            // Fill form with selected row data
            form.txtRoomNumber.Text = row.Cells["RoomNumber"].Value?.ToString();
            form.cmbRoomType.Text = row.Cells["RoomType"].Value?.ToString();
            form.numFloor.Value = Convert.ToDecimal(row.Cells["Floor"].Value);
            form.numCapacity.Value = Convert.ToDecimal(row.Cells["Capacity"].Value);
            form.numRatePerDay.Value = Convert.ToDecimal(row.Cells["RatePerDay"].Value);
            form.cmbStatus.Text = row.Cells["Status"].Value?.ToString();

            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE tblRoom SET RoomType=@RoomType, Floor=@Floor, 
                                   Capacity=@Capacity, RatePerDay=@RatePerDay, Status=@Status 
                                   WHERE RoomNumber=@RoomNumber";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@RoomNumber", form.txtRoomNumber.Text);
                    cmd.Parameters.AddWithValue("@RoomType", form.cmbRoomType.Text);
                    cmd.Parameters.AddWithValue("@Floor", form.numFloor.Value);
                    cmd.Parameters.AddWithValue("@Capacity", form.numCapacity.Value);
                    cmd.Parameters.AddWithValue("@RatePerDay", form.numRatePerDay.Value);
                    cmd.Parameters.AddWithValue("@Status", form.cmbStatus.Text);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadRooms();
                MessageBox.Show("Room updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Log out (to be implemented)
            MessageBox.Show("Log Out clicked");
        }
    }
} 