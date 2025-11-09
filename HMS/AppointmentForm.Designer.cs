using System;
using System.Windows.Forms;
using System.Drawing;

namespace HMS
{
    public partial class AppointmentForm : Form
    {
        public ComboBox cmbPatient;
        public ComboBox cmbDoctor;
        public DateTimePicker dtpDate;
        public DateTimePicker dtpTime;
        public TextBox txtReason;
        public ComboBox cmbStatus;
        private Button btnSave;
        private Button btnCancel;

        private void InitializeComponent()
        {
            this.cmbPatient = new ComboBox();
            this.cmbDoctor = new ComboBox();
            this.dtpDate = new DateTimePicker();
            this.dtpTime = new DateTimePicker();
            this.txtReason = new TextBox();
            this.cmbStatus = new ComboBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            
            this.SuspendLayout();
            
            // Form settings
            this.Text = "Appointment Information";
            this.Size = new Size(450, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            
            this.ResumeLayout(false);
        }
    }
} 