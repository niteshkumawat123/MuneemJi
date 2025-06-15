using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MUNEEMJI.Models
{
    public class ItemModel
    {
        public class Category
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public int ItemCount { get; set; } // Number of items in this category

        }
        public class Unit
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string FullName { get; set; }
            public string ShortName { get; set; }
        }
        public class Tax
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Rate { get; set; }
        }
        public class Item
        {
            public int Id { get; set; }

            // Item details
            public string Name { get; set; }
            public string HSN { get; set; }
            public int CategoryId { get; set; }
            public string ItemCode { get; set; }
            public int UnitId { get; set; }
            public string ImagePath { get; set; }

            public int TaxType { get; set; }

            public int DiscountType { get; set; }

            public string ServiceName { get; set; }

            public string ServiceHSN { get; set; }

            public int Category { get; set; }

            public string ServiceCode { get; set; }
            // Pricing details
            public decimal SalePrice { get; set; }
            public decimal Discount { get; set; }
            public decimal PurchasePrice { get; set; }
            public int TaxId { get; set; }

            // Stock details
            public decimal OpeningQuantity { get; set; }
            public decimal AtPrice { get; set; }
            public decimal MinStock { get; set; }
            public string Location { get; set; }
            public DateTime EntryDate { get; set; }

            // File upload (image) - not stored in DB
            public IFormFile ItemImage { get; set; }
        }
        public class WholesalePrice
        {
            public int Id { get; set; }
            public int ItemId { get; set; }
            public decimal Price { get; set; }
            public int MinQuantity { get; set; }
        }
    }
}
