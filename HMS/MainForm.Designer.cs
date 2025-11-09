using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using HMS.UI;

namespace HMS
{
    public partial class MainForm : Form
    {
        private Button btnDoctors;
        private Button btnPatients;
        private Button btnAppointments;
        private Button btnRooms;
        private Button btnInvoices;
        private Button btnDiseases;
        private Button btnSuppliers;
        private Button btnMedicines;
        private Button btnBilling;
        private Panel navBorder;

        private void InitializeComponent()
        {
            try
            {
                this.Text = "Hospital Management System";
                this.Size = new Size(1200, 750);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.MinimumSize = new Size(1000, 600);
                this.BackColor = Color.White;

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

                this.contentPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(250, 250, 250),
                    Padding = new Padding(15)
                };

                PictureBox logoBox = new PictureBox();
                try
                {
                    string imageDir = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Image");
                    string imagePath = Path.Combine(imageDir, "Loading Image.png");
                    
                    if (!System.IO.File.Exists(imagePath))
                        imagePath = Path.Combine(imageDir, "Loading Image.jpg");
                    
                    if (System.IO.File.Exists(imagePath))
                        logoBox.Image = Image.FromFile(imagePath);
                }
                catch { }
                logoBox.SizeMode = PictureBoxSizeMode.Zoom;
                logoBox.Dock = DockStyle.Fill;
                this.contentPanel.Controls.Add(logoBox);

                this.navPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    Padding = new Padding(0, 16, 0, 0)
                };
                
                this.navBorder = new Panel
                {
                    Dock = DockStyle.Right,
                    Width = 1,
                    BackColor = Color.FromArgb(224, 224, 224)
                };
                this.navPanel.Controls.Add(this.navBorder);
                mainLayout.Controls.Add(this.navPanel, 0, 0);
                mainLayout.Controls.Add(contentPanel, 1, 0);

                string[] buttonNames = {
                    "Patients", "Doctors", "Appointments", "Rooms",
                    "Invoices", "Diseases", "Suppliers", "Medicines", "Billing"
                };

                foreach (string name in buttonNames)
                {
                    Button btn = new Button();
                    btn.Text = name;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.Height = 50;
                    btn.Dock = DockStyle.Top;
                    btn.TextAlign = ContentAlignment.MiddleLeft;
                    btn.ImageAlign = ContentAlignment.MiddleLeft;
                    btn.TextImageRelation = TextImageRelation.ImageBeforeText;
                    btn.Padding = new Padding(40, 0, 0, 0);
                    btn.BackColor = Color.Transparent;
                    btn.ForeColor = Color.Black;
                    btn.Font = new Font("Segoe UI", 10);
                    btn.Cursor = Cursors.Hand;
                    btn.Margin = new Padding(0, 0, 0, 2);
                    btn.Click += new EventHandler(Button_Click);

                    string imagePath = null;
                    switch (name)
                    {
                        case "Patients": imagePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Image", "patient.png"); break;
                        case "Doctors": imagePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Image", "doctor.png"); break;
                        case "Appointments": imagePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Image", "appointment.png"); break;
                        case "Rooms": imagePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Image", "Room.png"); break;
                        case "Invoices": imagePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Image", "invoice.png"); break;
                        case "Diseases": imagePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Image", "diseases.png"); break;
                        case "Suppliers": imagePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Image", "supplier.png"); break;
                        case "Medicines": imagePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Image", "medicine.png"); break;
                        case "Billing": imagePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "Image", "bill.png"); break;
                    }
                    if (!string.IsNullOrEmpty(imagePath) && System.IO.File.Exists(imagePath))
                    {
                        try
                        {
                            btn.Image = new Bitmap(Image.FromFile(imagePath), new Size(24, 24));
                        }
                        catch { }
                    }

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
                navPanel.Controls.SetChildIndex(btnBilling, 0);
                navPanel.Controls.SetChildIndex(btnMedicines, 1);
                navPanel.Controls.SetChildIndex(btnSuppliers, 2);
                navPanel.Controls.SetChildIndex(btnDiseases, 3);
                navPanel.Controls.SetChildIndex(btnInvoices, 4);
                navPanel.Controls.SetChildIndex(btnRooms, 5);
                navPanel.Controls.SetChildIndex(btnAppointments, 6);
                navPanel.Controls.SetChildIndex(btnDoctors, 7);
                navPanel.Controls.SetChildIndex(btnPatients, 8);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error initializing main form: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
