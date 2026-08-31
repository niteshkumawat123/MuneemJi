namespace MUNEEMJI.PdfServices.Quest
{
    /// <summary>Which composer renders a theme.</summary>
    public enum ThemeFamily
    {
        /// <summary>Bordered grid, Tally style.</summary>
        Tally,
        /// <summary>Open layout, accent rules, no outer box (GST Theme 1, Theme 1-4).</summary>
        GstPlain,
        /// <summary>Boxed layout with filled caption bars (GST Theme 2-6).</summary>
        GstBoxed,
        /// <summary>Landscape bordered layouts.</summary>
        Landscape,
        /// <summary>Double Divine - navy and lavender banner.</summary>
        DoubleDivine,
        /// <summary>French Elite - filled title block.</summary>
        FrenchElite
    }

    /// <summary>How the tax breakdown is presented under the item grid.</summary>
    public enum TaxBlockKind
    {
        /// <summary>Tax type | Taxable amount | Rate | Tax amount.</summary>
        TaxType,
        /// <summary>HSN/SAC grid with CGST and SGST sub-columns.</summary>
        HsnGrid,
        /// <summary>Compact two-row "Tax details" strip.</summary>
        TaxDetails,
        /// <summary>No separate block - rates are folded into the totals list.</summary>
        FoldedIntoTotals
    }

    /// <summary>Where the logo sits relative to the company text.</summary>
    public enum LogoSide { Left, Right }

    /// <summary>
    /// Per-theme layout switches. Everything that differs between the shipped
    /// themes is expressed here so one composer can render a whole family
    /// faithfully instead of the layouts being copy-pasted per theme.
    /// </summary>
    public class QuestThemeStyle
    {
        public string LayoutKey { get; set; }
        public ThemeFamily Family { get; set; }

        // ---- Header ----
        public LogoSide Logo { get; set; } = LogoSide.Left;
        /// <summary>Company text is right-aligned (logo then sits on the left).</summary>
        public bool CompanyTextRight { get; set; }
        /// <summary>Company block sits on a filled accent banner with white text.</summary>
        public bool BannerHeader { get; set; }
        /// <summary>Logo is boxed in a white card - only used with a banner.</summary>
        public bool LogoCard { get; set; }
        /// <summary>Rule drawn under the company block.</summary>
        public bool RuleUnderHeader { get; set; } = true;
        /// <summary>Header rule uses the accent colour rather than grey.</summary>
        public bool AccentRules { get; set; } = true;

        // ---- Title ----
        public bool RuleUnderTitle { get; set; }

        // ---- Party band ----
        /// <summary>Bill To / Shipping To / Invoice Details sit on filled caption bars.</summary>
        public bool FilledPartyBars { get; set; }
        /// <summary>Party band is a bordered three-column grid.</summary>
        public bool BoxedPartyBand { get; set; }
        /// <summary>GST Theme 3 prints a label/value grid of shipping fields instead.</summary>
        public bool InvoiceDetailGrid { get; set; }
        /// <summary>Theme 4 runs the party name onto the caption line.</summary>
        public bool InlinePartyName { get; set; }

        // ---- Item table ----
        public bool FilledItemHeader { get; set; } = true;
        /// <summary>CGST and SGST get their own columns instead of a single GST column.</summary>
        public bool SplitGstColumns { get; set; }
        public bool BorderedItemTable { get; set; }

        // ---- Lower half ----
        public TaxBlockKind TaxBlock { get; set; } = TaxBlockKind.TaxType;
        /// <summary>Captions in the lower half sit on filled bars.</summary>
        public bool FilledLowerBars { get; set; }
        /// <summary>Words / Description come before the tax block (GST Theme 4).</summary>
        public bool WordsBeforeTaxBlock { get; set; }
        /// <summary>Whole document is wrapped in a border.</summary>
        public bool BoxedOuter { get; set; }
        /// <summary>Totals list is banded rather than ruled.</summary>
        public bool FilledTotalsCaption { get; set; }

        // =================================================================
        //  Registry
        // =================================================================
        private static readonly Dictionary<string, QuestThemeStyle> Registry =
            new Dictionary<string, QuestThemeStyle>(StringComparer.OrdinalIgnoreCase)
        {
            // ---------- Tally ----------
            ["tally"] = new QuestThemeStyle
            {
                LayoutKey = "tally",
                Family = ThemeFamily.Tally
            },

            // ---------- GST Theme 1: open layout, company left, logo right ----------
            ["gst1"] = new QuestThemeStyle
            {
                LayoutKey = "gst1",
                Family = ThemeFamily.GstPlain,
                Logo = LogoSide.Right,
                CompanyTextRight = false,
                RuleUnderHeader = true,
                RuleUnderTitle = true,
                FilledItemHeader = true,
                TaxBlock = TaxBlockKind.FoldedIntoTotals,
                FilledTotalsCaption = false
            },

            // ---------- GST Theme 2: boxed, filled bars, split GST columns ----------
            ["gst2"] = new QuestThemeStyle
            {
                LayoutKey = "gst2",
                Family = ThemeFamily.GstBoxed,
                Logo = LogoSide.Left,
                CompanyTextRight = true,
                FilledPartyBars = true,
                BoxedPartyBand = true,
                SplitGstColumns = true,
                BorderedItemTable = true,
                TaxBlock = TaxBlockKind.TaxType,
                FilledLowerBars = true,
                BoxedOuter = true
            },

            // ---------- GST Theme 3: boxed, invoice detail grid, HSN grid at foot ----------
            ["gst3"] = new QuestThemeStyle
            {
                LayoutKey = "gst3",
                Family = ThemeFamily.GstBoxed,
                Logo = LogoSide.Left,
                CompanyTextRight = false,
                InvoiceDetailGrid = true,
                FilledPartyBars = false,
                BoxedPartyBand = true,
                FilledItemHeader = false,
                BorderedItemTable = true,
                TaxBlock = TaxBlockKind.HsnGrid,
                WordsBeforeTaxBlock = true,
                FilledLowerBars = false,
                BoxedOuter = true
            },

            // ---------- GST Theme 4: boxed, words/amounts before the tax table ----------
            ["gst4"] = new QuestThemeStyle
            {
                LayoutKey = "gst4",
                Family = ThemeFamily.GstBoxed,
                Logo = LogoSide.Left,
                CompanyTextRight = true,
                FilledPartyBars = true,
                BoxedPartyBand = true,
                BorderedItemTable = true,
                TaxBlock = TaxBlockKind.TaxType,
                WordsBeforeTaxBlock = true,
                FilledLowerBars = true,
                BoxedOuter = true
            },

            // ---------- GST Theme 5: boxed, tax table beside amounts ----------
            ["gst5"] = new QuestThemeStyle
            {
                LayoutKey = "gst5",
                Family = ThemeFamily.GstBoxed,
                Logo = LogoSide.Left,
                CompanyTextRight = true,
                FilledPartyBars = true,
                BoxedPartyBand = true,
                BorderedItemTable = true,
                TaxBlock = TaxBlockKind.TaxType,
                FilledLowerBars = true,
                BoxedOuter = true
            },

            // ---------- GST Theme 6: boxed, split GST columns, compact tax strip ----------
            ["gst6"] = new QuestThemeStyle
            {
                LayoutKey = "gst6",
                Family = ThemeFamily.GstBoxed,
                Logo = LogoSide.Left,
                CompanyTextRight = true,
                FilledPartyBars = true,
                BoxedPartyBand = true,
                SplitGstColumns = true,
                BorderedItemTable = true,
                TaxBlock = TaxBlockKind.TaxDetails,
                FilledLowerBars = true,
                BoxedOuter = true
            },

            // ---------- Theme 1: open, company right, accent rules ----------
            ["theme1"] = new QuestThemeStyle
            {
                LayoutKey = "theme1",
                Family = ThemeFamily.GstPlain,
                Logo = LogoSide.Left,
                CompanyTextRight = true,
                AccentRules = true,
                RuleUnderHeader = true,
                RuleUnderTitle = true,
                TaxBlock = TaxBlockKind.TaxType,
                FilledLowerBars = true,
                FilledTotalsCaption = true
            },

            // ---------- Theme 2: same skeleton, neutral rules, no rule under title ----------
            ["theme2"] = new QuestThemeStyle
            {
                LayoutKey = "theme2",
                Family = ThemeFamily.GstPlain,
                Logo = LogoSide.Left,
                CompanyTextRight = true,
                AccentRules = false,
                RuleUnderHeader = true,
                RuleUnderTitle = false,
                TaxBlock = TaxBlockKind.TaxType,
                FilledLowerBars = true,
                FilledTotalsCaption = true
            },

            // ---------- Theme 3: filled banner header, logo in a white card ----------
            ["theme3"] = new QuestThemeStyle
            {
                LayoutKey = "theme3",
                Family = ThemeFamily.GstPlain,
                Logo = LogoSide.Left,
                CompanyTextRight = true,
                BannerHeader = true,
                LogoCard = true,
                RuleUnderHeader = false,
                FilledPartyBars = true,
                TaxBlock = TaxBlockKind.TaxType,
                FilledLowerBars = true,
                FilledTotalsCaption = true
            },

            // ---------- Theme 4: company left, logo right, inline party name ----------
            ["theme4"] = new QuestThemeStyle
            {
                LayoutKey = "theme4",
                Family = ThemeFamily.GstPlain,
                Logo = LogoSide.Right,
                CompanyTextRight = false,
                AccentRules = false,
                RuleUnderHeader = true,
                RuleUnderTitle = false,
                InlinePartyName = true,
                TaxBlock = TaxBlockKind.TaxType,
                FilledLowerBars = false,
                FilledTotalsCaption = false
            },

            // ---------- Landscape ----------
            ["landscape1"] = new QuestThemeStyle
            {
                LayoutKey = "landscape1",
                Family = ThemeFamily.Landscape,
                Logo = LogoSide.Left,
                TaxBlock = TaxBlockKind.HsnGrid,
                BoxedOuter = true
            },
            ["landscape2"] = new QuestThemeStyle
            {
                LayoutKey = "landscape2",
                Family = ThemeFamily.Landscape,
                Logo = LogoSide.Left,
                TaxBlock = TaxBlockKind.HsnGrid,
                BoxedOuter = true,
                // Totals run down the right of the tax grid instead of as strips.
                WordsBeforeTaxBlock = false,
                FilledLowerBars = false
            },

            // ---------- Designer themes ----------
            ["divine"] = new QuestThemeStyle
            {
                LayoutKey = "divine",
                Family = ThemeFamily.DoubleDivine,
                Logo = LogoSide.Left,
                LogoCard = true,
                SplitGstColumns = false,
                TaxBlock = TaxBlockKind.FoldedIntoTotals
            },
            ["french"] = new QuestThemeStyle
            {
                LayoutKey = "french",
                Family = ThemeFamily.FrenchElite,
                Logo = LogoSide.Right,
                TaxBlock = TaxBlockKind.FoldedIntoTotals
            }
        };

        /// <summary>
        /// Style for a layout key. Unknown keys fall back to the open GST layout
        /// so a newly seeded theme still renders something sensible.
        /// </summary>
        public static QuestThemeStyle For(string layoutKey)
        {
            if (!string.IsNullOrWhiteSpace(layoutKey) && Registry.TryGetValue(layoutKey.Trim(), out var style))
                return style;

            return Registry["theme1"];
        }

        public static bool IsKnown(string layoutKey)
        {
            return !string.IsNullOrWhiteSpace(layoutKey) && Registry.ContainsKey(layoutKey.Trim());
        }
    }
}
