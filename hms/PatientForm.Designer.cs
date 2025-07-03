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

            // Form settings
            this.Text = "Patient Information";
            this.Size = new Size(500, 780);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            // Main layout panel
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainLayout.RowCount = 14;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
            for (int i = 1; i <= 12; i++)
            {
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            }
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            mainLayout.Padding = new Padding(20);
            mainLayout.AutoScroll = true;

            // Profile photo section
            FlowLayoutPanel photoSectionPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, Padding = new Padding(0), Anchor = AnchorStyles.None };
            photoSectionPanel.Controls.Add(this.picPhoto);
            photoSectionPanel.Controls.Add(this.btnUploadPhoto);
            photoSectionPanel.BackColor = Color.Transparent;

            this.picPhoto.Width = 100;
            this.picPhoto.Height = 100;
            this.picPhoto.SizeMode = PictureBoxSizeMode.Zoom;
            this.picPhoto.BorderStyle = BorderStyle.FixedSingle;
            this.picPhoto.Margin = new Padding(0, 0, 0, 5);

            this.btnUploadPhoto.Text = "Upload Photo";
            this.btnUploadPhoto.Width = 100;
            this.btnUploadPhoto.Height = 30;
            this.btnUploadPhoto.Click += new EventHandler(btnUploadPhoto_Click);

            mainLayout.Controls.Add(photoSectionPanel, 0, 0);
            mainLayout.SetColumnSpan(photoSectionPanel, 2);
            photoSectionPanel.Anchor = AnchorStyles.None;

            // Helper to add a row of label and control
            Action<string, Control, int> addRow = (labelText, control, row) =>
            {
                Label lbl = new Label { Text = labelText, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left };
                UIHelper.StyleModernLabel(lbl);
                mainLayout.Controls.Add(lbl, 0, row);
                control.Dock = DockStyle.Fill;
                mainLayout.Controls.Add(control, 1, row);
            };

            addRow("First Name:", txtFirstName, 1);
            addRow("Last Name:", txtLastName, 2);
            addRow("Date of Birth:", dtpDateOfBirth, 3);
            this.dtpDateOfBirth.Format = DateTimePickerFormat.Short;
            addRow("Gender:", cmbGender, 4);
            this.cmbGender.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbGender.Items.AddRange(new object[] { "Male", "Female", "Other" });
            addRow("Blood Type:", cmbBloodType, 5);
            this.cmbBloodType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbBloodType.Items.AddRange(new object[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" });
            addRow("Contact Number:", txtContactNumber, 6);
            addRow("Email:", txtEmail, 7);
            addRow("Address:", txtAddress, 8);
            addRow("Insurance Number:", txtInsuranceNumber, 9);
            addRow("Patient's Family:", txtFamily, 10);
            addRow("Status:", cmbStatus, 11);
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new object[] { "Active", "Inactive", "Discharged" });
            addRow("Room Number:", txtRoomNumber, 12);

            // Buttons Panel
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Padding = new Padding(0, 10, 0, 0);
            buttonPanel.BackColor = Color.Transparent;

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Width = 100;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            buttonPanel.Controls.Add(this.btnCancel);

            this.btnSave.Text = "Save";
            this.btnSave.Width = 100;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            buttonPanel.Controls.Add(this.btnSave);

            mainLayout.Controls.Add(buttonPanel, 0, 13);
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);

            // Apply modern UI styles
            UIHelper.StyleModernButton(btnCancel);
            UIHelper.StyleModernButton(btnSave);
            UIHelper.StyleModernTextBox(txtFirstName);
            UIHelper.StyleModernTextBox(txtLastName);
            UIHelper.StyleModernTextBox(txtContactNumber);
            UIHelper.StyleModernTextBox(txtEmail);
            UIHelper.StyleModernTextBox(txtAddress);
            UIHelper.StyleModernTextBox(txtInsuranceNumber);
            UIHelper.StyleModernTextBox(txtFamily);
            UIHelper.StyleModernTextBox(txtRoomNumber);
            UIHelper.StyleModernPanel(mainLayout);
            UIHelper.StyleModernPanel(photoSectionPanel);
            UIHelper.StyleModernPanel(buttonPanel);
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
                    try
                    {
                        picPhoto.Image = Image.FromFile(ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image: " + ex.Message, "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
} 