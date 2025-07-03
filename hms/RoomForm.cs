using System;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;

namespace HMS
{
    public partial class RoomForm : Form
    {
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";

        public RoomForm()
        {
            InitializeComponent();
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