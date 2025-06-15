namespace MUNEEMJI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    namespace BusinessDashboard.Models
    {
        // Main Dashboard ViewModel
        public class DashboardViewModel
        {
            public decimal TotalReceivable { get; set; }
            public decimal TotalPayable { get; set; }
            public int PayablePartyCount { get; set; }
            public bool HasReceivables { get; set; }
            public decimal TotalSalesThisMonth { get; set; }
            public SalesChartData SalesChartData { get; set; }
            public List<ReportViewModel> MostUsedReports { get; set; }
            public List<WidgetViewModel> Widgets { get; set; }
            public string ReceivableMessage => HasReceivables ? $"From {PayablePartyCount} Parties" : "You don't have any receivables as of now.";
            public string PayableMessage => $"From {PayablePartyCount} Party";
        }

        // Sales Chart Data
        public class SalesChartData
        {
            public List<string> Labels { get; set; }
            public List<decimal> Values { get; set; }
            public string Period { get; set; }
            public decimal MaxValue => Values?.Count > 0 ? Values.Max() : 1;
        }

        // Report ViewModel
        public class ReportViewModel
        {
            public string Name { get; set; }
            public string Action { get; set; }
            public string Controller { get; set; }
            public string Icon { get; set; }
            public int UsageCount { get; set; }
        }

        // Widget ViewModel
        public class WidgetViewModel
        {
            public int Id { get; set; }
            public string Type { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public int Position { get; set; }
            public bool IsActive { get; set; }
        }

        // Sale ViewModels
        public class SaleViewModel
        {
            [Required]
            public DateTime Date { get; set; }

            [Required]
            [Display(Name = "Party Name")]
            public string PartyName { get; set; }

            [Required]
            [Display(Name = "Invoice Number")]
            public string InvoiceNumber { get; set; }

            public string Notes { get; set; }

            [Required]
            public List<SaleItemViewModel> Items { get; set; }

            public decimal TotalAmount => Items?.Sum(i => i.TotalAmount) ?? 0;
            public decimal TotalTax => Items?.Sum(i => i.TaxAmount) ?? 0;
            public decimal GrandTotal => TotalAmount + TotalTax;
        }

        public class SaleItemViewModel
        {
            [Required]
            [Display(Name = "Item Name")]
            public string ItemName { get; set; }

            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
            public int Quantity { get; set; }

            [Required]
            [Range(0.01, double.MaxValue, ErrorMessage = "Rate must be greater than 0")]
            public decimal Rate { get; set; }

            [Range(0, 100, ErrorMessage = "Tax percentage must be between 0 and 100")]
            public decimal TaxPercentage { get; set; }

            public decimal TotalAmount => Quantity * Rate;
            public decimal TaxAmount => TotalAmount * (TaxPercentage / 100);
        }

        // Purchase ViewModels
        public class PurchaseViewModel
        {
            [Required]
            public DateTime Date { get; set; }

            [Required]
            [Display(Name = "Supplier Name")]
            public string SupplierName { get; set; }

            [Required]
            [Display(Name = "Purchase Order Number")]
            public string PurchaseOrderNumber { get; set; }

            public string Notes { get; set; }

            [Required]
            public List<PurchaseItemViewModel> Items { get; set; }

            public decimal TotalAmount => Items?.Sum(i => i.TotalAmount) ?? 0;
            public decimal TotalTax => Items?.Sum(i => i.TaxAmount) ?? 0;
            public decimal GrandTotal => TotalAmount + TotalTax;
        }

        public class PurchaseItemViewModel
        {
            [Required]
            [Display(Name = "Item Name")]
            public string ItemName { get; set; }

            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
            public int Quantity { get; set; }

            [Required]
            [Range(0.01, double.MaxValue, ErrorMessage = "Rate must be greater than 0")]
            public decimal Rate { get; set; }

            [Range(0, 100, ErrorMessage = "Tax percentage must be between 0 and 100")]
            public decimal TaxPercentage { get; set; }

            public decimal TotalAmount => Quantity * Rate;
            public decimal TaxAmount => TotalAmount * (TaxPercentage / 100);
        }

        // Transaction ViewModels
        public class TransactionViewModel
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string Type { get; set; } // Sale, Purchase, Payment, Receipt
            public string PartyName { get; set; }
            public string DocumentNumber { get; set; }
            public decimal Amount { get; set; }
            public string Status { get; set; }
            public string Notes { get; set; }
        }

        public class TransactionListViewModel
        {
            public List<TransactionViewModel> Transactions { get; set; }
            public int CurrentPage { get; set; }
            public int PageSize { get; set; }
            public int TotalCount { get; set; }
            public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
            public bool HasPrevious => CurrentPage > 1;
            public bool HasNext => CurrentPage < TotalPages;
        }

        public class TransactionSearchResult
        {
            public int Id { get; set; }
            public string Type { get; set; }
            public string PartyName { get; set; }
            public string DocumentNumber { get; set; }
            public decimal Amount { get; set; }
            public DateTime Date { get; set; }
        }

        // Report ViewModels
        public class SaleReportViewModel
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public List<SaleReportItem> Sales { get; set; }
            public decimal TotalSales => Sales?.Sum(s => s.Amount) ?? 0;
            public int TotalInvoices => Sales?.Count ?? 0;
        }

        public class SaleReportItem
        {
            public DateTime Date { get; set; }
            public string InvoiceNumber { get; set; }
            public string PartyName { get; set; }
            public decimal Amount { get; set; }
            public decimal TaxAmount { get; set; }
            public decimal TotalAmount { get; set; }
        }

        public class DaybookReportViewModel
        {
            public DateTime Date { get; set; }
            public List<DaybookEntryViewModel> Entries { get; set; }
            public decimal TotalDebit => Entries?.Sum(e => e.DebitAmount) ?? 0;
            public decimal TotalCredit => Entries?.Sum(e => e.CreditAmount) ?? 0;
        }

        public class DaybookEntryViewModel
        {
            public string AccountName { get; set; }
            public string Description { get; set; }
            public decimal DebitAmount { get; set; }
            public decimal CreditAmount { get; set; }
            public string Reference { get; set; }
        }

        public class PartyStatementViewModel
        {
            public int? PartyId { get; set; }
            public string PartyName { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public List<PartyStatementItem> Statements { get; set; }
            public decimal OpeningBalance { get; set; }
            public decimal ClosingBalance { get; set; }
            public decimal TotalDebits => Statements?.Sum(s => s.DebitAmount) ?? 0;
            public decimal TotalCredits => Statements?.Sum(s => s.CreditAmount) ?? 0;
        }

        public class PartyStatementItem
        {
            public DateTime Date { get; set; }
            public string Description { get; set; }
            public string DocumentNumber { get; set; }
            public decimal DebitAmount { get; set; }
            public decimal CreditAmount { get; set; }
            public decimal Balance { get; set; }
        }

        // Dashboard Summary for AJAX updates
        public class DashboardSummaryViewModel
        {
            public decimal TotalReceivable { get; set; }
            public decimal TotalPayable { get; set; }
            public decimal TotalSalesToday { get; set; }
            public decimal TotalPurchasesToday { get; set; }
            public int PendingInvoices { get; set; }
            public int OverduePayments { get; set; }
        }

        // Operation Result for API responses
        public class OperationResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public object Data { get; set; }
        }

        public class OperationResult<T> : OperationResult
        {
            public new T Data { get; set; }
        }

        // Error ViewModel
        public class ErrorViewModel
        {
            public string RequestId { get; set; }
            public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
            public string Message { get; set; }
            public string Details { get; set; }
        }

        // Pagination Helper
        public class PaginatedList<T> : List<T>
        {
            public int PageIndex { get; private set; }
            public int TotalPages { get; private set; }
            public int TotalCount { get; private set; }

            public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
            {
                PageIndex = pageIndex;
                TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                TotalCount = count;
                AddRange(items);
            }

            public bool HasPreviousPage => PageIndex > 1;
            public bool HasNextPage => PageIndex < TotalPages;

            public static PaginatedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
            {
                var count = source.Count();
                var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return new PaginatedList<T>(items, count, pageIndex, pageSize);
            }
        }

        // Enums
        public enum TransactionType
        {
            Sale,
            Purchase,
            Payment,
            Receipt,
            Journal
        }

        public enum TransactionStatus
        {
            Draft,
            Confirmed,
            Paid,
            PartiallyPaid,
            Overdue,
            Cancelled
        }

        public enum WidgetType
        {
            SalesChart,
            RecentTransactions,
            QuickStats,
            TodoList,
            Calendar,
            Notifications
        }

        public enum ReportPeriod
        {
            Today,
            ThisWeek,
            ThisMonth,
            ThisQuarter,
            ThisYear,
            Custom
        }
    }
}
