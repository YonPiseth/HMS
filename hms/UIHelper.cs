using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace HMS
{
    public static class UIHelper
    {
        public static void StyleDataGridView(DataGridView dgv)
        {
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 14);
            dgv.RowTemplate.Height = 32;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.ColumnHeadersHeight = 48;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240); // Light gray for alternating rows
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 215); // Darker blue header
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.LightGray;
            dgv.RowHeadersVisible = false; // Hide row headers for a cleaner look
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.BorderStyle = BorderStyle.None; // No border on the grid itself

            // Adjust columns after data binding
            dgv.DataBindingComplete += (s, e) =>
            {
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    // Set a minimum width for all columns
                    if (col.Width < 120) col.Width = 120;
                    // Make image columns larger
                    if (col is DataGridViewImageColumn)
                    {
                        col.Width = 120;
                        ((DataGridViewImageColumn)col).ImageLayout = DataGridViewImageCellLayout.Zoom;
                    }
                }
            };
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

        // Modern accent color
        public static Color AccentColor = Color.FromArgb(33, 150, 243); // Material Blue
        public static Color AccentHover = Color.FromArgb(30, 136, 229);
        public static Color LightGray = Color.FromArgb(245, 245, 245);
        public static Font ModernFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static Font ModernTitleFont = new Font("Segoe UI", 18F, FontStyle.Bold);

        public static void StyleModernButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = AccentColor;
            btn.ForeColor = Color.White;
            btn.Font = ModernFont;
            btn.Height = 40;
            btn.Width = 140;
            btn.Margin = new Padding(10, 0, 0, 0);
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Region = System.Drawing.Region.FromHrgn(
                NativeMethods.CreateRoundRectRgn(0, 0, btn.Width, btn.Height, 12, 12));
            btn.MouseEnter += (s, e) => btn.BackColor = AccentHover;
            btn.MouseLeave += (s, e) => btn.BackColor = AccentColor;
        }

        public static void StyleModernTextBox(TextBox txt)
        {
            txt.Font = ModernFont;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.BackColor = Color.White;
            txt.ForeColor = Color.Black;
            txt.Padding = new Padding(6);
        }

        public static void StyleModernPanel(Panel panel)
        {
            panel.BackColor = LightGray;
            panel.Padding = new Padding(15);
            panel.BorderStyle = BorderStyle.None;
        }

        public static void StyleModernLabel(Label lbl)
        {
            lbl.Font = ModernFont;
            lbl.ForeColor = Color.Black;
            lbl.AutoSize = true;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
        }

        public static void StyleModernTitle(Label lbl)
        {
            lbl.Font = ModernTitleFont;
            lbl.ForeColor = AccentColor;
            lbl.AutoSize = true;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
        }

        public static void StyleModernComboBox(ComboBox cmb)
        {
            cmb.Font = ModernFont;
            cmb.FlatStyle = FlatStyle.Flat;
            cmb.BackColor = Color.White;
            cmb.ForeColor = Color.Black;
            cmb.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        // Native method for rounded corners
        private static class NativeMethods
        {
            [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
            public static extern IntPtr CreateRoundRectRgn(
                int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);
        }
    }
} 