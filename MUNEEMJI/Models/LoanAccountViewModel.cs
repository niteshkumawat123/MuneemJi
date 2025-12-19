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
    public class LoanAccountModel
    {
        public int Id { get; set; }

        public string? AccountName { get; set; }
        public string? LenderBank { get; set; }
        public string? AccountNumber { get; set; }
        public string? Description { get; set; }

        public decimal? CurrentBalance { get; set; }
        public DateTime? BalanceAsOf { get; set; }

        public string? LoanReceivedIn { get; set; }

        public decimal? InterestRate { get; set; }
        public int? TermDuration { get; set; }
        public decimal? ProcessingFee { get; set; }
        public string? ProcessingFeePaidFrom { get; set; }
    }

    public class MakePaymentModel
    {
        public int Id { get; set; }
        public int LoanAccountId { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime Date { get; set; }
        public string PaidFrom { get; set; }
    }

    public class TakeMoreLoanModel
    {
        public int Id { get; set; }
        public int LoanAccountId { get; set; }
        public decimal LoanAmount { get; set; }
        public DateTime Date { get; set; }
        public string LoanReceivedIn { get; set; }
        public decimal InterestRate { get; set; }
        public int TermDuration { get; set; }
    }
    public class ChargesOnLoanModel
    {
        public int Id { get; set; }
        public int LoanAccountId { get; set; }
        public string TransactionTypeName { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string LoanReceivedIn { get; set; }
    }
}
