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
            this.Size = new System.Drawing.Size(500, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Main layout panel
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            mainLayout.RowCount = 6;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Disease Name
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Description
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Symptoms
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100)); // Treatment
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Spacing
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // Buttons
            mainLayout.Padding = new Padding(15);
            UIHelper.ApplyPanelStyles(mainLayout);

            // Disease Name
            Label lblDiseaseName = new Label { Text = "Disease Name:" };
            UIHelper.StyleLabel(lblDiseaseName);
            this.txtDiseaseName.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(this.txtDiseaseName);

            // Description
            Label lblDescription = new Label { Text = "Description:" };
            UIHelper.StyleLabel(lblDescription);
            this.txtDescription.Dock = DockStyle.Fill;
            UIHelper.StyleTextBox(this.txtDescription);

            // Symptoms
            Label lblSymptoms = new Label { Text = "Symptoms:" };
            UIHelper.StyleLabel(lblSymptoms);
            this.txtSymptoms.Dock = DockStyle.Fill;
            this.txtSymptoms.Multiline = true;
            UIHelper.StyleTextBox(this.txtSymptoms);

            // Treatment
            Label lblTreatment = new Label { Text = "Treatment:" };
            UIHelper.StyleLabel(lblTreatment);
            this.txtTreatment.Dock = DockStyle.Fill;
            this.txtTreatment.Multiline = true;
            UIHelper.StyleTextBox(this.txtTreatment);

            // Buttons Panel
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel();
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Padding = new Padding(0, 5, 0, 0);
            UIHelper.ApplyPanelStyles(buttonPanel);

            this.btnSave.Text = "Save";
            UIHelper.StyleButton(this.btnSave);
            this.btnSave.Width = 100;
            this.btnSave.Click += new EventHandler(this.btnSave_Click);

            this.btnCancel.Text = "Cancel";
            UIHelper.StyleButton(this.btnCancel);
            this.btnCancel.Width = 100;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);

            buttonPanel.Controls.Add(this.btnCancel);
            buttonPanel.Controls.Add(this.btnSave);

            // Add controls to main layout
            mainLayout.Controls.Add(lblDiseaseName, 0, 0);
            mainLayout.Controls.Add(this.txtDiseaseName, 1, 0);

            mainLayout.Controls.Add(lblDescription, 0, 1);
            mainLayout.Controls.Add(this.txtDescription, 1, 1);

            mainLayout.Controls.Add(lblSymptoms, 0, 2);
            mainLayout.Controls.Add(this.txtSymptoms, 1, 2);

            mainLayout.Controls.Add(lblTreatment, 0, 3);
            mainLayout.Controls.Add(this.txtTreatment, 1, 3);

            mainLayout.Controls.Add(buttonPanel, 0, 5);
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);
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