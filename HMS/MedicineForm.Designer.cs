using System;
using System.Windows.Forms;
using System.Drawing;

namespace HMS
{
    public partial class MedicineForm : Form
    {
        public TextBox txtMedicineName;
        public ComboBox cmbSupplier;
        public TextBox txtDosage;
        public TextBox txtSideEffects;
        public NumericUpDown numPrice;
        public ComboBox cmbCategory;
        public TextBox txtDescription;
        public NumericUpDown numUnitPrice;
        public NumericUpDown numStockQuantity;
        private Button btnSave;
        private Button btnCancel;

        private void InitializeComponent()
        {
            this.txtMedicineName = new TextBox();
            this.cmbSupplier = new ComboBox();
            this.txtDosage = new TextBox();
            this.txtSideEffects = new TextBox();
            this.numPrice = new NumericUpDown();
            this.cmbCategory = new ComboBox();
            this.txtDescription = new TextBox();
            this.numUnitPrice = new NumericUpDown();
            this.numStockQuantity = new NumericUpDown();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            this.SuspendLayout();

            // Form settings
            this.Text = "Medicine Information";
            this.Size = new Size(450, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            this.ResumeLayout(false);
        }
    }
}
