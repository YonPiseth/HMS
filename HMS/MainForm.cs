using System;
using System.Windows.Forms;
using HMS;
using System.Drawing;
using Microsoft.Data.SqlClient;
using HMS.UI;
using Microsoft.Extensions.DependencyInjection;

namespace HMS
{
    public partial class MainForm : Form
    {
        private Panel contentPanel;
        private Panel navPanel;
        private string userRole;
        private int userID;
        private bool _isAnimating = false;
        public bool IsLogout { get; private set; } = false;
        private readonly IServiceProvider _serviceProvider;

        public MainForm(string userRole, int userID, IServiceProvider serviceProvider)
        {
            this.userRole = userRole;
            this.userID = userID;
            this._serviceProvider = serviceProvider;
            
            InitializeComponent();
            this.DoubleBuffered = true;

            // Set Premium Backgrounds
            this.BackColor = UITheme.BackgroundWhite;
            if (navPanel != null) 
            {
                navPanel.BackColor = UITheme.SidebarBackground;
                UIHelper.SetDoubleBuffered(navPanel);
            }
            if (contentPanel != null) 
            {
                contentPanel.BackColor = UITheme.BackgroundLight;
                UIHelper.SetDoubleBuffered(contentPanel);
            }

            // Add Logo/Title to Sidebar
            Panel logoPanel = new Panel { 
                Dock = DockStyle.Top, 
                Height = 100, 
                Padding = new Padding(20, 30, 20, 0),
                BackColor = Color.Transparent
            };
            Label lblLogo = new Label { 
                Text = "HMS PRO", 
                ForeColor = UITheme.BrandPrimary, 
                Font = new Font("Segoe UI", 20, FontStyle.Bold), 
                AutoSize = true, 
                Location = new Point(20, 30) 
            };
            logoPanel.Controls.Add(lblLogo);
            if (navPanel != null)
            {
                navPanel.Controls.Add(logoPanel);
                logoPanel.BringToFront();
                
                // Style all buttons in navPanel
                foreach (Control control in navPanel.Controls)
                {
                    if (control is Button navButton)
                    {
                        UIHelper.StyleNavButton(navButton, navButton.Name == "btnPatients");
                    }
                }
            }
            
            if (userRole == "Patient")
            {
                if (navPanel != null) navPanel.Visible = false;
                ShowPatientView();
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
                BackColor = UITheme.BackgroundGray,
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
                BackColor = UITheme.BackgroundLight,
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

            // Show corresponding control resolved from DI
            switch (clickedButton.Text)
            {
                case "Patients": ShowControl(_serviceProvider.GetRequiredService<PatientControl>()); break;
                case "Doctors": ShowControl(_serviceProvider.GetRequiredService<DoctorControl>()); break;
                case "Appointments": ShowControl(_serviceProvider.GetRequiredService<AppointmentControl>()); break;
                case "Rooms": ShowControl(_serviceProvider.GetRequiredService<RoomControl>()); break;
                case "Invoices": ShowControl(_serviceProvider.GetRequiredService<InvoiceControl>()); break;
                case "Diseases": ShowControl(_serviceProvider.GetRequiredService<DiseaseControl>()); break;
                case "Suppliers": ShowControl(_serviceProvider.GetRequiredService<SupplierControl>()); break;
                case "Medicines": ShowControl(_serviceProvider.GetRequiredService<MedicineControl>()); break;
                case "Billing": ShowControl(_serviceProvider.GetRequiredService<BillingControl>()); break;
            }
        }

        private async void ShowControl(UserControl control)
        {
            if (control == null) return;

            // Prevent re-entrant calls during animation
            if (_isAnimating) return;
            _isAnimating = true;

            try
            {
                // Identify the current control
                Control oldControl = contentPanel.Controls.Count > 0 ? contentPanel.Controls[0] : null;

                if (oldControl == control) return;

                // -- KEY FIX: Hide old control immediately so it doesn't show through --
                if (oldControl != null)
                    oldControl.Visible = false;

                // Prepare new control off-screen to the right
                control.Dock = DockStyle.None;
                control.Size = contentPanel.ClientSize;
                control.BackColor = Color.White;
                UIHelper.SetDoubleBuffered(control);
                control.Location = new Point(contentPanel.ClientSize.Width, 0);
                control.Visible = true;

                contentPanel.SuspendLayout();
                contentPanel.Controls.Add(control);
                control.BringToFront();
                contentPanel.ResumeLayout(false);

                // Animate: slide in with Cubic-Out easing (250ms)
                int duration = 250;
                var sw = System.Diagnostics.Stopwatch.StartNew();

                while (true)
                {
                    long elapsed = sw.ElapsedMilliseconds;
                    if (elapsed >= duration) break;

                    double t = (double)elapsed / duration;
                    double easedT = 1.0 - Math.Pow(1.0 - t, 3.0); // Cubic Out

                    control.Left = (int)(contentPanel.ClientSize.Width * (1.0 - easedT));
                    control.Update();

                    await Task.Delay(8);
                }

                // Snap to final position and re-dock
                control.Left = 0;
                control.Dock = DockStyle.Fill;

                // Remove and dispose old control
                if (oldControl != null)
                {
                    contentPanel.Controls.Remove(oldControl);
                    if (!oldControl.IsDisposed) oldControl.Dispose();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Transition error: " + ex.Message);
                contentPanel.Controls.Clear();
                control.Dock = DockStyle.Fill;
                contentPanel.Controls.Add(control);
            }
            finally
            {
                _isAnimating = false;
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
                        navButton.Visible = (navButton.Text == "Patients" || navButton.Text == "Logout");
                    }
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
            }
        }
    }
} 