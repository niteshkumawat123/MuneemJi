using System.ComponentModel.DataAnnotations;

namespace MUNEEMJI.Models
{
    public class CashAdjustmentModel
    {

        public int Id { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public decimal Amount { get; set; }
    }

    public class CashAdjustmentViewModel
    {
        public decimal CurrentCashInHand { get; set; }
        public string AdjustmentType { get; set; } = "Add Cash";
        public decimal AdjustmentAmount { get; set; }
        public DateTime AdjustmentDate { get; set; } = DateTime.Now;
        public string Description { get; set; } = string.Empty;
    }

    public class CashAdjustmenttransactionsViewModel
    {
        public decimal CashInHand { get; set; }
        public List<CashAdjustmentModel> Transactions { get; set; } = new List<CashAdjustmentModel>();
        public string SearchTerm { get; set; } = string.Empty;
    }

}
