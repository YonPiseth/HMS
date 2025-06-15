using System;
using System.Windows.Forms;
using HMS;
using System.Drawing;

namespace HMS
{
    public class MainForm : Form
    {
        private DoctorControl doctorControl;
        private PatientControl patientControl;
        private AppointmentControl appointmentControl;
        private RoomControl roomControl;
        private InvoiceControl invoiceControl;
        private DiseaseControl diseaseControl;
        private SupplierControl supplierControl;
        private MedicineControl medicineControl;
        private BillingControl billingControl;

        private Button btnDoctors;
        private Button btnPatients;
        private Button btnAppointments;
        private Button btnRooms;
        private Button btnInvoices;
        private Button btnDiseases;
        private Button btnSuppliers;
        private Button btnMedicines;
        private Button btnBilling;
        private Panel contentPanel;
        private Panel navPanel;
        private string userRole;
        private int userID;

        public MainForm(string role, int id)
        {
            userRole = role;
            userID = id;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            try
            {
                this.Text = "Hospital Management System";
                this.Size = new Size(1200, 750); // Adjusted size for more content
                this.StartPosition = FormStartPosition.CenterScreen;
                this.MinimumSize = new Size(1000, 600);
                this.BackColor = Color.White;

                // Main layout panel for the entire form
                TableLayoutPanel mainLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 1,
                    ColumnStyles = { new ColumnStyle(SizeType.Absolute, 220), new ColumnStyle(SizeType.Percent, 100F) },
                    RowStyles = { new RowStyle(SizeType.Percent, 100F) },
                    Padding = new Padding(0)
                };
                this.Controls.Add(mainLayout);

                // Content panel (always exists)
                contentPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = System.Drawing.Color.White,
                    Padding = new Padding(15) // Add padding to the content area
                };

                // Show default control based on user role
                if (userRole == "Patient")
                {
                    // Patient view is full screen, no nav panel
                    mainLayout.ColumnStyles[0] = new ColumnStyle(SizeType.Absolute, 0); // Collapse nav column
                    mainLayout.Controls.Add(contentPanel, 0, 0);
                    mainLayout.SetColumnSpan(contentPanel, 2); // Span both columns
                    ShowPatientView();
                }
                else // Non-patient roles (e.g., Admin)
                {
                    // Navigation panel
                    this.navPanel = new Panel
                    {
                        Dock = DockStyle.Fill,
                        BackColor = Color.FromArgb(24, 33, 54), // Dark blue/grey background for nav
                        Padding = new Padding(0, 20, 0, 0) // Padding at the top for spacing
                    };
                    mainLayout.Controls.Add(this.navPanel, 0, 0); // Add nav panel to the first column
                    mainLayout.Controls.Add(contentPanel, 1, 0); // Add content panel to the second column

                    // Create navigation buttons dynamically and apply styles
                    string[] buttonNames = {
                        "Patients", "Doctors", "Appointments", "Rooms",
                        "Invoices", "Diseases", "Suppliers", "Medicines", "Billing"
                    };

                    foreach (string name in buttonNames)
                    {
                        Button btn = new Button();
                        btn.Text = name;
                        UIHelper.StyleButton(btn); // Apply standard button style
                        btn.Dock = DockStyle.Top;
                        btn.TextAlign = ContentAlignment.MiddleLeft; // Align text to left
                        btn.Padding = new Padding(20, 0, 0, 0); // Indent text
                        btn.Height = 45; // Taller buttons
                        btn.FlatAppearance.BorderSize = 0; // No border
                        btn.BackColor = Color.FromArgb(24, 33, 54); // Override button color for nav panel
                        btn.ForeColor = Color.White;
                        btn.Font = new Font("Segoe UI", 11, FontStyle.Bold); // Slightly larger, bold font
                        btn.Margin = new Padding(0, 0, 0, 5); // Add margin between buttons
                        btn.Click += new EventHandler(Button_Click);

                        // Assign to specific button variables for easier access if needed elsewhere
                        switch (name)
                        {
                            case "Patients": btnPatients = btn; break;
                            case "Doctors": btnDoctors = btn; break;
                            case "Appointments": btnAppointments = btn; break;
                            case "Rooms": btnRooms = btn; break;
                            case "Invoices": btnInvoices = btn; break;
                            case "Diseases": btnDiseases = btn; break;
                            case "Suppliers": btnSuppliers = btn; break;
                            case "Medicines": btnMedicines = btn; break;
                            case "Billing": btnBilling = btn; break;
                        }

                        navPanel.Controls.Add(btn);
                    }
                    // Reverse the order of controls in the navPanel to make them appear top-to-bottom as added
                    // (Buttons are added bottom-to-top with DockStyle.Top if not reversed)
                    navPanel.Controls.SetChildIndex(btnBilling, 0);
                    navPanel.Controls.SetChildIndex(btnMedicines, 1);
                    navPanel.Controls.SetChildIndex(btnSuppliers, 2);
                    navPanel.Controls.SetChildIndex(btnDiseases, 3);
                    navPanel.Controls.SetChildIndex(btnInvoices, 4);
                    navPanel.Controls.SetChildIndex(btnRooms, 5);
                    navPanel.Controls.SetChildIndex(btnAppointments, 6);
                    navPanel.Controls.SetChildIndex(btnDoctors, 7);
                    navPanel.Controls.SetChildIndex(btnPatients, 8);

                    ShowControl(new PatientControl()); // Default view for admin
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing main form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Dispose();
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
                    navButton.BackColor = Color.FromArgb(24, 33, 54); // Default nav color
                }
            }

            // Set background of clicked button
            clickedButton.BackColor = Color.FromArgb(0, 120, 215); // Highlight selected button

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