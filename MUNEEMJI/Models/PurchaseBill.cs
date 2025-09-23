using MUNEEMJI.Controllers;
using MUNEEMJI.Models.Setting;
using System.ComponentModel.DataAnnotations;

namespace MUNEEMJI.Models
{
    public class PurchaseBill
    {
        public int Id { get; set; }
        public string BillNumber { get; set; } = string.Empty;
        public DateTime BillDate { get; set; } = DateTime.Now;
        public string StateOfSupply { get; set; } = string.Empty;
        public string PhoneNo { get; set; } = string.Empty;
        public string PONo { get; set; } = string.Empty;
        public DateTime? PODate { get; set; } = DateTime.Now;
        public string EWayBillNo { get; set; } = string.Empty;
        public string TransportName { get; set; } = string.Empty;
        public string DeliveryLocation { get; set; } = string.Empty;
        public string VehicleNumber { get; set; } = string.Empty;
        public DateTime? DeliveryDate { get; set; }
        public string PaymentType { get; set; } = "Cash";
        public string Description { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string DocumentPath { get; set; } = string.Empty;
        public bool RoundOff { get; set; } = true;
        public decimal RoundOffValue { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public decimal paidReciveamount { get; set; }
        public int PartyId { get; set; }
        public string PartyName { get; set; }
        public int orderstatusid { get; set; }
        public DateTime DueDate { get; set; } = DateTime.Now;
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string ChallanNo { get; set; }
        public DateTime Challandate { get; set; } = DateTime.Now;
        public int PaymentTermId { get; set; }
        public List<PurchaseBillItem> BillItems { get; set; } = new List<PurchaseBillItem>();
        public TransactionSettingsViewModel transactionSettings { get; set; }
        public ItemSettingsViewModel itemSettings { get; set; }

        // New properties based on table schema
        public int StateId { get; set; }
        public DateTime? InvoiceDate { get; set; } = DateTime.UtcNow;
        public TimeSpan? Time { get; set; }
        public string Field5 { get; set; } = string.Empty;
        public string Field6 { get; set; } = string.Empty;
        public IFormFile? DocumentFile { get; set; } 
        public IFormFile? imageFile { get; set; }

        public int NoOfCopi { get; set; } = 1;
        public decimal DiscountPercent { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public decimal TaxPercentage { get; set; } = 0;
        public decimal TaxAmount { get; set; } = 0;
        public decimal ShippingAmount { get; set; } = 0;
        public decimal PackingAmount { get; set; } = 0;
        public decimal AdjustmentAmount { get; set; } = 0;
        public TCSTDSEnum TCSTDSType { get; set; } = TCSTDSEnum.TCS; 
        public decimal TdsTcsPercentage { get; set; } = 0;
        public decimal TdsTcsAmount { get; set; } = 0;
        public bool IsRoundOff { get; set; } = false;
        public decimal FinalAmount { get; set; } = 0;
        public bool IsCredit { get; set; } = false;
        public string BillingName { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public int? InvoiceNumber { get; set; }
        public bool IsReceive { get; set; }

        // use for Debit note receive no
        public decimal ReturnNo { get; set; }
        // show on debi note list 
        public string CategoryName { get; set; }
        public bool IsDeleteImage { get; set; }
    }
    public class PurchaseBillItem
    {
        public int Id { get; set; } = 0;
        public int BillId { get; set; } = 0;
        public string? Item { get; set; } = string.Empty;
        public int ItemId { get; set; } = 0;
        public string serialno { get; set; } = string.Empty;
        public string batchno { get; set; } = string.Empty;
        public string modelno { get; set; } = string.Empty;
        public string expirydate { get; set; } = string.Empty;
        public string mfgdate { get; set; } = string.Empty;
        public int categoryid { get; set; } = 0;
        public decimal Quantity { get; set; } = 0;
        public string Unit { get; set; } = "NONE";
        public decimal PricePerUnit { get; set; } = 0;
        public decimal DiscountPercentage { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;
        public string Tax { get; set; } = "Select";
        public decimal TaxAmount { get; set; } = 0;
        public decimal Amount { get; set; } = 0;
        public int CategoryId { get; set; } = 0;
        public string ItemCode { get; set; } = string.Empty;
        public string HSNCode { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? Count { get; set; } = 0;
        public string BatchNumber { get; set; } = string.Empty;
        public string ModelNumber { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; } = null;
        public DateTime? ManufacturingDate { get; set; } = null;
        public decimal? MRP { get; set; } = 0;
        public string Size { get; set; } = string.Empty;
        public decimal? FreeQuantity { get; set; } = 0;
        public decimal? AddCessAmount { get; set; } = 0;
        public decimal? TotalAmount { get; set; } = 0;
        public PurchaseBill Bill { get; set; } = new PurchaseBill();

        // New properties based on table schema
        public int TradeDocumentsId { get; set; } = 0;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public decimal TaxPercentage { get; set; } = 0;
    }

    public class PurchaseBillViewModel
    {
        public PurchaseBill Bill { get; set; } = new PurchaseBill();
        public List<string> UnitOptions { get; set; } = new List<string> { "NONE", "KG", "PIECE", "LITER", "METER" };
        public List<string> TaxOptions { get; set; } = new List<string> { "Select", "0%", "5%", "12%", "18%", "28%" };
        public List<string> PaymentTypes { get; set; } = new List<string> { "Cash", "Credit", "Debit Card", "UPI", "Net Banking" };
        public List<string> StateOptions { get; set; } = new List<string>
        {
            "Select", "Andhra Pradesh", "Arunachal Pradesh", "Assam", "Bihar", "Chhattisgarh",
            "Goa", "Gujarat", "Haryana", "Himachal Pradesh", "Jharkhand", "Karnataka",
            "Kerala", "Madhya Pradesh", "Maharashtra", "Manipur", "Meghalaya", "Mizoram",
            "Nagaland", "Odisha", "Punjab", "Rajasthan", "Sikkim", "Tamil Nadu",
            "Telangana", "Tripura", "Uttar Pradesh", "Uttarakhand", "West Bengal"
        };
        public List<BillItem> DropDownItem { get; set; }
        public List<CategoryDropdownModel> DropDownCategory { get; set; }
        public int ViewTypeId { get; set; }
        public bool ItemCategory { get; set; }
        public bool ItemCode { get; set; }
        public bool HsnSacCode { get; set; }
        public bool Description { get; set; }
        public bool ItemWiseDiscount { get; set; }
    }
    public enum TradeDocumentTypes
    {
        PurchaseOrder = 1,
        SalesOrder = 2,
        DeliveryChallan = 3,
        PurchaseChallan = 4,
        SalesChallan = 5,
        DebitNote = 6,
        CreditNote = 7,
        Estimation=8,
        PaymentIn =9,
        PaymentOut = 10
    }

    public enum TradeDocumentStatusEnum
    {
        OrderOverdue=1,
        OrderCompleted=2,
        open = 3,
        Closed = 4

    }
    public enum ViewTypeEnum
    { 
      Create=0,
      View=1,
      Edit=2,
      Delete=3
    
    }

    public enum TCSTDSEnum
    {
        TCS = 1,
        TDS = 2
    }


}
