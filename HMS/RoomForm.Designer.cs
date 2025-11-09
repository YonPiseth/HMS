using System;
using System.Windows.Forms;
using System.Drawing;

namespace HMS
{
    public partial class RoomForm : Form
    {
        public TextBox txtRoomNumber;
        public ComboBox cmbRoomType;
        public NumericUpDown numFloor;
        public NumericUpDown numCapacity;
        public NumericUpDown numRatePerDay;
        public ComboBox cmbStatus;
        private Button btnSave;
        private Button btnCancel;

        private void InitializeComponent()
        {
            this.txtRoomNumber = new TextBox();
            this.cmbRoomType = new ComboBox();
            this.numFloor = new NumericUpDown();
            this.numCapacity = new NumericUpDown();
            this.numRatePerDay = new NumericUpDown();
            this.cmbStatus = new ComboBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            this.SuspendLayout();

            // Form settings
            this.Text = "Room Information";
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
