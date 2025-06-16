using System.ComponentModel.DataAnnotations;

namespace MUNEEMJI.Models.Setting
{
    public class GeneralSettingsViewModel
    {
        // Application Settings
        public bool EnablePasscode { get; set; }

        [Display(Name = "Business Currency")]
        public string BusinessCurrency { get; set; } = "₹";

        [Display(Name = "Amount (Decimal Places)")]
        public int DecimalPlaces { get; set; } = 2;

        public bool GSTINNumber { get; set; } = true;

        [Display(Name = "Stop Sale on Negative Stock")]
        public bool StopSaleOnNegativeStock { get; set; }

        // Multi Firm Settings
        public bool MultiFirm { get; set; }
        public string SelectedFirmName { get; set; } = "hellovezel";
        public bool IsDefaultFirm { get; set; } = true;

        // More Transactions
        [Display(Name = "Estimate/Quotation")]
        public bool EstimateQuotation { get; set; } = true;

        [Display(Name = "Sale/Purchase Order")]
        public bool SalePurchaseOrder { get; set; } = true;

        [Display(Name = "Other Income")]
        public bool OtherIncome { get; set; } = true;

        [Display(Name = "Fixed Assets (FA)")]
        public bool FixedAssets { get; set; }

        [Display(Name = "Delivery Challan")]
        public bool DeliveryChallan { get; set; } = true;

        [Display(Name = "Goods return on Delivery Challan")]
        public bool GoodsReturnOnDeliveryChallan { get; set; } = true;

        [Display(Name = "Print amount in Delivery Challan")]
        public bool PrintAmountInDeliveryChallan { get; set; }

        // Stock Transfer Between Godowns
        [Display(Name = "Godown management & Stock transfer")]
        public bool GodownManagementStockTransfer { get; set; } = true;

        // Backup & History
        [Display(Name = "Auto Backup")]
        public bool AutoBackup { get; set; }

        [Display(Name = "Last Backup")]
        public string LastBackup { get; set; } = "14/06/2025 | 11:06 AM";

        [Display(Name = "Audit Trail")]
        public bool AuditTrail { get; set; } = true;

        // Customize Your View
        [Display(Name = "Screen Zoom/Scale")]
        public string ScreenZoom { get; set; } = "100%";

        public List<string> AvailableZoomLevels { get; set; } = new List<string>
        {
            "70%", "80%", "90%", "100%", "110%", "115%", "120%", "130%"
        };
    }
}
