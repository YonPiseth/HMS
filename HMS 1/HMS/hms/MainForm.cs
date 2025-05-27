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

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            try
            {
                this.Text = "Hospital Management System";
                this.Size = new System.Drawing.Size(1000, 600);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.MinimumSize = new System.Drawing.Size(1000, 600);

                // Create navigation panel
                Panel navPanel = new Panel
                {
                    Dock = DockStyle.Left,
                    Width = 200,
                    BackColor = System.Drawing.Color.FromArgb(24, 33, 54)
                };

                // Create navigation buttons
                btnDoctors = CreateNavButton("Doctors", btnDoctors_Click);
                btnPatients = CreateNavButton("Patients", btnPatients_Click);
                btnAppointments = CreateNavButton("Appointments", btnAppointments_Click);
                btnRooms = CreateNavButton("Rooms", btnRooms_Click);
                btnInvoices = CreateNavButton("Invoices", btnInvoices_Click);
                btnDiseases = CreateNavButton("Diseases", btnDiseases_Click);
                btnSuppliers = CreateNavButton("Suppliers", btnSuppliers_Click);
                btnMedicines = CreateNavButton("Medicines", btnMedicines_Click);
                btnBilling = CreateNavButton("Billing", btnBilling_Click);

                // Create content panel
                contentPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = System.Drawing.Color.White
                };

                // Add controls to navigation panel
                navPanel.Controls.Add(btnBilling);
                navPanel.Controls.Add(btnMedicines);
                navPanel.Controls.Add(btnSuppliers);
                navPanel.Controls.Add(btnDiseases);
                navPanel.Controls.Add(btnInvoices);
                navPanel.Controls.Add(btnRooms);
                navPanel.Controls.Add(btnAppointments);
                navPanel.Controls.Add(btnPatients);
                navPanel.Controls.Add(btnDoctors);

                // Add panels to form
                this.Controls.Add(contentPanel);
                this.Controls.Add(navPanel);

                // Initialize controls
                doctorControl = new DoctorControl();
                patientControl = new PatientControl();
                appointmentControl = new AppointmentControl();
                roomControl = new RoomControl();
                invoiceControl = new InvoiceControl();
                diseaseControl = new DiseaseControl();
                supplierControl = new SupplierControl();
                medicineControl = new MedicineControl();
                billingControl = new BillingControl();

                // Show doctors by default
                ShowControl(doctorControl, btnDoctors);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing MainForm: {ex.Message}\n{ex.StackTrace}", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Button CreateNavButton(string text, EventHandler clickHandler)
        {
            Button button = new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                BackColor = System.Drawing.Color.FromArgb(24, 33, 54),
                ForeColor = System.Drawing.Color.White,
                Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += clickHandler;
            return button;
        }

        private void ResetButtonColors()
        {
            btnDoctors.BackColor = System.Drawing.Color.FromArgb(24, 33, 54);
            btnPatients.BackColor = System.Drawing.Color.FromArgb(24, 33, 54);
            btnAppointments.BackColor = System.Drawing.Color.FromArgb(24, 33, 54);
            btnRooms.BackColor = System.Drawing.Color.FromArgb(24, 33, 54);
            btnInvoices.BackColor = System.Drawing.Color.FromArgb(24, 33, 54);
            btnDiseases.BackColor = System.Drawing.Color.FromArgb(24, 33, 54);
            btnSuppliers.BackColor = System.Drawing.Color.FromArgb(24, 33, 54);
            btnMedicines.BackColor = System.Drawing.Color.FromArgb(24, 33, 54);
            btnBilling.BackColor = System.Drawing.Color.FromArgb(24, 33, 54);
        }

        private void ShowControl(UserControl control, Button activeButton)
        {
            contentPanel.Controls.Clear();
            control.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(control);
            ResetButtonColors();
            activeButton.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
        }

        private void btnDoctors_Click(object sender, EventArgs e) => ShowControl(doctorControl, btnDoctors);
        private void btnPatients_Click(object sender, EventArgs e) => ShowControl(patientControl, btnPatients);
        private void btnAppointments_Click(object sender, EventArgs e) => ShowControl(appointmentControl, btnAppointments);
        private void btnRooms_Click(object sender, EventArgs e) => ShowControl(roomControl, btnRooms);
        private void btnInvoices_Click(object sender, EventArgs e) => ShowControl(invoiceControl, btnInvoices);
        private void btnDiseases_Click(object sender, EventArgs e) => ShowControl(diseaseControl, btnDiseases);
        private void btnSuppliers_Click(object sender, EventArgs e) => ShowControl(supplierControl, btnSuppliers);
        private void btnMedicines_Click(object sender, EventArgs e) => ShowControl(medicineControl, btnMedicines);
        private void btnBilling_Click(object sender, EventArgs e) => ShowControl(billingControl, btnBilling);
    }
} 