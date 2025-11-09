using System.Drawing;
using System.Drawing.Drawing2D;

namespace HMS.UI
{
    /// <summary>
    /// Modern UI theme with consistent colors, fonts, and design tokens
    /// Follows Material Design principles
    /// </summary>
    public static class UITheme
    {
        // Primary Colors
        public static Color PrimaryBlue = Color.FromArgb(33, 150, 243);      // Material Blue 500
        public static Color PrimaryBlueDark = Color.FromArgb(25, 118, 210);  // Material Blue 600
        public static Color PrimaryBlueLight = Color.FromArgb(66, 165, 245); // Material Blue 400
        public static Color PrimaryBlueAccent = Color.FromArgb(0, 122, 204); // Visual Studio Blue

        // Secondary Colors
        public static Color SecondaryGreen = Color.FromArgb(76, 175, 80);    // Material Green 500
        public static Color SecondaryRed = Color.FromArgb(244, 67, 54);      // Material Red 500
        public static Color SecondaryOrange = Color.FromArgb(255, 152, 0);   // Material Orange 500
        public static Color SecondaryPurple = Color.FromArgb(156, 39, 176);  // Material Purple 500

        // Neutral Colors
        public static Color BackgroundLight = Color.FromArgb(250, 250, 250); // Almost white
        public static Color BackgroundWhite = Color.White;
        public static Color BackgroundGray = Color.FromArgb(245, 245, 245);  // Light gray
        public static Color BackgroundDark = Color.FromArgb(33, 33, 33);     // Dark gray

        // Text Colors
        public static Color TextPrimary = Color.FromArgb(33, 33, 33);        // Almost black
        public static Color TextSecondary = Color.FromArgb(117, 117, 117);   // Medium gray
        public static Color TextDisabled = Color.FromArgb(189, 189, 189);    // Light gray
        public static Color TextOnPrimary = Color.White;                     // White on colored backgrounds

        // Border Colors
        public static Color BorderLight = Color.FromArgb(224, 224, 224);     // Light border
        public static Color BorderMedium = Color.FromArgb(189, 189, 189);    // Medium border
        public static Color BorderDark = Color.FromArgb(117, 117, 117);      // Dark border

        // Status Colors
        public static Color Success = Color.FromArgb(76, 175, 80);           // Green
        public static Color Warning = Color.FromArgb(255, 152, 0);           // Orange
        public static Color Error = Color.FromArgb(244, 67, 54);             // Red
        public static Color Info = Color.FromArgb(33, 150, 243);             // Blue

        // Hover/Active States
        public static Color HoverBlue = Color.FromArgb(25, 118, 210);
        public static Color ActiveBlue = Color.FromArgb(21, 101, 192);
        public static Color HoverGray = Color.FromArgb(245, 245, 245);
        public static Color ActiveGray = Color.FromArgb(238, 238, 238);

        // Sidebar/Navigation Colors
        public static Color SidebarBackground = Color.FromArgb(42, 42, 42);  // Dark sidebar
        public static Color SidebarHover = Color.FromArgb(55, 55, 55);       // Darker on hover
        public static Color SidebarActive = Color.FromArgb(33, 150, 243);    // Blue for active
        public static Color SidebarText = Color.White;
        public static Color SidebarTextHover = Color.White;

        // Card Colors
        public static Color CardBackground = Color.White;
        public static Color CardShadow = Color.FromArgb(0, 0, 0, 25);        // Semi-transparent black
        public static Color CardBorder = Color.FromArgb(224, 224, 224);

        // DataGrid Colors
        public static Color GridHeader = Color.FromArgb(33, 150, 243);
        public static Color GridHeaderText = Color.White;
        public static Color GridRowAlternate = Color.FromArgb(250, 250, 250);
        public static Color GridRowHover = Color.FromArgb(236, 245, 253);
        public static Color GridRowSelected = Color.FromArgb(187, 222, 251);

        // Fonts
        public static Font FontHeading1 = new Font("Segoe UI", 28, FontStyle.Bold);
        public static Font FontHeading2 = new Font("Segoe UI", 22, FontStyle.Bold);
        public static Font FontHeading3 = new Font("Segoe UI", 18, FontStyle.Bold);
        public static Font FontHeading4 = new Font("Segoe UI", 16, FontStyle.Bold);
        public static Font FontBodyLarge = new Font("Segoe UI", 12, FontStyle.Regular);
        public static Font FontBody = new Font("Segoe UI", 10, FontStyle.Regular);
        public static Font FontBodySmall = new Font("Segoe UI", 9, FontStyle.Regular);
        public static Font FontButton = new Font("Segoe UI", 10, FontStyle.Bold);
        public static Font FontCaption = new Font("Segoe UI", 8, FontStyle.Regular);

        // Spacing
        public const int SpacingXS = 4;
        public const int SpacingSM = 8;
        public const int SpacingMD = 16;
        public const int SpacingLG = 24;
        public const int SpacingXL = 32;

        // Border Radius
        public const int RadiusSM = 4;
        public const int RadiusMD = 8;
        public const int RadiusLG = 12;
        public const int RadiusXL = 16;

        // Shadows
        public static void DrawCardShadow(Graphics g, Rectangle rect, int radius = RadiusMD)
        {
            // Draw multiple shadow layers for depth
            for (int i = 3; i > 0; i--)
            {
                Rectangle shadowRect = new Rectangle(
                    rect.X + i,
                    rect.Y + i,
                    rect.Width,
                    rect.Height
                );
                using (GraphicsPath path = CreateRoundedRectangle(shadowRect, radius))
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(10 * i, 0, 0, 0)))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        public static GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        // Gradient Brushes
        public static LinearGradientBrush CreateGradientBrush(Rectangle rect, Color startColor, Color endColor, LinearGradientMode mode = LinearGradientMode.Vertical)
        {
            return new LinearGradientBrush(rect, startColor, endColor, mode);
        }

        // Helper to get lighter/darker color
        public static Color Lighten(Color color, float percent)
        {
            float r = color.R + (255 - color.R) * percent;
            float g = color.G + (255 - color.G) * percent;
            float b = color.B + (255 - color.B) * percent;
            return Color.FromArgb(color.A, (int)r, (int)g, (int)b);
        }

        public static Color Darken(Color color, float percent)
        {
            float r = color.R * (1 - percent);
            float g = color.G * (1 - percent);
            float b = color.B * (1 - percent);
            return Color.FromArgb(color.A, (int)r, (int)g, (int)b);
        }
    }
}

