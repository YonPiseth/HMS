using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using HMS.Forms;

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
        private Button btnGenerateInvoice;
        private FlowLayoutPanel buttonPanel;
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";

        public BillingControl()
        {
            InitializeComponent();
            LoadBills();
            // StyleGridAndButtons(); // Removed: Styling handled in InitializeComponent
        }

        private void InitializeComponent()
        {
            this.dataGridView1 = new DataGridView();
            this.txtSearch = new TextBox();
            this.btnAdd = new Button();
            this.btnDelete = new Button();
            this.btnUpdate = new Button();
            this.btnLogout = new Button();
            this.btnGenerateInvoice = new Button();
            this.buttonPanel = new FlowLayoutPanel();

            // txtSearch
            UIHelper.StyleTextBox(this.txtSearch); // Apply text box styling
            this.txtSearch.Location = new System.Drawing.Point(20, 20);
            this.txtSearch.Size = new System.Drawing.Size(300, 27);
            this.txtSearch.PlaceholderText = "Search Patient";
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);

            // dataGridView1
            UIHelper.StyleDataGridView(this.dataGridView1); // Apply DataGridView styling
            this.dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dataGridView1.Dock = DockStyle.Fill;
            this.dataGridView1.DataBindingComplete += (s, e) => {
                foreach (DataGridViewColumn col in this.dataGridView1.Columns)
                    col.MinimumWidth = 80;
            };

            // Create button panel as FlowLayoutPanel
            UIHelper.ApplyPanelStyles(this.buttonPanel); // Apply panel styling
            this.buttonPanel.Dock = DockStyle.Bottom;
            this.buttonPanel.Height = 60;
            this.buttonPanel.Padding = new Padding(10, 10, 10, 10);
            this.buttonPanel.FlowDirection = FlowDirection.LeftToRight;
            this.buttonPanel.WrapContents = false;
            this.buttonPanel.AutoSize = false;

            // Style and add buttons to buttonPanel
            this.btnAdd.Text = "Add Bill";
            this.btnDelete.Text = "Delete";
            this.btnUpdate.Text = "Update";
            this.btnLogout.Text = "Log Out";
            this.btnGenerateInvoice.Text = "Generate Invoice";
            foreach (Button btn in new[] { btnAdd, btnDelete, btnUpdate, btnLogout, btnGenerateInvoice })
            {
                UIHelper.StyleButton(btn); // Apply button styling
                btn.Width = 120; // Specific width for this control
                btn.Margin = new Padding(10, 0, 0, 0);
                this.buttonPanel.Controls.Add(btn);
            }
            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            this.btnUpdate.Click += new EventHandler(this.btnUpdate_Click);
            this.btnLogout.Click += new EventHandler(this.btnLogout_Click);
            this.btnGenerateInvoice.Click += new EventHandler(this.btnGenerateInvoice_Click);

            // Add controls
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.txtSearch);
            this.Size = new System.Drawing.Size(900, 500);
            this.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // Add a title label and search panel at the top, similar to DoctorControl/PatientControl
            Label lblTitle = new Label();
            lblTitle.Text = "Billing";
            UIHelper.StyleLabelTitle(lblTitle); // Apply title label styling
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 48;

            Panel searchPanel = new Panel();
            UIHelper.ApplyPanelStyles(searchPanel); // Apply panel styling
            searchPanel.Dock = DockStyle.Top;
            searchPanel.Height = 48;
            this.txtSearch.Dock = DockStyle.Left;
            this.txtSearch.Width = 320;
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

        private void LoadBills(string search = "")
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT b.BillingID, p.FirstName + ' ' + p.LastName as PatientName, 
                                b.BillingDate, b.SubTotal, b.DiscountPercentage, b.TaxPercentage, 
                                b.GrandTotal 
                               FROM tblBilling b 
                               LEFT JOIN tblPatient p ON b.PatientID = p.PatientID 
                               WHERE b.IsDeleted = 0 AND 
                               (p.FirstName + ' ' + p.LastName LIKE @search OR 
                                CONVERT(NVARCHAR, b.BillingDate, 101) LIKE @search)";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;

                // Optionally hide original ID columns if PatientName is sufficient
                if (dataGridView1.Columns.Contains("PatientID"))
                {
                    dataGridView1.Columns["PatientID"].Visible = false;
                }
                if (dataGridView1.Columns.Contains("BillingID"))
                {
                    dataGridView1.Columns["BillingID"].Visible = false;
                }
                if (dataGridView1.Columns.Contains("BillingDate"))
                {
                    dataGridView1.Columns["BillingDate"].HeaderText = "Billing Date";
                    dataGridView1.Columns["BillingDate"].DefaultCellStyle.Format = "MM/dd/yyyy";
                }
                if (dataGridView1.Columns.Contains("SubTotal"))
                {
                    dataGridView1.Columns["SubTotal"].DefaultCellStyle.Format = "C";
                }
                if (dataGridView1.Columns.Contains("GrandTotal"))
                {
                    dataGridView1.Columns["GrandTotal"].DefaultCellStyle.Format = "C";
                }
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
                    con.Open(); // Open connection once for all inserts

                    // Insert into tblBilling
                    string billingQuery = @"INSERT INTO tblBilling (PatientID, BillingDate, 
                                        SubTotal, DiscountPercentage, TaxPercentage, GrandTotal, IsDeleted) 
                                        VALUES (@PatientID, @BillingDate, 
                                        @SubTotal, @DiscountPercentage, @TaxPercentage, @GrandTotal, 0);
                                        SELECT SCOPE_IDENTITY();"; // Get the newly created BillingID
                    SqlCommand billingCmd = new SqlCommand(billingQuery, con);
                    billingCmd.Parameters.AddWithValue("@PatientID", form.cmbPatient.SelectedValue);
                    billingCmd.Parameters.AddWithValue("@BillingDate", form.dtpInvoiceDate.Value.Date);
                    billingCmd.Parameters.AddWithValue("@SubTotal", decimal.Parse(form.txtSubTotal.Text));
                    billingCmd.Parameters.AddWithValue("@DiscountPercentage", form.numDiscount.Value);
                    billingCmd.Parameters.AddWithValue("@TaxPercentage", form.numTaxRate.Value);
                    billingCmd.Parameters.AddWithValue("@GrandTotal", decimal.Parse(form.txtGrandTotal.Text));

                    int billingID = Convert.ToInt32(billingCmd.ExecuteScalar());

                    // Insert into tblBillingLineItem
                    foreach (DataGridViewRow row in form.dgvLineItems.Rows)
                    {
                        if (row.IsNewRow) continue;

                        string description = row.Cells["Description"].Value?.ToString();
                        decimal quantity = 0m;
                        decimal unitPrice = 0m;
                        decimal lineTotal = 0m;

                        if (decimal.TryParse(row.Cells["Quantity"].Value?.ToString(), out quantity) &&
                            decimal.TryParse(row.Cells["UnitPrice"].Value?.ToString(), out unitPrice) &&
                            decimal.TryParse(row.Cells["LineTotal"].Value?.ToString(), out lineTotal))
                        {
                            string lineItemQuery = @"INSERT INTO tblBillingLineItem (BillingID, Description, Quantity, UnitPrice, LineTotal) 
                                                   VALUES (@BillingID, @Description, @Quantity, @UnitPrice, @LineTotal)";
                            SqlCommand lineItemCmd = new SqlCommand(lineItemQuery, con);
                            lineItemCmd.Parameters.AddWithValue("@BillingID", billingID);
                            lineItemCmd.Parameters.AddWithValue("@Description", description);
                            lineItemCmd.Parameters.AddWithValue("@Quantity", quantity);
                            lineItemCmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                            lineItemCmd.Parameters.AddWithValue("@LineTotal", lineTotal);
                            lineItemCmd.ExecuteNonQuery();
                        }
                    }
                }
                LoadBills();
                MessageBox.Show("Bill created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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

            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            int billingID = Convert.ToInt32(selectedRow.Cells["BillingID"].Value);
            
            BillingForm form = new BillingForm(billingID);
            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Update tblBilling
                    string billingUpdateQuery = @"UPDATE tblBilling SET PatientID=@PatientID, BillingDate=@BillingDate, 
                                                SubTotal=@SubTotal, DiscountPercentage=@DiscountPercentage, 
                                                TaxPercentage=@TaxPercentage, GrandTotal=@GrandTotal, 
                                                LastModifiedDate=@LastModifiedDate 
                                                WHERE BillingID=@BillingID";
                    SqlCommand billingCmd = new SqlCommand(billingUpdateQuery, con);
                    billingCmd.Parameters.AddWithValue("@PatientID", form.cmbPatient.SelectedValue);
                    billingCmd.Parameters.AddWithValue("@BillingDate", form.dtpInvoiceDate.Value.Date);
                    billingCmd.Parameters.AddWithValue("@SubTotal", decimal.Parse(form.txtSubTotal.Text));
                    billingCmd.Parameters.AddWithValue("@DiscountPercentage", form.numDiscount.Value);
                    billingCmd.Parameters.AddWithValue("@TaxPercentage", form.numTaxRate.Value);
                    billingCmd.Parameters.AddWithValue("@GrandTotal", decimal.Parse(form.txtGrandTotal.Text));
                    billingCmd.Parameters.AddWithValue("@LastModifiedDate", DateTime.Now);
                    billingCmd.Parameters.AddWithValue("@BillingID", billingID);
                    billingCmd.ExecuteNonQuery();

                    // Clear existing line items and re-insert for simplicity
                    string deleteLineItemsQuery = "DELETE FROM tblBillingLineItem WHERE BillingID = @BillingID";
                    SqlCommand deleteLineItemsCmd = new SqlCommand(deleteLineItemsQuery, con);
                    deleteLineItemsCmd.Parameters.AddWithValue("@BillingID", billingID);
                    deleteLineItemsCmd.ExecuteNonQuery();

                    foreach (DataGridViewRow row in form.dgvLineItems.Rows)
                    {
                        if (row.IsNewRow) continue;

                        string description = row.Cells["Description"].Value?.ToString();
                        decimal quantity = 0m;
                        decimal unitPrice = 0m;
                        decimal lineTotal = 0m;

                        if (decimal.TryParse(row.Cells["Quantity"].Value?.ToString(), out quantity) &&
                            decimal.TryParse(row.Cells["UnitPrice"].Value?.ToString(), out unitPrice) &&
                            decimal.TryParse(row.Cells["LineTotal"].Value?.ToString(), out lineTotal))
                        {
                            string lineItemQuery = @"INSERT INTO tblBillingLineItem (BillingID, Description, Quantity, UnitPrice, LineTotal) 
                                                   VALUES (@BillingID, @Description, @Quantity, @UnitPrice, @LineTotal)";
                            SqlCommand lineItemCmd = new SqlCommand(lineItemQuery, con);
                            lineItemCmd.Parameters.AddWithValue("@BillingID", billingID);
                            lineItemCmd.Parameters.AddWithValue("@Description", description);
                            lineItemCmd.Parameters.AddWithValue("@Quantity", quantity);
                            lineItemCmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                            lineItemCmd.Parameters.AddWithValue("@LineTotal", lineTotal);
                            lineItemCmd.ExecuteNonQuery();
                        }
                    }
                }
                LoadBills();
                MessageBox.Show("Bill updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            // Handle logout
            // For a UserControl, you might raise an event or access the parent form
            if (this.ParentForm is MainForm mainForm)
            {
                mainForm.Logout();
            }
        }

        private void btnGenerateInvoice_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a bill to generate an invoice for.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Assuming a single bill is selected for invoice generation
            DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
            int billingID = Convert.ToInt32(selectedRow.Cells["BillingID"].Value);
            int patientID = Convert.ToInt32(selectedRow.Cells["PatientID"].Value); // Assuming PatientID is available or can be fetched
            decimal totalAmount = Convert.ToDecimal(selectedRow.Cells["GrandTotal"].Value);

            // Create a new invoice record in tblInvoice
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // Check if an invoice already exists for this bill
                    string checkInvoiceQuery = "SELECT InvoiceID FROM tblBilling WHERE BillingID = @BillingID AND InvoiceID IS NOT NULL";
                    SqlCommand checkInvoiceCmd = new SqlCommand(checkInvoiceQuery, con, transaction);
                    checkInvoiceCmd.Parameters.AddWithValue("@BillingID", billingID);
                    object existingInvoiceID = checkInvoiceCmd.ExecuteScalar();

                    if (existingInvoiceID != null)
                    {
                        MessageBox.Show("An invoice for this bill already exists. Opening existing invoice.", "Invoice Exists", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        int existingInvoiceId = Convert.ToInt32(existingInvoiceID);
                        InvoiceReceiptForm receiptFormInstance = new InvoiceReceiptForm(existingInvoiceId);
                        receiptFormInstance.ShowDialog();
                        transaction.Commit();
                        return;
                    }

                    // 1. Insert into tblInvoice
                    string insertInvoiceQuery = @"INSERT INTO tblInvoice (
                                                PatientID, InvoiceDate, DueDate, TotalAmount, PaymentStatus, Notes, CreatedBy, CreatedDate)
                                                VALUES (@PatientID, @InvoiceDate, @DueDate, @TotalAmount, @PaymentStatus, @Notes, @CreatedBy, @CreatedDate);
                                                SELECT SCOPE_IDENTITY();";
                    SqlCommand invoiceCmd = new SqlCommand(insertInvoiceQuery, con, transaction);
                    invoiceCmd.Parameters.AddWithValue("@PatientID", patientID);
                    invoiceCmd.Parameters.AddWithValue("@InvoiceDate", DateTime.Now);
                    invoiceCmd.Parameters.AddWithValue("@DueDate", DateTime.Now.AddDays(30)); // Due in 30 days
                    invoiceCmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                    invoiceCmd.Parameters.AddWithValue("@PaymentStatus", "Pending");
                    invoiceCmd.Parameters.AddWithValue("@Notes", "Invoice generated from bill.");
                    invoiceCmd.Parameters.AddWithValue("@CreatedBy", 1); // Assuming a default user for now
                    invoiceCmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                    int invoiceID = Convert.ToInt32(invoiceCmd.ExecuteScalar());

                    // 2. Insert line items into tblInvoiceLineItem from tblBillingLineItem for this BillingID
                    string lineItemsQuery = "SELECT Description, Quantity, UnitPrice, LineTotal FROM tblBillingLineItem WHERE BillingID = @BillingID";
                    SqlCommand lineItemsCmd = new SqlCommand(lineItemsQuery, con, transaction);
                    lineItemsCmd.Parameters.AddWithValue("@BillingID", billingID);
                    SqlDataReader reader = lineItemsCmd.ExecuteReader();

                    // Use a separate command for insert within the same transaction
                    string insertInvoiceLineItemQuery = @"INSERT INTO tblInvoiceLineItem (InvoiceID, Description, Quantity, UnitPrice, LineTotal)
                                                        VALUES (@InvoiceID, @Description, @Quantity, @UnitPrice, @LineTotal)";
                    SqlCommand insertLineItemCmd = new SqlCommand(insertInvoiceLineItemQuery, con, transaction);
                    insertLineItemCmd.Parameters.Add("@InvoiceID", SqlDbType.Int).Value = invoiceID;
                    insertLineItemCmd.Parameters.Add("@Description", SqlDbType.NVarChar, 255);
                    insertLineItemCmd.Parameters.Add("@Quantity", SqlDbType.Decimal);
                    insertLineItemCmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal);
                    insertLineItemCmd.Parameters.Add("@LineTotal", SqlDbType.Decimal);

                    while (reader.Read())
                    {
                        insertLineItemCmd.Parameters["@Description"].Value = reader["Description"];
                        insertLineItemCmd.Parameters["@Quantity"].Value = reader["Quantity"];
                        insertLineItemCmd.Parameters["@UnitPrice"].Value = reader["UnitPrice"];
                        insertLineItemCmd.Parameters["@LineTotal"].Value = reader["LineTotal"];
                        insertLineItemCmd.ExecuteNonQuery();
                    }
                    reader.Close();

                    // 3. Update the tblBilling record to link it to the newly created InvoiceID
                    string updateBillingQuery = "UPDATE tblBilling SET InvoiceID = @InvoiceID WHERE BillingID = @BillingID";
                    SqlCommand updateBillingCmd = new SqlCommand(updateBillingQuery, con, transaction);
                    updateBillingCmd.Parameters.AddWithValue("@InvoiceID", invoiceID);
                    updateBillingCmd.Parameters.AddWithValue("@BillingID", billingID);
                    updateBillingCmd.ExecuteNonQuery();

                    transaction.Commit();
                    MessageBox.Show($"Invoice successfully generated with ID: {invoiceID}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Open the newly generated invoice receipt
                    InvoiceReceiptForm newReceiptForm = new InvoiceReceiptForm(invoiceID);
                    newReceiptForm.ShowDialog();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error generating invoice: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            LoadBills(); // Reload bills after invoice generation
        }
    }
}
