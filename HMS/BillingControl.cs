using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using HMS.Forms;
using HMS.UI;
using HMS;

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

        public BillingControl()
        {
            InitializeComponent();
            LoadBills();
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

            UIHelper.StyleModernTextBox(this.txtSearch);
            this.txtSearch.Location = new Point(20, 20);
            this.txtSearch.Size = new Size(300, 27);
            this.txtSearch.PlaceholderText = "Search Patient";
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);

            UIHelper.StyleDataGridView(this.dataGridView1);
            Panel gridContainer = UIHelper.WrapDataGridViewInRoundedPanel(this.dataGridView1);
            this.dataGridView1.DataBindingComplete += (s, e) => {
                foreach (DataGridViewColumn col in this.dataGridView1.Columns)
                    col.MinimumWidth = 80;
            };

            this.buttonPanel.Dock = DockStyle.Fill;
            this.buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            this.buttonPanel.Padding = new Padding(0, UITheme.SpacingSM, UITheme.SpacingMD, UITheme.SpacingSM);
            this.buttonPanel.Height = 60;
            this.buttonPanel.AutoSize = false;
            this.buttonPanel.WrapContents = false;

            this.btnAdd.Text = "Add Bill";
            this.btnUpdate.Text = "Update";
            this.btnDelete.Text = "Delete";
            this.btnGenerateInvoice.Text = "Generate Invoice";
            this.btnLogout.Text = "Log Out";
            
            foreach (Button btn in new[] { btnAdd, btnUpdate, btnDelete, btnGenerateInvoice, btnLogout })
            {
                UIHelper.StyleModernButton(btn);
                btn.Width = 120;
                btn.Height = 40;
                btn.Margin = new Padding(UITheme.SpacingSM, 0, 0, 0);
            }
            
            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
            this.btnUpdate.Click += new EventHandler(this.btnUpdate_Click);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            this.btnGenerateInvoice.Click += new EventHandler(this.btnGenerateInvoice_Click);
            this.btnLogout.Click += new EventHandler(this.btnLogout_Click);
            
            this.buttonPanel.Controls.Add(this.btnLogout);
            this.buttonPanel.Controls.Add(this.btnGenerateInvoice);
            this.buttonPanel.Controls.Add(this.btnDelete);
            this.buttonPanel.Controls.Add(this.btnUpdate);
            this.buttonPanel.Controls.Add(this.btnAdd);

            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.txtSearch);
            this.Size = new Size(900, 500);
            this.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            Label lblTitle = new Label();
            lblTitle.Text = "Billing";
            UIHelper.StyleHeading(lblTitle, 3);
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 48;

            Panel titlePanel = new Panel();
            titlePanel.Dock = DockStyle.Fill;
            titlePanel.BackColor = Color.Transparent;
            titlePanel.Controls.Add(lblTitle);

            Panel searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Fill;
            searchPanel.Height = 48;
            this.txtSearch.Dock = DockStyle.Left;
            this.txtSearch.Width = 320;
            searchPanel.Controls.Add(this.txtSearch);

            this.Controls.Clear();
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
            layout.Controls.Add(titlePanel, 0, 0);
            layout.Controls.Add(searchPanel, 0, 1);
            layout.Controls.Add(gridContainer, 0, 2);
            layout.Controls.Add(this.buttonPanel, 0, 3);
            this.Controls.Add(layout);
        }

        private void LoadBills(string search = "")
        {
            using (Microsoft.Data.SqlClient.SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT b.BillingID, p.FirstName + ' ' + p.LastName as PatientName, 
                                b.BillingDate, b.SubTotal, b.DiscountPercentage, b.TaxPercentage, 
                                b.GrandTotal 
                               FROM tblBilling b 
                               LEFT JOIN tblPatient p ON b.PatientID = p.PatientID 
                               WHERE b.IsDeleted = 0 AND 
                               (p.FirstName + ' ' + p.LastName LIKE @search OR 
                                CONVERT(NVARCHAR, b.BillingDate, 101) LIKE @search)";
                Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(query, con);
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
                using (Microsoft.Data.SqlClient.SqlConnection con = DatabaseHelper.GetConnection())
                {
                    con.Open(); // Open connection once for all inserts

                    // Insert into tblBilling
                    string billingQuery = @"INSERT INTO tblBilling (PatientID, BillingDate, 
                                        SubTotal, DiscountPercentage, TaxPercentage, GrandTotal, IsDeleted) 
                                        VALUES (@PatientID, @BillingDate, 
                                        @SubTotal, @DiscountPercentage, @TaxPercentage, @GrandTotal, 0);
                                        SELECT SCOPE_IDENTITY();"; // Get the newly created BillingID
                    Microsoft.Data.SqlClient.SqlCommand billingCmd = new Microsoft.Data.SqlClient.SqlCommand(billingQuery, con);
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
                            Microsoft.Data.SqlClient.SqlCommand lineItemCmd = new Microsoft.Data.SqlClient.SqlCommand(lineItemQuery, con);
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
                using (Microsoft.Data.SqlClient.SqlConnection con = DatabaseHelper.GetConnection())
                {
                    string query = @"UPDATE tblBilling SET IsDeleted = 1 WHERE BillingID = @BillingID";
                    Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con);
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
                using (Microsoft.Data.SqlClient.SqlConnection con = DatabaseHelper.GetConnection())
                {
                    con.Open();

                    // Update tblBilling
                    string billingUpdateQuery = @"UPDATE tblBilling SET PatientID=@PatientID, BillingDate=@BillingDate, 
                                                SubTotal=@SubTotal, DiscountPercentage=@DiscountPercentage, 
                                                TaxPercentage=@TaxPercentage, GrandTotal=@GrandTotal, 
                                                LastModifiedDate=@LastModifiedDate 
                                                WHERE BillingID=@BillingID";
                    Microsoft.Data.SqlClient.SqlCommand billingCmd = new Microsoft.Data.SqlClient.SqlCommand(billingUpdateQuery, con);
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
                    Microsoft.Data.SqlClient.SqlCommand deleteLineItemsCmd = new Microsoft.Data.SqlClient.SqlCommand(deleteLineItemsQuery, con);
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
                            Microsoft.Data.SqlClient.SqlCommand lineItemCmd = new Microsoft.Data.SqlClient.SqlCommand(lineItemQuery, con);
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
            using (Microsoft.Data.SqlClient.SqlConnection con = DatabaseHelper.GetConnection())
            {
                con.Open();
                Microsoft.Data.SqlClient.SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    // Check if an invoice already exists for this bill
                    string checkInvoiceQuery = "SELECT InvoiceID FROM tblBilling WHERE BillingID = @BillingID AND InvoiceID IS NOT NULL";
                    Microsoft.Data.SqlClient.SqlCommand checkInvoiceCmd = new Microsoft.Data.SqlClient.SqlCommand(checkInvoiceQuery, con, transaction);
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
                    Microsoft.Data.SqlClient.SqlCommand invoiceCmd = new Microsoft.Data.SqlClient.SqlCommand(insertInvoiceQuery, con, transaction);
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
                    Microsoft.Data.SqlClient.SqlCommand lineItemsCmd = new Microsoft.Data.SqlClient.SqlCommand(lineItemsQuery, con, transaction);
                    lineItemsCmd.Parameters.AddWithValue("@BillingID", billingID);
                    Microsoft.Data.SqlClient.SqlDataReader reader = lineItemsCmd.ExecuteReader();

                    // Use a separate command for insert within the same transaction
                    string insertInvoiceLineItemQuery = @"INSERT INTO tblInvoiceLineItem (InvoiceID, Description, Quantity, UnitPrice, LineTotal)
                                                        VALUES (@InvoiceID, @Description, @Quantity, @UnitPrice, @LineTotal)";
                    Microsoft.Data.SqlClient.SqlCommand insertLineItemCmd = new Microsoft.Data.SqlClient.SqlCommand(insertInvoiceLineItemQuery, con, transaction);
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
                    Microsoft.Data.SqlClient.SqlCommand updateBillingCmd = new Microsoft.Data.SqlClient.SqlCommand(updateBillingQuery, con, transaction);
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
