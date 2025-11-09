using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;
using HMS;

namespace HMS.Forms
{
    public partial class PatientInvoiceHistoryForm : Form
    {
        private int patientId;
        private DataGridView dgvInvoices;
        private Button btnViewReceipt;
        private Button btnClose;
        private Button btnUpdateStatus;

        public PatientInvoiceHistoryForm(int patientId)
        {
            this.patientId = patientId;
            InitializeComponent();
            this.Load += PatientInvoiceHistoryForm_Load;
        }

        private void InitializeComponent()
        {
            this.Text = "Patient Invoice History";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // DataGridView for invoices
            this.dgvInvoices = new DataGridView();
            this.dgvInvoices.Dock = DockStyle.Fill;
            this.dgvInvoices.AllowUserToAddRows = false;
            this.dgvInvoices.AllowUserToDeleteRows = false;
            this.dgvInvoices.ReadOnly = false;
            this.dgvInvoices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvInvoices.MultiSelect = false;
            this.dgvInvoices.AutoGenerateColumns = false;
            this.dgvInvoices.RowHeadersVisible = false;
            this.dgvInvoices.EditMode = DataGridViewEditMode.EditOnEnter;
            this.dgvInvoices.Columns.Add(new DataGridViewTextBoxColumn() { Name = "InvoiceID", HeaderText = "Invoice ID", DataPropertyName = "InvoiceID", Width = 80 });
            this.dgvInvoices.Columns.Add(new DataGridViewTextBoxColumn() { Name = "InvoiceDate", HeaderText = "Invoice Date", DataPropertyName = "InvoiceDate", Width = 120 });
            this.dgvInvoices.Columns.Add(new DataGridViewTextBoxColumn() { Name = "TotalAmount", HeaderText = "Total Amount", DataPropertyName = "TotalAmount", Width = 100 });

            DataGridViewComboBoxColumn statusColumn = new DataGridViewComboBoxColumn();
            statusColumn.Name = "PaymentStatus";
            statusColumn.HeaderText = "Status";
            statusColumn.DataPropertyName = "PaymentStatus";
            statusColumn.Width = 100;
            statusColumn.FlatStyle = FlatStyle.Flat;
            statusColumn.Items.AddRange("Pending", "Paid", "Cancelled"); // Add payment status options
            this.dgvInvoices.Columns.Add(statusColumn);

            this.dgvInvoices.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Notes", HeaderText = "Notes", DataPropertyName = "Notes", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            this.dgvInvoices.Columns["TotalAmount"].DefaultCellStyle.Format = "C";
            this.dgvInvoices.Columns["InvoiceDate"].DefaultCellStyle.Format = "MM/dd/yyyy";

            // Button Panel
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10),
                AutoSize = true
            };
            UIHelper.ApplyPanelStyles(buttonPanel);

            this.btnViewReceipt = new Button { Text = "View Receipt", AutoSize = true };
            this.btnClose = new Button { Text = "Close", AutoSize = true };
            this.btnUpdateStatus = new Button { Text = "Update Status", AutoSize = true };

            UIHelper.StyleButton(this.btnViewReceipt);
            UIHelper.StyleButton(this.btnClose);
            UIHelper.StyleButton(this.btnUpdateStatus);

            this.btnViewReceipt.Click += new EventHandler(this.btnViewReceipt_Click);
            this.btnClose.Click += new EventHandler(this.btnClose_Click);
            this.btnUpdateStatus.Click += new EventHandler(this.btnUpdateStatus_Click);

            buttonPanel.Controls.AddRange(new Control[] { this.btnClose, this.btnViewReceipt, this.btnUpdateStatus });

            // Main layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(10)
            };
            UIHelper.ApplyPanelStyles(mainLayout);
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // DataGrid
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Buttons

            mainLayout.Controls.Add(this.dgvInvoices, 0, 0);
            mainLayout.Controls.Add(buttonPanel, 0, 1);

            this.Controls.Add(mainLayout);

            this.dgvInvoices.CellValueChanged += new DataGridViewCellEventHandler(this.dgvInvoices_CellValueChanged);
            this.dgvInvoices.CurrentCellDirtyStateChanged += new EventHandler(this.dgvInvoices_CurrentCellDirtyStateChanged);
            this.dgvInvoices.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(this.dgvInvoices_EditingControlShowing);
        }

        private void PatientInvoiceHistoryForm_Load(object sender, EventArgs e)
        {
            LoadPatientInvoices();
        }

        private void LoadPatientInvoices()
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"
                    SELECT
                        InvoiceID,
                        InvoiceDate,
                        TotalAmount,
                        PaymentStatus,
                        Notes
                    FROM tblInvoice
                    WHERE PatientID = @PatientID
                    ORDER BY InvoiceDate DESC";

                using (Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PatientID", patientId);
                    Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvInvoices.DataSource = dt;
                }
            }
        }

        private void btnViewReceipt_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.SelectedRows.Count > 0)
            {
                int invoiceId = Convert.ToInt32(dgvInvoices.SelectedRows[0].Cells["InvoiceID"].Value);
                var receiptForm = new InvoiceReceiptForm(invoiceId);
                receiptForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select an invoice to view its receipt.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            dgvInvoices.EndEdit(); // End any current cell edit
            dgvInvoices.CommitEdit(DataGridViewDataErrorContexts.Commit);

            bool changesMade = false;
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                con.Open();
                foreach (DataGridViewRow row in dgvInvoices.Rows)
                {
                    if (row.Cells["InvoiceID"].Value != null)
                    {
                        int invoiceId = Convert.ToInt32(row.Cells["InvoiceID"].Value);
                        string currentStatusInDB = GetInvoicePaymentStatusFromDB(invoiceId);
                        string newStatus = row.Cells["PaymentStatus"].Value?.ToString();

                        if (newStatus != null && newStatus != currentStatusInDB)
                        {
                            string updateQuery = "UPDATE tblInvoice SET PaymentStatus = @PaymentStatus WHERE InvoiceID = @InvoiceID";
                            using (Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(updateQuery, con))
                            {
                                cmd.Parameters.AddWithValue("@PaymentStatus", newStatus);
                                cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);
                                cmd.ExecuteNonQuery();
                                changesMade = true;
                            }
                        }
                    }
                }
            }

            if (changesMade)
            {
                MessageBox.Show("Invoice payment statuses updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPatientInvoices();
            }
            else
            {
                MessageBox.Show("No changes to update.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string GetInvoicePaymentStatusFromDB(int invoiceId)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = "SELECT PaymentStatus FROM tblInvoice WHERE InvoiceID = @InvoiceID";
                using (Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? result.ToString() : string.Empty;
                }
            }
        }

        private void dgvInvoices_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvInvoices.Columns["PaymentStatus"].Index)
                dgvInvoices.EndEdit();
        }

        private void dgvInvoices_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvInvoices.IsCurrentCellDirty)
                dgvInvoices.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvInvoices_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvInvoices.CurrentCell.ColumnIndex == dgvInvoices.Columns["PaymentStatus"].Index)
            {
                ComboBox comboBox = e.Control as ComboBox;
                if (comboBox != null)
                {
                    comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                    comboBox.Visible = true;
                    comboBox.Enabled = true;
                }
            }
        }
    }
}