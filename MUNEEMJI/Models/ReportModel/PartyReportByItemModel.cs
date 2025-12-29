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

    public class LoanReportViewModel
    { 
      public int AccountID { get; set; }
      public string AccountName { get; set; }
      public List<LoanTransectionReprotModel>LoanTransections { get; set; }
    
    }

    public class LoanTransectionReprotModel
    {

        public int loanaccountid { get; set; }
        public DateTime? Date { get; set; }
        public string Type { get; set; }
        public decimal? Amount { get; set; }
        public decimal? EndingBalance { get; set; }
    }
}
