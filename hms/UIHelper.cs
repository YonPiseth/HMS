using System.Drawing;
using System.Windows.Forms;

namespace HMS
{
    public static class UIHelper
    {
        public static void StyleDataGridView(DataGridView dgv)
        {
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240); // Light gray for alternating rows
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 215); // Darker blue header
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.LightGray;
            dgv.RowHeadersVisible = false; // Hide row headers for a cleaner look
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.BorderStyle = BorderStyle.None; // No border on the grid itself
        }

        public static void StyleButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.FromArgb(0, 120, 215); // Standard blue
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Height = 36;
            btn.Width = 120;
            btn.Margin = new Padding(10, 0, 0, 0);
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
        }

        public static void StyleTextBox(TextBox txt)
        {
            txt.Font = new Font("Segoe UI", 10);
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Padding = new Padding(5);
        }
        public static void StyleComboBox(ComboBox cmb)
        {
            cmb.Font = new Font("Segoe UI", 10);
            cmb.FlatStyle = FlatStyle.Flat;
        }

        public static void StyleLabelTitle(Label lbl)
        {
            lbl.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(24, 33, 54); // Dark blue/grey
            lbl.AutoSize = true;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
        }

        public static void StyleLabel(Label lbl)
        {
            lbl.Font = new Font("Segoe UI", 10);
            lbl.ForeColor = Color.Black;
            lbl.AutoSize = true;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
        }

        public static void ApplyPanelStyles(Panel panel)
        {
            panel.BackColor = Color.WhiteSmoke;
            panel.Padding = new Padding(10);
        }
    }
} 