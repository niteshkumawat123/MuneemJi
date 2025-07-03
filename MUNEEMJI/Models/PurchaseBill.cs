using System.ComponentModel.DataAnnotations;

namespace MUNEEMJI.Models
{
    public class PurchaseBill
    {
        public int Id { get; set; }

        [Display(Name = "Bill Number")]
        public string BillNumber { get; set; } = string.Empty;

        [Display(Name = "Bill Date")]
        public DateTime BillDate { get; set; } = DateTime.Now;

        [Display(Name = "State of Supply")]
        public string StateOfSupply { get; set; } = string.Empty;

        [Display(Name = "Phone No.")]
        public string PhoneNo { get; set; } = string.Empty;

        [Display(Name = "P.O No.")]
        public string PONo { get; set; } = string.Empty;

        [Display(Name = "P.O Date")]
        public DateTime? PODate { get; set; }

        [Display(Name = "E-Way Bill No.")]
        public string EWayBillNo { get; set; } = string.Empty;

        [Display(Name = "Transport Name")]
        public string TransportName { get; set; } = string.Empty;

        [Display(Name = "Delivery Location")]
        public string DeliveryLocation { get; set; } = string.Empty;

        [Display(Name = "Vehicle Number")]
        public string VehicleNumber { get; set; } = string.Empty;

        [Display(Name = "Delivery Date")]
        public DateTime? DeliveryDate { get; set; }

        [Display(Name = "Payment Type")]
        public string PaymentType { get; set; } = "Cash";

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Image Path")]
        public string ImagePath { get; set; } = string.Empty;

        [Display(Name = "Round Off")]
        public bool RoundOff { get; set; } = true;

        public decimal RoundOffValue { get; set; }
        public decimal Total { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public decimal paidReciveamount { get; set; }

        public List<PurchaseBillItem> BillItems { get; set; } = new List<PurchaseBillItem>();

    }
    public class PurchaseBillItem
    {
        public int Id { get; set; }
        public int BillId { get; set; }

        [Display(Name = "Item")]
        public string? Item { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string serialno { get; set; }
        public string batchno { get; set; }
        public string modelno { get; set; }
        public string expirydate { get; set; }
        public string mfgdate { get; set; }
        public int categoryid { get; set; }

        [Display(Name = "QTY")]
        public decimal Quantity { get; set; }

        [Display(Name = "Unit")]
        public string Unit { get; set; } = "NONE";

        [Display(Name = "Price/Unit")]
        public decimal PricePerUnit { get; set; }

        [Display(Name = "Discount %")]
        public decimal DiscountPercentage { get; set; }

        [Display(Name = "Discount Amount")]
        public decimal DiscountAmount { get; set; }

        [Display(Name = "Tax")]
        public string Tax { get; set; } = "Select";

        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }

        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        public PurchaseBill Bill { get; set; }
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
    }
    public enum TradeDocumentTypes
    {
        PurchaseOrder = 1,
        SalesOrder = 2,
        DeliveryChallan = 3,
        PurchaseChallan = 4,
        SalesChallan = 5,
        PurchaseDebitNote = 6,
        SalesDebitNote = 7,
        CreditNote = 8
    }
}
