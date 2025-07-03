using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;

namespace HMS
{
    public partial class MedicineForm : Form
    {
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";

        public MedicineForm()
        {
            InitializeComponent();
            LoadSuppliers();
            LoadCategories();
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