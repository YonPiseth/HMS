using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace HMS
{
    public partial class MedicineControl : UserControl
    {
        private DataGridView dataGridView1;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnUpdate;
        private Button btnLogout;
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";
        private FlowLayoutPanel buttonPanel;

        public MedicineControl()
        {
            InitializeComponent();
            LoadMedicines();
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);
            this.dataGridView1.DataBindingComplete += (s, e) => {
                if (this.dataGridView1.Columns.Contains("IsDeleted"))
                    this.dataGridView1.Columns["IsDeleted"].Visible = false;
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

            // Title
            Label lblTitle = new Label();
            lblTitle.Text = "Medicines";
            UIHelper.StyleLabelTitle(lblTitle); // Apply title label styling

            // Search
            Panel searchPanel = new Panel();
            UIHelper.ApplyPanelStyles(searchPanel); // Apply panel styling
            searchPanel.Dock = DockStyle.Fill;
            searchPanel.Height = 40;
            this.txtSearch.Dock = DockStyle.Left;
            this.txtSearch.Width = 320;
            UIHelper.StyleTextBox(this.txtSearch); // Apply text box styling
            searchPanel.Controls.Add(this.txtSearch);

            // DataGridView
            this.dataGridView1.Dock = DockStyle.Fill;
            UIHelper.StyleDataGridView(this.dataGridView1); // Apply DataGridView styling

            // Button panel
            UIHelper.ApplyPanelStyles(this.buttonPanel); // Apply panel styling
            this.buttonPanel.Dock = DockStyle.Fill;
            this.buttonPanel.Height = 48;
            this.buttonPanel.Padding = new Padding(0, 8, 0, 0);

            // Style and add buttons to buttonPanel
            this.btnAdd.Text = "Add Medicine";
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

        private void LoadMedicines(string search = "")
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT * FROM tblMedicine WHERE (CAST(MedicineID AS VARCHAR) LIKE @search OR MedicineName LIKE @search OR Category LIKE @search OR Description LIKE @search)";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadMedicines(txtSearch.Text);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new MedicineForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO tblMedicine (MedicineName, Category, Description, UnitPrice, StockQuantity)
                                   VALUES (@MedicineName, @Category, @Description, @UnitPrice, @StockQuantity)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MedicineName", form.txtMedicineName.Text);
                    cmd.Parameters.AddWithValue("@Category", form.cmbCategory.Text);
                    cmd.Parameters.AddWithValue("@Description", form.txtDescription.Text);
                    cmd.Parameters.AddWithValue("@UnitPrice", form.numUnitPrice.Value);
                    cmd.Parameters.AddWithValue("@StockQuantity", form.numStockQuantity.Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadMedicines();
                MessageBox.Show("Medicine added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a medicine to delete.");
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete this medicine?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM tblMedicine WHERE MedicineID=@MedicineID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MedicineID", row.Cells["MedicineID"].Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadMedicines();
                MessageBox.Show("Medicine deleted successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a medicine to update.");
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            var form = new MedicineForm();
            // Fill form with selected row data
            form.txtMedicineName.Text = row.Cells["MedicineName"].Value?.ToString();
            form.cmbCategory.Text = row.Cells["Category"].Value?.ToString();
            form.txtDescription.Text = row.Cells["Description"].Value?.ToString();
            form.numUnitPrice.Value = Convert.ToDecimal(row.Cells["UnitPrice"].Value);
            form.numStockQuantity.Value = Convert.ToDecimal(row.Cells["StockQuantity"].Value);
            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE tblMedicine SET MedicineName=@MedicineName, Category=@Category, 
                                   Description=@Description, UnitPrice=@UnitPrice, StockQuantity=@StockQuantity 
                                   WHERE MedicineID=@MedicineID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@MedicineID", row.Cells["MedicineID"].Value);
                    cmd.Parameters.AddWithValue("@MedicineName", form.txtMedicineName.Text);
                    cmd.Parameters.AddWithValue("@Category", form.cmbCategory.Text);
                    cmd.Parameters.AddWithValue("@Description", form.txtDescription.Text);
                    cmd.Parameters.AddWithValue("@UnitPrice", form.numUnitPrice.Value);
                    cmd.Parameters.AddWithValue("@StockQuantity", form.numStockQuantity.Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadMedicines();
                MessageBox.Show("Medicine updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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