namespace MUNEEMJI.Models.ReportModel
{
    public class DayBookReportModel
    {
        public int PartyId { get; set; }
        public int tradedocumenttypesid { get;set; }
        public decimal FinalAmount { get; set; }
        public decimal Total { get; set; }
        public decimal MoneyIn { get; set; }
        public decimal MoneyOut { get; set; }
        public string PartyName { get; set; }
        public string invoicenumber { get; set; }
        public string TradeDocumentType { get;set; }
    }
}
