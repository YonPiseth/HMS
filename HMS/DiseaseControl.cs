using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using HMS;
using HMS.UI;

namespace HMS
{
    public partial class DiseaseControl : UserControl
    {
        private DataGridView dataGridView1;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnDelete;
        private Button btnUpdate;
        private Button btnLogout;
        private FlowLayoutPanel buttonPanel;

        public DiseaseControl()
        {
            InitializeComponent();
            LoadDiseases();
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
            UIHelper.ApplyPanelStyles(layout);

            Label lblTitle = new Label();
            lblTitle.Text = "Diseases";
            UIHelper.StyleHeading(lblTitle, 3);
            lblTitle.Dock = DockStyle.Fill;

            Panel titlePanel = new Panel();
            titlePanel.Dock = DockStyle.Fill;
            titlePanel.BackColor = Color.Transparent;
            titlePanel.Controls.Add(lblTitle);

            Panel searchPanel = new Panel();
            searchPanel.Dock = DockStyle.Fill;
            searchPanel.Height = 40;
            searchPanel.Padding = new Padding(0, 8, 0, 8);
            this.txtSearch.Dock = DockStyle.Left;
            this.txtSearch.Width = 320;
            UIHelper.StyleModernTextBox(this.txtSearch);
            searchPanel.Controls.Add(this.txtSearch);

            UIHelper.StyleDataGridView(this.dataGridView1);
            Panel gridContainer = UIHelper.WrapDataGridViewInRoundedPanel(this.dataGridView1);
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            this.buttonPanel.Dock = DockStyle.Fill;
            this.buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            this.buttonPanel.Padding = new Padding(0, UITheme.SpacingSM, UITheme.SpacingMD, UITheme.SpacingSM);
            this.buttonPanel.Height = 60;
            this.buttonPanel.AutoSize = false;
            this.buttonPanel.WrapContents = false;

            this.btnAdd.Text = "Add Disease";
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
            this.Size = new System.Drawing.Size(900, 500);
        }

        private void LoadDiseases(string search = "")
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                string query = @"SELECT * FROM tblDisease WHERE IsDeleted = 0 AND (CAST(DiseaseID AS VARCHAR) LIKE @search OR DiseaseName LIKE @search OR Description LIKE @search)";
                Microsoft.Data.SqlClient.SqlDataAdapter da = new Microsoft.Data.SqlClient.SqlDataAdapter(query, con);
                da.SelectCommand.Parameters.AddWithValue("@search", "%" + search + "%");
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadDiseases(txtSearch.Text);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new DiseaseForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = DatabaseHelper.GetConnection())
                {
                    string query = @"INSERT INTO tblDisease (DiseaseName, Description, IsDeleted) 
                                   VALUES (@DiseaseName, @Description, 0)";
                    Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DiseaseName", form.txtDiseaseName.Text);
                    cmd.Parameters.AddWithValue("@Description", form.txtDescription.Text);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadDiseases();
                MessageBox.Show("Disease added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a disease to delete.");
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete this disease?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                using (SqlConnection con = DatabaseHelper.GetConnection())
                {
                    string query = @"UPDATE tblDisease SET IsDeleted = 1 WHERE DiseaseID = @DiseaseID";
                    Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DiseaseID", row.Cells["DiseaseID"].Value);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadDiseases();
                MessageBox.Show("Disease deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a disease to update.");
                return;
            }
            DataGridViewRow row = dataGridView1.SelectedRows[0];
            var form = new DiseaseForm();
            
            // Fill form with selected row data
            form.txtDiseaseName.Text = row.Cells["DiseaseName"].Value?.ToString();
            form.txtDescription.Text = row.Cells["Description"].Value?.ToString();

            if (form.ShowDialog() == DialogResult.OK)
            {
                using (SqlConnection con = DatabaseHelper.GetConnection())
                {
                    string query = @"UPDATE tblDisease SET DiseaseName=@DiseaseName, 
                                   Description=@Description WHERE DiseaseID=@DiseaseID";
                    Microsoft.Data.SqlClient.SqlCommand cmd = new Microsoft.Data.SqlClient.SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@DiseaseID", row.Cells["DiseaseID"].Value);
                    cmd.Parameters.AddWithValue("@DiseaseName", form.txtDiseaseName.Text);
                    cmd.Parameters.AddWithValue("@Description", form.txtDescription.Text);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadDiseases();
                MessageBox.Show("Disease updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
