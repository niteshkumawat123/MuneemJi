using MUNEEMJI.Controllers;

namespace MUNEEMJI.Models.ReportModel
{
    public class PartyReportByItemModel
    {
        public int PartyId { get; set; }
        public string PartyName { get; set; }
        public int SaleQuantity { get; set; }
        public decimal SaleAmount { get; set; }
        public int PurchaseQuantity { get; set; }  
        public decimal PurchaseAmount { get;set; }

    }
}
