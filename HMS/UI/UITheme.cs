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
        // Primary Colors (Medical Emerald & Slate)
        public static Color PrimaryEmerald = Color.FromArgb(16, 185, 129);   // Modern Emerald 500
        public static Color PrimarySlate = Color.FromArgb(30, 41, 59);     // Slate 800 (for depth)
        public static Color PrimaryBlue = Color.FromArgb(14, 165, 233);      // Sky Blue 500
        
        // Semantic Colors
        public static Color BrandPrimary = PrimaryEmerald;
        public static Color BrandSecondary = PrimarySlate;
        public static Color BrandAccent = PrimaryBlue;

        // Neutral Colors (Premium Grays)
        public static Color BackgroundLight = Color.FromArgb(248, 250, 252); // Slate 50
        public static Color BackgroundWhite = Color.White;
        public static Color BackgroundGray = Color.FromArgb(241, 245, 249);  // Slate 100
        public static Color BackgroundDark = Color.FromArgb(15, 23, 42);     // Slate 900
        public static Color BackgroundSidebar = Color.FromArgb(30, 41, 59);  // Slate 800

        // Text Colors
        public static Color TextPrimary = Color.FromArgb(15, 23, 42);        // Slate 900
        public static Color TextSecondary = Color.FromArgb(71, 85, 105);     // Slate 500
        public static Color TextDisabled = Color.FromArgb(148, 163, 184);    // Slate 400
        public static Color TextOnPrimary = Color.White;
        public static Color TextOnSidebar = Color.FromArgb(226, 232, 240);   // Slate 200

        // Border Colors
        public static Color BorderLight = Color.FromArgb(226, 232, 240);     // Slate 200
        public static Color BorderMedium = Color.FromArgb(203, 213, 225);    // Slate 300
        public static Color BorderDark = Color.FromArgb(148, 163, 184);      // Slate 400

        // Status Colors (Vibrant)
        public static Color Success = Color.FromArgb(34, 197, 94);           // Green 500
        public static Color Warning = Color.FromArgb(245, 158, 11);           // Amber 500
        public static Color Error = Color.FromArgb(239, 68, 68);             // Red 500
        public static Color Info = Color.FromArgb(59, 130, 246);             // Blue 500

        // Hover/Active States
        public static Color HoverBrand = Color.FromArgb(5, 150, 105);        // Emerald 600
        public static Color ActiveBrand = Color.FromArgb(4, 120, 87);         // Emerald 700
        public static Color HoverGray = Color.FromArgb(241, 245, 249);       // Slate 100
        public static Color ActiveGray = Color.FromArgb(226, 232, 240);      // Slate 200

        // Sidebar/Navigation Colors
        public static Color SidebarBackground = PrimarySlate;
        public static Color SidebarHover = Color.FromArgb(51, 65, 85);       // Slate 700
        public static Color SidebarActive = PrimaryEmerald;
        public static Color SidebarText = Color.FromArgb(226, 232, 240);
        public static Color SidebarTextActive = Color.White;

        // Card & Surface Colors
        public static Color CardBackground = Color.White;
        public static Color CardShadow = Color.FromArgb(0, 0, 0, 15);        // Softer shadow
        public static Color CardBorder = Color.FromArgb(241, 245, 249);

        // DataGrid Colors
        public static Color GridHeader = PrimaryBlue;
        public static Color GridHeaderText = Color.White;
        public static Color GridRowAlternate = Color.FromArgb(250, 250, 250);
        public static Color GridRowHover = Color.FromArgb(236, 245, 253);
        public static Color GridRowSelected = Color.FromArgb(187, 222, 251);

        // Compatibility Aliases (for older code)
        public static Color HoverBlue => HoverBrand;
        public static Color ActiveBlue => ActiveBrand;
        public static Color SecondaryGreen => Success;
        public static Color SecondaryRed => Error;
        public static Color SecondaryOrange => Warning;
        public static Color SecondaryPurple = Color.FromArgb(156, 39, 176);
        public static Color PrimaryBlueDark => PrimarySlate;
        public static Color PrimaryBlueLight => PrimaryBlue;
        public static Color SidebarTextHover => Color.White;

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

