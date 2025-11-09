using System;
using System.Windows.Forms;
using System.Drawing;

namespace HMS
{
    public partial class SupplierForm : Form
    {
        public TextBox txtSupplierName;
        public TextBox txtContactPerson;
        public TextBox txtEmail;
        public TextBox txtPhone;
        public TextBox txtAddress;
        public Button btnSave;
        public Button btnCancel;

        private void InitializeComponent()
        {
            this.txtSupplierName = new TextBox();
            this.txtContactPerson = new TextBox();
            this.txtEmail = new TextBox();
            this.txtPhone = new TextBox();
            this.txtAddress = new TextBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            this.SuspendLayout();

            this.Text = "Supplier Information";
            this.Size = new Size(450, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            this.ResumeLayout(false);
        }
    }
}

