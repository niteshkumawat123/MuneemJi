namespace MUNEEMJI.Models
{
    public class PurchaseItemModel
    {
        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public int ProductId { get; set; }
    }
    public class PurchaseHeaderModel
    {
        public int Id { get; set; }
        public string PaymentType { get; set; }
        public bool RoundOff { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalTax { get; set; }
        public string Description { get; set; }
        public IFormFile BillFile { get; set; }      // File upload for bill/invoice
        public List<IFormFile> ImageFiles { get; set; }
        public List<IFormFile> DocumentFiles { get; set; }
        public List<PurchaseItemModel> Items { get; set; }
        public int CustomerId { get; set; }     // selected customer
        public string BillNumber { get; set; }  // new bill number field
        public DateTime BillDate { get; set; }  // new bill date field
        public string StateOfSupply { get; set; } // new state dropdown field
        public string PhoneNo { get; set; } // new state dropdown field
        public int SupplierId { get; set; } // new state dropdown field
        public List<IFormFile> BillDocument { get; set; }
    }

    public class PurchaseFileModel
    {
        public int Id { get; set; }
        public int PurchaseId { get; set; }
        public string FileType { get; set; }  // e.g. "Image" or "Document"
        public string FilePath { get; set; }
    }
}
