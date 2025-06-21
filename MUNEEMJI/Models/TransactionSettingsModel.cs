using System.ComponentModel.DataAnnotations;

namespace MUNEEMJI.Models
{
    public class TransactionSettingsModel
    {
        // Transaction Header
        [Display(Name = "Invoice/Bill No.")]
        public bool InvoiceBillNo { get; set; } = true;

        [Display(Name = "Add Time on Transactions")]
        public bool AddTimeOnTransactions { get; set; }

        [Display(Name = "Cash Sale by default")]
        public bool CashSaleByDefault { get; set; }

        [Display(Name = "Billing Name of Parties")]
        public bool BillingNameOfParties { get; set; } = true;

        [Display(Name = "Customers P.O. Details on Transactions")]
        public bool CustomersPODetailsOnTransactions { get; set; } = true;

        // Items Table
        [Display(Name = "Inclusive/Exclusive Tax on Rate(Price/Unit)")]
        public bool InclusiveExclusiveTaxOnRate { get; set; } = true;

        [Display(Name = "Display Purchase Price of Items")]
        public bool DisplayPurchasePriceOfItems { get; set; } = true;

        [Display(Name = "Show last 5 Sale Price of Items")]
        public bool ShowLast5SalePriceOfItems { get; set; }

        [Display(Name = "Free Item Quantity")]
        public bool FreeItemQuantity { get; set; }

        [Display(Name = "Count")]
        public bool Count { get; set; }

        // Taxes, Discount & Totals
        [Display(Name = "Transaction wise Tax")]
        public bool TransactionWiseTax { get; set; }

        [Display(Name = "Transaction wise Discount")]
        public bool TransactionWiseDiscount { get; set; }

        [Display(Name = "Round Off Total")]
        public bool RoundOffTotal { get; set; } = true;

        [Display(Name = "Round Off To")]
        public string RoundOffTo { get; set; } = "Nearest";

        [Display(Name = "Round Off Precision")]
        public int RoundOffPrecision { get; set; } = 1;

        // More Transaction Features
        [Display(Name = "E-way bill no")]
        public bool EWayBillNo { get; set; } = true;

        [Display(Name = "Quick Entry")]
        public bool QuickEntry { get; set; }

        [Display(Name = "Do not Show Invoice Preview")]
        public bool DoNotShowInvoicePreview { get; set; }

        [Display(Name = "Enable Passcode for transaction edit/delete")]
        public bool EnablePasscodeForTransactionEditDelete { get; set; }

        [Display(Name = "Discount During Payments")]
        public bool DiscountDuringPayments { get; set; }

        [Display(Name = "Link Payments to Invoices")]
        public bool LinkPaymentsToInvoices { get; set; }

        [Display(Name = "Due Dates and Payment Terms")]
        public bool DueDatesAndPaymentTerms { get; set; }

        [Display(Name = "Show Profit while making Sale Invoice")]
        public bool ShowProfitWhileMakingSaleInvoice { get; set; }

        // Transaction Prefixes
        [Display(Name = "Firm")]
        public string Firm { get; set; } = "hellovezel";

        [Display(Name = "Sale")]
        public string SalePrefix { get; set; } = "None";

        [Display(Name = "Credit Note")]
        public string CreditNotePrefix { get; set; } = "None";

        [Display(Name = "Sale Order")]
        public string SaleOrderPrefix { get; set; } = "None";

        [Display(Name = "Purchase Order")]
        public string PurchaseOrderPrefix { get; set; } = "None";

        [Display(Name = "Estimate")]
        public string EstimatePrefix { get; set; } = "None";

        [Display(Name = "Delivery Challan")]
        public string DeliveryChallanPrefix { get; set; } = "None";

        [Display(Name = "Payment In")]
        public string PaymentInPrefix { get; set; } = "None";
    }
}
