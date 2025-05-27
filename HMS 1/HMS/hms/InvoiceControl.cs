using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace HMS
{
    public partial class InvoiceControl : UserControl
    {
        private DataGridView dataGridView1;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnUpdate;
        private Button btnLogout;
        private string connectionString = @"Data Source=LAPTOP-NP47E1QQ\SQLEXPRESS01;Initial Catalog=HMS;Integrated Security=True";
        private FlowLayoutPanel buttonPanel;

        public InvoiceControl()
        {
            InitializeComponent();
            LoadInvoices();
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
            lblTitle.Text = "Invoices";
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
            this.btnAdd.Text = "Add Invoice";
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

        private void LoadInvoices(string search = "")
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT i.*, p.FirstName + ' ' + p.LastName as PatientName FROM tblInvoice i LEFT JOIN tblPatient p ON i.PatientID = p.PatientID WHERE (CAST(i.InvoiceID AS VARCHAR) LIKE @search OR p.FirstName + ' ' + p.LastName LIKE @search OR CAST(i.TotalAmount AS VARCHAR) LIKE @search OR i.PaymentStatus LIKE @search)";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadInvoices(txtSearch.Text);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new InvoiceForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO tblInvoice (PatientID, TotalAmount, InvoiceDate, PaymentStatus, DueDate)
                                   VALUES (@PatientID, @TotalAmount, @InvoiceDate, @PaymentStatus, @DueDate)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@PatientID", form.cmbPatient.SelectedValue);
                    cmd.Parameters.AddWithValue("@TotalAmount", form.numTotalAmount.Value);
                    cmd.Parameters.AddWithValue("@InvoiceDate", form.dtpInvoiceDate.Value);
                    cmd.Parameters.AddWithValue("@PaymentStatus", form.cmbPaymentStatus.Text);
                    cmd.Parameters.AddWithValue("@DueDate", form.dtpDueDate.Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadInvoices();
                MessageBox.Show("Invoice added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an invoice to delete.");
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete this invoice?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM tblInvoice WHERE InvoiceID=@InvoiceID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@InvoiceID", row.Cells["InvoiceID"].Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadInvoices();
                MessageBox.Show("Invoice deleted successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an invoice to update.");
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            var form = new InvoiceForm();
            // Fill form with selected row data
            form.cmbPatient.SelectedValue = row.Cells["PatientID"].Value;
            form.numTotalAmount.Value = Convert.ToDecimal(row.Cells["TotalAmount"].Value);
            form.dtpInvoiceDate.Value = Convert.ToDateTime(row.Cells["InvoiceDate"].Value);
            form.cmbPaymentStatus.Text = row.Cells["PaymentStatus"].Value?.ToString();
            form.dtpDueDate.Value = Convert.ToDateTime(row.Cells["DueDate"].Value);
            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE tblInvoice SET PatientID=@PatientID, TotalAmount=@TotalAmount, InvoiceDate=@InvoiceDate, PaymentStatus=@PaymentStatus, DueDate=@DueDate WHERE InvoiceID=@InvoiceID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@InvoiceID", row.Cells["InvoiceID"].Value);
                    cmd.Parameters.AddWithValue("@PatientID", form.cmbPatient.SelectedValue);
                    cmd.Parameters.AddWithValue("@TotalAmount", form.numTotalAmount.Value);
                    cmd.Parameters.AddWithValue("@InvoiceDate", form.dtpInvoiceDate.Value);
                    cmd.Parameters.AddWithValue("@PaymentStatus", form.cmbPaymentStatus.Text);
                    cmd.Parameters.AddWithValue("@DueDate", form.dtpDueDate.Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadInvoices();
                MessageBox.Show("Invoice updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Log Out clicked");
        }
    }
} 