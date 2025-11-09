using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;
using HMS;

namespace HMS
{
    public partial class MedicineForm : Form
    {
        public MedicineForm()
        {
            InitializeComponent();
            SetupUI();
            LoadSuppliers();
            LoadCategories();
        }

        private void SetupUI()
        {
            // Main layout panel
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            mainLayout.RowCount = 10;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            mainLayout.Padding = new Padding(15);
            mainLayout.AutoScroll = true;

            // Medicine Name
            Label lblMedicineName = new Label();
            lblMedicineName.Text = "Medicine Name:";
            UIHelper.StyleLabel(lblMedicineName);
            this.txtMedicineName.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(this.txtMedicineName);

            // Category
            Label lblCategory = new Label();
            lblCategory.Text = "Category:";
            UIHelper.StyleLabel(lblCategory);
            this.cmbCategory.Dock = DockStyle.Fill;
            UIHelper.StyleComboBox(this.cmbCategory);
            this.cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            // Supplier
            Label lblSupplier = new Label();
            lblSupplier.Text = "Supplier:";
            UIHelper.StyleLabel(lblSupplier);
            this.cmbSupplier.Dock = DockStyle.Fill;
            UIHelper.StyleComboBox(this.cmbSupplier);
            this.cmbSupplier.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbSupplier.DisplayMember = "SupplierName";
            this.cmbSupplier.ValueMember = "SupplierID";

            // Description
            Label lblDescription = new Label();
            lblDescription.Text = "Description:";
            UIHelper.StyleLabel(lblDescription);
            this.txtDescription.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(this.txtDescription);
            this.txtDescription.Multiline = true;
            this.txtDescription.Height = 60;

            // Dosage
            Label lblDosage = new Label();
            lblDosage.Text = "Dosage:";
            UIHelper.StyleLabel(lblDosage);
            this.txtDosage.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(this.txtDosage);

            // Side Effects
            Label lblSideEffects = new Label();
            lblSideEffects.Text = "Side Effects:";
            UIHelper.StyleLabel(lblSideEffects);
            this.txtSideEffects.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(this.txtSideEffects);
            this.txtSideEffects.Multiline = true;
            this.txtSideEffects.Height = 60;

            // Unit Price
            Label lblUnitPrice = new Label();
            lblUnitPrice.Text = "Unit Price:";
            UIHelper.StyleLabel(lblUnitPrice);
            this.numUnitPrice.Dock = DockStyle.Fill;
            this.numUnitPrice.DecimalPlaces = 2;
            this.numUnitPrice.Maximum = 999999.99m;
            this.numUnitPrice.ThousandsSeparator = true;
            this.numUnitPrice.Font = new Font("Segoe UI", 10);

            // Stock Quantity
            Label lblStockQuantity = new Label();
            lblStockQuantity.Text = "Stock Quantity:";
            UIHelper.StyleLabel(lblStockQuantity);
            this.numStockQuantity.Dock = DockStyle.Fill;
            this.numStockQuantity.Maximum = 999999;
            this.numStockQuantity.ThousandsSeparator = true;
            this.numStockQuantity.Font = new Font("Segoe UI", 10);

            // Buttons Panel
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Padding = new Padding(0, 5, 0, 0);

            this.btnSave.Text = "Save";
            UIHelper.StyleButton(this.btnSave);
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            this.btnSave.Width = 100;

            this.btnCancel.Text = "Cancel";
            UIHelper.StyleButton(this.btnCancel);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            this.btnCancel.Width = 100;

            buttonPanel.Controls.Add(this.btnCancel);
            buttonPanel.Controls.Add(this.btnSave);

            // Add controls to main layout
            mainLayout.Controls.Add(lblMedicineName, 0, 0);
            mainLayout.Controls.Add(this.txtMedicineName, 1, 0);

            mainLayout.Controls.Add(lblCategory, 0, 1);
            mainLayout.Controls.Add(this.cmbCategory, 1, 1);

            mainLayout.Controls.Add(lblSupplier, 0, 2);
            mainLayout.Controls.Add(this.cmbSupplier, 1, 2);

            mainLayout.Controls.Add(lblDescription, 0, 3);
            mainLayout.Controls.Add(this.txtDescription, 1, 3);
            mainLayout.SetRowSpan(this.txtDescription, 2);

            mainLayout.Controls.Add(lblDosage, 0, 5);
            mainLayout.Controls.Add(this.txtDosage, 1, 5);

            mainLayout.Controls.Add(lblSideEffects, 0, 6);
            mainLayout.Controls.Add(this.txtSideEffects, 1, 6);
            mainLayout.SetRowSpan(this.txtSideEffects, 2);

            mainLayout.Controls.Add(lblUnitPrice, 0, 8);
            mainLayout.Controls.Add(this.numUnitPrice, 1, 8);

            mainLayout.Controls.Add(lblStockQuantity, 0, 9);
            mainLayout.Controls.Add(this.numStockQuantity, 1, 9);

            mainLayout.Controls.Add(buttonPanel, 0, 10);
            mainLayout.SetColumnSpan(buttonPanel, 2);
            mainLayout.RowCount = 11;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            this.Controls.Add(mainLayout);
        }

        private void LoadSuppliers()
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = "SELECT SupplierID, SupplierName FROM tblSupplier WHERE IsDeleted = 0";
                Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbSupplier.DataSource = dt;
            }
        }

        private void LoadCategories()
        {
            cmbCategory.Items.AddRange(new string[] { "Tablet", "Capsule", "Syrup", "Injection", "Cream", "Ointment", "Drops", "Other" });
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMedicineName.Text) || 
                cmbSupplier.SelectedIndex == -1 || 
                cmbCategory.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtDescription.Text))
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