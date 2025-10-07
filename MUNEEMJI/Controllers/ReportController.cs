using Microsoft.AspNetCore.Mvc;

namespace MUNEEMJI.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Sale Report - Returns Partial View
        public IActionResult Sale()
        {
            ViewBag.ReportTitle = "Sale Invoices";
            ViewBag.ReportType = "sale";
            return PartialView("Sale_Report");
        }

        // Purchase Report - Returns Partial View
        public IActionResult Purchase()
        {
            ViewBag.ReportTitle = "Purchase Bills";
            ViewBag.ReportType = "purchase";
            return PartialView("_ReportTemplate");
        }

        // Day Book Report - Returns Partial View
        public IActionResult DayBook()
        {
            ViewBag.ReportTitle = "Day Book";
            ViewBag.ReportType = "daybook";
            return PartialView("_ReportTemplate");
        }

        // All Transactions Report - Returns Partial View
        public IActionResult AllTransactions()
        {
            ViewBag.ReportTitle = "All Transactions";
            ViewBag.ReportType = "alltransactions";
            return PartialView("_ReportTemplate");
        }

        // Profit And Loss Report - Returns Partial View
        public IActionResult ProfitAndLoss()
        {
            ViewBag.ReportTitle = "Profit And Loss";
            ViewBag.ReportType = "profitloss";
            return PartialView("_ReportTemplate");
        }

        // Bill Wise Profit Report - Returns Partial View
        public IActionResult BillWiseProfit()
        {
            ViewBag.ReportTitle = "Bill Wise Profit";
            ViewBag.ReportType = "billwiseprofit";
            return PartialView("_ReportTemplate");
        }

        // Sale Aging Report - Returns Partial View
        public IActionResult SaleAging()
        {
            ViewBag.ReportTitle = "Sale Aging";
            ViewBag.ReportType = "saleaging";
            return PartialView("_ReportTemplate");
        }

        // Cash Flow Report - Returns Partial View
        public IActionResult CashFlow()
        {
            ViewBag.ReportTitle = "Cash Flow";
            ViewBag.ReportType = "cashflow";
            return PartialView("_ReportTemplate");
        }

        // Trial Balance Report - Returns Partial View
        public IActionResult TrialBalanceReport()
        {
            ViewBag.ReportTitle = "Trial Balance Report";
            ViewBag.ReportType = "trialbalance";
            return PartialView("_ReportTemplate");
        }

        // Balance Sheet Report - Returns Partial View
        public IActionResult BalanceSheet()
        {
            ViewBag.ReportTitle = "Balance Sheet";
            ViewBag.ReportType = "balancesheet";
            return PartialView("_ReportTemplate");
        }

        // Party Report - Returns Partial View
        public IActionResult PartyReport()
        {
            ViewBag.ReportTitle = "Party Report";
            ViewBag.ReportType = "partyreport";
            return PartialView("_ReportTemplate");
        }

        // Party Statement - Returns Partial View
        public IActionResult PartyStatement()
        {
            ViewBag.ReportTitle = "Party Statement";
            ViewBag.ReportType = "partystatement";
            return PartialView("_ReportTemplate");
        }

        // Party Wise Profit & Loss - Returns Partial View
        public IActionResult PartyWiseProfitLoss()
        {
            ViewBag.ReportTitle = "Party Wise Profit & Loss";
            ViewBag.ReportType = "partywiseprofitloss";
            return PartialView("_ReportTemplate");
        }

        // All Parties Report - Returns Partial View
        public IActionResult AllParties()
        {
            ViewBag.ReportTitle = "All Parties";
            ViewBag.ReportType = "allparties";
            return PartialView("_ReportTemplate");
        }

        // Party Report By Item - Returns Partial View
        public IActionResult PartyReportByItem()
        {
            ViewBag.ReportTitle = "Party Report By Item";
            ViewBag.ReportType = "partyreportbyitem";
            return PartialView("_ReportTemplate");
        }

        // Sale Purchase By Party - Returns Partial View
        public IActionResult SalePurchaseByParty()
        {
            ViewBag.ReportTitle = "Sale Purchase By Party";
            ViewBag.ReportType = "salepurchasebyparty";
            return PartialView("_ReportTemplate");
        }

        // Sale Purchase By Party Group - Returns Partial View
        public IActionResult SalePurchaseByPartyGroup()
        {
            ViewBag.ReportTitle = "Sale Purchase By Party Group";
            ViewBag.ReportType = "salepurchasebypartygroup";
            return PartialView("_ReportTemplate");
        }

        // GSTR 3 B Report - Returns Partial View
        public IActionResult GSTR3B()
        {
            ViewBag.ReportTitle = "GSTR 3 B";
            ViewBag.ReportType = "gstr3b";
            return PartialView("_ReportTemplate");
        }

        // GSTR 4 Report - Returns Partial View
        public IActionResult GSTR4()
        {
            ViewBag.ReportTitle = "GSTR 4";
            ViewBag.ReportType = "gstr4";
            return PartialView("_ReportTemplate");
        }
    }
}

