using System;
using System.Windows.Forms;
using HMS;
using System.Drawing;
using Microsoft.Data.SqlClient;
using HMS.UI;

namespace HMS
{
    public partial class MainForm : Form
    {
        private Panel contentPanel;
        private Panel navPanel;
        private string userRole;
        private int userID;

        public bool IsLogout { get; private set; } = false;

        public MainForm(string userRole, int userID)
        {
            this.userRole = userRole;
            this.userID = userID;
            
            InitializeComponent();
            
            if (userRole == "Patient")
            {
                if (navPanel != null) navPanel.Visible = false;
                if (this.Controls.Count > 0 && this.Controls[0] is TableLayoutPanel mainLayout)
                {
                    mainLayout.ColumnStyles[0] = new ColumnStyle(SizeType.Absolute, 0);
                    mainLayout.SetColumnSpan(contentPanel, 2);
                }
                ShowPatientView();
            }
            
            try
            {
                UIHelper.ApplyModernTheme(this);
                
                if (navPanel != null && navPanel.Visible)
                {
                    foreach (Control control in navPanel.Controls)
                    {
                        if (control is Button navButton && control != navBorder)
                            UIHelper.StyleNavButton(navButton, false);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Theme application error: " + ex.Message);
            }
            
            AdjustNavigationVisibility(userRole);
            this.SizeChanged += (s, e) => AdjustNavigationVisibility(userRole);
        }

        private void ShowPatientDetails()
        {
            contentPanel.Controls.Clear();
            Panel patientInfoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.Color.FromArgb(240, 240, 240)
            };
            
            using (var con = DatabaseHelper.GetConnection())
            {
                con.Open();
                var cmd = new Microsoft.Data.SqlClient.SqlCommand(
                    "SELECT p.*, r.RoomNumber FROM tblPatient p " +
                    "LEFT JOIN tblRoom r ON p.PatientID = r.PatientID " +
                    "WHERE p.UserID = @UserID", con);
                cmd.Parameters.AddWithValue("@UserID", userID);

                using (var reader = cmd.ExecuteReader())
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
            contentPanel.Controls.Clear();
            var appointmentControl = new AppointmentControl();
            appointmentControl.LoadPatientAppointments(userID);
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
                Padding = new Padding(10, 0, 10, 10)
            };
            patientDetailsPanel.Controls.Add(detailsContentPanel);
            detailsContentPanel.BringToFront();

            using (var con = DatabaseHelper.GetConnection())
            {
                con.Open();
                var cmd = new Microsoft.Data.SqlClient.SqlCommand(
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
                            RowCount = 7,
                            ColumnStyles = { new ColumnStyle(SizeType.Percent, 40F), new ColumnStyle(SizeType.Percent, 60F) },
                            Padding = new Padding(5)
                        };

                        Action<string, string, int> addDetailRow = (label, value, row) =>
                        {
                            Label lblKey = new Label { Text = label, Font = new Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true, Anchor = AnchorStyles.Left };
                            Label lblValue = new Label { Text = value, Font = new Font("Segoe UI", 10), AutoSize = true, Anchor = AnchorStyles.Left };
                            UIHelper.StyleLabel(lblKey);
                            UIHelper.StyleLabel(lblValue);
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

            AppointmentControl patientAppointmentControl = new AppointmentControl();
            patientAppointmentControl.LoadPatientAppointments(userID);
            patientAppointmentControl.Dock = DockStyle.Fill;
            patientAppointmentsPanel.Controls.Add(patientAppointmentControl);
            patientAppointmentControl.BringToFront();

            dashboardLayout.Controls.Add(patientDetailsPanel, 0, 0);
            dashboardLayout.Controls.Add(patientAppointmentsPanel, 0, 1);

            contentPanel.Controls.Add(dashboardLayout);
            dashboardLayout.BringToFront();
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
                if (control is Button navButton && navButton != clickedButton)
                {
                    UIHelper.StyleNavButton(navButton, false); // Reset to inactive state
                }
            }

            // Set active state for clicked button
            UIHelper.StyleNavButton(clickedButton, true); // Set to active state

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

        private async void ShowControl(UserControl control)
        {
            try
            {
                // Smooth transition: slide in from right
                Control oldControl = null;
                if (contentPanel.Controls.Count > 0)
                {
                    oldControl = contentPanel.Controls[0];
                }

                // Add new control (hidden initially, positioned off-screen to the right)
                control.Dock = DockStyle.None; // Temporarily disable dock for animation
                control.Visible = false;
                control.Location = new Point(contentPanel.Width, 0); // Start off-screen to the right
                control.Size = contentPanel.Size;
                contentPanel.Controls.Add(control);
                control.BringToFront();

                // Slide in animation
                int targetX = 0;
                int startX = contentPanel.Width;
                int steps = 15;
                int stepSize = (startX - targetX) / steps;
                int delay = 10; // milliseconds per step

                for (int i = 0; i <= steps; i++)
                {
                    control.Left = startX - (stepSize * i);
                    control.Visible = true;
                    await Task.Delay(delay);
                    Application.DoEvents();
                }

                control.Left = targetX;
                control.Dock = DockStyle.Fill; // Re-enable dock after animation

                // Remove old control after transition
                if (oldControl != null)
                {
                    contentPanel.Controls.Remove(oldControl);
                    oldControl.Dispose();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error showing control: " + ex.Message);
                // Fallback to simple show
                contentPanel.Controls.Clear();
                control.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(control);
                control.BringToFront();
            }
        }

        private void AdjustNavigationVisibility(string role)
        {
            if (navPanel == null) return;

            // Always show Patient for patient role, hide others
            if (role == "Patient")
            {
                foreach (Control control in navPanel.Controls)
                {
                    if (control is Button navButton)
                    {
                        navButton.Visible = (navButton.Text == "Patients");
                    }
                }
                // You might want to automatically show the patient view here if not already shown
                if (!(contentPanel.Controls.Count > 0 && contentPanel.Controls[0] is PatientControl))
                {
                    ShowPatientView();
                }
            }
            else // Admin/Doctor/etc. roles - show all relevant navigation
            {
                foreach (Control control in navPanel.Controls)
                {
                    if (control is Button navButton)
                    {
                        navButton.Visible = true; // Show all navigation buttons
                    }
                }
                // Set a default view for non-patient roles if desired
                if (!(contentPanel.Controls.Count > 0 && contentPanel.Controls[0] is PatientControl || contentPanel.Controls[0] is DoctorControl || contentPanel.Controls[0] is AppointmentControl))
                {
                    ShowControl(new PatientControl()); // Default to patient management for admin
                }
            }
        }
    }
} 