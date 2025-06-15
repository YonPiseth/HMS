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
            // StyleGridAndButtons(); // Removed: Styling handled in InitializeComponent
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
            UIHelper.StyleLabelTitle(lblTitle); // Apply title label styling

            // Search
            Panel searchPanel = new Panel();
            UIHelper.ApplyPanelStyles(searchPanel); // Apply panel styling
            searchPanel.Dock = DockStyle.Fill;
            searchPanel.Height = 40;
            this.txtSearch.Dock = DockStyle.Left;
            this.txtSearch.Width = 320;
            UIHelper.StyleTextBox(this.txtSearch); // Apply text box styling
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);

            this.cmbStatus.Dock = DockStyle.Left;
            this.cmbStatus.Width = 120;
            UIHelper.StyleComboBox(this.cmbStatus); // Apply combo box styling
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new string[] { "All", "Available", "Occupied", "Maintenance" });
            this.cmbStatus.SelectedIndex = 0;
            this.cmbStatus.SelectedIndexChanged += new EventHandler(this.cmbStatus_SelectedIndexChanged);
            searchPanel.Controls.Add(this.cmbStatus);
            searchPanel.Controls.Add(this.txtSearch);

            // DataGridView
            this.dataGridView1.Dock = DockStyle.Fill;
            UIHelper.StyleDataGridView(this.dataGridView1); // Apply DataGridView styling
            this.dataGridView1.DataBindingComplete += (s, e) => {
                if (this.dataGridView1.Columns.Contains("IsDeleted"))
                    this.dataGridView1.Columns["IsDeleted"].Visible = false;
            };

            // Button panel
            UIHelper.ApplyPanelStyles(this.buttonPanel); // Apply panel styling
            this.buttonPanel.Dock = DockStyle.Fill;
            this.buttonPanel.Height = 48;
            this.buttonPanel.Padding = new Padding(0, 8, 0, 0);

            // Style and add buttons to buttonPanel
            this.btnAdd.Text = "Add Room";
            this.btnDelete.Text = "Delete";
            this.btnUpdate.Text = "Update";
            this.btnLogout.Text = "Log Out";
            foreach (Button btn in new[] { btnAdd, btnDelete, btnUpdate, btnLogout })
            {
                UIHelper.StyleButton(btn); // Apply button styling
                btn.Width = 120; // Specific width for this control
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
            MessageBox.Show("Log Out clicked");
        }
    }
} 