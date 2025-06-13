using System;
using System.Windows.Forms;
using HMS;

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
            ShowSplashScreen();
        }

        private void ShowSplashScreen()
        {
            using (var splash = new SplashForm())
            {
                splash.ShowDialog();
            }
        }

        private void InitializeComponent()
        {
            try
            {
                this.Text = "Hospital Management System";
                this.Size = new System.Drawing.Size(1200, 700);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.MinimumSize = new System.Drawing.Size(1000, 600);

                // Create content panel (always exists)
                contentPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = System.Drawing.Color.White
                };
                this.Controls.Add(contentPanel);

                // Show default control based on user role
                if (userRole == "Patient")
                {
                    ShowPatientView(); // Only show patient-specific view
                }
                else // Non-patient roles (e.g., Admin)
                {
                    // Create navigation panel (only for non-patient roles)
                    this.navPanel = new Panel
                    {
                        Dock = DockStyle.Left,
                        Width = 200,
                        BackColor = System.Drawing.Color.FromArgb(24, 33, 54)
                    };
                    this.Controls.Add(navPanel);

                    // Create buttons with consistent styling
                    void StyleButton(Button btn, string text)
                    {
                        btn.Text = text;
                        btn.Dock = DockStyle.Top;
                        btn.Height = 50;
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.FlatAppearance.BorderSize = 0;
                        btn.ForeColor = System.Drawing.Color.White;
                        btn.Font = new System.Drawing.Font("Segoe UI", 10);
                        btn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                        btn.Padding = new Padding(20, 0, 0, 0);
                        btn.BackColor = System.Drawing.Color.FromArgb(24, 33, 54);
                        btn.Click += new EventHandler(Button_Click);
                    }

                    // Create and style buttons
                    btnPatients = new Button();
                    btnAppointments = new Button();
                    btnRooms = new Button();
                    btnDoctors = new Button();
                    btnInvoices = new Button();
                    btnDiseases = new Button();
                    btnSuppliers = new Button();
                    btnMedicines = new Button();
                    btnBilling = new Button();

                    // Add buttons to nav panel in desired order
                    navPanel.Controls.AddRange(new Control[] {
                        btnPatients,
                        btnAppointments,
                        btnRooms,
                        btnDoctors,
                        btnInvoices,
                        btnDiseases,
                        btnSuppliers,
                        btnMedicines,
                        btnBilling
                    });

                    // Apply styling and handlers for all navigation buttons
                    StyleButton(btnPatients, "Patients");
                    StyleButton(btnAppointments, "Appointments");
                    StyleButton(btnRooms, "Rooms");
                    StyleButton(btnDoctors, "Doctors");
                    StyleButton(btnInvoices, "Invoices");
                    StyleButton(btnDiseases, "Diseases");
                    StyleButton(btnSuppliers, "Suppliers");
                    StyleButton(btnMedicines, "Medicines");
                    StyleButton(btnBilling, "Billing");

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
                        Label lblPatientDetails = new Label
                        {
                            Text = $"Name: {reader["FirstName"]} {reader["LastName"]}\n" +
                                   $"DOB: {((DateTime)reader["DateOfBirth"]).ToShortDateString()}\n" +
                                   $"Gender: {reader["Gender"]}\n" +
                                   $"Contact: {reader["ContactNumber"]}\n" +
                                   $"Email: {reader["Email"]}\n" +
                                   $"Address: {reader["Address"]}\n" +
                                   $"Room: {(reader["RoomNumber"] != DBNull.Value ? reader["RoomNumber"] : "Not Assigned")}",
                            Font = new System.Drawing.Font("Segoe UI", 11),
                            Dock = DockStyle.Fill,
                            TextAlign = System.Drawing.ContentAlignment.TopLeft,
                            Padding = new Padding(10)
                        };
                        detailsContentPanel.Controls.Add(lblPatientDetails);
                    }
                    else
                    {
                        Label lblNoPatientDetails = new Label
                        {
                            Text = "No patient details found for this user.",
                            Font = new System.Drawing.Font("Segoe UI", 11),
                            Dock = DockStyle.Fill,
                            TextAlign = System.Drawing.ContentAlignment.MiddleCenter
                        };
                        detailsContentPanel.Controls.Add(lblNoPatientDetails);
                    }
                }
            }

            // ------------------ Appointments Panel ------------------
            Panel appointmentsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.FromArgb(230, 255, 245), // Light green background
                Padding = new Padding(15),
                Margin = new Padding(5)
            };
            appointmentsPanel.BorderStyle = BorderStyle.FixedSingle;

            Label lblAppointmentsTitle = new Label
            {
                Text = "Your Appointments",
                Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(24, 33, 54),
                Dock = DockStyle.Top,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                Height = 40
            };
            appointmentsPanel.Controls.Add(lblAppointmentsTitle);

            // Load patient appointments into appointmentsPanel
            var appointmentControl = new AppointmentControl();
            appointmentControl.LoadPatientAppointments(userID); 
            appointmentControl.Dock = DockStyle.Fill;

            Panel appointmentContentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 0, 10, 10) // Adjust padding
            };
            appointmentContentPanel.Controls.Add(appointmentControl);
            appointmentsPanel.Controls.Add(appointmentContentPanel);
            appointmentContentPanel.BringToFront(); // Ensure content is on top of title

            // Add panels to the dashboard layout
            dashboardLayout.Controls.Add(patientDetailsPanel, 0, 0);
            dashboardLayout.Controls.Add(appointmentsPanel, 0, 1);

            contentPanel.Controls.Add(dashboardLayout);
        }

        private void Button_Click(object sender, EventArgs e)
        {
            if (userRole == "Patient")
            {
                MessageBox.Show("Access denied. This feature is only available to administrators.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Button clickedButton = (Button)sender;
            UserControl controlToShow = null;

            switch (clickedButton.Text)
            {
                case "Patients":
                    controlToShow = new PatientControl();
                    break;
                case "Appointments":
                    controlToShow = new AppointmentControl();
                    break;
                case "Rooms":
                    controlToShow = new RoomControl();
                    break;
                case "Doctors":
                    controlToShow = new DoctorControl();
                    break;
                case "Invoices":
                    controlToShow = new InvoiceControl();
                    break;
                case "Diseases":
                    controlToShow = new DiseaseControl();
                    break;
                case "Suppliers":
                    controlToShow = new SupplierControl();
                    break;
                case "Medicines":
                    controlToShow = new MedicineControl();
                    break;
                case "Billing":
                    controlToShow = new BillingControl();
                    break;
            }

            if (controlToShow != null)
            {
                ShowControl(controlToShow);
            }
        }

        private void ShowControl(UserControl control)
        {
            contentPanel.Controls.Clear();
            control.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(control);
        }
    }
} 