using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;
using HMS.Forms;
using HMS;

namespace HMS
{
    public partial class InvoiceForm : Form
    {
        private int selectedPatientId;
        private SplitContainer splitContainer;
        private FlowLayoutPanel buttonPanel;

        public InvoiceForm(int patientId)
        {
            selectedPatientId = patientId;
            InitializeComponent();
            LoadPatientInfo();
            LoadPatientBills();
            this.ActiveControl = btnGenerateInvoice;
            this.SizeChanged += new EventHandler(InvoiceForm_SizeChanged);
        }

        private void InvoiceForm_SizeChanged(object sender, EventArgs e)
        {
            AdjustSplitterDistance();
        }

        private void SetupPatientBillsDataGridView()
        {
            dgvPatientBills.Columns.Clear();

            DataGridViewCheckBoxColumn selectColumn = new DataGridViewCheckBoxColumn();
            selectColumn.Name = "Select";
            selectColumn.HeaderText = "Select";
            selectColumn.Width = 50;
            selectColumn.MinimumWidth = 50;
            selectColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            selectColumn.ReadOnly = false;
            selectColumn.DataPropertyName = "IsSelected";
            dgvPatientBills.Columns.Add(selectColumn);

            dgvPatientBills.Columns.Add("BillingID", "Billing ID");
            dgvPatientBills.Columns["BillingID"].DataPropertyName = "BillingID";
            dgvPatientBills.Columns.Add("BillingDate", "Bill Date");
            dgvPatientBills.Columns["BillingDate"].DataPropertyName = "BillingDate";
            dgvPatientBills.Columns.Add("GrandTotal", "Total Amount");
            dgvPatientBills.Columns["GrandTotal"].DataPropertyName = "GrandTotal";
            dgvPatientBills.Columns.Add("ItemCount", "Items");
            dgvPatientBills.Columns["ItemCount"].DataPropertyName = "ItemCount";

            dgvPatientBills.Columns["BillingID"].Visible = false;
            dgvPatientBills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            foreach (DataGridViewColumn column in dgvPatientBills.Columns)
            {
                column.HeaderCell.Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void AdjustSplitterDistance()
        {
            if (splitContainer != null)
                splitContainer.SplitterDistance = (int)(splitContainer.Height * 0.6);
        }

        private void SetupSelectedBillsDataGridView()
        {
            dgvSelectedBills.Columns.Clear();
            dgvSelectedBills.Columns.Add("Description", "Description");
            dgvSelectedBills.Columns["Description"].DataPropertyName = "Description";
            dgvSelectedBills.Columns.Add("Quantity", "Qty");
            dgvSelectedBills.Columns["Quantity"].DataPropertyName = "Quantity";
            dgvSelectedBills.Columns.Add("UnitPrice", "Unit Price");
            dgvSelectedBills.Columns["UnitPrice"].DataPropertyName = "UnitPrice";
            dgvSelectedBills.Columns.Add("LineTotal", "Total");
            dgvSelectedBills.Columns["LineTotal"].DataPropertyName = "LineTotal";
            dgvSelectedBills.Columns.Add("BillingID", "BillingID");
            dgvSelectedBills.Columns["BillingID"].DataPropertyName = "BillingID";

            dgvSelectedBills.Columns["BillingID"].Visible = false;
            dgvSelectedBills.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            foreach (DataGridViewColumn column in dgvSelectedBills.Columns)
            {
                column.HeaderCell.Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void LoadPatientInfo()
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT FirstName, LastName, PatientID 
                               FROM tblPatient 
                               WHERE PatientID = @PatientID AND IsDeleted = 0";
                Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@PatientID", selectedPatientId);
                con.Open();
                using (Microsoft.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lblPatientInfo.Text = $"Patient: {reader["FirstName"]} {reader["LastName"]} (ID: {reader["PatientID"]})";
                    }
                }
            }
        }

        private void LoadPatientBills()
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT b.BillingID, b.BillingDate, b.GrandTotal,
                               (SELECT COUNT(*) FROM tblBillingLineItem WHERE BillingID = b.BillingID) as ItemCount
                               FROM tblBilling b
                               WHERE b.PatientID = @PatientID AND b.IsDeleted = 0 AND b.InvoiceID IS NULL
                               ORDER BY b.BillingDate DESC";
                Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@PatientID", selectedPatientId);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (!dt.Columns.Contains("IsSelected"))
                {
                    dt.Columns.Add("IsSelected", typeof(bool));
                    foreach (DataRow row in dt.Rows)
                        row["IsSelected"] = false;
                }

                dgvPatientBills.DataSource = dt;
                dgvPatientBills.ClearSelection();
                dgvPatientBills.CurrentCell = null;
                UpdateSelectedBillsGrid();
            }
        }

        private void DgvPatientBills_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvPatientBills.Columns["Select"].Index && e.RowIndex >= 0)
            {
                DataGridViewRow clickedRow = dgvPatientBills.Rows[e.RowIndex];
                DataRowView rowView = clickedRow.DataBoundItem as DataRowView;

                if (rowView != null && rowView.Row.Table.Columns.Contains("IsSelected"))
                {
                    bool currentValue = false;
                    if (rowView.Row["IsSelected"] != DBNull.Value)
                    {
                        currentValue = (bool)rowView.Row["IsSelected"];
                    }
                    rowView.Row["IsSelected"] = !currentValue; // Toggle the value

                    Console.WriteLine($"DEBUG: IsSelected for row {e.RowIndex} is now: {rowView.Row["IsSelected"]}");

                    dgvPatientBills.InvalidateRow(e.RowIndex); // Force the row to redraw
                    UpdateSelectedBillsGrid(); // Update the lower grid based on new selections
                }
            }
        }

        private void UpdateSelectedBillsGrid()
        {
            DataTable combinedLineItems = new DataTable();
            combinedLineItems.Columns.Add("Description", typeof(string));
            combinedLineItems.Columns.Add("Quantity", typeof(decimal));
            combinedLineItems.Columns.Add("UnitPrice", typeof(decimal));
            combinedLineItems.Columns.Add("LineTotal", typeof(decimal));
            combinedLineItems.Columns.Add("BillingID", typeof(int));

            foreach (DataGridViewRow row in dgvPatientBills.Rows)
            {
                if (row.Cells["Select"].Value != null && (bool)row.Cells["Select"].Value == true)
                {
                    int billingId = Convert.ToInt32(row.Cells["BillingID"].Value);
                    LoadBillDetailsIntoDataTable(billingId, combinedLineItems);
                }
            }
            dgvSelectedBills.DataSource = combinedLineItems;
            dgvSelectedBills.ClearSelection();
            dgvSelectedBills.CurrentCell = null;
        }

        private void LoadBillDetailsIntoDataTable(int billingId, DataTable targetTable)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT Description, Quantity, UnitPrice, LineTotal, BillingID
                               FROM tblBillingLineItem
                               WHERE BillingID = @BillingID";
                Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@BillingID", billingId);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // --- DIAGNOSTIC START ---
                Console.WriteLine($"DEBUG: Loading details for BillingID: {billingId}");
                if (dt.Columns.Contains("BillingID"))
                {
                    Console.WriteLine("DEBUG: dt contains BillingID column.");
                }
                else
                {
                    Console.WriteLine("DEBUG: dt DOES NOT contain BillingID column.");
                }
                Console.WriteLine("DEBUG: dt Columns: " + string.Join(", ", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
                Console.WriteLine($"DEBUG: dt Rows Count: {dt.Rows.Count}");
                // --- DIAGNOSTIC END ---

                foreach (DataRow dr in dt.Rows)
                {
                    targetTable.Rows.Add(dr["Description"], dr["Quantity"], dr["UnitPrice"], dr["LineTotal"], dr["BillingID"]);
                }
            }
        }

        private void BtnGenerateInvoice_Click(object sender, EventArgs e)
        {
            if (dgvSelectedBills.Rows.Count == 0)
            {
                MessageBox.Show("Please select at least one bill to generate an invoice.", "No Bills Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    using (Microsoft.Data.SqlClient.SqlTransaction transaction = con.BeginTransaction())
                    {
                        try
                        {
                            string insertInvoiceQuery = @"
                                INSERT INTO tblInvoice (
                                    PatientID, 
                                    InvoiceDate, 
                                    DueDate,
                                    TotalAmount, 
                                    PaymentStatus,
                                    Notes,
                                    CreatedBy,
                                    CreatedDate
                                ) VALUES (
                                    @PatientID, 
                                    @InvoiceDate, 
                                    @DueDate,
                                    @TotalAmount, 
                                    @PaymentStatus,
                                    @Notes,
                                    @CreatedBy,
                                    GETDATE()
                                );
                                SELECT SCOPE_IDENTITY();";

                            decimal totalAmount = CalculateTotalAmount();
                            DateTime invoiceDate = DateTime.Now;
                            DateTime dueDate = invoiceDate.AddDays(30); // Standard 30-day payment term

                            Microsoft.Data.SqlClient.SqlCommand cmdInvoice = new Microsoft.Data.SqlClient.SqlCommand(insertInvoiceQuery, con, transaction);
                            cmdInvoice.Parameters.AddWithValue("@PatientID", selectedPatientId);
                            cmdInvoice.Parameters.AddWithValue("@InvoiceDate", invoiceDate);
                            cmdInvoice.Parameters.AddWithValue("@DueDate", dueDate);
                            cmdInvoice.Parameters.AddWithValue("@TotalAmount", totalAmount);
                            cmdInvoice.Parameters.AddWithValue("@PaymentStatus", "Pending");
                            cmdInvoice.Parameters.AddWithValue("@Notes", "Generated from selected bills");
                            cmdInvoice.Parameters.AddWithValue("@CreatedBy", 1);

                            int invoiceId = Convert.ToInt32(cmdInvoice.ExecuteScalar());

                            string insertLineItemQuery = @"
                                INSERT INTO tblInvoiceLineItem (
                                    InvoiceID,
                                    Description,
                                    Quantity,
                                    UnitPrice,
                                    LineTotal,
                                    BillingID
                                ) VALUES (
                                    @InvoiceID,
                                    @Description,
                                    @Quantity,
                                    @UnitPrice,
                                    @LineTotal,
                                    @BillingID
                                )";

                            foreach (DataGridViewRow row in dgvSelectedBills.Rows)
                            {
                                DataRowView drv = row.DataBoundItem as DataRowView;
                                if (drv != null)
                                {
                                    Microsoft.Data.SqlClient.SqlCommand cmdLineItem = new Microsoft.Data.SqlClient.SqlCommand(insertLineItemQuery, con, transaction);
                                    cmdLineItem.Parameters.AddWithValue("@InvoiceID", invoiceId);
                                    cmdLineItem.Parameters.AddWithValue("@Description", drv["Description"]);
                                    cmdLineItem.Parameters.AddWithValue("@Quantity", drv["Quantity"]);
                                    cmdLineItem.Parameters.AddWithValue("@UnitPrice", drv["UnitPrice"]);
                                    cmdLineItem.Parameters.AddWithValue("@LineTotal", drv["LineTotal"]);
                                    cmdLineItem.Parameters.AddWithValue("@BillingID", drv["BillingID"]);
                                    cmdLineItem.ExecuteNonQuery();
                                }
                            }

                            string updateBillsQuery = @"
                                UPDATE tblBilling 
                                SET InvoiceID = @InvoiceID,
                                    LastModifiedDate = GETDATE()
                                WHERE BillingID = @BillingID";

                            foreach (DataGridViewRow row in dgvSelectedBills.Rows)
                            {
                                DataRowView drv = row.DataBoundItem as DataRowView;
                                if (drv != null)
                                {
                                    Microsoft.Data.SqlClient.SqlCommand cmdUpdateBill = new Microsoft.Data.SqlClient.SqlCommand(updateBillsQuery, con, transaction);
                                    cmdUpdateBill.Parameters.AddWithValue("@InvoiceID", invoiceId);
                                    cmdUpdateBill.Parameters.AddWithValue("@BillingID", drv["BillingID"]);
                                    cmdUpdateBill.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();

                            MessageBox.Show("Invoice generated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Open the InvoiceReceiptForm to display the newly generated invoice as a receipt
                            var receiptForm = new InvoiceReceiptForm(invoiceId);
                            receiptForm.ShowDialog();

                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Error generating invoice: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private decimal CalculateTotalAmount()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dgvSelectedBills.Rows)
            {
                DataRowView drv = row.DataBoundItem as DataRowView;
                if (drv != null)
                {
                    total += Convert.ToDecimal(drv["LineTotal"]);
                }
            }
            return total;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
} 