using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace HMS
{
    public partial class SupplierControl : UserControl
    {
        private DataGridView dataGridView1;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnUpdate;
        private Button btnLogout;
        private FlowLayoutPanel buttonPanel;
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";

        public SupplierControl()
        {
            InitializeComponent();
            LoadSuppliers();
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
            lblTitle.Text = "Suppliers";
            UIHelper.StyleLabelTitle(lblTitle);

            // Search
            Panel searchPanel = new Panel();
            UIHelper.ApplyPanelStyles(searchPanel);
            searchPanel.Dock = DockStyle.Fill;
            searchPanel.Height = 40;
            this.txtSearch.Dock = DockStyle.Left;
            this.txtSearch.Width = 320;
            UIHelper.StyleTextBox(this.txtSearch);
            searchPanel.Controls.Add(this.txtSearch);

            // DataGridView
            this.dataGridView1.Dock = DockStyle.Fill;
            UIHelper.StyleDataGridView(this.dataGridView1);

            // Button panel
            UIHelper.ApplyPanelStyles(this.buttonPanel);
            this.buttonPanel.Dock = DockStyle.Fill;
            this.buttonPanel.Height = 48;
            this.buttonPanel.Padding = new Padding(0, 8, 0, 0);

            // Style and add buttons to buttonPanel
            this.btnAdd.Text = "Add Supplier";
            this.btnDelete.Text = "Delete";
            this.btnUpdate.Text = "Update";
            this.btnLogout.Text = "Log Out";
            foreach (Button btn in new[] { btnAdd, btnDelete, btnUpdate, btnLogout })
            {
                UIHelper.StyleButton(btn);
                btn.Width = 120;
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

        private void LoadSuppliers(string search = "")
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT * FROM tblSupplier WHERE IsDeleted = 0 AND (CAST(SupplierID AS VARCHAR) LIKE @search OR SupplierName LIKE @search OR ContactPerson LIKE @search OR Email LIKE @search OR Phone LIKE @search OR Address LIKE @search)";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadSuppliers(txtSearch.Text);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new SupplierForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO tblSupplier (SupplierName, ContactPerson, 
                                   Email, Phone, Address, IsDeleted) 
                                   VALUES (@SupplierName, @ContactPerson, 
                                   @Email, @Phone, @Address, 0)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SupplierName", form.txtSupplierName.Text);
                    cmd.Parameters.AddWithValue("@ContactPerson", form.txtContactPerson.Text);
                    cmd.Parameters.AddWithValue("@Email", form.txtEmail.Text);
                    cmd.Parameters.AddWithValue("@Phone", form.txtPhone.Text);
                    cmd.Parameters.AddWithValue("@Address", form.txtAddress.Text);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadSuppliers();
                MessageBox.Show("Supplier added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a supplier to delete.");
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"UPDATE tblSupplier SET IsDeleted = 1 WHERE SupplierID = @SupplierID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@SupplierID", row.Cells["SupplierID"].Value);
                con.Open();
                cmd.ExecuteNonQuery();
            }
            LoadSuppliers();
            MessageBox.Show("Supplier deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a supplier to update.");
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            var form = new SupplierForm();
            
            // Fill form with selected row data
            form.txtSupplierName.Text = row.Cells["SupplierName"].Value?.ToString();
            form.txtContactPerson.Text = row.Cells["ContactPerson"].Value?.ToString();
            form.txtEmail.Text = row.Cells["Email"].Value?.ToString();
            form.txtPhone.Text = row.Cells["Phone"].Value?.ToString();
            form.txtAddress.Text = row.Cells["Address"].Value?.ToString();

            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE tblSupplier SET SupplierName=@SupplierName, 
                                   ContactPerson=@ContactPerson, Email=@Email, 
                                   Phone=@Phone, Address=@Address 
                                   WHERE SupplierID=@SupplierID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SupplierID", row.Cells["SupplierID"].Value);
                    cmd.Parameters.AddWithValue("@SupplierName", form.txtSupplierName.Text);
                    cmd.Parameters.AddWithValue("@ContactPerson", form.txtContactPerson.Text);
                    cmd.Parameters.AddWithValue("@Email", form.txtEmail.Text);
                    cmd.Parameters.AddWithValue("@Phone", form.txtPhone.Text);
                    cmd.Parameters.AddWithValue("@Address", form.txtAddress.Text);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadSuppliers();
                MessageBox.Show("Supplier updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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