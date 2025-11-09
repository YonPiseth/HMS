using System;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Drawing;
using HMS;

namespace HMS
{
    public partial class RoomForm : Form
    {

        public RoomForm()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainLayout.RowCount = 7;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            mainLayout.Padding = new Padding(20);
            mainLayout.AutoScroll = true;
            UIHelper.StyleModernPanel(mainLayout);

            Action<string, Control, int> addRow = (labelText, control, row) =>
            {
                Label lbl = new Label { Text = labelText, AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Anchor = AnchorStyles.Left };
                UIHelper.StyleModernLabel(lbl);
                mainLayout.Controls.Add(lbl, 0, row);
                control.Dock = DockStyle.Fill;
                mainLayout.Controls.Add(control, 1, row);
            };

            addRow("Room Number:", txtRoomNumber, 0);
            UIHelper.StyleModernTextBox(this.txtRoomNumber);

            addRow("Room Type:", cmbRoomType, 1);
            UIHelper.StyleModernComboBox(this.cmbRoomType);
            this.cmbRoomType.DropDownStyle = ComboBoxStyle.DropDownList;

            addRow("Floor:", numFloor, 2);
            this.numFloor.Dock = DockStyle.Fill;
            this.numFloor.Minimum = 1;
            this.numFloor.Maximum = 100;
            this.numFloor.Font = new Font("Segoe UI", 10);

            addRow("Capacity:", numCapacity, 3);
            this.numCapacity.Dock = DockStyle.Fill;
            this.numCapacity.Minimum = 1;
            this.numCapacity.Maximum = 10;
            this.numCapacity.Font = new Font("Segoe UI", 10);

            addRow("Rate Per Day:", numRatePerDay, 4);
            this.numRatePerDay.Dock = DockStyle.Fill;
            this.numRatePerDay.Minimum = 0;
            this.numRatePerDay.Maximum = 100000;
            this.numRatePerDay.DecimalPlaces = 2;
            this.numRatePerDay.Font = new Font("Segoe UI", 10);

            addRow("Status:", cmbStatus, 5);
            UIHelper.StyleModernComboBox(this.cmbStatus);
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new string[] { "Available", "Occupied", "Maintenance" });

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Padding = new Padding(0, 10, 0, 0);
            buttonPanel.BackColor = Color.Transparent;

            this.btnCancel.Text = "Cancel";
            UIHelper.StyleModernButton(this.btnCancel);
            this.btnCancel.Width = 100;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            buttonPanel.Controls.Add(this.btnCancel);

            this.btnSave.Text = "Save";
            UIHelper.StyleModernButton(this.btnSave);
            this.btnSave.Width = 100;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            buttonPanel.Controls.Add(this.btnSave);

            mainLayout.Controls.Add(buttonPanel, 0, 6);
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
            using (Microsoft.Data.SqlClient.SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = "SELECT RoomTypeID, RoomTypeName FROM tblRoomType WHERE IsDeleted = 0";
                Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(query, con);
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