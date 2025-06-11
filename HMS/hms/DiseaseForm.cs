using System;
using System.Windows.Forms;

namespace HMS
{
    public partial class DiseaseForm : Form
    {
        public TextBox txtDiseaseName;
        public TextBox txtDescription;
        public TextBox txtSymptoms;
        public TextBox txtTreatment;
        private Button btnSave;
        private Button btnCancel;

        public DiseaseForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtDiseaseName = new TextBox();
            this.txtDescription = new TextBox();
            this.txtSymptoms = new TextBox();
            this.txtTreatment = new TextBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();

            // Form settings
            this.Text = "Disease Information";
            this.Size = new System.Drawing.Size(400, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int labelX = 20;
            int controlX = 150;
            int startY = 20;
            int spacing = 40;

            // Disease Name
            Label lblDiseaseName = new Label();
            lblDiseaseName.Text = "Disease Name:";
            lblDiseaseName.Location = new System.Drawing.Point(labelX, startY);
            lblDiseaseName.AutoSize = true;
            this.txtDiseaseName.Location = new System.Drawing.Point(controlX, startY);
            this.txtDiseaseName.Size = new System.Drawing.Size(200, 23);

            // Description
            Label lblDescription = new Label();
            lblDescription.Text = "Description:";
            lblDescription.Location = new System.Drawing.Point(labelX, startY + spacing);
            lblDescription.AutoSize = true;
            this.txtDescription.Location = new System.Drawing.Point(controlX, startY + spacing);
            this.txtDescription.Size = new System.Drawing.Size(200, 23);

            // Symptoms
            Label lblSymptoms = new Label();
            lblSymptoms.Text = "Symptoms:";
            lblSymptoms.Location = new System.Drawing.Point(labelX, startY + spacing * 2);
            lblSymptoms.AutoSize = true;
            this.txtSymptoms.Location = new System.Drawing.Point(controlX, startY + spacing * 2);
            this.txtSymptoms.Size = new System.Drawing.Size(200, 60);
            this.txtSymptoms.Multiline = true;

            // Treatment
            Label lblTreatment = new Label();
            lblTreatment.Text = "Treatment:";
            lblTreatment.Location = new System.Drawing.Point(labelX, startY + spacing * 3);
            lblTreatment.AutoSize = true;
            this.txtTreatment.Location = new System.Drawing.Point(controlX, startY + spacing * 3);
            this.txtTreatment.Size = new System.Drawing.Size(200, 60);
            this.txtTreatment.Multiline = true;

            // Save Button
            this.btnSave.Text = "Save";
            this.btnSave.Location = new System.Drawing.Point(controlX, startY + spacing * 4 + 40);
            this.btnSave.Size = new System.Drawing.Size(90, 30);
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(controlX + 100, startY + spacing * 4 + 40);
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            this.Controls.AddRange(new Control[] {
                lblDiseaseName, txtDiseaseName,
                lblDescription, txtDescription,
                lblSymptoms, txtSymptoms,
                lblTreatment, txtTreatment,
                btnSave, btnCancel
            });
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDiseaseName.Text) || string.IsNullOrWhiteSpace(txtDescription.Text))
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