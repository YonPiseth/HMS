using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;
using HMS.Forms;
using HMS;
using HMS.UI;

namespace HMS
{
    public partial class InvoiceControl : UserControl
    {
        private DataGridView dataGridView1;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnUpdate;
        private Button btnLogout;
        private Button btnViewReceipts;
        private FlowLayoutPanel buttonPanel;
        private Label lblPatientDetails;
        private Panel patientDetailsPanel;

        public InvoiceControl()
        {
            InitializeComponent();
            LoadPatients();
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);
            this.dataGridView1.SelectionChanged += new EventHandler(this.dataGridView1_SelectionChanged);
        }

        private void InitializeComponent()
        {
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(10)
            };
            UIHelper.ApplyPanelStyles(mainLayout);
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Label lblTitle = new Label
            {
                Text = "Patient Invoices",
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };
            UIHelper.StyleLabelTitle(lblTitle);
            mainLayout.Controls.Add(lblTitle, 0, 0);

            FlowLayoutPanel searchFlowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 5, 0, 0),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };
            searchFlowPanel.Controls.Add(new Label { Text = "Search Patient: ", AutoSize = true, TextAlign = ContentAlignment.MiddleLeft });
            UIHelper.StyleLabel((Label)searchFlowPanel.Controls[0]);

            this.txtSearch = new TextBox
            {
                Width = 250,
                Height = 30,
                Anchor = AnchorStyles.Left
            };
            UIHelper.StyleTextBox(this.txtSearch);
            searchFlowPanel.Controls.Add(this.txtSearch);
            mainLayout.Controls.Add(searchFlowPanel, 0, 1);

            patientDetailsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            UIHelper.ApplyPanelStyles(patientDetailsPanel);

            lblPatientDetails = new Label
            {
                Text = "Select a patient to view details and create invoice/receipt",
                AutoSize = true,
                Location = new Point(10, 10)
            };
            UIHelper.StyleLabel(lblPatientDetails);
            patientDetailsPanel.Controls.Add(lblPatientDetails);
            mainLayout.Controls.Add(patientDetailsPanel, 0, 2);
            this.dataGridView1 = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoGenerateColumns = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing, // Disable resizing for headers
                AllowUserToResizeRows = false // Prevent row height changes
            };
            UIHelper.StyleDataGridView(this.dataGridView1);
            SetupDataGridViewColumns();
            Panel gridContainer = UIHelper.WrapDataGridViewInRoundedPanel(this.dataGridView1);
            mainLayout.Controls.Add(gridContainer, 0, 3); // Use wrapped grid container

            // Button Panel - Standardized
            this.buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, UITheme.SpacingSM, UITheme.SpacingMD, UITheme.SpacingSM),
                Height = 60,
                AutoSize = false,
                WrapContents = false
            };

            this.btnAdd = new Button { Text = "Create Invoice" };
            this.btnUpdate = new Button { Text = "Update" };
            this.btnDelete = new Button { Text = "Delete" };
            this.btnViewReceipts = new Button { Text = "View Receipts" };
            this.btnLogout = new Button { Text = "Log Out" };

            foreach (Button btn in new[] { btnAdd, btnUpdate, btnDelete, btnViewReceipts, btnLogout })
            {
                UIHelper.StyleModernButton(btn);
                btn.Width = 120;
                btn.Height = 40;
                btn.Margin = new Padding(UITheme.SpacingSM, 0, 0, 0);
            }

            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
            this.btnUpdate.Click += new EventHandler(this.btnUpdate_Click);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            this.btnViewReceipts.Click += new EventHandler(this.btnViewReceipts_Click);
            this.btnLogout.Click += new EventHandler(this.btnLogout_Click);

            // Add in reverse order for RightToLeft flow
            this.buttonPanel.Controls.Add(this.btnLogout);
            this.buttonPanel.Controls.Add(this.btnViewReceipts);
            this.buttonPanel.Controls.Add(this.btnDelete);
            this.buttonPanel.Controls.Add(this.btnUpdate);
            this.buttonPanel.Controls.Add(this.btnAdd);
            
            // Update layout to include button panel row
            mainLayout.RowCount = 5;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Button panel
            mainLayout.Controls.Add(this.buttonPanel, 0, 4); // Added to row 4

            this.Controls.Clear();
            this.Controls.Add(mainLayout);
        }

        private void SetupDataGridViewColumns()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("PatientID", "Patient ID");
            dataGridView1.Columns["PatientID"].DataPropertyName = "PatientID";
            dataGridView1.Columns.Add("FirstName", "First Name");
            dataGridView1.Columns["FirstName"].DataPropertyName = "FirstName";
            dataGridView1.Columns.Add("LastName", "Last Name");
            dataGridView1.Columns["LastName"].DataPropertyName = "LastName";
            dataGridView1.Columns.Add("ContactNumber", "Phone");
            dataGridView1.Columns["ContactNumber"].DataPropertyName = "ContactNumber";
            dataGridView1.Columns.Add("Email", "Email");
            dataGridView1.Columns["Email"].DataPropertyName = "Email";
            dataGridView1.Columns.Add("LastVisit", "Last Visit");
            dataGridView1.Columns["LastVisit"].DataPropertyName = "LastVisit";
            dataGridView1.Columns.Add("OutstandingBalance", "Outstanding Balance");
            dataGridView1.Columns["OutstandingBalance"].DataPropertyName = "OutstandingBalance";

            // Set column widths
            dataGridView1.Columns["PatientID"].Width = 80;
            dataGridView1.Columns["FirstName"].Width = 120;
            dataGridView1.Columns["LastName"].Width = 120;
            dataGridView1.Columns["ContactNumber"].Width = 100;
            dataGridView1.Columns["Email"].Width = 150;
            dataGridView1.Columns["LastVisit"].Width = 100;
            dataGridView1.Columns["OutstandingBalance"].Width = 120;

            // Format columns
            dataGridView1.Columns["OutstandingBalance"].DefaultCellStyle.Format = "C";
            dataGridView1.Columns["LastVisit"].DefaultCellStyle.Format = "MM/dd/yyyy";

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.HeaderCell.Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void LoadPatients(string search = "")
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT p.PatientID, p.FirstName, p.LastName, p.ContactNumber, p.Email,
                               (SELECT MAX(InvoiceDate) FROM tblInvoice WHERE PatientID = p.PatientID) as LastVisit,
                               (SELECT SUM(TotalAmount) FROM tblInvoice WHERE PatientID = p.PatientID AND PaymentStatus = 'Pending') as OutstandingBalance
                               FROM tblPatient p
                               WHERE p.IsDeleted = 0
                               AND (p.FirstName + ' ' + p.LastName LIKE @search 
                               OR p.ContactNumber LIKE @search 
                               OR p.Email LIKE @search)
                               ORDER BY p.PatientID ASC"; // Order by PatientID in ascending order

                Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                string patientDetails = $"Patient: {row.Cells["FirstName"].Value} {row.Cells["LastName"].Value}\n" +
                                     $"Phone: {row.Cells["ContactNumber"].Value}\n" +
                                     $"Email: {row.Cells["Email"].Value}\n" +
                                     $"Last Visit: {row.Cells["LastVisit"].Value}\n" +
                                     $"Outstanding Balance: {row.Cells["OutstandingBalance"].Value:C}";
                lblPatientDetails.Text = patientDetails;
            }
            else
            {
                lblPatientDetails.Text = "Select a patient to view details and create invoice/receipt";
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadPatients(txtSearch.Text);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a patient to create an invoice.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int patientId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["PatientID"].Value);
            var invoiceForm = new InvoiceForm(patientId);
            invoiceForm.ShowDialog();
            LoadPatients(txtSearch.Text); // Refresh the list
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a patient to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this patient?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int patientId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["PatientID"].Value);
                using (SqlConnection con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    string query = "UPDATE tblPatient SET IsDeleted = 1 WHERE PatientID = @PatientID";
                    Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@PatientID", patientId);
                    cmd.ExecuteNonQuery();
                }
                LoadPatients(txtSearch.Text);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a patient to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int patientId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["PatientID"].Value);
            var patientForm = new PatientRegistrationForm(patientId);
            patientForm.ShowDialog();
            LoadPatients(txtSearch.Text);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (this.ParentForm is MainForm mainForm)
            {
                mainForm.Logout();
            }
        }

        private void btnViewReceipts_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a patient to view their invoice history.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int patientId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["PatientID"].Value);
            var patientInvoiceHistoryForm = new HMS.Forms.PatientInvoiceHistoryForm(patientId);
            patientInvoiceHistoryForm.ShowDialog();
        }
    }
} 