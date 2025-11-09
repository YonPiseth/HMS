using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using HMS.UI;
using HMS;

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
        private FlowLayoutPanel buttonPanel;

        public RoomControl()
        {
            InitializeComponent();
            LoadRooms();
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

            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 4;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            layout.Padding = new Padding(16);
            layout.BackColor = Color.White;

            Label lblTitle = new Label();
            lblTitle.Text = "Rooms";
            UIHelper.StyleHeading(lblTitle, 3);

            Panel titlePanel = new Panel();
            titlePanel.Dock = DockStyle.Fill;
            titlePanel.BackColor = Color.Transparent;
            titlePanel.Controls.Add(lblTitle);

            Panel searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Fill;
            searchPanel.Height = 40;
            this.txtSearch.Dock = DockStyle.Left;
            this.txtSearch.Width = 320;
            UIHelper.StyleModernTextBox(this.txtSearch);
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);

            this.cmbStatus.Dock = DockStyle.Left;
            this.cmbStatus.Width = 120;
            UIHelper.StyleModernComboBox(this.cmbStatus);
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new string[] { "All", "Available", "Occupied", "Maintenance" });
            this.cmbStatus.SelectedIndex = 0;
            this.cmbStatus.SelectedIndexChanged += new EventHandler(this.cmbStatus_SelectedIndexChanged);
            searchPanel.Controls.Add(this.cmbStatus);
            searchPanel.Controls.Add(this.txtSearch);

            UIHelper.StyleDataGridView(this.dataGridView1);
            Panel gridContainer = UIHelper.WrapDataGridViewInRoundedPanel(this.dataGridView1);
            this.dataGridView1.DataBindingComplete += (s, e) => {
                if (this.dataGridView1.Columns.Contains("IsDeleted"))
                    this.dataGridView1.Columns["IsDeleted"].Visible = false;
            };

            this.buttonPanel.Dock = DockStyle.Fill;
            this.buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            this.buttonPanel.Padding = new Padding(0, UITheme.SpacingSM, UITheme.SpacingMD, UITheme.SpacingSM);
            this.buttonPanel.Height = 60;
            this.buttonPanel.AutoSize = false;
            this.buttonPanel.WrapContents = false;

            this.btnAdd.Text = "Add Room";
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
            layout.Controls.Add(searchPanel, 0, 1);
            layout.Controls.Add(gridContainer, 0, 2);
            layout.Controls.Add(this.buttonPanel, 0, 3);
            
            this.Controls.Clear();
            this.Controls.Add(layout);
            this.Size = new Size(900, 500);
        }

        private void LoadRooms(string search = "", string status = "All")
        {
            using (Microsoft.Data.SqlClient.SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT * FROM tblRoom WHERE IsDeleted = 0 AND (CAST(RoomID AS VARCHAR) LIKE @search OR RoomNumber LIKE @search OR RoomType LIKE @search OR CAST(Floor AS VARCHAR) LIKE @search OR CAST(Capacity AS VARCHAR) LIKE @search OR CAST(RatePerDay AS VARCHAR) LIKE @search OR Status LIKE @search)";
                if (status != "All")
                {
                    query += " AND Status = @status";
                }
                Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(query, con);
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
                using (Microsoft.Data.SqlClient.SqlConnection con = DatabaseHelper.GetConnection())
                {
                    string query = @"INSERT INTO tblRoom (RoomNumber, RoomType, Floor, Capacity, 
                                   RatePerDay, Status, IsDeleted) 
                                   VALUES (@RoomNumber, @RoomType, @Floor, @Capacity, 
                                   @RatePerDay, @Status, 0)";
                    Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con);
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
                using (Microsoft.Data.SqlClient.SqlConnection con = DatabaseHelper.GetConnection())
                {
                    string query = @"UPDATE tblRoom SET IsDeleted = 1 WHERE RoomNumber = @RoomNumber";
                    Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con);
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
            
            form.txtRoomNumber.Text = row.Cells["RoomNumber"].Value?.ToString();
            form.cmbRoomType.Text = row.Cells["RoomType"].Value?.ToString();
            form.numFloor.Value = Convert.ToDecimal(row.Cells["Floor"].Value);
            form.numCapacity.Value = Convert.ToDecimal(row.Cells["Capacity"].Value);
            form.numRatePerDay.Value = Convert.ToDecimal(row.Cells["RatePerDay"].Value);
            form.cmbStatus.Text = row.Cells["Status"].Value?.ToString();

            if (form.ShowDialog() == DialogResult.OK)
            {
                using (Microsoft.Data.SqlClient.SqlConnection con = DatabaseHelper.GetConnection())
                {
                    string query = @"UPDATE tblRoom SET RoomType=@RoomType, Floor=@Floor, 
                                   Capacity=@Capacity, RatePerDay=@RatePerDay, Status=@Status 
                                   WHERE RoomNumber=@RoomNumber";
                    Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con);
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
            if (this.ParentForm is MainForm mainForm)
            {
                mainForm.Logout();
            }
        }
    }
} 