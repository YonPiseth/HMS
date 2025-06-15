using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Drawing; // Added for Color and Padding

namespace HMS
{
    public partial class DoctorForm : Form
    {
        public TextBox txtFirstName;
        public TextBox txtLastName;
        public ComboBox cmbSpecialization;
        public TextBox txtContactNumber;
        public TextBox txtEmail;
        public NumericUpDown numYearsOfExperience;
        public PictureBox picPhoto;
        public Button btnUploadPhoto;
        private Button btnSave;
        private Button btnCancel;
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";

        public DoctorForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtFirstName = new TextBox();
            this.txtLastName = new TextBox();
            this.cmbSpecialization = new ComboBox();
            this.txtContactNumber = new TextBox();
            this.txtEmail = new TextBox();
            this.numYearsOfExperience = new NumericUpDown();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.picPhoto = new PictureBox(); // Initialize picPhoto
            this.btnUploadPhoto = new Button(); // Initialize btnUploadPhoto

            // Form settings
            this.Text = "Doctor Information";
            this.Size = new Size(500, 600); // Adjusted size for better field visibility
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
            mainLayout.RowCount = 8; // Photo + 6 fields + Buttons
            // Adjust row heights for better spacing
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120)); // Photo row
            for (int i = 1; i <= 6; i++) 
            {
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Standard field rows
            }
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Buttons row
            mainLayout.Padding = new Padding(20); // More generous padding
            mainLayout.AutoScroll = true;
            UIHelper.ApplyPanelStyles(mainLayout); // Apply panel style to main layout

            // Profile photo section
            FlowLayoutPanel photoSectionPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true, Padding = new Padding(0), Anchor = AnchorStyles.None };
            photoSectionPanel.Controls.Add(this.picPhoto);
            photoSectionPanel.Controls.Add(this.btnUploadPhoto);
            photoSectionPanel.BackColor = Color.Transparent; // Ensure background transparency

            this.picPhoto.Width = 100;
            this.picPhoto.Height = 100;
            this.picPhoto.SizeMode = PictureBoxSizeMode.Zoom;
            this.picPhoto.BorderStyle = BorderStyle.FixedSingle;
            this.picPhoto.Margin = new Padding(0, 0, 0, 5); // Margin below photo

            this.btnUploadPhoto.Text = "Upload Photo";
            UIHelper.StyleButton(this.btnUploadPhoto); // Apply button styling
            this.btnUploadPhoto.Width = 100; // Consistent width
            this.btnUploadPhoto.Height = 30; // Consistent height
            this.btnUploadPhoto.Click += new EventHandler(btnUploadPhoto_Click);
            
            mainLayout.Controls.Add(photoSectionPanel, 0, 0);
            mainLayout.SetColumnSpan(photoSectionPanel, 2);
            photoSectionPanel.Anchor = AnchorStyles.None; // Center the entire photo section

            // Helper to add a row of label and control
            Action<string, Control, int> addRow = (labelText, control, row) =>
            {
                Label lbl = new Label { Text = labelText, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left };
                UIHelper.StyleLabel(lbl);
                mainLayout.Controls.Add(lbl, 0, row);
                control.Dock = DockStyle.Fill;
                mainLayout.Controls.Add(control, 1, row);
            };

            // First Name
            addRow("First Name:", txtFirstName, 1);
            UIHelper.StyleTextBox(this.txtFirstName);

            // Last Name
            addRow("Last Name:", txtLastName, 2);
            UIHelper.StyleTextBox(this.txtLastName);

            // Specialization
            addRow("Specialization:", cmbSpecialization, 3);
            UIHelper.StyleComboBox(this.cmbSpecialization);
            this.cmbSpecialization.DropDownStyle = ComboBoxStyle.DropDownList;
            // cmbSpecialization.Items.AddRange handled in DoctorForm_Load via data binding

            // Contact Number
            addRow("Contact Number:", txtContactNumber, 4);
            UIHelper.StyleTextBox(this.txtContactNumber);

            // Email
            addRow("Email:", txtEmail, 5);
            UIHelper.StyleTextBox(this.txtEmail);

            // Years of Experience
            addRow("Years of Experience:", numYearsOfExperience, 6);
            this.numYearsOfExperience.Dock = DockStyle.Fill;
            this.numYearsOfExperience.Font = new Font("Segoe UI", 10); // Manual style for NumericUpDown
            this.numYearsOfExperience.Minimum = 0;
            this.numYearsOfExperience.Maximum = 100;

            // Buttons Panel
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft; // Buttons align to the right
            buttonPanel.Padding = new Padding(0, 10, 0, 0); // Padding from top
            buttonPanel.BackColor = Color.Transparent; // Ensure background transparency

            this.btnCancel.Text = "Cancel";
            UIHelper.StyleButton(this.btnCancel); // Apply button styling
            this.btnCancel.Width = 100; 
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            buttonPanel.Controls.Add(this.btnCancel);

            this.btnSave.Text = "Save";
            UIHelper.StyleButton(this.btnSave); // Apply button styling
            this.btnSave.Width = 100; 
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            buttonPanel.Controls.Add(this.btnSave);
            
            mainLayout.Controls.Add(buttonPanel, 0, 7); // Buttons at the bottom
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);
        }

        public void DoctorForm_Load(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT SpecializationID, SpecializationName FROM tblDoctorType";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbSpecialization.DataSource = dt;
                cmbSpecialization.DisplayMember = "SpecializationName";
                cmbSpecialization.ValueMember = "SpecializationID";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                cmbSpecialization.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtContactNumber.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text))
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