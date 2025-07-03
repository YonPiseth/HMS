using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing; // Added for Color and Padding

namespace HMS
{
    public partial class AppointmentForm : Form
    {
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True";

        public AppointmentForm()
        {
            InitializeComponent();
            LoadPatients();
            LoadDoctors();
        }

        private void LoadPatients()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT PatientID, FirstName + ' ' + LastName as FullName FROM tblPatient WHERE IsDeleted = 0";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbPatient.DataSource = dt;
                cmbPatient.DisplayMember = "FullName";
                cmbPatient.ValueMember = "PatientID";
            }
        }

        private void LoadDoctors()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT DoctorID, FirstName + ' ' + LastName as FullName FROM tblDoctor WHERE IsDeleted = 0";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cmbDoctor.DataSource = dt;
                cmbDoctor.DisplayMember = "FullName";
                cmbDoctor.ValueMember = "DoctorID";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbPatient.SelectedIndex == -1 ||
                cmbDoctor.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtReason.Text) ||
                cmbStatus.SelectedIndex == -1)
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
