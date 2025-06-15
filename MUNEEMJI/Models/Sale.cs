using System.ComponentModel.DataAnnotations;

namespace MUNEEMJI.Models
{
    public class Sale
    {
        public int SaleId { get; set; }

        [Required]
        public string InvoiceNumber { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime InvoiceDate { get; set; }

        // True = Cash, False = Credit
        public bool IsCash { get; set; }

        public int? StateOfSupplyId { get; set; }

        public string BillingName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string BillingAddress { get; set; }
        [Required]
        public string ShippingAddress { get; set; }

        public decimal Total { get; set; }
        public Boolean RoundOff { get; set; }
        public decimal GrandTotal { get; set; }

        // List of line items
        public List<SaleItem> Items { get; set; } = new List<SaleItem>();

    }
    public class SaleItem
    {
        public int SaleItemId { get; set; }
        public int SaleId { get; set; }

        [Required]
        public int ItemId { get; set; }
        [Required]
        public int UnitId { get; set; }

        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public decimal PricePerUnit { get; set; }

        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }

        public int? TaxId { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }

    // For dropdown lists
    public class ItemDropdownModel { public int ItemId { get; set; } public string Name { get; set; } }
    public class UnitModel { public int UnitId { get; set; } public string Name { get; set; } }
    public class TaxModel { public int TaxId { get; set; } public string Name { get; set; } public decimal Rate { get; set; } }
    public class StateModel { public int StateId { get; set; } public string Name { get; set; } }

}
