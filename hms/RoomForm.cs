using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
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
            this.Size = new Size(450, 480); // Adjusted size
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White; // Set form background color

            // Main layout panel
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainLayout.RowCount = 7; // Number of rows for controls + buttons
            for (int i = 0; i < 6; i++) // Set height for control rows
            {
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            }
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Buttons row
            mainLayout.Padding = new Padding(20); // More generous padding
            mainLayout.AutoScroll = true;
            UIHelper.ApplyPanelStyles(mainLayout); // Apply panel style to main layout

            // Helper to add a row of label and control
            Action<string, Control, int> addRow = (labelText, control, row) =>
            {
                Label lbl = new Label { Text = labelText, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left };
                UIHelper.StyleLabel(lbl);
                mainLayout.Controls.Add(lbl, 0, row);
                control.Dock = DockStyle.Fill;
                mainLayout.Controls.Add(control, 1, row);
            };

            // Room Number
            addRow("Room Number:", txtRoomNumber, 0);
            UIHelper.StyleTextBox(this.txtRoomNumber);

            // Room Type
            addRow("Room Type:", cmbRoomType, 1);
            UIHelper.StyleComboBox(this.cmbRoomType);
            this.cmbRoomType.DropDownStyle = ComboBoxStyle.DropDownList;

            // Floor
            addRow("Floor:", numFloor, 2);
            this.numFloor.Dock = DockStyle.Fill;
            this.numFloor.Minimum = 1;
            this.numFloor.Maximum = 100;
            this.numFloor.Font = new Font("Segoe UI", 10); // Manual style for NumericUpDown

            // Capacity
            addRow("Capacity:", numCapacity, 3);
            this.numCapacity.Dock = DockStyle.Fill;
            this.numCapacity.Minimum = 1;
            this.numCapacity.Maximum = 10;
            this.numCapacity.Font = new Font("Segoe UI", 10); // Manual style for NumericUpDown

            // Rate Per Day
            addRow("Rate Per Day:", numRatePerDay, 4);
            this.numRatePerDay.Dock = DockStyle.Fill;
            this.numRatePerDay.Minimum = 0;
            this.numRatePerDay.Maximum = 100000;
            this.numRatePerDay.DecimalPlaces = 2;
            this.numRatePerDay.Font = new Font("Segoe UI", 10); // Manual style for NumericUpDown

            // Status
            addRow("Status:", cmbStatus, 5);
            UIHelper.StyleComboBox(this.cmbStatus);
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new string[] { "Available", "Occupied", "Maintenance" });

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

            mainLayout.Controls.Add(buttonPanel, 0, 6); // Buttons at the bottom
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);
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
    }
} 