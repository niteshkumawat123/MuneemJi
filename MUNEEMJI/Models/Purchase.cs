namespace MUNEEMJI.Models
{
    public class Purchase
    {
        public int PurchaseId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string PONumber { get; set; }
        public DateTime? PODate { get; set; }
        public string EWayBillNumber { get; set; }
        public int? StateOfSupplyId { get; set; }
        public string PaymentType { get; set; }
        public string TransportName { get; set; }
        public string DeliveryLocation { get; set; }
        public string VehicleNumber { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public bool RoundOff { get; set; }
        public decimal RoundOffAmount { get; set; }
        public decimal Total { get; set; }
        public decimal GrandTotal { get; set; }

        public List<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
    }
    public class PurchaseItem
    {
        public int PurchaseItemId { get; set; }
        public int PurchaseId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public int UnitId { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public int TaxId { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Amount { get; set; }
    }
}
