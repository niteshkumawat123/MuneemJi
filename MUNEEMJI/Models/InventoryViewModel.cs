using Microsoft.CodeAnalysis;

namespace MUNEEMJI.Models
{
    public class InventoryViewModel
    {
        public ProductInfo Product { get; set; }
        public List<TransactionRecord> Transactions { get; set; }
        public List<string> Godowns { get; set; }
        public string SelectedGodown { get; set; }

        public InventoryViewModel()
        {
            Product = new ProductInfo();
            Transactions = new List<TransactionRecord>();
            Godowns = new List<string>();
        }
    }

    public class ItemViewModel
        {
        public List<BillItem> ItemView { get; set; }
        public BillItem SelectedItem { get; set; }




    }

    public class ProductInfo
    {
        public string Name { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public int StockQuantity { get; set; }
        public decimal StockValue { get; set; }
        public bool IsExcludingTax { get; set; } = true;
    }

    public class TransactionRecord
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string InvoiceRef { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
        public string Status { get; set; }
        public string StatusColor { get; set; }
    }

    public enum TransactionType
    {
        Purchase,
        Sale,
        Return,
        Adjustment
    }

    public enum TransactionStatus
    {
        Partial,
        Complete,
        Pending,
        Cancelled
    }
}
