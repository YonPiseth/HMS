using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace HMS
{
    public partial class BillingForm : Form
    {
        public ComboBox cmbPatient;
        public NumericUpDown numConsultationFee;
        public NumericUpDown numRoomCharges;
        public NumericUpDown numMedicineCharges;
        public NumericUpDown numOtherCharges;
        public NumericUpDown numTotalAmount;
        public ComboBox cmbPaymentStatus;
        private Button btnSave;
        private Button btnCancel;
        private string connectionString = @"Data Source=LAPTOP-NP47E1QQ\SQLEXPRESS01;Initial Catalog=HMS;Integrated Security=True";

        public BillingForm()
        {
            InitializeComponent();
            LoadPatients();
            SetupNumericUpDowns();
        }

        private void InitializeComponent()
        {
            this.cmbPatient = new ComboBox();
            this.numConsultationFee = new NumericUpDown();
            this.numRoomCharges = new NumericUpDown();
            this.numMedicineCharges = new NumericUpDown();
            this.numOtherCharges = new NumericUpDown();
            this.numTotalAmount = new NumericUpDown();
            this.cmbPaymentStatus = new ComboBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            // Form settings
            this.Text = "Create Invoice";
            this.Size = new System.Drawing.Size(400, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Labels and Controls
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

            // Consultation Fee
            Label lblConsultationFee = new Label();
            lblConsultationFee.Text = "Consultation Fee:";
            lblConsultationFee.Location = new System.Drawing.Point(labelX, startY + spacing);
            lblConsultationFee.AutoSize = true;
            this.numConsultationFee.Location = new System.Drawing.Point(controlX, startY + spacing);
            this.numConsultationFee.Size = new System.Drawing.Size(200, 23);

            // Room Charges
            Label lblRoomCharges = new Label();
            lblRoomCharges.Text = "Room Charges:";
            lblRoomCharges.Location = new System.Drawing.Point(labelX, startY + spacing * 2);
            lblRoomCharges.AutoSize = true;
            this.numRoomCharges.Location = new System.Drawing.Point(controlX, startY + spacing * 2);
            this.numRoomCharges.Size = new System.Drawing.Size(200, 23);

            // Medicine Charges
            Label lblMedicineCharges = new Label();
            lblMedicineCharges.Text = "Medicine Charges:";
            lblMedicineCharges.Location = new System.Drawing.Point(labelX, startY + spacing * 3);
            lblMedicineCharges.AutoSize = true;
            this.numMedicineCharges.Location = new System.Drawing.Point(controlX, startY + spacing * 3);
            this.numMedicineCharges.Size = new System.Drawing.Size(200, 23);

            // Other Charges
            Label lblOtherCharges = new Label();
            lblOtherCharges.Text = "Other Charges:";
            lblOtherCharges.Location = new System.Drawing.Point(labelX, startY + spacing * 4);
            lblOtherCharges.AutoSize = true;
            this.numOtherCharges.Location = new System.Drawing.Point(controlX, startY + spacing * 4);
            this.numOtherCharges.Size = new System.Drawing.Size(200, 23);

            // Total Amount
            Label lblTotalAmount = new Label();
            lblTotalAmount.Text = "Total Amount:";
            lblTotalAmount.Location = new System.Drawing.Point(labelX, startY + spacing * 5);
            lblTotalAmount.AutoSize = true;
            this.numTotalAmount.Location = new System.Drawing.Point(controlX, startY + spacing * 5);
            this.numTotalAmount.Size = new System.Drawing.Size(200, 23);
            this.numTotalAmount.ReadOnly = true;

            // Payment Status
            Label lblPaymentStatus = new Label();
            lblPaymentStatus.Text = "Payment Status:";
            lblPaymentStatus.Location = new System.Drawing.Point(labelX, startY + spacing * 6);
            lblPaymentStatus.AutoSize = true;
            this.cmbPaymentStatus.Location = new System.Drawing.Point(controlX, startY + spacing * 6);
            this.cmbPaymentStatus.Size = new System.Drawing.Size(200, 23);
            this.cmbPaymentStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbPaymentStatus.Items.AddRange(new string[] { "Pending", "Paid", "Partially Paid" });

            // Buttons
            this.btnSave.Text = "Save";
            this.btnSave.Location = new System.Drawing.Point(controlX, startY + spacing * 7);
            this.btnSave.Size = new System.Drawing.Size(90, 30);
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(controlX + 100, startY + spacing * 7);
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                lblPatient, cmbPatient,
                lblConsultationFee, numConsultationFee,
                lblRoomCharges, numRoomCharges,
                lblMedicineCharges, numMedicineCharges,
                lblOtherCharges, numOtherCharges,
                lblTotalAmount, numTotalAmount,
                lblPaymentStatus, cmbPaymentStatus,
                btnSave, btnCancel
            });
        }

        private void SetupNumericUpDowns()
        {
            foreach (NumericUpDown num in new[] { numConsultationFee, numRoomCharges, numMedicineCharges, numOtherCharges, numTotalAmount })
            {
                num.DecimalPlaces = 2;
                num.Maximum = 999999.99m;
                num.ThousandsSeparator = true;
                num.ValueChanged += new EventHandler(CalculateTotal);
            }
            numTotalAmount.ReadOnly = true;
        }

        private void CalculateTotal(object sender, EventArgs e)
        {
            decimal total = numConsultationFee.Value + 
                          numRoomCharges.Value + 
                          numMedicineCharges.Value + 
                          numOtherCharges.Value;
            numTotalAmount.Value = total;
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
            if (cmbPatient.SelectedIndex == -1 || cmbPaymentStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a patient and payment status.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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