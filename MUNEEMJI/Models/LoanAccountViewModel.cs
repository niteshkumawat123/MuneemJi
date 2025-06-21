using System.ComponentModel.DataAnnotations;

namespace MUNEEMJI.Models
{
    public class LoanAccountViewModel
    {
        public string AccountName { get; set; } = string.Empty;
        public string LendingBank { get; set; } = string.Empty;
        public string Agency { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public decimal BalanceAmount { get; set; }
        public List<LoanTransactionViewModel> Transactions { get; set; } = new List<LoanTransactionViewModel>();
    }

    public class LoanTransactionViewModel
    {
        public string Type { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Principal { get; set; }
        public decimal InterestAndOtherCharges { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class LoanPaymentViewModel
    {
        [Display(Name = "Principal Amount")]
        public decimal PrincipalAmount { get; set; }

        [Display(Name = "Interest Amount")]
        public decimal InterestAmount { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Paid from")]
        public string PaidFrom { get; set; } = "Cash";
    }

    public class LoanIncreaseViewModel
    {
        [Display(Name = "Increase Loan By")]
        [Required(ErrorMessage = "Please enter the loan increase amount")]
        public decimal IncreaseAmount { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Loan received In")]
        public string LoanReceivedIn { get; set; } = "Cash";
    }

    public class LoanChargesViewModel
    {
        [Display(Name = "Date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Transaction Type Name")]
        [Required(ErrorMessage = "Please enter transaction type name")]
        public string TransactionTypeName { get; set; } = string.Empty;

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Please enter amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Paid From")]
        public string PaidFrom { get; set; } = "Cash";
    }

    public class LoanDashboardViewModel
    {
        public LoanAccountViewModel Account { get; set; } = new LoanAccountViewModel();
        public List<string> PaymentFromOptions { get; set; } = new List<string> { "Cash", "Bank", "Other" };
    }
}
