using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

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

            // Form settings
            this.Text = "Doctor Information";
            this.Size = new System.Drawing.Size(400, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Profile photo
            picPhoto = new PictureBox { Left = 150, Top = 10, Width = 100, Height = 100, SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle };
            // picPhoto.Image = Properties.Resources.doctor_profile; // Add a default doctor image to Resources
            picPhoto.Image = null; // No default image set. You can set a default image here if desired.
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

            // Specialization
            Label lblSpecialization = new Label();
            lblSpecialization.Text = "Specialization:";
            lblSpecialization.Location = new System.Drawing.Point(labelX, startY + spacing * 2);
            lblSpecialization.AutoSize = true;
            this.cmbSpecialization.Location = new System.Drawing.Point(controlX, startY + spacing * 2);
            this.cmbSpecialization.Size = new System.Drawing.Size(200, 23);
            this.cmbSpecialization.DropDownStyle = ComboBoxStyle.DropDownList;

            // Contact Number
            Label lblContactNumber = new Label();
            lblContactNumber.Text = "Contact Number:";
            lblContactNumber.Location = new System.Drawing.Point(labelX, startY + spacing * 3);
            lblContactNumber.AutoSize = true;
            this.txtContactNumber.Location = new System.Drawing.Point(controlX, startY + spacing * 3);
            this.txtContactNumber.Size = new System.Drawing.Size(200, 23);

            // Email
            Label lblEmail = new Label();
            lblEmail.Text = "Email:";
            lblEmail.Location = new System.Drawing.Point(labelX, startY + spacing * 4);
            lblEmail.AutoSize = true;
            this.txtEmail.Location = new System.Drawing.Point(controlX, startY + spacing * 4);
            this.txtEmail.Size = new System.Drawing.Size(200, 23);

            // Years of Experience
            Label lblYearsOfExperience = new Label();
            lblYearsOfExperience.Text = "Years of Experience:";
            lblYearsOfExperience.Location = new System.Drawing.Point(labelX, startY + spacing * 5);
            lblYearsOfExperience.AutoSize = true;
            this.numYearsOfExperience.Location = new System.Drawing.Point(controlX, startY + spacing * 5);
            this.numYearsOfExperience.Size = new System.Drawing.Size(200, 23);
            this.numYearsOfExperience.Minimum = 0;
            this.numYearsOfExperience.Maximum = 100;

            // Save Button
            this.btnSave.Text = "Save";
            this.btnSave.Location = new System.Drawing.Point(controlX, startY + spacing * 6);
            this.btnSave.Size = new System.Drawing.Size(90, 30);
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(controlX + 100, startY + spacing * 6);
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            this.Controls.AddRange(new Control[] {
                picPhoto, btnUploadPhoto,
                lblFirstName, txtFirstName,
                lblLastName, txtLastName,
                lblSpecialization, cmbSpecialization,
                lblContactNumber, txtContactNumber,
                lblEmail, txtEmail,
                lblYearsOfExperience, numYearsOfExperience,
                btnSave, btnCancel
            });
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
                    picPhoto.Image = System.Drawing.Image.FromFile(ofd.FileName);
                }
            }
        }
    }
} 