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

        private void InitializeComponent()
        {
            txtDiseaseName = new TextBox();
            txtDescription = new TextBox();
            txtSymptoms = new TextBox();
            txtTreatment = new TextBox();
            btnSave = new Button();
            btnCancel = new Button();
            mainLayout = new TableLayoutPanel();
            lblDiseaseName = new Label();
            lblDescription = new Label();
            lblSymptoms = new Label();
            lblTreatment = new Label();
            buttonPanel = new FlowLayoutPanel();
            mainLayout.SuspendLayout();
            buttonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // txtDiseaseName
            // 
            txtDiseaseName.Dock = DockStyle.Fill;
            txtDiseaseName.Location = new Point(153, 18);
            txtDiseaseName.Name = "txtDiseaseName";
            txtDiseaseName.Size = new Size(311, 27);
            txtDiseaseName.TabIndex = 1;
            // 
            // txtDescription
            // 
            txtDescription.Dock = DockStyle.Fill;
            txtDescription.Location = new Point(153, 58);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(311, 27);
            txtDescription.TabIndex = 3;
            // 
            // txtSymptoms
            // 
            txtSymptoms.Dock = DockStyle.Fill;
            txtSymptoms.Location = new Point(153, 98);
            txtSymptoms.Multiline = true;
            txtSymptoms.Name = "txtSymptoms";
            txtSymptoms.Size = new Size(311, 94);
            txtSymptoms.TabIndex = 5;
            // 
            // txtTreatment
            // 
            txtTreatment.Dock = DockStyle.Fill;
            txtTreatment.Location = new Point(153, 198);
            txtTreatment.Multiline = true;
            txtTreatment.Name = "txtTreatment";
            txtTreatment.Size = new Size(311, 94);
            txtTreatment.TabIndex = 7;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(237, 8);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(100, 23);
            btnSave.TabIndex = 1;
            btnSave.Text = "Save";
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(343, 8);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 23);
            btnCancel.TabIndex = 0;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // mainLayout
            // 
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            mainLayout.Controls.Add(lblDiseaseName, 0, 0);
            mainLayout.Controls.Add(txtDiseaseName, 1, 0);
            mainLayout.Controls.Add(lblDescription, 0, 1);
            mainLayout.Controls.Add(txtDescription, 1, 1);
            mainLayout.Controls.Add(lblSymptoms, 0, 2);
            mainLayout.Controls.Add(txtSymptoms, 1, 2);
            mainLayout.Controls.Add(lblTreatment, 0, 3);
            mainLayout.Controls.Add(txtTreatment, 1, 3);
            mainLayout.Controls.Add(buttonPanel, 0, 5);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Location = new Point(0, 0);
            mainLayout.Name = "mainLayout";
            mainLayout.Padding = new Padding(15);
            mainLayout.RowCount = 6;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            mainLayout.Size = new Size(482, 453);
            mainLayout.TabIndex = 0;
            // 
            // lblDiseaseName
            // 
            lblDiseaseName.Location = new Point(18, 15);
            lblDiseaseName.Name = "lblDiseaseName";
            lblDiseaseName.Size = new Size(100, 23);
            lblDiseaseName.TabIndex = 0;
            // 
            // lblDescription
            // 
            lblDescription.Location = new Point(18, 55);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(100, 23);
            lblDescription.TabIndex = 2;
            // 
            // lblSymptoms
            // 
            lblSymptoms.Location = new Point(18, 95);
            lblSymptoms.Name = "lblSymptoms";
            lblSymptoms.Size = new Size(100, 23);
            lblSymptoms.TabIndex = 4;
            // 
            // lblTreatment
            // 
            lblTreatment.Location = new Point(18, 195);
            lblTreatment.Name = "lblTreatment";
            lblTreatment.Size = new Size(100, 23);
            lblTreatment.TabIndex = 6;
            // 
            // buttonPanel
            // 
            mainLayout.SetColumnSpan(buttonPanel, 2);
            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(btnSave);
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.FlowDirection = FlowDirection.RightToLeft;
            buttonPanel.Location = new Point(18, 348);
            buttonPanel.Name = "buttonPanel";
            buttonPanel.Padding = new Padding(0, 5, 0, 0);
            buttonPanel.Size = new Size(446, 87);
            buttonPanel.TabIndex = 8;
            // 
            // DiseaseForm
            // 
            ClientSize = new Size(482, 453);
            Controls.Add(mainLayout);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DiseaseForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Disease Information";
            mainLayout.ResumeLayout(false);
            mainLayout.PerformLayout();
            buttonPanel.ResumeLayout(false);
            ResumeLayout(false);
        }
        private TableLayoutPanel mainLayout;
        private Label lblDiseaseName;
        private Label lblDescription;
        private Label lblSymptoms;
        private Label lblTreatment;
        private FlowLayoutPanel buttonPanel;
    }
} 