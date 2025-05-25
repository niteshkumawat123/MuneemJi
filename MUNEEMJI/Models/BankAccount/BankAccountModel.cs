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
}
