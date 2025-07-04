using System;
using System.Windows.Forms;
using HMS;
using System.Drawing;

namespace HMS
{
    public partial class MainForm : Form
    {
        private Panel contentPanel;
        private Panel navPanel;
        private string userRole;
        private int userID;

        public bool IsLogout { get; private set; } = false;

        public MainForm(string role, int id)
        {
            userRole = role;
            userID = id;
            InitializeComponent();
        }

        private void ShowPatientDetails()
        {
            // This method is no longer directly used as ShowPatientView now combines details and appointments.
            // Keeping it here for now, but its logic is integrated into ShowPatientView.
            contentPanel.Controls.Clear();
            Panel patientInfoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.FromArgb(240, 240, 240)
            };
            
            using (var con = new System.Data.SqlClient.SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True"))
            {
                con.Open();
                var cmd = new System.Data.SqlClient.SqlCommand(
                    "SELECT p.*, r.RoomNumber FROM tblPatient p " +
                    "LEFT JOIN tblRoom r ON p.PatientID = r.PatientID " +
                    "WHERE p.UserID = @UserID", con);
                cmd.Parameters.AddWithValue("@UserID", userID);

                using (var reader = cmd.ExecuteReader()) // Use a different reader variable name
                {
                    if (reader.Read())
                    {
                        Label lblInfo = new Label
                        {
                            Text = $"Patient Name: {reader["FirstName"]} {reader["LastName"]}\n" +
                                  $"Date of Birth: {((DateTime)reader["DateOfBirth"]).ToShortDateString()}\n" +
                                  $"Gender: {reader["Gender"]}\n" +
                                  $"Contact: {reader["ContactNumber"]}\n" +
                                  $"Email: {reader["Email"]}\n" +
                                  $"Address: {reader["Address"]}\n" +
                                  $"Room: {(reader["RoomNumber"] != DBNull.Value ? reader["RoomNumber"] : "Not Assigned")}",
                            Font = new System.Drawing.Font("Segoe UI", 12),
                            Dock = DockStyle.Fill,
                            TextAlign = System.Drawing.ContentAlignment.TopLeft,
                            Padding = new Padding(20, 20, 0, 0)
                        };
                        patientInfoPanel.Controls.Add(lblInfo);
                    }
                    else
                    {
                        Label lblNoInfo = new Label
                        {
                            Text = "No patient information found.",
                            Font = new System.Drawing.Font("Segoe UI", 12),
                            Dock = DockStyle.Fill,
                            TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                        };
                        patientInfoPanel.Controls.Add(lblNoInfo);
                    }
                }
            }
            contentPanel.Controls.Add(patientInfoPanel);
        }

        private void ShowPatientAppointments()
        {
            // This method is no longer directly used as ShowPatientView now combines details and appointments.
            // Keeping it here for now, but its logic is integrated into ShowPatientView.
            contentPanel.Controls.Clear();
            var appointmentControl = new AppointmentControl();
            appointmentControl.LoadPatientAppointments(userID); // Assuming LoadPatientAppointments exists and uses UserID for filtering
            ShowControl(appointmentControl);
        }

        private void ShowPatientView()
        {
            contentPanel.Controls.Clear();

            // Main layout panel for patient dashboard
            TableLayoutPanel dashboardLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                RowStyles = { new RowStyle(SizeType.Percent, 40F), new RowStyle(SizeType.Percent, 60F) },
                Padding = new Padding(10),
                BackColor = System.Drawing.Color.White
            };

            // ------------------ Patient Details Panel ------------------
            Panel patientDetailsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.FromArgb(230, 245, 255), // Light blue background
                Padding = new Padding(15),
                Margin = new Padding(5)
            };
            patientDetailsPanel.BorderStyle = BorderStyle.FixedSingle;

            Label lblDetailsTitle = new Label
            {
                Text = "Your Profile",
                Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(24, 33, 54),
                Dock = DockStyle.Top,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Height = 40
            };
            patientDetailsPanel.Controls.Add(lblDetailsTitle);

            Panel detailsContentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 0, 10, 10) // Adjust padding
            };
            patientDetailsPanel.Controls.Add(detailsContentPanel);
            detailsContentPanel.BringToFront(); // Ensure content is on top of title

            // Load patient details into detailsContentPanel
            using (var con = new System.Data.SqlClient.SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True"))
            {
                con.Open();
                var cmd = new System.Data.SqlClient.SqlCommand(
                    "SELECT p.FirstName, p.LastName, p.DateOfBirth, p.Gender, p.ContactNumber, p.Email, p.Address, r.RoomNumber " +
                    "FROM tblPatient p LEFT JOIN tblRoom r ON p.PatientID = r.PatientID " +
                    "WHERE p.UserID = @UserID", con);
                cmd.Parameters.AddWithValue("@UserID", userID);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        TableLayoutPanel detailsTable = new TableLayoutPanel
                        {
                            Dock = DockStyle.Fill,
                            ColumnCount = 2,
                            RowCount = 7, // Number of detail rows
                            ColumnStyles = { new ColumnStyle(SizeType.Percent, 40F), new ColumnStyle(SizeType.Percent, 60F) },
                            Padding = new Padding(5)
                        };

                        // Helper to add a row to the details table
                        Action<string, string, int> addDetailRow = (label, value, row) =>
                        {
                            Label lblKey = new Label { Text = label, Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Anchor = AnchorStyles.Left };
                            Label lblValue = new Label { Text = value, Font = new Font("Segoe UI", 10), AutoSize = true, Anchor = AnchorStyles.Left };
                            UIHelper.StyleLabel(lblKey); // Apply UIHelper style
                            UIHelper.StyleLabel(lblValue); // Apply UIHelper style
                            detailsTable.Controls.Add(lblKey, 0, row);
                            detailsTable.Controls.Add(lblValue, 1, row);
                        };

                        addDetailRow("First Name:", reader["FirstName"].ToString(), 0);
                        addDetailRow("Last Name:", reader["LastName"].ToString(), 1);
                        addDetailRow("Date of Birth:", ((DateTime)reader["DateOfBirth"]).ToShortDateString(), 2);
                        addDetailRow("Gender:", reader["Gender"].ToString(), 3);
                        addDetailRow("Contact:", reader["ContactNumber"].ToString(), 4);
                        addDetailRow("Email:", reader["Email"].ToString(), 5);
                        addDetailRow("Address:", reader["Address"].ToString(), 6);
                        // Add more fields as needed

                        detailsContentPanel.Controls.Add(detailsTable);
                    }
                    else
                    {
                        Label lblNoInfo = new Label
                        {
                            Text = "No patient information found.",
                            Font = new System.Drawing.Font("Segoe UI", 12),
                            Dock = DockStyle.Fill,
                            TextAlign = ContentAlignment.MiddleCenter
                        };
                        detailsContentPanel.Controls.Add(lblNoInfo);
                    }
                }
            }

            // ------------------ Patient Appointments Panel ------------------
            Panel patientAppointmentsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.FromArgb(240, 240, 240), // Light gray background
                Padding = new Padding(15),
                Margin = new Padding(5)
            };
            patientAppointmentsPanel.BorderStyle = BorderStyle.FixedSingle;

            Label lblAppointmentsTitle = new Label
            {
                Text = "Your Appointments",
                Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(24, 33, 54),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 40
            };
            patientAppointmentsPanel.Controls.Add(lblAppointmentsTitle);

            // Placeholder for appointments list (e.g., a DataGridView or custom control)
            AppointmentControl patientAppointmentControl = new AppointmentControl();
            patientAppointmentControl.LoadPatientAppointments(userID); // Load appointments for the specific patient
            patientAppointmentControl.Dock = DockStyle.Fill;
            patientAppointmentsPanel.Controls.Add(patientAppointmentControl);
            patientAppointmentControl.BringToFront();

            // Add panels to dashboard layout
            dashboardLayout.Controls.Add(patientDetailsPanel, 0, 0);
            dashboardLayout.Controls.Add(patientAppointmentsPanel, 0, 1);

            contentPanel.Controls.Add(dashboardLayout);
            dashboardLayout.BringToFront(); // Ensure dashboard is visible
        }

        public void Logout()
        {
            IsLogout = true;
            this.Close();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton == null) return;

            // Reset background of all nav buttons
            foreach (Control control in navPanel.Controls)
            {
                if (control is Button navButton)
                {
                    navButton.BackColor = Color.White; // Default nav color (white)
                    navButton.ForeColor = Color.Black; // Default font color (black)
                }
            }

            // Set background of clicked button
            clickedButton.BackColor = Color.Gainsboro; // Light gray highlight
            clickedButton.ForeColor = Color.Black;

            // Show corresponding control
            switch (clickedButton.Text)
            {
                case "Patients": ShowControl(new PatientControl()); break;
                case "Doctors": ShowControl(new DoctorControl()); break;
                case "Appointments": ShowControl(new AppointmentControl()); break;
                case "Rooms": ShowControl(new RoomControl()); break;
                case "Invoices": ShowControl(new InvoiceControl()); break;
                case "Diseases": ShowControl(new DiseaseControl()); break;
                case "Suppliers": ShowControl(new SupplierControl()); break;
                case "Medicines": ShowControl(new MedicineControl()); break;
                case "Billing": ShowControl(new BillingControl()); break;
            }
        }

        private void ShowControl(UserControl control)
        {
            contentPanel.Controls.Clear();
            control.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(control);
            control.BringToFront();
        }
    }
} 