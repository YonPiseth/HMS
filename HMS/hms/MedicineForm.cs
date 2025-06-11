using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

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
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";

        public MedicineForm()
        {
            InitializeComponent();
            LoadSuppliers();
            LoadCategories();
        }

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
            this.Size = new System.Drawing.Size(400, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Medicine Name
            Label lblMedicineName = new Label();
            lblMedicineName.Text = "Medicine Name:";
            lblMedicineName.Location = new System.Drawing.Point(20, 20);
            lblMedicineName.Size = new System.Drawing.Size(120, 20);
            this.txtMedicineName.Location = new System.Drawing.Point(150, 20);
            this.txtMedicineName.Size = new System.Drawing.Size(200, 27);

            // Category
            Label lblCategory = new Label();
            lblCategory.Text = "Category:";
            lblCategory.Location = new System.Drawing.Point(20, 60);
            lblCategory.Size = new System.Drawing.Size(120, 20);
            this.cmbCategory.Location = new System.Drawing.Point(150, 60);
            this.cmbCategory.Size = new System.Drawing.Size(200, 27);
            this.cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            // Supplier
            Label lblSupplier = new Label();
            lblSupplier.Text = "Supplier:";
            lblSupplier.Location = new System.Drawing.Point(20, 100);
            lblSupplier.Size = new System.Drawing.Size(120, 20);
            this.cmbSupplier.Location = new System.Drawing.Point(150, 100);
            this.cmbSupplier.Size = new System.Drawing.Size(200, 27);
            this.cmbSupplier.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbSupplier.DisplayMember = "SupplierName";
            this.cmbSupplier.ValueMember = "SupplierID";

            // Description
            Label lblDescription = new Label();
            lblDescription.Text = "Description:";
            lblDescription.Location = new System.Drawing.Point(20, 140);
            lblDescription.Size = new System.Drawing.Size(120, 20);
            this.txtDescription.Location = new System.Drawing.Point(150, 140);
            this.txtDescription.Size = new System.Drawing.Size(200, 60);
            this.txtDescription.Multiline = true;

            // Dosage
            Label lblDosage = new Label();
            lblDosage.Text = "Dosage:";
            lblDosage.Location = new System.Drawing.Point(20, 220);
            lblDosage.Size = new System.Drawing.Size(120, 20);
            this.txtDosage.Location = new System.Drawing.Point(150, 220);
            this.txtDosage.Size = new System.Drawing.Size(200, 27);

            // Side Effects
            Label lblSideEffects = new Label();
            lblSideEffects.Text = "Side Effects:";
            lblSideEffects.Location = new System.Drawing.Point(20, 260);
            lblSideEffects.Size = new System.Drawing.Size(120, 20);
            this.txtSideEffects.Location = new System.Drawing.Point(150, 260);
            this.txtSideEffects.Size = new System.Drawing.Size(200, 60);
            this.txtSideEffects.Multiline = true;

            // Unit Price
            Label lblUnitPrice = new Label();
            lblUnitPrice.Text = "Unit Price:";
            lblUnitPrice.Location = new System.Drawing.Point(20, 340);
            lblUnitPrice.Size = new System.Drawing.Size(120, 20);
            this.numUnitPrice.Location = new System.Drawing.Point(150, 340);
            this.numUnitPrice.Size = new System.Drawing.Size(200, 27);
            this.numUnitPrice.DecimalPlaces = 2;
            this.numUnitPrice.Maximum = 999999.99m;
            this.numUnitPrice.ThousandsSeparator = true;

            // Stock Quantity
            Label lblStockQuantity = new Label();
            lblStockQuantity.Text = "Stock Quantity:";
            lblStockQuantity.Location = new System.Drawing.Point(20, 380);
            lblStockQuantity.Size = new System.Drawing.Size(120, 20);
            this.numStockQuantity.Location = new System.Drawing.Point(150, 380);
            this.numStockQuantity.Size = new System.Drawing.Size(200, 27);
            this.numStockQuantity.Maximum = 999999;
            this.numStockQuantity.ThousandsSeparator = true;

            // Price
            Label lblPrice = new Label();
            lblPrice.Text = "Price:";
            lblPrice.Location = new System.Drawing.Point(20, 420);
            lblPrice.Size = new System.Drawing.Size(120, 20);
            this.numPrice.Location = new System.Drawing.Point(150, 420);
            this.numPrice.Size = new System.Drawing.Size(200, 27);
            this.numPrice.DecimalPlaces = 2;
            this.numPrice.Maximum = 999999.99m;
            this.numPrice.ThousandsSeparator = true;

            // Save Button
            this.btnSave.Text = "Save";
            this.btnSave.Location = new System.Drawing.Point(150, 480);
            this.btnSave.Size = new System.Drawing.Size(90, 35);
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            // Cancel Button
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(260, 480);
            this.btnCancel.Size = new System.Drawing.Size(90, 35);
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(220, 220, 220);
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            // Add controls
            this.Controls.Add(lblMedicineName);
            this.Controls.Add(this.txtMedicineName);
            this.Controls.Add(lblCategory);
            this.Controls.Add(this.cmbCategory);
            this.Controls.Add(lblSupplier);
            this.Controls.Add(this.cmbSupplier);
            this.Controls.Add(lblDescription);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(lblDosage);
            this.Controls.Add(this.txtDosage);
            this.Controls.Add(lblSideEffects);
            this.Controls.Add(this.txtSideEffects);
            this.Controls.Add(lblUnitPrice);
            this.Controls.Add(this.numUnitPrice);
            this.Controls.Add(lblStockQuantity);
            this.Controls.Add(this.numStockQuantity);
            this.Controls.Add(lblPrice);
            this.Controls.Add(this.numPrice);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
        }

        private void LoadSuppliers()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT SupplierID, SupplierName FROM tblSupplier WHERE IsDeleted = 0";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
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