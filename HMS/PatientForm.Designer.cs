using System;
using System.Windows.Forms;
using System.Drawing;

namespace HMS
{
    public partial class PatientForm : Form
    {
        public TextBox txtFirstName;
        public TextBox txtLastName;
        public DateTimePicker dtpDateOfBirth;
        public ComboBox cmbGender;
        public ComboBox cmbBloodType;
        public TextBox txtContactNumber;
        public TextBox txtEmail;
        public TextBox txtAddress;
        public TextBox txtInsuranceNumber;
        public TextBox txtFamily;
        public ComboBox cmbStatus;
        public PictureBox picPhoto;
        public Button btnUploadPhoto;
        public TextBox txtRoomNumber;
        private Button btnSave;
        private Button btnCancel;

        private void InitializeComponent()
        {
            this.txtFirstName = new TextBox();
            this.txtLastName = new TextBox();
            this.dtpDateOfBirth = new DateTimePicker();
            this.cmbGender = new ComboBox();
            this.cmbBloodType = new ComboBox();
            this.txtContactNumber = new TextBox();
            this.txtEmail = new TextBox();
            this.txtAddress = new TextBox();
            this.txtInsuranceNumber = new TextBox();
            this.txtFamily = new TextBox();
            this.cmbStatus = new ComboBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.picPhoto = new PictureBox();
            this.btnUploadPhoto = new Button();
            this.txtRoomNumber = new TextBox();

            this.SuspendLayout();

            this.Text = "Patient Information";
            this.Size = new Size(500, 780);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            this.ResumeLayout(false);
        }
    }
}
