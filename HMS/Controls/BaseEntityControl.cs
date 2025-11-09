using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using HMS.UI;
using HMS.Repositories;

namespace HMS.Controls
{
    /// <summary>
    /// Base control class for entity management controls
    /// Provides common functionality for CRUD operations and UI layout
    /// Uses Template Method Pattern - derived classes implement specific entity operations
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    /// <typeparam name="TRepository">The repository type for the entity</typeparam>
    public abstract class BaseEntityControl<T, TRepository> : UserControl 
        where T : class 
        where TRepository : IRepository<T>, new()
    {
        protected DataGridView dataGridView1;
        protected TextBox txtSearch;
        protected Button btnAdd;
        protected Button btnDelete;
        protected Button btnUpdate;
        protected Button btnLogout;
        protected FlowLayoutPanel buttonPanel;
        protected TRepository repository;

        protected BaseEntityControl()
        {
            repository = new TRepository();
            InitializeComponent();
            LoadData();
        }

        protected virtual void InitializeComponent()
        {
            this.dataGridView1 = new DataGridView();
            this.txtSearch = new TextBox();
            this.btnAdd = new Button();
            this.btnDelete = new Button();
            this.btnUpdate = new Button();
            this.btnLogout = new Button();
            this.buttonPanel = new FlowLayoutPanel();

            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 4;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            layout.Padding = new Padding(15);
            layout.BackColor = Color.White;

            Panel titlePanel = CreateTitlePanel();
            Panel searchPanel = CreateSearchPanel();
            UIHelper.StyleDataGridView(this.dataGridView1);
            Panel gridContainer = UIHelper.WrapDataGridViewInRoundedPanel(this.dataGridView1);
            this.buttonPanel = CreateButtonPanel();

            layout.Controls.Add(titlePanel, 0, 0);
            layout.Controls.Add(searchPanel, 0, 1);
            layout.Controls.Add(gridContainer, 0, 2);
            layout.Controls.Add(this.buttonPanel, 0, 3);

            this.Controls.Clear();
            this.Controls.Add(layout);
            this.Size = new Size(950, 550);

            this.txtSearch.TextChanged += new EventHandler(this.txtSearch_TextChanged);
            this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
            this.btnUpdate.Click += new EventHandler(this.btnUpdate_Click);
            this.btnDelete.Click += new EventHandler(this.btnDelete_Click);
            this.btnLogout.Click += new EventHandler(this.btnLogout_Click);
            this.dataGridView1.DataBindingComplete += DataGridView_DataBindingComplete;
        }

        protected virtual Panel CreateTitlePanel()
        {
            Panel titlePanel = new Panel();
            titlePanel.Dock = DockStyle.Fill;
            titlePanel.BackColor = Color.Transparent;
            
            Label lblTitle = new Label();
            lblTitle.Text = GetTitle();
            UIHelper.StyleHeading(lblTitle, 3);
            lblTitle.Location = new Point(0, 0);
            titlePanel.Controls.Add(lblTitle);
            
            return titlePanel;
        }

        protected virtual Panel CreateSearchPanel()
        {
            FlowLayoutPanel searchFlowPanel = new FlowLayoutPanel();
            searchFlowPanel.Dock = DockStyle.Fill;
            searchFlowPanel.Padding = new Padding(0, UITheme.SpacingSM, 0, 0);
            searchFlowPanel.FlowDirection = FlowDirection.LeftToRight;
            searchFlowPanel.BackColor = Color.Transparent;

            Label lblSearch = new Label
            {
                Text = "Search:",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 8, UITheme.SpacingSM, 0)
            };
            UIHelper.StyleModernLabel(lblSearch);
            searchFlowPanel.Controls.Add(lblSearch);

            this.txtSearch.Width = 250;
            this.txtSearch.Height = 30;
            UIHelper.StyleModernTextBox(this.txtSearch);
            searchFlowPanel.Controls.Add(this.txtSearch);

            Panel searchPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            searchPanel.Controls.Add(searchFlowPanel);

            return searchPanel;
        }

        protected virtual FlowLayoutPanel CreateButtonPanel()
        {
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, UITheme.SpacingSM, UITheme.SpacingMD, UITheme.SpacingSM),
                Height = 60,
                AutoSize = false,
                WrapContents = false
            };

            this.btnAdd.Text = GetAddButtonText();
            this.btnUpdate.Text = "Update";
            this.btnDelete.Text = "Delete";
            this.btnLogout.Text = "Log Out";

            foreach (Button btn in new[] { btnAdd, btnUpdate, btnDelete, btnLogout })
            {
                UIHelper.StyleModernButton(btn);
                btn.Width = 120;
                btn.Height = 40;
                btn.Margin = new Padding(UITheme.SpacingSM, 0, 0, 0);
            }

            buttonPanel.Controls.Add(this.btnLogout);
            buttonPanel.Controls.Add(this.btnDelete);
            buttonPanel.Controls.Add(this.btnUpdate);
            buttonPanel.Controls.Add(this.btnAdd);

            return buttonPanel;
        }

        protected virtual void DataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (this.dataGridView1.Columns.Contains("IsDeleted"))
                this.dataGridView1.Columns["IsDeleted"].Visible = false;
            
            ConfigureDataGridViewColumns();
        }

        protected abstract string GetTitle();
        protected abstract string GetAddButtonText();
        protected abstract void LoadData(string search = "");
        protected abstract void ConfigureDataGridViewColumns();
        protected abstract Form CreateEntityForm(int? entityId = null);
        protected abstract int GetSelectedEntityId();
        protected abstract string GetEntityName();
        protected virtual void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadData(txtSearch.Text);
        }

        protected virtual void btnAdd_Click(object sender, EventArgs e)
        {
            var form = CreateEntityForm();
            if (form != null && form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
                MessageBox.Show($"{GetEntityName()} added successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        protected virtual void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show($"Please select a {GetEntityName().ToLower()} to update.", 
                    "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int entityId = GetSelectedEntityId();
            var form = CreateEntityForm(entityId);
            if (form != null && form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
                MessageBox.Show($"{GetEntityName()} updated successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        protected virtual void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show($"Please select a {GetEntityName().ToLower()} to delete.", 
                    "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dialogResult = MessageBox.Show(
                $"Are you sure you want to delete this {GetEntityName().ToLower()}?", 
                "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (dialogResult == DialogResult.Yes)
            {
                int entityId = GetSelectedEntityId();
                if (repository.Delete(entityId))
                {
                    LoadData();
                    MessageBox.Show($"{GetEntityName()} deleted successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Failed to delete {GetEntityName().ToLower()}.", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected virtual void btnLogout_Click(object sender, EventArgs e)
        {
            if (this.ParentForm is HMS.MainForm mainForm)
            {
                mainForm.Logout();
            }
        }
    }
}

