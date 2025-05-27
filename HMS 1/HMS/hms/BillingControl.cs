using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace HMS
{
    public partial class BillingControl : UserControl
    {
        private DataGridView dataGridView1;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnUpdate;
        private Button btnLogout;
        private FlowLayoutPanel buttonPanel;
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";

        public BillingControl()
        {
            InitializeComponent();
            LoadBills();
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

            // txtSearch
            this.txtSearch.Location = new System.Drawing.Point(20, 20);
            this.txtSearch.Size = new System.Drawing.Size(300, 27);
            this.txtSearch.PlaceholderText = "Search Patient";
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);

            // dataGridView1
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView1.Dock = DockStyle.Fill;
            this.dataGridView1.DataBindingComplete += (s, e) => {
                foreach (DataGridViewColumn col in this.dataGridView1.Columns)
                    col.MinimumWidth = 80;
                this.dataGridView1.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
                this.dataGridView1.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10);
                this.dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.AliceBlue;
            };

            // Create button panel as FlowLayoutPanel
            this.buttonPanel = new FlowLayoutPanel();
            this.buttonPanel.Dock = DockStyle.Bottom;
            this.buttonPanel.Height = 60;
            this.buttonPanel.Padding = new Padding(10, 10, 10, 10);
            this.buttonPanel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.buttonPanel.FlowDirection = FlowDirection.LeftToRight;
            this.buttonPanel.WrapContents = false;
            this.buttonPanel.AutoSize = false;

            // Style and add buttons to buttonPanel
            this.btnAdd.Text = "Add Bill";
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
                this.buttonPanel.Controls.Add(btn);
            }
            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            this.btnUpdate.Click += new EventHandler(this.btnUpdate_Click);
            this.btnLogout.Click += new EventHandler(this.btnLogout_Click);

            // Add controls
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.txtSearch);
            this.Size = new System.Drawing.Size(900, 500);
            this.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // Add a title label and search panel at the top, similar to DoctorControl/PatientControl
            Label lblTitle = new Label();
            lblTitle.Text = "Billing";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 18, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.FromArgb(24, 33, 54);
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 48;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            Panel searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Height = 48;
            searchPanel.Padding = new Padding(0, 8, 0, 8);
            this.txtSearch.Dock = DockStyle.Left;
            this.txtSearch.Width = 320;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 11);
            this.txtSearch.BorderStyle = BorderStyle.FixedSingle;
            searchPanel.Controls.Add(this.txtSearch);

            // Remove old layout and add new layout
            this.Controls.Clear();
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 3;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48)); // Title
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48)); // Search
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // DataGridView
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Button panel
            layout.Padding = new Padding(16);
            layout.BackColor = System.Drawing.Color.White;
            layout.Controls.Add(lblTitle, 0, 0);
            layout.Controls.Add(searchPanel, 0, 1);
            layout.Controls.Add(this.dataGridView1, 0, 2);
            layout.Controls.Add(this.buttonPanel, 0, 3);
            this.Controls.Add(layout);
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

        private void LoadBills(string search = "")
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT b.*, p.FirstName + ' ' + p.LastName as PatientName 
                               FROM tblBilling b 
                               LEFT JOIN tblPatient p ON b.PatientID = p.PatientID 
                               WHERE b.IsDeleted = 0 AND 
                               (p.FirstName + ' ' + p.LastName LIKE @search OR 
                                b.InvoiceNumber LIKE @search)";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadBills(txtSearch.Text);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new BillingForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO tblBilling (PatientID, InvoiceNumber, InvoiceDate, 
                                   ConsultationFee, RoomCharges, MedicineCharges, OtherCharges, 
                                   TotalAmount, PaymentStatus, IsDeleted) 
                                   VALUES (@PatientID, @InvoiceNumber, @InvoiceDate, 
                                   @ConsultationFee, @RoomCharges, @MedicineCharges, @OtherCharges, 
                                   @TotalAmount, @PaymentStatus, 0)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@PatientID", form.cmbPatient.SelectedValue);
                    cmd.Parameters.AddWithValue("@InvoiceNumber", GenerateInvoiceNumber());
                    cmd.Parameters.AddWithValue("@InvoiceDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@ConsultationFee", form.numConsultationFee.Value);
                    cmd.Parameters.AddWithValue("@RoomCharges", form.numRoomCharges.Value);
                    cmd.Parameters.AddWithValue("@MedicineCharges", form.numMedicineCharges.Value);
                    cmd.Parameters.AddWithValue("@OtherCharges", form.numOtherCharges.Value);
                    cmd.Parameters.AddWithValue("@TotalAmount", form.numTotalAmount.Value);
                    cmd.Parameters.AddWithValue("@PaymentStatus", form.cmbPaymentStatus.Text);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadBills();
                MessageBox.Show("Invoice created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string GenerateInvoiceNumber()
        {
            return "INV-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(1000, 9999);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a bill to delete.");
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete this bill?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE tblBilling SET IsDeleted = 1 WHERE BillingID = @BillingID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@BillingID", row.Cells["BillingID"].Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadBills();
                MessageBox.Show("Bill deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a bill to update.");
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            var form = new BillingForm();
            
            // Fill form with selected row data
            form.cmbPatient.SelectedValue = row.Cells["PatientID"].Value;
            form.numConsultationFee.Value = Convert.ToDecimal(row.Cells["ConsultationFee"].Value);
            form.numRoomCharges.Value = Convert.ToDecimal(row.Cells["RoomCharges"].Value);
            form.numMedicineCharges.Value = Convert.ToDecimal(row.Cells["MedicineCharges"].Value);
            form.numOtherCharges.Value = Convert.ToDecimal(row.Cells["OtherCharges"].Value);
            form.numTotalAmount.Value = Convert.ToDecimal(row.Cells["TotalAmount"].Value);
            form.cmbPaymentStatus.Text = row.Cells["PaymentStatus"].Value?.ToString();

            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE tblBilling SET PatientID=@PatientID, ConsultationFee=@ConsultationFee, 
                                   RoomCharges=@RoomCharges, MedicineCharges=@MedicineCharges, 
                                   OtherCharges=@OtherCharges, TotalAmount=@TotalAmount, 
                                   PaymentStatus=@PaymentStatus WHERE BillingID=@BillingID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@BillingID", row.Cells["BillingID"].Value);
                    cmd.Parameters.AddWithValue("@PatientID", form.cmbPatient.SelectedValue);
                    cmd.Parameters.AddWithValue("@ConsultationFee", form.numConsultationFee.Value);
                    cmd.Parameters.AddWithValue("@RoomCharges", form.numRoomCharges.Value);
                    cmd.Parameters.AddWithValue("@MedicineCharges", form.numMedicineCharges.Value);
                    cmd.Parameters.AddWithValue("@OtherCharges", form.numOtherCharges.Value);
                    cmd.Parameters.AddWithValue("@TotalAmount", form.numTotalAmount.Value);
                    cmd.Parameters.AddWithValue("@PaymentStatus", form.cmbPaymentStatus.Text);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadBills();
                MessageBox.Show("Bill updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Log out (to be implemented)
            MessageBox.Show("Log Out clicked");
        }
    }
} 