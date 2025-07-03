using System;
using System.Drawing;
using System.Windows.Forms;

public static class UIThemeHelper
{
    // Main dark theme colors
    private static readonly Color FormBackColor = Color.FromArgb(34, 40, 49);
    private static readonly Color ControlBackColor = Color.FromArgb(57, 62, 70);
    private static readonly Color ButtonBackColor = Color.FromArgb(0, 173, 181);
    private static readonly Color ButtonForeColor = Color.White;
    private static readonly Color LabelForeColor = Color.WhiteSmoke;
    private static readonly Color TextBoxBackColor = Color.FromArgb(44, 54, 63);
    private static readonly Color TextBoxForeColor = Color.White;
    private static readonly Font DefaultFont = new Font("Segoe UI", 10F, FontStyle.Regular);

    public static void ApplyTheme(Form form)
    {
        form.BackColor = FormBackColor;
        form.Font = DefaultFont;
        foreach (Control control in form.Controls)
        {
            ApplyThemeToControl(control);
        }
    }

    private static void ApplyThemeToControl(Control control)
    {
        if (control is Button btn)
        {
            btn.BackColor = ButtonBackColor;
            btn.ForeColor = ButtonForeColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = DefaultFont;
        }
        else if (control is Label lbl)
        {
            lbl.ForeColor = LabelForeColor;
            lbl.Font = DefaultFont;
        }
        else if (control is TextBox txt)
        {
            txt.BackColor = TextBoxBackColor;
            txt.ForeColor = TextBoxForeColor;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Font = DefaultFont;
        }
        else if (control is DataGridView dgv)
        {
            dgv.BackgroundColor = ControlBackColor;
            dgv.DefaultCellStyle.BackColor = TextBoxBackColor;
            dgv.DefaultCellStyle.ForeColor = TextBoxForeColor;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ButtonBackColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ButtonForeColor;
            dgv.EnableHeadersVisualStyles = false;
            dgv.Font = DefaultFont;
        }
        else
        {
            control.BackColor = ControlBackColor;
            control.ForeColor = LabelForeColor;
            control.Font = DefaultFont;
        }

        // Recursively apply to child controls
        foreach (Control child in control.Controls)
        {
            ApplyThemeToControl(child);
        }
    }
} 