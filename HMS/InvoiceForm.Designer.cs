using System;
using System.Windows.Forms;
using System.Drawing;

namespace HMS
{
    public partial class InvoiceForm : Form
    {
        private DataGridView dgvPatientBills;
        private DataGridView dgvSelectedBills;
        private Button btnGenerateInvoice;
        private Button btnCancel;
        private Label lblPatientInfo;

        private void InitializeComponent()
        {
            this.Text = "Generate Invoice";
            this.Size = new Size(800, 600);
            this.MinimumSize = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Main TableLayoutPanel for overall layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10)
            };
            UIHelper.ApplyPanelStyles(mainLayout);
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 85));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

            // Patient Info Label
            lblPatientInfo = new Label
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "Patient: Loading...",
                Padding = new Padding(10, 20, 10, 0)
            };
            UIHelper.StyleLabelTitle(lblPatientInfo);
            mainLayout.Controls.Add(lblPatientInfo, 0, 0);

            // SplitContainer for Patient Bills and Selected Bills
            splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = (int)(this.Height * 0.5),
                FixedPanel = FixedPanel.Panel1,
                BorderStyle = BorderStyle.None
            };
            splitContainer.Panel1.Padding = new Padding(0, 0, 0, 5);
            splitContainer.Panel2.Padding = new Padding(0, 5, 0, 0);
            mainLayout.Controls.Add(splitContainer, 0, 1);

            // Patient Bills DataGridView
            dgvPatientBills = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = true,
                ReadOnly = false,
                RowHeadersVisible = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AutoGenerateColumns = false,
                ColumnHeadersVisible = true,
                EnableHeadersVisualStyles = false,
                RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders,
                TabIndex = 0
            };
            UIHelper.StyleDataGridView(dgvPatientBills);
            SetupPatientBillsDataGridView();
            dgvPatientBills.CellContentClick += DgvPatientBills_CellContentClick;
            dgvPatientBills.EditMode = DataGridViewEditMode.EditOnEnter;
            splitContainer.Panel1.Controls.Add(dgvPatientBills);

            // Selected Bills DataGridView
            dgvSelectedBills = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersVisible = true,
                EnableHeadersVisualStyles = false,
                RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders
            };
            UIHelper.StyleDataGridView(dgvSelectedBills);
            SetupSelectedBillsDataGridView();
            splitContainer.Panel2.Controls.Add(dgvSelectedBills);

            // Buttons Panel
            buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 5, 0, 0)
            };
            UIHelper.ApplyPanelStyles(buttonPanel);

            btnGenerateInvoice = new Button
            {
                Text = "Generate Invoice",
                AutoSize = true,
                Padding = new Padding(15, 8, 15, 8),
                TabIndex = 1
            };
            UIHelper.StyleButton(btnGenerateInvoice);
            btnGenerateInvoice.Click += BtnGenerateInvoice_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                AutoSize = true,
                Padding = new Padding(15, 8, 15, 8)
            };
            UIHelper.StyleButton(btnCancel);
            btnCancel.Click += BtnCancel_Click;

            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(btnGenerateInvoice);
            mainLayout.Controls.Add(buttonPanel, 0, 2);

            this.Controls.Clear();
            this.Controls.Add(mainLayout);

            AdjustSplitterDistance();
        }
    }
} 