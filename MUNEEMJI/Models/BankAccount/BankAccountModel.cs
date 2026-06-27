using Microsoft.Data.SqlClient.DataClassification;
using System.ComponentModel.DataAnnotations;

namespace MUNEEMJI.Models.BankAccount
{
    public class BankAccountModel
    {
        public int Id { get; set; }
        public string AccountDisplayName { get; set; }
        public decimal? OpeningBalance { get; set; }
        public DateTime? AsOfDate { get; set; }
        public bool PrintUPIQrCode { get; set; }
        public bool PrintBankDetails { get; set; }
        public string AccountNumber { get; set; }
        public string IFSCCode { get; set; }
        public string UPIID { get; set; }
        public string BankName { get; set; }
        public string AccountHolderName { get; set; }
        public int RequestTypeId { get; set; } 
    }
    public class BankAccount
    {
        public int Id { get; set; }
        public string AccountDisplayName { get; set; }
        public string AccountNumber { get; set; }
        public decimal OpeningBalance { get; set; }
        public DateTime AsOfDate { get; set; }
        public string IfscCode { get; set; }
        public string UpiId { get; set; }
    }

    // ViewModel containing list and selected account
    public class BankViewModel
    {
        public List<BankAccount> Accounts { get; set; }
        public BankAccount SelectedAccount { get; set; }
    }

    public class BankCash
    {
        public int Id { get; set; }

      
        public int AdjustTypeId { get; set; }

        public decimal Amount { get; set; }

        public DateTime? AdjustmentDate { get; set; }

        public string Description { get; set; }
        public decimal TotalCash { get; set; }
    }


    public class AdjustCashRequest
    {
        public int Id { get; set; }
        public int AdjustTypeId { get; set; } 
        public decimal Amount { get; set; }

        public DateTime AdjustmentDate { get; set; }

        public string Description { get; set; }
    }

    public class BankTransferRequest
    {
        public string TransactionType { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
    }
}
