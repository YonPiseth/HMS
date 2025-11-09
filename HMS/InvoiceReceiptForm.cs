using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Linq;
using HMS; // Added for DatabaseHelper

namespace HMS.Forms
{
    public partial class InvoiceReceiptForm : Form
    {
        private int invoiceId;
        private Label lblInvoiceId;
        private Label lblPatientName;
        private Label lblInvoiceDate;
        private Label lblDueDate;
        private Label lblTotalAmount;
        private Label lblPaymentStatus;
        private DataGridView dgvLineItems;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panelHeader;
        private Panel panelFooter;
        private Button btnPrint;
        private Button btnClose;

        public InvoiceReceiptForm(int invoiceId)
        {
            this.invoiceId = invoiceId;
            InitializeComponent();
            this.Load += InvoiceReceiptForm_Load;
        }

        private void InvoiceReceiptForm_Load(object sender, EventArgs e)
        {
            LoadInvoiceDetails();
            LoadInvoiceLineItems();

            // Temporary: Display invoiceId to confirm it's passed correctly
            lblInvoiceId.Text = $"Invoice #: {invoiceId} (Debug)";
        }

        private void LoadInvoiceDetails()
        {
            using (Microsoft.Data.SqlClient.SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"
                    SELECT 
                        I.InvoiceID, 
                        P.FirstName + ' ' + P.LastName AS PatientName,
                        P.Address AS PatientAddress,
                        P.ContactNumber AS PatientPhone,
                        I.InvoiceDate, 
                        I.DueDate,
                        I.TotalAmount, 
                        I.PaymentStatus,
                        I.Notes
                    FROM tblInvoice I
                    JOIN tblPatient P ON I.PatientID = P.PatientID
                    WHERE I.InvoiceID = @InvoiceID";

                using (Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);
                    con.Open();
                    Microsoft.Data.SqlClient.SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        lblInvoiceId.Text = $"Invoice #: {reader["InvoiceID"]}"; // Overwrite debug text with actual ID
                        lblPatientName.Text = $"Patient: {reader["PatientName"]}\nAddress: {reader["PatientAddress"]}\nPhone: {reader["PatientPhone"]}";
                        lblInvoiceDate.Text = $"Invoice Date: {((DateTime)reader["InvoiceDate"]).ToShortDateString()}";
                        lblDueDate.Text = $"Due Date: {((DateTime)reader["DueDate"]).ToShortDateString()}";
                        lblTotalAmount.Text = $"Total Amount: {Convert.ToDecimal(reader["TotalAmount"]):C}";
                        lblPaymentStatus.Text = $"Payment Status: {reader["PaymentStatus"]}";
                    }
                    else
                    {
                        lblInvoiceId.Text = $"Invoice #: {invoiceId} (No Data)";
                        lblPatientName.Text = "Patient: Not Found";
                        lblInvoiceDate.Text = "";
                        lblDueDate.Text = "";
                        lblTotalAmount.Text = "";
                        lblPaymentStatus.Text = "";
                    }
                    reader.Close();
                }
            }
        }

        private void LoadInvoiceLineItems()
        {
            using (Microsoft.Data.SqlClient.SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"
                    SELECT 
                        BLI.Description, 
                        BLI.Quantity, 
                        BLI.UnitPrice, 
                        BLI.LineTotal
                    FROM tblInvoiceLineItem BLI
                    WHERE BLI.InvoiceID = @InvoiceID
                    ORDER BY BLI.Description";

                using (Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);
                    Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvLineItems.DataSource = dt;

                    // Format currency columns
                    if (dgvLineItems.Columns.Contains("UnitPrice"))
                        dgvLineItems.Columns["UnitPrice"].DefaultCellStyle.Format = "C";
                    if (dgvLineItems.Columns.Contains("LineTotal"))
                        dgvLineItems.Columns["LineTotal"].DefaultCellStyle.Format = "C";
                }
            }
        }

        private string[] GetDataReaderColumnNames(Microsoft.Data.SqlClient.SqlDataReader reader)
        {
            string[] columns = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns[i] = reader.GetName(i);
            }
            return columns;
        }

        private void InitializeComponent()
        {
            this.lblInvoiceId = new System.Windows.Forms.Label();
            this.lblPatientName = new System.Windows.Forms.Label();
            this.lblInvoiceDate = new System.Windows.Forms.Label();
            this.lblDueDate = new System.Windows.Forms.Label();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.lblPaymentStatus = new System.Windows.Forms.Label();
            this.dgvLineItems = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();

            this.Text = "Invoice Receipt";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            this.tableLayoutPanel1.Dock = DockStyle.Fill;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 200));
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            this.tableLayoutPanel1.Padding = new Padding(20);
            UIHelper.ApplyPanelStyles(this.tableLayoutPanel1);

            this.panelHeader.Dock = DockStyle.Fill;
            UIHelper.ApplyPanelStyles(this.panelHeader);

            Label lblHospitalName = new Label();
            lblHospitalName.Text = "Hospital Management System";
            UIHelper.StyleLabelTitle(lblHospitalName);
            lblHospitalName.Location = new Point(20, 20);

            UIHelper.StyleLabelTitle(this.lblInvoiceId);
            this.lblInvoiceId.Location = new Point(20, 60);

            UIHelper.StyleLabel(this.lblPatientName);
            this.lblPatientName.Location = new Point(20, 90);
            this.lblPatientName.AutoSize = true;

            UIHelper.StyleLabel(this.lblInvoiceDate);
            this.lblInvoiceDate.Location = new Point(400, 60);

            UIHelper.StyleLabel(this.lblDueDate);
            this.lblDueDate.Location = new Point(400, 90);

            UIHelper.StyleLabelTitle(this.lblTotalAmount);
            this.lblTotalAmount.Location = new Point(400, 120);

            UIHelper.StyleLabel(this.lblPaymentStatus);
            this.lblPaymentStatus.Location = new Point(400, 150);

            this.panelHeader.Controls.AddRange(new Control[] {
                lblHospitalName,
                this.lblInvoiceId,
                this.lblPatientName,
                this.lblInvoiceDate,
                this.lblDueDate,
                this.lblTotalAmount,
                this.lblPaymentStatus
            });

            // Line items grid
            this.dgvLineItems.Dock = DockStyle.Fill;
            UIHelper.StyleDataGridView(this.dgvLineItems);
            this.dgvLineItems.AutoGenerateColumns = false;
            this.dgvLineItems.AllowUserToAddRows = false;
            this.dgvLineItems.AllowUserToDeleteRows = false;
            this.dgvLineItems.ReadOnly = true;
            this.dgvLineItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvLineItems.MultiSelect = false;
            this.dgvLineItems.RowHeadersVisible = false;
            this.dgvLineItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLineItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Add columns manually
            this.dgvLineItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Description", HeaderText = "Description", DataPropertyName = "Description", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            this.dgvLineItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Quantity", HeaderText = "Qty", DataPropertyName = "Quantity", Width = 70 });
            this.dgvLineItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "UnitPrice", HeaderText = "Unit Price", DataPropertyName = "UnitPrice", Width = 100 });
            this.dgvLineItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "LineTotal", HeaderText = "Total", DataPropertyName = "LineTotal", Width = 100 });

            // Footer panel with buttons
            this.panelFooter.Dock = DockStyle.Fill;
            UIHelper.ApplyPanelStyles(this.panelFooter);

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Padding = new Padding(0, 10, 0, 0);
            UIHelper.ApplyPanelStyles(buttonPanel);

            this.btnPrint.Text = "Print Invoice";
            UIHelper.StyleButton(this.btnPrint);
            this.btnPrint.Width = 120;
            this.btnPrint.Click += new EventHandler(this.btnPrint_Click);

            this.btnClose.Text = "Close";
            UIHelper.StyleButton(this.btnClose);
            this.btnClose.Width = 120;
            this.btnClose.Click += new EventHandler(this.btnClose_Click);

            buttonPanel.Controls.Add(this.btnClose);
            buttonPanel.Controls.Add(this.btnPrint);
            this.panelFooter.Controls.Add(buttonPanel);

            this.tableLayoutPanel1.Controls.Add(this.panelHeader, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dgvLineItems, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panelFooter, 0, 2);

            this.Controls.Add(this.tableLayoutPanel1);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintPage);
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = pd;
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Segoe UI", 16, FontStyle.Bold);
            Font headerFont = new Font("Segoe UI", 12, FontStyle.Bold);
            Font normalFont = new Font("Segoe UI", 10);
            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;
            float yPos = topMargin;

            // Hospital Name
            string hospitalName = "Hospital Management System";
            g.DrawString(hospitalName, titleFont, Brushes.Black, leftMargin, yPos);
            yPos += titleFont.GetHeight(g) + 10;

            // Invoice ID
            g.DrawString(lblInvoiceId.Text, headerFont, Brushes.Black, leftMargin, yPos);
            yPos += headerFont.GetHeight(g) + 5;

            // Patient Info (multiline)
            string[] patientInfoLines = lblPatientName.Text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in patientInfoLines)
            {
                g.DrawString(line, normalFont, Brushes.Black, leftMargin, yPos);
                yPos += normalFont.GetHeight(g);
            }
            yPos += 10;

            // Invoice and Due Dates
            g.DrawString(lblInvoiceDate.Text, normalFont, Brushes.Black, leftMargin, yPos);
            yPos += normalFont.GetHeight(g);
            g.DrawString(lblDueDate.Text, normalFont, Brushes.Black, leftMargin, yPos);
            yPos += normalFont.GetHeight(g) + 10;

            // Line Items Header
            g.DrawString("Description", headerFont, Brushes.Black, leftMargin, yPos);
            g.DrawString("Qty", headerFont, Brushes.Black, leftMargin + 200, yPos);
            g.DrawString("Unit Price", headerFont, Brushes.Black, leftMargin + 300, yPos);
            g.DrawString("Total", headerFont, Brushes.Black, leftMargin + 450, yPos);
            yPos += headerFont.GetHeight(g) + 5;

            foreach (DataGridViewRow row in dgvLineItems.Rows)
            {
                if (row.IsNewRow) continue;
                g.DrawString(row.Cells["Description"].Value?.ToString(), normalFont, Brushes.Black, leftMargin, yPos);
                g.DrawString(row.Cells["Quantity"].Value?.ToString(), normalFont, Brushes.Black, leftMargin + 200, yPos);
                g.DrawString(Convert.ToDecimal(row.Cells["UnitPrice"].Value).ToString("C"), normalFont, Brushes.Black, leftMargin + 300, yPos);
                g.DrawString(Convert.ToDecimal(row.Cells["LineTotal"].Value).ToString("C"), normalFont, Brushes.Black, leftMargin + 450, yPos);
                yPos += normalFont.GetHeight(g);
            }
            yPos += 10;

            g.DrawString(lblTotalAmount.Text, headerFont, Brushes.Black, leftMargin, yPos);
            yPos += headerFont.GetHeight(g);
            g.DrawString(lblPaymentStatus.Text, normalFont, Brushes.Black, leftMargin, yPos);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}