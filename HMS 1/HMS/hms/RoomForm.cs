using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;

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
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";

        public RoomForm()
        {
            InitializeComponent();
        }

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

            // Form settings
            this.Text = "Room Information";
            this.Size = new System.Drawing.Size(400, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Room Number
            Label lblRoomNumber = new Label();
            lblRoomNumber.Text = "Room Number:";
            lblRoomNumber.Location = new System.Drawing.Point(20, 20);
            lblRoomNumber.Size = new System.Drawing.Size(120, 20);

            this.txtRoomNumber.Location = new System.Drawing.Point(150, 20);
            this.txtRoomNumber.Size = new System.Drawing.Size(200, 27);

            // Room Type
            Label lblRoomType = new Label();
            lblRoomType.Text = "Room Type:";
            lblRoomType.Location = new System.Drawing.Point(20, 60);
            lblRoomType.Size = new System.Drawing.Size(120, 20);

            this.cmbRoomType.Location = new System.Drawing.Point(150, 60);
            this.cmbRoomType.Size = new System.Drawing.Size(200, 27);
            this.cmbRoomType.DropDownStyle = ComboBoxStyle.DropDownList;

            // Floor
            Label lblFloor = new Label();
            lblFloor.Text = "Floor:";
            lblFloor.Location = new System.Drawing.Point(20, 100);
            lblFloor.Size = new System.Drawing.Size(120, 20);

            this.numFloor.Location = new System.Drawing.Point(150, 100);
            this.numFloor.Size = new System.Drawing.Size(200, 27);
            this.numFloor.Minimum = 1;
            this.numFloor.Maximum = 100;

            // Capacity
            Label lblCapacity = new Label();
            lblCapacity.Text = "Capacity:";
            lblCapacity.Location = new System.Drawing.Point(20, 140);
            lblCapacity.Size = new System.Drawing.Size(120, 20);

            this.numCapacity.Location = new System.Drawing.Point(150, 140);
            this.numCapacity.Size = new System.Drawing.Size(200, 27);
            this.numCapacity.Minimum = 1;
            this.numCapacity.Maximum = 10;

            // Rate Per Day
            Label lblRatePerDay = new Label();
            lblRatePerDay.Text = "Rate Per Day:";
            lblRatePerDay.Location = new System.Drawing.Point(20, 180);
            lblRatePerDay.Size = new System.Drawing.Size(120, 20);

            this.numRatePerDay.Location = new System.Drawing.Point(150, 180);
            this.numRatePerDay.Size = new System.Drawing.Size(200, 27);
            this.numRatePerDay.Minimum = 0;
            this.numRatePerDay.Maximum = 100000;
            this.numRatePerDay.DecimalPlaces = 2;

            // Status
            Label lblStatus = new Label();
            lblStatus.Text = "Status:";
            lblStatus.Location = new System.Drawing.Point(20, 220);
            lblStatus.Size = new System.Drawing.Size(120, 20);

            this.cmbStatus.Location = new System.Drawing.Point(150, 220);
            this.cmbStatus.Size = new System.Drawing.Size(200, 27);
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new string[] { "Available", "Occupied", "Maintenance" });

            // Save Button
            this.btnSave.Text = "Save";
            this.btnSave.Location = new System.Drawing.Point(150, 280);
            this.btnSave.Size = new System.Drawing.Size(90, 35);
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            // Cancel Button
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(260, 280);
            this.btnCancel.Size = new System.Drawing.Size(90, 35);
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(220, 220, 220);
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // Add controls
            this.Controls.Add(lblRoomNumber);
            this.Controls.Add(this.txtRoomNumber);
            this.Controls.Add(lblRoomType);
            this.Controls.Add(this.cmbRoomType);
            this.Controls.Add(lblFloor);
            this.Controls.Add(this.numFloor);
            this.Controls.Add(lblCapacity);
            this.Controls.Add(this.numCapacity);
            this.Controls.Add(lblRatePerDay);
            this.Controls.Add(this.numRatePerDay);
            this.Controls.Add(lblStatus);
            this.Controls.Add(this.cmbStatus);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadRoomTypes();
        }

        private void LoadRoomTypes()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT RoomTypeID, RoomTypeName FROM tblRoomType WHERE IsDeleted = 0";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbRoomType.DataSource = dt;
                cmbRoomType.DisplayMember = "RoomTypeName";
                cmbRoomType.ValueMember = "RoomTypeID";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRoomNumber.Text) || cmbRoomType.SelectedIndex == -1 || cmbStatus.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill in all required fields.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
    }
} 