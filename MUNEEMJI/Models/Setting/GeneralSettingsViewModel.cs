using System.ComponentModel.DataAnnotations;

namespace MUNEEMJI.Models.Setting
{
    public class GeneralSettingsViewModel
    {
        public int Id { get; set; }
        public bool StopSaleOnNegativeStock { get; set; }
        public bool BlockNewItemsFromTxn { get; set; }
        public bool BlockNewPartiesFromTxn { get; set; }
        public bool GstinNumber { get; set; }
        public bool EstimateQuotation { get; set; }
        public bool ProformaInvoice { get; set; }
        public bool SalePurchaseOrder { get; set; }
        public bool OtherIncome { get; set; }
        public bool FixedAssets { get; set; }
        public bool DeliveryChallan { get; set; }
        public bool GoodsReturnOnDeliveryChallan { get; set; }
        public bool PrintAmountInDeliveryChallan { get; set; }
        public bool MultiFirm { get; set; }
        public bool GodownManagement { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
