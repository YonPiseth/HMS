using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

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
        private Button btnSave;
        private Button btnCancel;
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";

        public PatientForm()
        {
            InitializeComponent();
        }

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

            // Form settings
            this.Text = "Patient Information";
            this.Size = new System.Drawing.Size(400, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Profile photo
            picPhoto = new PictureBox { Left = 150, Top = 10, Width = 100, Height = 100, SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle };
            btnUploadPhoto = new Button { Text = "Upload Photo", Left = 150, Top = 115, Width = 100, Height = 28 };
            btnUploadPhoto.Click += new EventHandler(btnUploadPhoto_Click);

            int labelX = 20;
            int controlX = 150;
            int startY = 150;
            int spacing = 40;

            // First Name
            Label lblFirstName = new Label();
            lblFirstName.Text = "First Name:";
            lblFirstName.Location = new System.Drawing.Point(labelX, startY);
            lblFirstName.AutoSize = true;
            this.txtFirstName.Location = new System.Drawing.Point(controlX, startY);
            this.txtFirstName.Size = new System.Drawing.Size(200, 23);

            // Last Name
            Label lblLastName = new Label();
            lblLastName.Text = "Last Name:";
            lblLastName.Location = new System.Drawing.Point(labelX, startY + spacing);
            lblLastName.AutoSize = true;
            this.txtLastName.Location = new System.Drawing.Point(controlX, startY + spacing);
            this.txtLastName.Size = new System.Drawing.Size(200, 23);

            // Date of Birth
            Label lblDateOfBirth = new Label();
            lblDateOfBirth.Text = "Date of Birth:";
            lblDateOfBirth.Location = new System.Drawing.Point(labelX, startY + spacing * 2);
            lblDateOfBirth.AutoSize = true;
            this.dtpDateOfBirth.Location = new System.Drawing.Point(controlX, startY + spacing * 2);
            this.dtpDateOfBirth.Size = new System.Drawing.Size(200, 23);
            this.dtpDateOfBirth.Format = DateTimePickerFormat.Short;

            // Gender
            Label lblGender = new Label();
            lblGender.Text = "Gender:";
            lblGender.Location = new System.Drawing.Point(labelX, startY + spacing * 3);
            lblGender.AutoSize = true;
            this.cmbGender.Location = new System.Drawing.Point(controlX, startY + spacing * 3);
            this.cmbGender.Size = new System.Drawing.Size(200, 23);
            this.cmbGender.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbGender.Items.AddRange(new object[] { "Male", "Female", "Other" });

            // Blood Type
            Label lblBloodType = new Label();
            lblBloodType.Text = "Blood Type:";
            lblBloodType.Location = new System.Drawing.Point(labelX, startY + spacing * 4);
            lblBloodType.AutoSize = true;
            this.cmbBloodType.Location = new System.Drawing.Point(controlX, startY + spacing * 4);
            this.cmbBloodType.Size = new System.Drawing.Size(200, 23);
            this.cmbBloodType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbBloodType.Items.AddRange(new object[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" });

            // Contact Number
            Label lblContactNumber = new Label();
            lblContactNumber.Text = "Contact Number:";
            lblContactNumber.Location = new System.Drawing.Point(labelX, startY + spacing * 5);
            lblContactNumber.AutoSize = true;
            this.txtContactNumber.Location = new System.Drawing.Point(controlX, startY + spacing * 5);
            this.txtContactNumber.Size = new System.Drawing.Size(200, 23);

            // Email
            Label lblEmail = new Label();
            lblEmail.Text = "Email:";
            lblEmail.Location = new System.Drawing.Point(labelX, startY + spacing * 6);
            lblEmail.AutoSize = true;
            this.txtEmail.Location = new System.Drawing.Point(controlX, startY + spacing * 6);
            this.txtEmail.Size = new System.Drawing.Size(200, 23);

            // Address
            Label lblAddress = new Label();
            lblAddress.Text = "Address:";
            lblAddress.Location = new System.Drawing.Point(labelX, startY + spacing * 7);
            lblAddress.AutoSize = true;
            this.txtAddress.Location = new System.Drawing.Point(controlX, startY + spacing * 7);
            this.txtAddress.Size = new System.Drawing.Size(200, 23);

            // Insurance Number
            Label lblInsuranceNumber = new Label();
            lblInsuranceNumber.Text = "Insurance Number:";
            lblInsuranceNumber.Location = new System.Drawing.Point(labelX, startY + spacing * 8);
            lblInsuranceNumber.AutoSize = true;
            this.txtInsuranceNumber.Location = new System.Drawing.Point(controlX, startY + spacing * 8);
            this.txtInsuranceNumber.Size = new System.Drawing.Size(200, 23);

            // Family
            Label lblFamily = new Label();
            lblFamily.Text = "Family:";
            lblFamily.Location = new System.Drawing.Point(labelX, startY + spacing * 9);
            lblFamily.AutoSize = true;
            this.txtFamily.Location = new System.Drawing.Point(controlX, startY + spacing * 9);
            this.txtFamily.Size = new System.Drawing.Size(200, 23);

            // Status
            Label lblStatus = new Label();
            lblStatus.Text = "Status:";
            lblStatus.Location = new System.Drawing.Point(labelX, startY + spacing * 10);
            lblStatus.AutoSize = true;
            this.cmbStatus.Location = new System.Drawing.Point(controlX, startY + spacing * 10);
            this.cmbStatus.Size = new System.Drawing.Size(200, 23);
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new object[] { "Active", "Inactive", "Discharged" });

            // Save Button
            this.btnSave.Text = "Save";
            this.btnSave.Location = new System.Drawing.Point(controlX, startY + spacing * 11);
            this.btnSave.Size = new System.Drawing.Size(90, 30);
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(controlX + 100, startY + spacing * 11);
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            this.Controls.AddRange(new Control[] {
                picPhoto, btnUploadPhoto,
                lblFirstName, txtFirstName,
                lblLastName, txtLastName,
                lblDateOfBirth, dtpDateOfBirth,
                lblGender, cmbGender,
                lblBloodType, cmbBloodType,
                lblContactNumber, txtContactNumber,
                lblEmail, txtEmail,
                lblAddress, txtAddress,
                lblInsuranceNumber, txtInsuranceNumber,
                lblFamily, txtFamily,
                lblStatus, cmbStatus,
                btnSave, btnCancel
            });
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                cmbGender.SelectedIndex == -1 ||
                cmbBloodType.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtContactNumber.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text) ||
                cmbStatus.SelectedIndex == -1)
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

        private void btnUploadPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    picPhoto.Image = System.Drawing.Image.FromFile(ofd.FileName);
                }
            }
        }
    }
} 