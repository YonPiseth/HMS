using System;
using System.Windows.Forms;
using System.Drawing;

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

            this.SuspendLayout();

            // Form settings
            this.Text = "Create Invoice";
            this.Size = new Size(800, 750);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            this.ResumeLayout(false);
        }
    }
}
