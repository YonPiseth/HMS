using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Linq; // Added for FirstOrDefault

namespace HMS
{
    public partial class BillingForm : Form
    {
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
