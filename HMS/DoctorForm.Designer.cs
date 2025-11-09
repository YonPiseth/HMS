using System;
using System.Windows.Forms;
using System.Drawing;

namespace HMS
{
    public partial class DoctorForm : Form
    {
        public TextBox txtFirstName;
        public TextBox txtLastName;
        public ComboBox cmbSpecialization;
        public TextBox txtContactNumber;
        public TextBox txtEmail;
        public TextBox txtAddress;
        public TextBox txtQualification;
        public TextBox txtDepartment;
        public TextBox txtWorkingHours;
        public CheckBox chkIsAvailable;
        public NumericUpDown numYearsOfExperience;
        public PictureBox picPhoto;
        public Button btnUploadPhoto;
        private Button btnSave;
        private Button btnCancel;

        private void InitializeComponent()
        {
            this.txtFirstName = new TextBox();
            this.txtLastName = new TextBox();
            this.cmbSpecialization = new ComboBox();
            this.txtContactNumber = new TextBox();
            this.txtEmail = new TextBox();
            this.txtAddress = new TextBox();
            this.txtQualification = new TextBox();
            this.txtDepartment = new TextBox();
            this.txtWorkingHours = new TextBox();
            this.chkIsAvailable = new CheckBox();
            this.numYearsOfExperience = new NumericUpDown();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.picPhoto = new PictureBox();
            this.btnUploadPhoto = new Button();

            this.SuspendLayout();

            this.Text = "Doctor Information";
            this.Size = new Size(550, 750);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            this.ResumeLayout(false);
        }
    }
}
