using System;
using System.Windows.Forms;
using System.Drawing;

namespace HMS
{
    public partial class MainForm : Form
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
                    BackColor = Color.White,
                    Padding = new Padding(15) // Add padding to the content area
                };
                UIHelper.StyleModernPanel(contentPanel);

                // Show hospital logo by default after splash form
                PictureBox logoBox = new PictureBox();
                logoBox.Image = Image.FromFile("hms/Image/Loading Image.jpg");
                logoBox.SizeMode = PictureBoxSizeMode.Zoom;
                logoBox.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(logoBox);
                // ShowControl(new PatientControl()); // Default view for admin (now only on menu click)

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
                        BackColor = Color.White,
                        Padding = new Padding(0, 20, 0, 0)
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
                        // Restore simple button style
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.BackColor = Color.White;
                        btn.ForeColor = Color.Black;
                        btn.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                        btn.Height = 50;
                        btn.Width = 120;
                        btn.Margin = new Padding(0, 0, 0, 5);
                        btn.FlatAppearance.BorderSize = 1;
                        btn.FlatAppearance.BorderColor = Color.LightGray;
                        btn.TextAlign = ContentAlignment.MiddleLeft;
                        btn.ImageAlign = ContentAlignment.MiddleLeft;
                        btn.TextImageRelation = TextImageRelation.ImageBeforeText;
                        btn.Padding = new Padding(48, 0, 0, 0);
                        btn.Dock = DockStyle.Top;
                        btn.Click += new EventHandler(Button_Click);

                        // Fix image path to be relative to the executable output directory
                        string imagePath = null;
                        switch (name)
                        {
                            case "Patients": imagePath = "hms/Image/patient.png"; break;
                            case "Doctors": imagePath = "hms/Image/doctor.png"; break;
                            case "Appointments": imagePath = "hms/Image/appointment.png"; break;
                            case "Rooms": imagePath = "hms/Image/Room.png"; break;
                            case "Invoices": imagePath = "hms/Image/invoice.png"; break;
                            case "Diseases": imagePath = "hms/Image/diseases.png"; break;
                            case "Suppliers": imagePath = "hms/Image/supplier.png"; break;
                            case "Medicines": imagePath = "hms/Image/medicine.png"; break;
                            case "Billing": imagePath = "hms/Image/bill.png"; break;
                        }
                        if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                        {
                            btn.Image = new Bitmap(Image.FromFile(imagePath), new Size(24, 24));
                        }

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

                    // ShowControl(new PatientControl()); // Default view for admin (now only on menu click)
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing main form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
} 