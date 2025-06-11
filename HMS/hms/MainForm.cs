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

                // Create navigation panel
                Panel navPanel = new Panel
                {
                    Dock = DockStyle.Left,
                    Width = 200,
                    BackColor = System.Drawing.Color.FromArgb(24, 33, 54)
                };

                // Create content panel
                contentPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = System.Drawing.Color.White
                };

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

                StyleButton(btnPatients, "Patients");
                StyleButton(btnAppointments, "Appointments");
                StyleButton(btnRooms, "Rooms");
                StyleButton(btnDoctors, "Doctors");
                StyleButton(btnInvoices, "Invoices");
                StyleButton(btnDiseases, "Diseases");
                StyleButton(btnSuppliers, "Suppliers");
                StyleButton(btnMedicines, "Medicines");
                StyleButton(btnBilling, "Billing");

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

                // Add panels to form
                this.Controls.Add(contentPanel);
                this.Controls.Add(navPanel);

                // Show default control based on user role
                if (userRole == "Patient")
                {
                    ShowPatientView();
                }
                else
                {
                    ShowControl(new PatientControl());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing main form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowPatientView()
        {
            // For patient users, show only their information and appointments
            contentPanel.Controls.Clear();
            
            // Create a panel for patient info
            Panel patientInfoPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = System.Drawing.Color.FromArgb(240, 240, 240)
            };

            // Create a panel for appointments
            Panel appointmentsPanel = new Panel
            {
                Dock = DockStyle.Fill
            };

            // Load patient information
            using (var con = new System.Data.SqlClient.SqlConnection(@"Data Source=.\SQLEXPRESS;Initial Catalog=HMS;Integrated Security=True"))
            {
                con.Open();
                var cmd = new System.Data.SqlClient.SqlCommand(
                    "SELECT p.*, r.RoomNumber FROM tblPatient p " +
                    "LEFT JOIN tblRoom r ON p.PatientID = r.PatientID " +
                    "WHERE p.PatientID = @PatientID", con);
                cmd.Parameters.AddWithValue("@PatientID", userID);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Label lblInfo = new Label
                        {
                            Text = $"Welcome, {reader["FirstName"]} {reader["LastName"]}\n" +
                                  $"Room: {(reader["RoomNumber"] != DBNull.Value ? reader["RoomNumber"] : "Not Assigned")}",
                            Font = new System.Drawing.Font("Segoe UI", 12),
                            Dock = DockStyle.Fill,
                            TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                            Padding = new Padding(20, 0, 0, 0)
                        };
                        patientInfoPanel.Controls.Add(lblInfo);
                    }
                }
            }

            // Load appointments
            var appointmentControl = new AppointmentControl();
            appointmentControl.LoadPatientAppointments(userID);
            appointmentsPanel.Controls.Add(appointmentControl);

            contentPanel.Controls.Add(appointmentsPanel);
            contentPanel.Controls.Add(patientInfoPanel);
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