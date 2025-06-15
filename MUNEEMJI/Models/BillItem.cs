using System.ComponentModel.DataAnnotations;

namespace MUNEEMJI.Models
{
    public class BillItem
    {
        public int Id { get; set; }

        [Required]
        public string ItemType { get; set; } = "Product"; // Product or Service

        // Basic Information
        [Required]
        public string ItemName { get; set; } = string.Empty;
        public string? ItemHsn { get; set; }
        public string? ItemCode { get; set; }
        public string? Category { get; set; }
        public string? Unit { get; set; }
        public string? ItemImageUrl { get; set; }

        // Pricing Information
        public decimal? SalePrice { get; set; }
        public string SalePriceTaxType { get; set; } = "Without Tax";
        public decimal? DiscountOnSalePrice { get; set; }
        public string DiscountType { get; set; } = "Percentage";
        public decimal? PurchasePrice { get; set; }
        public string PurchasePriceTaxType { get; set; } = "Without Tax";
        public string TaxRate { get; set; } = "None";
        public decimal? WholesalePrice { get; set; }

        // Stock Information (for Products)
        public int OpeningQuantity { get; set; }
        public decimal? AtPrice { get; set; }
        public DateTime? AsOfDate { get; set; }
        public string? Location { get; set; }
        public int MinStockToMaintain { get; set; }

        // Online Store Information
        public decimal? OnlineStorePrice { get; set; }
        public string? Description { get; set; }

        // Manufacturing Information (for Products)
        public string? RawMaterials { get; set; } // JSON string
        public string? AdditionalCosts { get; set; } // JSON string
        public decimal TotalEstimatedCost { get; set; }

        // Service specific fields
        public string? ServiceName { get; set; }
        public string? ServiceHsn { get; set; }
        public string? ServiceCode { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class RawMaterial
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal PurchasePricePerUnit { get; set; }
        public decimal EstimatedCost { get; set; }
    }

    public class AdditionalCost
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
    }

    public class BillItemViewModel
    {
        public BillItem BillItem { get; set; } = new BillItem();
        public List<RawMaterial> RawMaterials { get; set; } = new List<RawMaterial>();
        public List<AdditionalCost> AdditionalCosts { get; set; } = new List<AdditionalCost>();

        // For dropdown lists
        public List<string> Categories { get; set; } = new List<string>();
        public List<string> Units { get; set; } = new List<string>();
        public List<string> TaxRates { get; set; } = new List<string>();
    }
}
