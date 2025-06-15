using System.ComponentModel.DataAnnotations;

namespace MUNEEMJI.Models
{
    public class StockAdjustmentViewModel
    {
        public string ItemName { get; set; } = "demo";

        [Required]
        public string Godown { get; set; } = "Main Godown";

        [Required]
        public DateTime AdjustmentDate { get; set; } = DateTime.Now;

        public decimal TotalQty { get; set; }
        public string Unit { get; set; } = "Btl";
        public decimal AtPrice { get; set; }
        public string Details { get; set; }
        public bool IsAddStock { get; set; } = true; // true for Add Stock, false for Reduce Stock

        // For dropdown options
        public List<string> GodownOptions { get; set; } = new List<string> { "Main Godown", "Secondary Godown", "Warehouse A" };
        public List<string> UnitOptions { get; set; } = new List<string> { "Btl", "Kg", "Pieces", "Liters" };

    }
}
