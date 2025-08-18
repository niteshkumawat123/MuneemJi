namespace MUNEEMJI.Models.Setting
{
    public class ItemSettingsViewModel
    {
        // Item Settings
        public bool EnableItem { get; set; } = false;

        public string WhatDoYouSell { get; set; } = "Product/Service";

        public bool BarcodeScanning { get; set; } = false;

        public bool DirectBarcodeScanning { get; set; } = false;

        public bool StockMaintenance { get; set; } = false;

        public bool Manufacturing { get; set; } = false;

        public bool ShowLowStockDialog { get; set; } = false;

        public bool ItemsUnit { get; set; } = false;

        public string DefaultUnit { get; set; } = "";

        public bool ItemCategory { get; set; } = false;

        public bool PartyWiseItemRate { get; set; } = false;

        public bool Description { get; set; } = false;

        public bool ItemWiseTax { get; set; } = false;

        public bool ItemWiseDiscount { get; set; } = false;

        public bool UpdateSalePriceFromTransaction { get; set; } = false;

        // MRP/Price Settings
        public bool MrpEnabled { get; set; } = false;

        public bool CalculateSalePriceFromMrp { get; set; } = false;

        public bool UseMrpForBatchTracking { get; set; } = false;

        // Serial No. Tracking
        public bool SerialNoTracking { get; set; } = false;

        // Batch Tracking
        public bool BatchNoEnabled { get; set; } = false;

        public bool ExpDateEnabled { get; set; } = false;

        public bool MfgDateEnabled { get; set; } = false;

        public bool ModelNoEnabled { get; set; } = false;

        public bool SizeEnabled { get; set; } = false;

       
        public bool ItemCode { get; set; } = false;
        public bool HsnSacCode { get; set; } = false;
        
    }
}
