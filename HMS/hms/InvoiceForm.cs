using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace HMS
{
    public partial class InvoiceForm : Form
    {
        public ComboBox cmbPatient;
        public NumericUpDown numTotalAmount;
        public DateTimePicker dtpInvoiceDate;
        public ComboBox cmbPaymentStatus;
        public DateTimePicker dtpDueDate;
        private Button btnSave;
        private Button btnCancel;
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";

        public InvoiceForm()
        {
            InitializeComponent();
            LoadPatients();
        }

        private void InitializeComponent()
        {
            this.cmbPatient = new ComboBox();
            this.numTotalAmount = new NumericUpDown();
            this.dtpInvoiceDate = new DateTimePicker();
            this.cmbPaymentStatus = new ComboBox();
            this.dtpDueDate = new DateTimePicker();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            // Form settings
            this.Text = "Invoice Information";
            this.Size = new System.Drawing.Size(400, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int labelX = 20;
            int controlX = 150;
            int startY = 20;
            int spacing = 40;

            // Patient
            Label lblPatient = new Label();
            lblPatient.Text = "Patient:";
            lblPatient.Location = new System.Drawing.Point(labelX, startY);
            lblPatient.AutoSize = true;
            this.cmbPatient.Location = new System.Drawing.Point(controlX, startY);
            this.cmbPatient.Size = new System.Drawing.Size(200, 23);
            this.cmbPatient.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbPatient.DisplayMember = "FullName";
            this.cmbPatient.ValueMember = "PatientID";

            // Total Amount
            Label lblTotalAmount = new Label();
            lblTotalAmount.Text = "Total Amount:";
            lblTotalAmount.Location = new System.Drawing.Point(labelX, startY + spacing);
            lblTotalAmount.AutoSize = true;
            this.numTotalAmount.Location = new System.Drawing.Point(controlX, startY + spacing);
            this.numTotalAmount.Size = new System.Drawing.Size(200, 23);
            this.numTotalAmount.DecimalPlaces = 2;
            this.numTotalAmount.Maximum = 1000000;

            // Invoice Date
            Label lblInvoiceDate = new Label();
            lblInvoiceDate.Text = "Invoice Date:";
            lblInvoiceDate.Location = new System.Drawing.Point(labelX, startY + spacing * 2);
            lblInvoiceDate.AutoSize = true;
            this.dtpInvoiceDate.Location = new System.Drawing.Point(controlX, startY + spacing * 2);
            this.dtpInvoiceDate.Size = new System.Drawing.Size(200, 23);
            this.dtpInvoiceDate.Format = DateTimePickerFormat.Short;

            // Payment Status
            Label lblPaymentStatus = new Label();
            lblPaymentStatus.Text = "Payment Status:";
            lblPaymentStatus.Location = new System.Drawing.Point(labelX, startY + spacing * 3);
            lblPaymentStatus.AutoSize = true;
            this.cmbPaymentStatus.Location = new System.Drawing.Point(controlX, startY + spacing * 3);
            this.cmbPaymentStatus.Size = new System.Drawing.Size(200, 23);
            this.cmbPaymentStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbPaymentStatus.Items.AddRange(new string[] { "Paid", "Unpaid" });

            // Due Date
            Label lblDueDate = new Label();
            lblDueDate.Text = "Due Date:";
            lblDueDate.Location = new System.Drawing.Point(labelX, startY + spacing * 4);
            lblDueDate.AutoSize = true;
            this.dtpDueDate.Location = new System.Drawing.Point(controlX, startY + spacing * 4);
            this.dtpDueDate.Size = new System.Drawing.Size(200, 23);
            this.dtpDueDate.Format = DateTimePickerFormat.Short;

            // Save Button
            this.btnSave.Text = "Save";
            this.btnSave.Location = new System.Drawing.Point(controlX, startY + spacing * 5);
            this.btnSave.Size = new System.Drawing.Size(90, 30);
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(controlX + 100, startY + spacing * 5);
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            this.Controls.AddRange(new Control[] {
                lblPatient, cmbPatient,
                lblTotalAmount, numTotalAmount,
                lblInvoiceDate, dtpInvoiceDate,
                lblPaymentStatus, cmbPaymentStatus,
                lblDueDate, dtpDueDate,
                btnSave, btnCancel
            });
        }

        private void LoadPatients()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT PatientID, FirstName + ' ' + LastName as FullName FROM tblPatient WHERE IsDeleted = 0";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbPatient.DataSource = dt;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbPatient.SelectedIndex == -1 || numTotalAmount.Value <= 0 || cmbPaymentStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
} 