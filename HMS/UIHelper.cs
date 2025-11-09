using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using HMS.UI;

namespace HMS
{
    public static class UIHelper
    {
        public static void StyleDataGridView(DataGridView dgv)
        {
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font(new FontFamily("Segoe UI"), 10, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font(new FontFamily("Segoe UI"), 10, FontStyle.Regular);
            
            dgv.RowTemplate.Height = 36;
            dgv.ColumnHeadersHeight = 36;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            
            dgv.BackgroundColor = UITheme.BackgroundWhite;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = UITheme.GridRowAlternate;
            
            dgv.DefaultCellStyle.SelectionBackColor = UITheme.GridRowSelected;
            dgv.DefaultCellStyle.SelectionForeColor = UITheme.TextPrimary;
            
            dgv.DefaultCellStyle.Padding = new Padding(8, 6, 8, 6);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 8, 8, 8);
            
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = UITheme.GridHeader;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = UITheme.GridHeaderText;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = UITheme.GridHeader;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.None;
            dgv.GridColor = Color.FromArgb(240, 240, 240);
            
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.BorderStyle = BorderStyle.None;
            
            dgv.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                    dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = UITheme.GridRowHover;
            };
            
            dgv.CellMouseLeave += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    if (e.RowIndex % 2 == 1)
                        dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = UITheme.GridRowAlternate;
                    else
                        dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = UITheme.BackgroundWhite;
                }
            };
            
            dgv.RowPostPaint += (s, e) =>
            {
                using (Pen separatorPen = new Pen(Color.FromArgb(240, 240, 240), 1))
                {
                    Rectangle rect = e.RowBounds;
                    int y = rect.Bottom - 1;
                    e.Graphics.DrawLine(separatorPen, rect.Left, y, rect.Right, y);
                }
            };

            dgv.DataBindingComplete += (s, e) =>
            {
                try
                {
                    if (dgv == null || dgv.IsDisposed || dgv.Columns == null || dgv.Columns.Count == 0)
                        return;
                    
                    foreach (DataGridViewColumn col in dgv.Columns)
                    {
                        if (col == null) continue;
                        
                        // Set minimum width but allow auto-sizing for content
                        if (col.Width < 100) col.Width = 100;
                        
                        // Image columns
                        if (col is DataGridViewImageColumn)
                        {
                            col.Width = 80;
                            ((DataGridViewImageColumn)col).ImageLayout = DataGridViewImageCellLayout.Zoom;
                        }
                        
                        // Better default cell alignment
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    }
                    
                    // Refresh to show the separator lines
                    dgv.Invalidate();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            };
        }

        /// <summary>
        /// Wraps a DataGridView in a rounded panel for a softer, more modern appearance
        /// </summary>
        public static Panel WrapDataGridViewInRoundedPanel(DataGridView dgv, int borderRadius = UITheme.RadiusMD)
        {
            Panel container = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UITheme.BackgroundWhite,
                Padding = new Padding(1)
            };

            dgv.Dock = DockStyle.Fill;
            dgv.BorderStyle = BorderStyle.None;
            
            container.Paint += (s, e) =>
            {
                Rectangle bounds = container.ClientRectangle;
                bounds.Width -= 1;
                bounds.Height -= 1;
                
                using (GraphicsPath path = UITheme.CreateRoundedRectangle(bounds, borderRadius))
                using (Pen borderPen = new Pen(UITheme.BorderLight, 1))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawPath(borderPen, path);
                }
            };

            container.Controls.Add(dgv);
            return container;
        }

        public static void StyleButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = Color.FromArgb(0, 120, 215);
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
            lbl.ForeColor = Color.FromArgb(24, 33, 54);
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

        public static Color AccentColor => UITheme.PrimaryBlue;
        public static Color AccentHover => UITheme.HoverBlue;
        public static Color LightGray => UITheme.BackgroundGray;
        public static Font ModernFont => UITheme.FontBody;
        public static Font ModernTitleFont => UITheme.FontHeading3;

        public static void StyleModernButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = AccentColor;
            btn.ForeColor = UITheme.TextOnPrimary;
            btn.Font = UITheme.FontButton;
            if (btn.Height == 0) btn.Height = 40;
            if (btn.Width == 0) btn.Width = 140;
            btn.Margin = new Padding(UITheme.SpacingSM, 0, 0, 0);
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Padding = new Padding(UITheme.SpacingMD, UITheme.SpacingSM, UITheme.SpacingMD, UITheme.SpacingSM);
            
            // Create rounded corners using region (only if button has valid size)
            try
            {
                if (btn.Width > 0 && btn.Height > 0)
                {
                    IntPtr regionHandle = NativeMethods.CreateRoundRectRgn(0, 0, btn.Width, btn.Height, UITheme.RadiusMD, UITheme.RadiusMD);
                    if (regionHandle != IntPtr.Zero)
                    {
                        btn.Region = System.Drawing.Region.FromHrgn(regionHandle);
                    }
                }
            }
            catch
            {
                // If region creation fails, continue without rounded corners
            }
            
            // Hover effects
            EventHandler updateRegion = (s, e) =>
            {
                try
                {
                    if (btn.Width > 0 && btn.Height > 0)
                    {
                        IntPtr regionHandle = NativeMethods.CreateRoundRectRgn(0, 0, btn.Width, btn.Height, UITheme.RadiusMD, UITheme.RadiusMD);
                        if (regionHandle != IntPtr.Zero)
                        {
                            if (btn.Region != null) btn.Region.Dispose();
                            btn.Region = System.Drawing.Region.FromHrgn(regionHandle);
                        }
                    }
                }
                catch { }
            };
            
            btn.MouseEnter += (s, e) => 
            {
                btn.BackColor = AccentHover;
                updateRegion(s, e);
            };
            btn.MouseLeave += (s, e) => 
            {
                btn.BackColor = AccentColor;
                updateRegion(s, e);
            };
            btn.MouseDown += (s, e) => btn.BackColor = UITheme.ActiveBlue;
            btn.MouseUp += (s, e) => btn.BackColor = AccentHover;
            
            // Update region when button is resized
            btn.Resize += updateRegion;
        }

        /// <summary>
        /// Creates a modern card panel with shadow and rounded corners
        /// </summary>
        public static Panel CreateModernCard(int borderRadius = UITheme.RadiusMD, bool showShadow = true)
        {
            Panel card = new Panel
            {
                BackColor = UITheme.CardBackground,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(UITheme.SpacingMD)
            };

            if (showShadow)
            {
                card.Paint += (s, e) =>
                {
                    Graphics g = e.Graphics;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    UITheme.DrawCardShadow(g, card.ClientRectangle, borderRadius);
                };
            }

            return card;
        }

        /// <summary>
        /// Styles a navigation button for sidebar
        /// </summary>
        public static void StyleNavButton(Button btn, bool isActive = false)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = UITheme.FontBody;
            btn.Height = 50;
            btn.Dock = DockStyle.Top;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(UITheme.SpacingMD, 0, 0, 0);
            btn.Margin = new Padding(0, 0, 0, UITheme.SpacingXS);
            btn.Cursor = Cursors.Hand;

            if (isActive)
            {
                // Active state: Dark blue background with white text for better contrast
                btn.BackColor = UITheme.PrimaryBlue;
                btn.ForeColor = Color.White; // White text on blue background - high contrast
                btn.Font = new Font(btn.Font, FontStyle.Bold);
            }
            else
            {
                btn.BackColor = Color.Transparent;
                btn.ForeColor = UITheme.TextPrimary; // Dark text on light background
            }

            // Hover effects
            btn.MouseEnter += (s, e) =>
            {
                if (!isActive)
                {
                    btn.BackColor = UITheme.HoverGray; // Light gray on hover
                    btn.ForeColor = UITheme.TextPrimary; // Keep dark text
                }
            };
            btn.MouseLeave += (s, e) =>
            {
                if (!isActive)
                {
                    btn.BackColor = Color.Transparent;
                    btn.ForeColor = UITheme.TextPrimary;
                }
            };
        }

        public static void StyleModernTextBox(TextBox txt)
        {
            txt.Font = UITheme.FontBody;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.BackColor = UITheme.BackgroundWhite;
            txt.ForeColor = UITheme.TextPrimary;
            txt.Padding = new Padding(UITheme.SpacingSM);
        }

        /// <summary>
        /// Creates a modern textbox with rounded corners and focus effects
        /// </summary>
        public static TextBox CreateModernTextBox()
        {
            TextBox txt = new TextBox
            {
                Font = UITheme.FontBody,
                BorderStyle = BorderStyle.None,
                BackColor = UITheme.BackgroundWhite,
                ForeColor = UITheme.TextPrimary,
                Padding = new Padding(UITheme.SpacingSM)
            };
            
            // Wrap in panel for border effect
            Panel container = new Panel
            {
                BackColor = UITheme.BorderLight,
                Padding = new Padding(1),
                Height = txt.Height + 2
            };
            container.Controls.Add(txt);
            txt.Dock = DockStyle.Fill;
            
            return txt;
        }

        public static void StyleModernPanel(Panel panel)
        {
            panel.BackColor = UITheme.BackgroundLight;
            panel.Padding = new Padding(UITheme.SpacingMD);
            panel.BorderStyle = BorderStyle.None;
        }

        /// <summary>
        /// Creates a modern card panel
        /// </summary>
        public static void StyleCardPanel(Panel panel, int borderRadius = UITheme.RadiusMD)
        {
            panel.BackColor = UITheme.CardBackground;
            panel.Padding = new Padding(UITheme.SpacingMD);
            panel.BorderStyle = BorderStyle.None;
            
            // Add shadow and rounded corners
            panel.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                UITheme.DrawCardShadow(g, panel.ClientRectangle, borderRadius);
                
                using (GraphicsPath path = UITheme.CreateRoundedRectangle(panel.ClientRectangle, borderRadius))
                using (SolidBrush brush = new SolidBrush(panel.BackColor))
                using (Pen pen = new Pen(UITheme.CardBorder, 1))
                {
                    g.FillPath(brush, path);
                    g.DrawPath(pen, path);
                }
            };
        }

        public static void StyleModernTitle(Label lbl)
        {
            lbl.Font = UITheme.FontHeading3;
            lbl.ForeColor = UITheme.PrimaryBlue;
            lbl.AutoSize = true;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
        }

        /// <summary>
        /// Styles a heading label (H1, H2, H3, H4)
        /// </summary>
        public static void StyleHeading(Label lbl, int level = 1)
        {
            switch (level)
            {
                case 1:
                    lbl.Font = UITheme.FontHeading1;
                    break;
                case 2:
                    lbl.Font = UITheme.FontHeading2;
                    break;
                case 3:
                    lbl.Font = UITheme.FontHeading3;
                    break;
                case 4:
                    lbl.Font = UITheme.FontHeading4;
                    break;
                default:
                    lbl.Font = UITheme.FontHeading3;
                    break;
            }
            lbl.ForeColor = UITheme.TextPrimary;
            lbl.AutoSize = true;
        }

        /// <summary>
        /// Styles a caption/subtitle label
        /// </summary>
        public static void StyleCaption(Label lbl)
        {
            lbl.Font = UITheme.FontCaption;
            lbl.ForeColor = UITheme.TextSecondary;
            lbl.AutoSize = true;
        }

        public static void StyleModernComboBox(ComboBox cmb)
        {
            cmb.Font = UITheme.FontBody;
            cmb.FlatStyle = FlatStyle.Flat;
            cmb.BackColor = UITheme.BackgroundWhite;
            cmb.ForeColor = UITheme.TextPrimary;
            cmb.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public static void StyleModernLabel(Label lbl)
        {
            lbl.Font = UITheme.FontBody;
            lbl.ForeColor = UITheme.TextPrimary;
            lbl.AutoSize = true;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
        }

        /// <summary>
        /// Creates a modern separator line
        /// </summary>
        public static Panel CreateSeparator(Orientation orientation = Orientation.Horizontal)
        {
            Panel separator = new Panel
            {
                BackColor = UITheme.BorderLight,
                Height = orientation == Orientation.Horizontal ? 1 : 100,
                Width = orientation == Orientation.Horizontal ? 100 : 1
            };
            return separator;
        }

        /// <summary>
        /// Creates a status badge label
        /// </summary>
        public static Label CreateStatusBadge(string text, Color backgroundColor, Color textColor)
        {
            Label badge = new Label
            {
                Text = text,
                BackColor = backgroundColor,
                ForeColor = textColor,
                Font = UITheme.FontCaption,
                AutoSize = true,
                Padding = new Padding(UITheme.SpacingSM, UITheme.SpacingXS, UITheme.SpacingSM, UITheme.SpacingXS),
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            // Create rounded corners
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, badge.Width, badge.Height);
            badge.Region = new Region(path);
            
            return badge;
        }

        public static void ApplyModernTheme(Control container)
        {
            container.BackColor = UITheme.BackgroundLight; // Set default background for containers
            foreach (Control control in container.Controls)
            {
                if (control is Button button) StyleModernButton(button);
                else if (control is TextBox textBox) StyleModernTextBox(textBox);
                else if (control is Label label) StyleModernLabel(label);
                else if (control is ComboBox comboBox) StyleModernComboBox(comboBox);
                else if (control is Panel panel) StyleModernPanel(panel);
                else if (control is DataGridView dgv) StyleDataGridView(dgv); // Using existing DataGridView style

                // Recursively apply to child controls
                if (control.HasChildren)
                {
                    ApplyModernTheme(control);
                }
            }
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