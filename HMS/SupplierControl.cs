using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using System.Linq;
using HMS.UI;
using HMS;
using HMS.Repositories;
using HMS.Models;

namespace HMS
{
    public partial class SupplierControl : UserControl
    {
        private DataGridView dataGridView1;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnUpdate;
        private Button btnLogout;
        private FlowLayoutPanel buttonPanel;
        private SupplierRepository repository;

        public SupplierControl()
        {
            repository = new SupplierRepository();
            InitializeComponent();
            LoadSuppliers();
            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);
            this.dataGridView1.DataBindingComplete += (s, e) => {
                if (this.dataGridView1.Columns.Contains("IsDeleted"))
                    this.dataGridView1.Columns["IsDeleted"].Visible = false;
            };
        }

        private void InitializeComponent()
        {
            this.dataGridView1 = new DataGridView();
            this.txtSearch = new TextBox();
            this.btnAdd = new Button();
            this.btnDelete = new Button();
            this.btnUpdate = new Button();
            this.btnLogout = new Button();
            this.buttonPanel = new FlowLayoutPanel();

            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 4;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            layout.Padding = new Padding(16);
            layout.BackColor = Color.White;

            Panel titlePanel = new Panel();
            titlePanel.Dock = DockStyle.Fill;
            titlePanel.BackColor = Color.Transparent;
            Label lblTitle = new Label();
            lblTitle.Text = "Suppliers";
            UIHelper.StyleHeading(lblTitle, 3);
            lblTitle.Location = new Point(0, 0);
            titlePanel.Controls.Add(lblTitle);

            FlowLayoutPanel searchFlowPanel = new FlowLayoutPanel();
            searchFlowPanel.Dock = DockStyle.Fill;
            searchFlowPanel.Padding = new Padding(0, UITheme.SpacingSM, 0, 0);
            searchFlowPanel.FlowDirection = FlowDirection.LeftToRight;
            searchFlowPanel.BackColor = Color.Transparent;
            
            Label lblSearch = new Label 
            { 
                Text = "Search:", 
                AutoSize = true, 
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 8, UITheme.SpacingSM, 0)
            };
            UIHelper.StyleModernLabel(lblSearch);
            searchFlowPanel.Controls.Add(lblSearch);

            this.txtSearch.Width = 250;
            this.txtSearch.Height = 30;
            UIHelper.StyleModernTextBox(this.txtSearch);
            searchFlowPanel.Controls.Add(this.txtSearch);

            Panel searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Fill;
            searchPanel.BackColor = Color.Transparent;
            searchPanel.Controls.Add(searchFlowPanel);

            UIHelper.StyleDataGridView(this.dataGridView1);
            Panel gridContainer = UIHelper.WrapDataGridViewInRoundedPanel(this.dataGridView1);

            this.buttonPanel.Dock = DockStyle.Fill;
            this.buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            this.buttonPanel.Padding = new Padding(0, UITheme.SpacingSM, UITheme.SpacingMD, UITheme.SpacingSM);
            this.buttonPanel.Height = 60;
            this.buttonPanel.AutoSize = false;
            this.buttonPanel.WrapContents = false;

            this.btnAdd.Text = "Add Supplier";
            this.btnUpdate.Text = "Update";
            this.btnDelete.Text = "Delete";
            this.btnLogout.Text = "Log Out";
            
            foreach (Button btn in new[] { btnAdd, btnUpdate, btnDelete, btnLogout })
            {
                UIHelper.StyleModernButton(btn);
                btn.Width = 120;
                btn.Height = 40;
                btn.Margin = new Padding(UITheme.SpacingSM, 0, 0, 0);
            }
            
            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
            this.btnUpdate.Click += new EventHandler(this.btnUpdate_Click);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            this.btnLogout.Click += new EventHandler(this.btnLogout_Click);
            
            this.buttonPanel.Controls.Add(this.btnLogout);
            this.buttonPanel.Controls.Add(this.btnDelete);
            this.buttonPanel.Controls.Add(this.btnUpdate);
            this.buttonPanel.Controls.Add(this.btnAdd);

            layout.Controls.Add(titlePanel, 0, 0);
            layout.Controls.Add(searchPanel, 0, 1);
            layout.Controls.Add(gridContainer, 0, 2);
            layout.Controls.Add(this.buttonPanel, 0, 3);
            
            this.Controls.Clear();
            this.Controls.Add(layout);
            this.Size = new Size(900, 500);
        }

        private void LoadSuppliers(string search = "")
        {
            var suppliers = string.IsNullOrWhiteSpace(search)
                ? repository.GetAll()
                : repository.Search(search);

            DataTable dt = new DataTable();
            dt.Columns.Add("SupplierID", typeof(int));
            dt.Columns.Add("SupplierName", typeof(string));
            dt.Columns.Add("ContactPerson", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("Phone", typeof(string));
            dt.Columns.Add("Address", typeof(string));

            foreach (var supplier in suppliers)
            {
                dt.Rows.Add(
                    supplier.SupplierID,
                    supplier.SupplierName,
                    supplier.ContactPerson,
                    supplier.Email,
                    supplier.Phone,
                    supplier.Address
                );
            }

            dataGridView1.DataSource = dt;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadSuppliers(txtSearch.Text);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new SupplierForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadSuppliers();
                MessageBox.Show("Supplier added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a supplier to delete.");
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            int supplierId = Convert.ToInt32(row.Cells["SupplierID"].Value);
            if (repository.Delete(supplierId))
            {
                LoadSuppliers();
                MessageBox.Show("Supplier deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to delete supplier.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a supplier to update.");
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            var form = new SupplierForm();
            
            form.txtSupplierName.Text = row.Cells["SupplierName"].Value?.ToString();
            form.txtContactPerson.Text = row.Cells["ContactPerson"].Value?.ToString();
            form.txtEmail.Text = row.Cells["Email"].Value?.ToString();
            form.txtPhone.Text = row.Cells["Phone"].Value?.ToString();
            form.txtAddress.Text = row.Cells["Address"].Value?.ToString();

            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadSuppliers();
                MessageBox.Show("Supplier updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (this.ParentForm is MainForm mainForm)
            {
                mainForm.Logout();
            }
        }
    }
} 