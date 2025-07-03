using System;
using System.Windows.Forms;
using System.Drawing;

namespace HMS
{
    public partial class MedicineForm : Form
    {
        public TextBox txtMedicineName;
        public ComboBox cmbSupplier;
        public TextBox txtDosage;
        public TextBox txtSideEffects;
        public NumericUpDown numPrice;
        public ComboBox cmbCategory;
        public TextBox txtDescription;
        public NumericUpDown numUnitPrice;
        public NumericUpDown numStockQuantity;
        private Button btnSave;
        private Button btnCancel;

        private void InitializeComponent()
        {
            this.txtMedicineName = new TextBox();
            this.cmbSupplier = new ComboBox();
            this.txtDosage = new TextBox();
            this.txtSideEffects = new TextBox();
            this.numPrice = new NumericUpDown();
            this.cmbCategory = new ComboBox();
            this.txtDescription = new TextBox();
            this.numUnitPrice = new NumericUpDown();
            this.numStockQuantity = new NumericUpDown();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            // Form settings
            this.Text = "Medicine Information";
            this.Size = new System.Drawing.Size(450, 650); // Adjusted size for better layout
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
            mainLayout.RowCount = 10; // Number of rows for controls + buttons
            for (int i = 0; i < 9; i++) // Set height for control rows
            {
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            }
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Buttons row
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
            this.txtDescription.Height = 60; // Allow multiple lines
            mainLayout.RowStyles[4] = new RowStyle(SizeType.Absolute, 70); // Adjust row height for multiline textbox

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
            this.txtSideEffects.Height = 60; // Allow multiple lines
            mainLayout.RowStyles[6] = new RowStyle(SizeType.Absolute, 70); // Adjust row height for multiline textbox

            // Unit Price
            Label lblUnitPrice = new Label();
            lblUnitPrice.Text = "Unit Price:";
            UIHelper.StyleLabel(lblUnitPrice);
            this.numUnitPrice.Dock = DockStyle.Fill;
            this.numUnitPrice.DecimalPlaces = 2;
            this.numUnitPrice.Maximum = 999999.99m;
            this.numUnitPrice.ThousandsSeparator = true;
            // No specific UIHelper for NumericUpDown, retain manual styling

            // Stock Quantity
            Label lblStockQuantity = new Label();
            lblStockQuantity.Text = "Stock Quantity:";
            UIHelper.StyleLabel(lblStockQuantity);
            this.numStockQuantity.Dock = DockStyle.Fill;
            this.numStockQuantity.Maximum = 999999;
            this.numStockQuantity.ThousandsSeparator = true;
            // No specific UIHelper for NumericUpDown, retain manual styling

            // Price (Assuming this is a redundant control or needs clarification, keeping for now but will adjust if needed)
            Label lblPrice = new Label();
            lblPrice.Text = "Price:";
            UIHelper.StyleLabel(lblPrice);
            this.numPrice.Dock = DockStyle.Fill;
            this.numPrice.DecimalPlaces = 2;
            this.numPrice.Maximum = 999999.99m;
            this.numPrice.ThousandsSeparator = true;

            // Buttons Panel
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Padding = new Padding(0, 5, 0, 0);

            this.btnSave.Text = "Save";
            UIHelper.StyleButton(this.btnSave); // Apply button styling
            this.btnSave.Click += new EventHandler(this.btnSave_Click);
            this.btnSave.Width = 100; // Adjusted width for form buttons

            this.btnCancel.Text = "Cancel";
            UIHelper.StyleButton(this.btnCancel); // Apply button styling
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            this.btnCancel.Width = 100; // Adjusted width for form buttons

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
            mainLayout.SetRowSpan(this.txtDescription, 2); // Span 2 rows for multiline textbox

            mainLayout.Controls.Add(lblDosage, 0, 5);
            mainLayout.Controls.Add(this.txtDosage, 1, 5);

            mainLayout.Controls.Add(lblSideEffects, 0, 6);
            mainLayout.Controls.Add(this.txtSideEffects, 1, 6);
            mainLayout.SetRowSpan(this.txtSideEffects, 2); // Span 2 rows for multiline textbox

            mainLayout.Controls.Add(lblUnitPrice, 0, 8);
            mainLayout.Controls.Add(this.numUnitPrice, 1, 8);

            mainLayout.Controls.Add(lblStockQuantity, 0, 9);
            mainLayout.Controls.Add(this.numStockQuantity, 1, 9);

            // Adjust the row for Price or consider removing if redundant
            // For now, removing Price label and control as it seems redundant with Unit Price and Stock Quantity
            // If 'Price' is meant to be 'Total Price' or something similar, it should be calculated.

            mainLayout.Controls.Add(buttonPanel, 0, 10); // Buttons at the bottom
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);
        }
    }
} 