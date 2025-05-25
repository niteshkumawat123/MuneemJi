namespace MUNEEMJI.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public int Quantity { get; set; }
        public int? CategoryId { get; set; }
    }
}
