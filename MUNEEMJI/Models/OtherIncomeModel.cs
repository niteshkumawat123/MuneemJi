namespace MUNEEMJI.Models
{
    public class OtherIncomeModel
    {
        public IncomeEntry OtherIncomeView { get; set; }
        public IncomeEntry SelectedItem { get; set; }
        public List<OtherIncomeCategory> Categories { get; set; }
    }
    public class OtherIncomeCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class IncomeEntry
    {
        public string IncomeCategory { get; set; }
        public int IncomeCategoryId { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Now;
        public decimal RoundOff { get; set; }
        public bool IsRoundOff { get; set; }
        public decimal Total { get; set; }
        public string PaymentType { get; set; }
        public string Description { get; set; }
        public IFormFile ImageUrl { get; set; }
        public string BaseImageUrl { get; set; }
        public List<IncomeEntryItem> Items { get; set; } = new();
    }

    public class IncomeEntryItem
    {
        public string ItemName { get; set; }
        public int Qty { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal Amount { get; set;  }
    }
}
