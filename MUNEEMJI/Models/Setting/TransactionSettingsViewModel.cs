namespace MUNEEMJI.Models.Setting
{
    public class TransactionSettingsViewModel
    {
        public int FirmId { get; set; }
        public bool InvoiceBillNo { get; set; }
        public bool AddTimeOnTransactions { get; set; }
        public bool PrintTimeOnInvoices { get; set; }
        public bool CashSaleByDefault { get; set; }
        public bool BillingNameOfParties { get; set; }
        public bool CustomerPODetails { get; set; }
        public bool EwayBillNo { get; set; }
        public bool QuickEntry { get; set; }
        public bool DoNotShowInvoicePreview { get; set; }
        public bool EnablePasscode { get; set; }
        public bool DiscountDuringPayments { get; set; }
        public bool LinkPaymentsToInvoices { get; set; }
        public bool DueDatesPaymentTerms { get; set; }
        public bool ShowProfitSaleInvoice { get; set; }
        public bool InclusiveExclusiveTax { get; set; }
        public bool DisplayPurchasePrice { get; set; }
        public bool ShowLast5SalePrice { get; set; }
        public bool FreeItemQuantity { get; set; }
        public bool CountEnabled { get; set; }
        public bool TransactionWiseTax { get; set; }
        public bool TransactionWiseDiscount { get; set; }
        public bool RoundOffTotal { get; set; }
        public string RoundOffType { get; set; } = "Nearest";
        public decimal RoundOffValue { get; set; } = 1;
        public string BillingType { get; set; } = "Full Sale";
        public string SalePrefix { get; set; } = "None";
        public string CreditNotePrefix { get; set; } = "None";
        public string SaleOrderPrefix { get; set; } = "None";
        public string PurchaseOrderPrefix { get; set; } = "None";
        public string EstimatePrefix { get; set; } = "None";
        public string ProformaInvoicePrefix { get; set; } = "None";
        public string DeliveryChallanPrefix { get; set; } = "None";
        public string PaymentInPrefix { get; set; } = "None";
    }
}
