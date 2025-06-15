using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Linq; // Added for FirstOrDefault

namespace HMS
{
    public partial class BillingForm : Form
    {
        public ComboBox cmbPatient;
        public DateTimePicker dtpInvoiceDate;
        public DataGridView dgvLineItems;
        public NumericUpDown numDiscount;
        public NumericUpDown numTaxRate;
        public TextBox txtSubTotal;
        public TextBox txtGrandTotal;
        private Button btnSave;
        private Button btnCancel;
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";
        private int? currentBillingID; // Nullable to indicate new or existing bill

        public BillingForm()
        {
            InitializeComponent();
            LoadPatients();
            SetupDataGridView();
            CalculateTotal(); // Initial calculation
        }

        public BillingForm(int billingID) : this()
        {
            this.Text = "Update Bill";
            currentBillingID = billingID;
            LoadBillData(billingID);
        }

        private void InitializeComponent()
        {
            this.cmbPatient = new ComboBox();
            this.dtpInvoiceDate = new DateTimePicker();
            this.dgvLineItems = new DataGridView();
            this.numDiscount = new NumericUpDown();
            this.numTaxRate = new NumericUpDown();
            this.txtSubTotal = new TextBox();
            this.txtGrandTotal = new TextBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            // Form settings
            this.Text = "Create Invoice";
            this.Size = new System.Drawing.Size(800, 750); // Increased size to give more room for line items
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Layout using TableLayoutPanel for better organization
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.RowCount = 10; // Changed to 10 rows to accommodate all controls
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

            // Row styles for each row
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Row 0: Patient
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Row 1: Invoice Date
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // Row 2: lblLineItems (Invoice Items Label)
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Row 3: dgvLineItems (takes remaining percentage height)
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Row 4: Sub Total
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Row 5: Discount
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Row 6: Tax
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Row 7: Grand Total
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Row 8: Buttons
            mainLayout.Padding = new Padding(10);
            mainLayout.AutoScroll = true;

            // Patient
            Label lblPatient = new Label { Text = "Patient:", TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
            this.cmbPatient.Dock = DockStyle.Fill;
            this.cmbPatient.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbPatient.DisplayMember = "FullName";
            this.cmbPatient.ValueMember = "PatientID";
            this.cmbPatient.SelectedIndexChanged += (s, e) => CalculateTotal(); // Recalculate on patient change

            // Invoice Date
            Label lblInvoiceDate = new Label { Text = "Invoice Date:", TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
            this.dtpInvoiceDate.Dock = DockStyle.Fill;
            this.dtpInvoiceDate.Format = DateTimePickerFormat.Short;
            this.dtpInvoiceDate.Value = DateTime.Now;

            // Line Items DataGridView
            Label lblLineItems = new Label { Text = "Invoice Items:", TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
            this.dgvLineItems.Dock = DockStyle.Fill;
            this.dgvLineItems.AllowUserToAddRows = true;
            this.dgvLineItems.AllowUserToDeleteRows = true;
            this.dgvLineItems.AutoGenerateColumns = false;
            this.dgvLineItems.CellEndEdit += (s, e) => CalculateTotal();
            this.dgvLineItems.RowsRemoved += (s, e) => CalculateTotal();
            this.dgvLineItems.DataError += (s, e) => {
                // Handle data entry errors (e.g., non-numeric input in numeric columns)
                if (e.ColumnIndex == dgvLineItems.Columns["Quantity"].Index || e.ColumnIndex == dgvLineItems.Columns["UnitPrice"].Index)
                {
                    MessageBox.Show("Please enter a valid number for Quantity or Unit Price.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true; // Prevents the error from propagating further
                }
            };
            this.dgvLineItems.EditingControlShowing += (s, e) =>
            {
                e.Control.KeyPress -= new KeyPressEventHandler(dgvLineItems_KeyPress); // Remove any previous handlers
                if (dgvLineItems.CurrentCell.ColumnIndex == dgvLineItems.Columns["Quantity"].Index ||
                    dgvLineItems.CurrentCell.ColumnIndex == dgvLineItems.Columns["UnitPrice"].Index)
                {
                    e.Control.KeyPress += new KeyPressEventHandler(dgvLineItems_KeyPress);
                }
            };

            // Sub Total
            Label lblSubTotal = new Label { Text = "Sub Total:", TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
            this.txtSubTotal.Dock = DockStyle.Fill;
            this.txtSubTotal.ReadOnly = true;
            this.txtSubTotal.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);

            // Discount
            Label lblDiscount = new Label { Text = "Discount (%):", TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
            this.numDiscount.Dock = DockStyle.Fill;
            this.numDiscount.DecimalPlaces = 2;
            this.numDiscount.Maximum = 100;
            this.numDiscount.ValueChanged += (s, e) => CalculateTotal();

            // Tax Rate
            Label lblTaxRate = new Label { Text = "Tax (%):", TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
            this.numTaxRate.Dock = DockStyle.Fill;
            this.numTaxRate.DecimalPlaces = 2;
            this.numTaxRate.Maximum = 100;
            this.numTaxRate.ValueChanged += (s, e) => CalculateTotal();

            // Grand Total
            Label lblGrandTotal = new Label { Text = "Grand Total:", TextAlign = System.Drawing.ContentAlignment.MiddleLeft, Dock = DockStyle.Fill };
            this.txtGrandTotal.Dock = DockStyle.Fill;
            this.txtGrandTotal.ReadOnly = true;
            this.txtGrandTotal.Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold);

            // Buttons
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 5, 0, 0) };
            this.btnSave.Text = "Save Invoice";
            this.btnSave.AutoSize = true;
            this.btnSave.Padding = new Padding(10, 5, 10, 5);
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.AutoSize = true;
            this.btnCancel.Padding = new Padding(10, 5, 10, 5);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            buttonPanel.Controls.Add(this.btnCancel);
            buttonPanel.Controls.Add(this.btnSave);

            // Add controls to main layout
            mainLayout.Controls.Add(lblPatient, 0, 0);
            mainLayout.Controls.Add(cmbPatient, 1, 0);
            mainLayout.Controls.Add(lblInvoiceDate, 0, 1);
            mainLayout.Controls.Add(dtpInvoiceDate, 1, 1);
            mainLayout.Controls.Add(lblLineItems, 0, 2);
            mainLayout.SetColumnSpan(lblLineItems, 2);
            mainLayout.Controls.Add(dgvLineItems, 0, 3); // Changed row index
            mainLayout.SetColumnSpan(dgvLineItems, 2);
            mainLayout.Controls.Add(lblSubTotal, 0, 4);
            mainLayout.Controls.Add(txtSubTotal, 1, 4);
            mainLayout.Controls.Add(lblDiscount, 0, 5);
            mainLayout.Controls.Add(numDiscount, 1, 5);
            mainLayout.Controls.Add(lblTaxRate, 0, 6);
            mainLayout.Controls.Add(numTaxRate, 1, 6);
            mainLayout.Controls.Add(lblGrandTotal, 0, 7);
            mainLayout.Controls.Add(txtGrandTotal, 1, 7);
            mainLayout.Controls.Add(buttonPanel, 0, 8); // New row for buttons
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);
        }

        private void SetupDataGridView()
        {
            dgvLineItems.Columns.Add("Description", "Description");
            dgvLineItems.Columns.Add("Quantity", "Qty");
            dgvLineItems.Columns.Add("UnitPrice", "Unit Price");
            dgvLineItems.Columns.Add("LineTotal", "Total");

            dgvLineItems.Columns["LineTotal"].ReadOnly = true;

            // Set column widths
            dgvLineItems.Columns["Description"].Width = 250;
            dgvLineItems.Columns["Quantity"].Width = 70;
            dgvLineItems.Columns["UnitPrice"].Width = 100;
            dgvLineItems.Columns["LineTotal"].Width = 120;
        }

        private void dgvLineItems_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow numbers, backspace, and decimal point
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // Only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void CalculateTotal(object sender = null, EventArgs e = null)
        {
            decimal subTotal = 0m;

            foreach (DataGridViewRow row in dgvLineItems.Rows)
            {
                if (row.IsNewRow) continue;

                decimal quantity = 0m;
                decimal unitPrice = 0m;

                if (row.Cells["Quantity"].Value != null && decimal.TryParse(row.Cells["Quantity"].Value.ToString(), out quantity) &&
                    row.Cells["UnitPrice"].Value != null && decimal.TryParse(row.Cells["UnitPrice"].Value.ToString(), out unitPrice))
                {
                    decimal lineTotal = quantity * unitPrice;
                    row.Cells["LineTotal"].Value = lineTotal.ToString("F2");
                    subTotal += lineTotal;
                }
                else
                {
                    row.Cells["LineTotal"].Value = "0.00"; // Set to 0 if invalid input
                }
            }

            decimal discountAmount = subTotal * (numDiscount.Value / 100m);
            decimal taxAmount = (subTotal - discountAmount) * (numTaxRate.Value / 100m);
            decimal grandTotal = subTotal - discountAmount + taxAmount;

            txtSubTotal.Text = subTotal.ToString("F2");
            txtGrandTotal.Text = grandTotal.ToString("F2");
        }

        private void LoadPatients()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT PatientID, FirstName + ' ' + LastName AS FullName FROM tblPatient WHERE IsDeleted = 0";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbPatient.DataSource = dt;
            }
        }

        private void LoadBillData(int billingID)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Load billing header data
                string billingQuery = @"SELECT PatientID, BillingDate, SubTotal, DiscountPercentage, TaxPercentage, GrandTotal 
                                      FROM tblBilling WHERE BillingID = @BillingID";
                SqlCommand billingCmd = new SqlCommand(billingQuery, con);
                billingCmd.Parameters.AddWithValue("@BillingID", billingID);

                SqlDataReader reader = billingCmd.ExecuteReader();
                if (reader.Read())
                {
                    cmbPatient.SelectedValue = reader["PatientID"];
                    dtpInvoiceDate.Value = Convert.ToDateTime(reader["BillingDate"]);
                    numDiscount.Value = Convert.ToDecimal(reader["DiscountPercentage"]);
                    numTaxRate.Value = Convert.ToDecimal(reader["TaxPercentage"]);
                    // SubTotal and GrandTotal will be recalculated from line items
                }
                reader.Close();

                // Load billing line items
                string lineItemsQuery = "SELECT Description, Quantity, UnitPrice, LineTotal FROM tblBillingLineItem WHERE BillingID = @BillingID";
                SqlCommand lineItemsCmd = new SqlCommand(lineItemsQuery, con);
                lineItemsCmd.Parameters.AddWithValue("@BillingID", billingID);
                SqlDataAdapter lineItemsDa = new SqlDataAdapter(lineItemsCmd);
                DataTable lineItemsDt = new DataTable();
                lineItemsDa.Fill(lineItemsDt);

                foreach (DataRow row in lineItemsDt.Rows)
                {
                    dgvLineItems.Rows.Add(row["Description"], row["Quantity"], row["UnitPrice"], row["LineTotal"]);
                }

                CalculateTotal(); // Recalculate totals after loading line items
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (dgvLineItems.Rows.Cast<DataGridViewRow>().Any(row => !row.IsNewRow && row.Cells["Description"].Value == null))
            {
                MessageBox.Show("Please ensure all line items have a description.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbPatient.SelectedValue == null)
            {
                MessageBox.Show("Please select a patient.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
