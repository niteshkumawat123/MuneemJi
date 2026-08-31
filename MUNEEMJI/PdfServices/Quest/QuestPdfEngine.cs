using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.Text;

namespace MUNEEMJI.PdfServices.Quest
{
    /// <summary>
    /// One-time QuestPDF bootstrap (licence + font registration) plus the
    /// formatting helpers shared by every generated document.
    /// Fonts are loaded from wwwroot/DataContainer/Font because the Linux
    /// production server has no Arial installed.
    /// </summary>
    public static class QuestPdfEngine
    {
        private static readonly object InitLock = new object();
        private static bool _initialised;

        public const string DefaultFontFamily = "Arial";

        /// <summary>
        /// Font QuestPDF ships in the box. None of the TTFs under
        /// DataContainer/Font carry U+20B9 (arial_with_rupee.ttf is the old
        /// Foradian trick - it draws the symbol in place of another character),
        /// so the rupee sign is served from here instead. Naming it explicitly in
        /// the family chain keeps that deterministic rather than relying on an
        /// implicit fallback, and works on the headless Linux server where no
        /// system fonts are installed.
        /// </summary>
        public const string FallbackFontFamily = "Lato";

        /// <summary>Indian Rupee sign, rendered via the fallback font.</summary>
        public const string Rupee = "₹";

        /// <summary>
        /// Safe to call on every request - the real work happens only once.
        /// </summary>
        public static void EnsureInitialised(IWebHostEnvironment env)
        {
            if (_initialised) return;

            lock (InitLock)
            {
                if (_initialised) return;

                QuestPDF.Settings.License = LicenseType.Community;

                // The server is headless Linux: never rely on OS-installed fonts.
                QuestPDF.Settings.CheckIfAllTextGlyphsAreAvailable = false;

                try
                {
                    var fontDir = Path.Combine(env?.WebRootPath ?? string.Empty, "DataContainer", "Font");
                    if (Directory.Exists(fontDir))
                    {
                        foreach (var file in Directory.GetFiles(fontDir, "*.ttf"))
                        {
                            try
                            {
                                using var stream = File.OpenRead(file);
                                QuestPDF.Drawing.FontManager.RegisterFont(stream);
                            }
                            catch
                            {
                                // A single unreadable font must not break PDF generation.
                            }
                        }
                    }
                }
                catch
                {
                    // Fall back to the bundled default font.
                }

                _initialised = true;
            }
        }


        // =================================================================
        //  Colours
        // =================================================================
        public static Color ParseColor(string hex, string fallbackHex)
        {
            var candidate = Normalise(hex) ?? Normalise(fallbackHex) ?? "#000000";
            try
            {
                return Color.FromHex(candidate);
            }
            catch
            {
                return Color.FromHex("#000000");
            }
        }

        /// <summary>
        /// Black or white, whichever stays readable on the given background.
        /// Lets one picked accent colour drive both a filled band and its text.
        /// </summary>
        public static string ContrastHex(string backgroundHex)
        {
            var value = Normalise(backgroundHex);
            if (value == null) return "#000000";

            // Expand #RGB to #RRGGBB
            if (value.Length == 4)
                value = "#" + value[1] + value[1] + value[2] + value[2] + value[3] + value[3];

            // Drop any alpha pair so #AARRGGBB and #RRGGBB both work.
            if (value.Length == 9)
                value = "#" + value.Substring(3);

            if (value.Length != 7) return "#000000";

            try
            {
                var r = Convert.ToInt32(value.Substring(1, 2), 16);
                var g = Convert.ToInt32(value.Substring(3, 2), 16);
                var b = Convert.ToInt32(value.Substring(5, 2), 16);

                // Perceived brightness (ITU-R BT.601)
                var luminance = (0.299 * r) + (0.587 * g) + (0.114 * b);
                return luminance > 150 ? "#000000" : "#FFFFFF";
            }
            catch
            {
                return "#000000";
            }
        }

        /// <summary>
        /// Lightens (amount &gt; 0) or darkens (amount &lt; 0) a colour by a fraction
        /// of the way to white or black. Lets a two-tone theme derive its slab and
        /// banner shades from the single accent the user picked.
        /// </summary>
        public static Color Shade(string hex, float amount)
        {
            var value = ExpandHex(hex);
            if (value == null) return Color.FromHex("#000000");

            try
            {
                var r = Convert.ToInt32(value.Substring(1, 2), 16);
                var g = Convert.ToInt32(value.Substring(3, 2), 16);
                var b = Convert.ToInt32(value.Substring(5, 2), 16);

                if (amount >= 0f)
                {
                    r = (int)(r + (255 - r) * amount);
                    g = (int)(g + (255 - g) * amount);
                    b = (int)(b + (255 - b) * amount);
                }
                else
                {
                    var k = 1f + amount;   // amount is negative
                    r = (int)(r * k);
                    g = (int)(g * k);
                    b = (int)(b * k);
                }

                r = Math.Clamp(r, 0, 255);
                g = Math.Clamp(g, 0, 255);
                b = Math.Clamp(b, 0, 255);

                return Color.FromHex($"#{r:X2}{g:X2}{b:X2}");
            }
            catch
            {
                return Color.FromHex("#000000");
            }
        }

        /// <summary>Normalises to #RRGGBB, dropping any alpha pair. Null when unusable.</summary>
        private static string ExpandHex(string hex)
        {
            var value = Normalise(hex);
            if (value == null) return null;

            if (value.Length == 4)
                value = "#" + value[1] + value[1] + value[2] + value[2] + value[3] + value[3];

            if (value.Length == 9)
                value = "#" + value.Substring(3);

            return value.Length == 7 ? value : null;
        }

        private static string Normalise(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex)) return null;
            var value = hex.Trim();
            if (!value.StartsWith("#")) value = "#" + value;
            if (value.Length != 4 && value.Length != 7 && value.Length != 9) return null;
            return value;
        }

        // =================================================================
        //  Page geometry
        // =================================================================
        public static PageSize ResolvePageSize(string paperSize, string orientation)
        {
            PageSize size;
            switch ((paperSize ?? "A4").Trim().ToUpperInvariant())
            {
                case "A3": size = PageSizes.A3; break;
                case "A5": size = PageSizes.A5; break;
                case "LETTER": size = PageSizes.Letter; break;
                case "LEGAL": size = PageSizes.Legal; break;
                default: size = PageSizes.A4; break;   // thermal rolls use ContinuousSize instead
            }

            if (string.Equals(orientation, "Landscape", StringComparison.OrdinalIgnoreCase))
                return size.Landscape();

            return size;
        }

        public static bool IsThermal(string paperSize)
        {
            var key = (paperSize ?? string.Empty).Replace(" ", string.Empty).ToUpperInvariant();
            return key == "2INCH" || key == "3INCH" || key == "4INCH" || key == "CUSTOM";
        }

        private const float PointsPerMm = 72f / 25.4f;

        /// <summary>
        /// Roll width in points, or 0 when this is not a thermal size.
        /// Widths follow the printable area the Print screen advertises:
        /// 2 Inch = 58 mm, 3 Inch = 68 mm, 4 Inch = 88 mm.
        /// "Custom" is derived from the configured characters-per-line.
        /// </summary>
        public static float ThermalWidth(string paperSize, int customChars = 48)
        {
            switch ((paperSize ?? string.Empty).Replace(" ", string.Empty).ToUpperInvariant())
            {
                case "2INCH": return 58f * PointsPerMm;    // ~164.4 pt
                case "3INCH": return 68f * PointsPerMm;    // ~192.8 pt
                case "4INCH": return 88f * PointsPerMm;    // ~249.4 pt
                case "CUSTOM":
                    var chars = customChars < 20 ? 48 : (customChars > 120 ? 120 : customChars);
                    // Approximate advance width at the thermal body size, plus both margins.
                    return chars * (ThermalBodyFontSize * AverageAdvanceRatio) + 12f;
                default: return 0f;
            }
        }

        /// <summary>Body text size used by the thermal receipt layout.</summary>
        public const float ThermalBodyFontSize = 6.5f;

        /// <summary>Average glyph advance as a fraction of point size, used to size Custom rolls.</summary>
        public const float AverageAdvanceRatio = 0.6f;

        // =================================================================
        //  Text sizing
        // =================================================================
        public static float CompanyNameSize(string token)
        {
            switch ((token ?? "Large").Trim().ToLowerInvariant())
            {
                case "small": return 12f;
                case "medium": return 15f;
                case "large": return 18f;
                case "extra large": return 22f;
                default: return 18f;
            }
        }

        public static float TitleSize(string token)
        {
            switch ((token ?? "Medium").Trim().ToLowerInvariant())
            {
                case "small": return 10f;
                case "medium": return 13f;
                case "large": return 16f;
                case "extra large": return 19f;
                default: return 13f;
            }
        }

        // =================================================================
        //  Number / date formatting
        // =================================================================
        private static readonly CultureInfo IndianCulture = CultureInfo.GetCultureInfo("en-IN");

        public static string Money(decimal value, bool withDecimal, bool withGrouping)
        {
            var rounded = withDecimal ? Math.Round(value, 2, MidpointRounding.AwayFromZero) : Math.Round(value, 0, MidpointRounding.AwayFromZero);

            if (withGrouping)
                return rounded.ToString(withDecimal ? "N2" : "N0", IndianCulture);

            return rounded.ToString(withDecimal ? "0.00" : "0", CultureInfo.InvariantCulture);
        }

        public static string Qty(decimal value)
        {
            if (value == Math.Floor(value))
                return value.ToString("0", CultureInfo.InvariantCulture);
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        public static string Percent(decimal value)
        {
            if (value == Math.Floor(value))
                return value.ToString("0", CultureInfo.InvariantCulture) + "%";
            return value.ToString("0.##", CultureInfo.InvariantCulture) + "%";
        }

        public static string DateOrDash(DateTime? value)
        {
            if (!value.HasValue || value.Value == DateTime.MinValue) return "-";
            return value.Value.ToString("dd-MM-yyyy");
        }

        public static string TimeOrDash(TimeSpan? value)
        {
            if (!value.HasValue || value.Value == TimeSpan.MinValue) return "-";
            var hours12 = value.Value.Hours % 12;
            if (hours12 == 0) hours12 = 12;
            var suffix = value.Value.Hours >= 12 ? "PM" : "AM";
            return $"{hours12:00}:{value.Value.Minutes:00} {suffix}";
        }

        public static string Dash(string value) => string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();

        // =================================================================
        //  Amount in words - Indian (lakh / crore) and International
        // =================================================================
        private static readonly string[] Units =
        {
            "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
            "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
            "Seventeen", "Eighteen", "Nineteen"
        };

        private static readonly string[] Tens =
        {
            "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
        };

        public static string AmountInWords(decimal amount, string format)
        {
            bool international = string.Equals(format, "International", StringComparison.OrdinalIgnoreCase);

            bool negative = amount < 0;
            amount = Math.Abs(amount);

            long whole = (long)Math.Truncate(amount);
            int paise = (int)Math.Round((amount - whole) * 100m, MidpointRounding.AwayFromZero);
            if (paise == 100) { whole += 1; paise = 0; }

            var sb = new StringBuilder();
            if (negative) sb.Append("Minus ");

            sb.Append(international ? InternationalWords(whole) : IndianWords(whole));
            sb.Append(" Rupees");

            if (paise > 0)
            {
                sb.Append(" and ");
                sb.Append(international ? InternationalWords(paise) : IndianWords(paise));
                sb.Append(" Paise");
            }

            sb.Append(" Only");
            return CollapseSpaces(sb.ToString());
        }

        private static string IndianWords(long value)
        {
            if (value == 0) return Units[0];
            if (value < 20) return Units[value];
            if (value < 100) return Tens[value / 10] + (value % 10 > 0 ? " " + IndianWords(value % 10) : string.Empty);
            if (value < 1000) return Units[value / 100] + " Hundred" + (value % 100 > 0 ? " " + IndianWords(value % 100) : string.Empty);
            if (value < 100000) return IndianWords(value / 1000) + " Thousand" + (value % 1000 > 0 ? " " + IndianWords(value % 1000) : string.Empty);
            if (value < 10000000) return IndianWords(value / 100000) + " Lakh" + (value % 100000 > 0 ? " " + IndianWords(value % 100000) : string.Empty);
            if (value < 1000000000000) return IndianWords(value / 10000000) + " Crore" + (value % 10000000 > 0 ? " " + IndianWords(value % 10000000) : string.Empty);
            return IndianWords(value / 1000000000000) + " Lakh Crore" + (value % 1000000000000 > 0 ? " " + IndianWords(value % 1000000000000) : string.Empty);
        }

        private static string InternationalWords(long value)
        {
            if (value == 0) return Units[0];
            if (value < 20) return Units[value];
            if (value < 100) return Tens[value / 10] + (value % 10 > 0 ? " " + InternationalWords(value % 10) : string.Empty);
            if (value < 1000) return Units[value / 100] + " Hundred" + (value % 100 > 0 ? " " + InternationalWords(value % 100) : string.Empty);
            if (value < 1000000) return InternationalWords(value / 1000) + " Thousand" + (value % 1000 > 0 ? " " + InternationalWords(value % 1000) : string.Empty);
            if (value < 1000000000) return InternationalWords(value / 1000000) + " Million" + (value % 1000000 > 0 ? " " + InternationalWords(value % 1000000) : string.Empty);
            if (value < 1000000000000) return InternationalWords(value / 1000000000) + " Billion" + (value % 1000000000 > 0 ? " " + InternationalWords(value % 1000000000) : string.Empty);
            return InternationalWords(value / 1000000000000) + " Trillion" + (value % 1000000000000 > 0 ? " " + InternationalWords(value % 1000000000000) : string.Empty);
        }

        private static string CollapseSpaces(string value)
        {
            return string.Join(" ", value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        // =================================================================
        //  Assets
        // =================================================================
        /// <summary>
        /// Turns a stored path (e.g. "/Web/uploads/logos/x.png" or "uploads/logos/x.png")
        /// into an absolute disk path under wwwroot, or null when the file is missing.
        /// </summary>
        public static string ResolveAssetPath(IWebHostEnvironment env, string storedPath)
        {
            if (env == null || string.IsNullOrWhiteSpace(storedPath)) return null;

            var relative = storedPath.Trim().Replace('\\', '/');

            // Uploads are saved with the "/Web" path base prefix.
            if (relative.StartsWith("/Web/", StringComparison.OrdinalIgnoreCase))
                relative = relative.Substring(5);
            else if (relative.StartsWith("Web/", StringComparison.OrdinalIgnoreCase))
                relative = relative.Substring(4);

            relative = relative.TrimStart('/');

            try
            {
                var full = Path.Combine(env.WebRootPath, relative.Replace('/', Path.DirectorySeparatorChar));
                return File.Exists(full) ? full : null;
            }
            catch
            {
                return null;
            }
        }

        public static byte[] ReadAsset(IWebHostEnvironment env, string storedPath)
        {
            var full = ResolveAssetPath(env, storedPath);
            if (full == null) return null;
            try
            {
                return File.ReadAllBytes(full);
            }
            catch
            {
                return null;
            }
        }
    }
}
