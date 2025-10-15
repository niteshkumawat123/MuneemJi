using Insight.Database;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Areas.Settings.Controllers;
using MUNEEMJI.Models;
using MUNEEMJI.Models.ReportModel;
using MUNEEMJI.Repositories;
using MUNEEMJI.Services;
using Npgsql;
using NuGet.Packaging.Signing;
using System.ComponentModel.Design;

namespace MUNEEMJI.Controllers
{
    public class ReportController : Controller
    {
        private readonly ICompanyTenancy _CompayTenancy;
        string _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
        private readonly IParty partyController;
        private readonly IGodownService _godownService;
        private readonly IUser _user;
        private readonly IOtherIncomeRepository _otherIncome;

        public ReportController(ICompanyTenancy compayTenancy, IParty party, IGodownService godownService, IUser user, IOtherIncomeRepository otherIncome)
        {
            _CompayTenancy = compayTenancy;
            partyController = party;
            _godownService = godownService;
            _user = user;
            _otherIncome = otherIncome;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Sale Report - Returns Partial View
        public async Task<IActionResult> Sale()
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();
                DateTime? startDate = DateTime.UtcNow;
                DateTime? endDate = DateTime.UtcNow;
                string firmFilter = "ALL FIRMS";
                string vendorFilter = null;
                var PartyList = await partyController.GetPartyDropDownAsync(companyId);
                var godowns = await _godownService.GetAllGodownsAsync(companyId);
                var UserList = await _user.GetUserDropdown(companyId);


                using var connection = new NpgsqlConnection(_connectionString);
                string query = @"
            SELECT 
                td.id AS ""Id"",
                td.bill_number AS ""BillNumber"",
                td.bill_date AS ""BillDate"",
                td.state_of_supply AS ""StateOfSupply"",
                td.phone_no AS ""PhoneNo"",
                td.po_no AS ""PONo"",
                td.po_date AS ""PODate"",
                td.eway_bill_no AS ""EWayBillNo"",
                td.transport_name AS ""TransportName"",
                td.delivery_location AS ""DeliveryLocation"",
                td.vehicle_number AS ""VehicleNumber"",
                td.delivery_date AS ""DeliveryDate"",
                td.payment_type AS ""PaymentType"",
                td.description AS ""Description"",
                td.image_path AS ""ImagePath"",
                td.round_off AS ""RoundOff"",
                td.total AS ""Total"",
                td.paidreciveamount AS ""paidReciveamount"",
                td.partyid AS ""PartyId"",
                pt.party_name as PartyName,
                td.final_amount as ""FinalAmount"",
                td.invoicenumber as ""InvoiceNumber"",
                td.IsCredit as ""IsCredit"",
                td.created_date as ""CreatedDate""
            FROM public.tradedocuments as td 
            LEFT JOIN parties as pt ON td.partyid = pt.id  
            WHERE td.TradeDocumentTypesid = @TradeDocumentTypesid 
            AND td.companyid = @p_companyid;
        ";

                var PurchaseBill = connection.QuerySql<PurchaseBill>(query,
                    new
                    {
                        TradeDocumentTypesid = (int)TradeDocumentTypes.SalesChallan,
                        p_companyid = companyId
                    }).ToList();

                if (PurchaseBill != null && PurchaseBill.Count > 0)
                {
                    // Handle potential null values with null-coalescing operator
                    var paidTotal = PurchaseBill.Any(x => x.paidReciveamount > 0) ? PurchaseBill.Sum(b => b.paidReciveamount) : decimal.Zero;
                    var unpaidTotal = PurchaseBill.Any(y => y.Total > 0) ? (PurchaseBill.Sum(x => (x.Total)) - paidTotal) : decimal.Zero;
                    var grandTotal = paidTotal + unpaidTotal;

                    ViewBag.PaidTotal = paidTotal;
                    ViewBag.UnpaidTotal = unpaidTotal;
                    ViewBag.GrandTotal = grandTotal;
                    ViewBag.StartDate = startDate.Value.ToString("dd/MM/yyyy");
                    ViewBag.EndDate = endDate.Value.ToString("dd/MM/yyyy");
                    ViewBag.FirmFilter = firmFilter;
                    ViewBag.VendorFilter = vendorFilter;
                    ViewBag.PartyList = PartyList;
                    ViewBag.GodawonList = godowns;
                    ViewBag.UserList = UserList;
                }
                else
                {
                    ViewBag.PaidTotal = 0m;
                    ViewBag.UnpaidTotal = 0m;
                    ViewBag.GrandTotal = 0m;
                    ViewBag.StartDate = startDate.Value.ToString("dd/MM/yyyy");
                    ViewBag.EndDate = endDate.Value.ToString("dd/MM/yyyy");
                    ViewBag.FirmFilter = firmFilter;
                    ViewBag.VendorFilter = vendorFilter;
                    ViewBag.PartyList = PartyList;
                    ViewBag.GodawonList = godowns;
                    ViewBag.UserList = UserList;


                    PurchaseBill = new List<PurchaseBill>();
                }

                return PartialView("Sale_Report", PurchaseBill);
            }
            catch (Exception ex)
            {
                ViewBag.PaidTotal = 0m;
                ViewBag.UnpaidTotal = 0m;
                ViewBag.GrandTotal = 0m;
                ViewBag.StartDate = DateTime.UtcNow.ToString("dd/MM/yyyy");
                ViewBag.EndDate = DateTime.UtcNow.ToString("dd/MM/yyyy");
                ViewBag.FirmFilter = "ALL FIRMS";
                ViewBag.VendorFilter = null;
                ViewBag.Error = "An error occurred while loading purchase bills.";

                return PartialView("Sale_Report", new List<PurchaseBill>());
            }
        }

        // Purchase Report - Returns Partial View
        public async Task<IActionResult> Purchase()
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();
                DateTime? startDate = DateTime.UtcNow;
                DateTime? endDate = DateTime.UtcNow;
                string firmFilter = "ALL FIRMS";
                string vendorFilter = null;
                var PartyList = await partyController.GetPartyDropDownAsync(companyId);
                var godowns = await _godownService.GetAllGodownsAsync(companyId);
                var UserList = await _user.GetUserDropdown(companyId);


                using var connection = new NpgsqlConnection(_connectionString);
                string query = @"
            SELECT 
                td.id AS ""Id"",
                td.bill_number AS ""BillNumber"",
                td.bill_date AS ""BillDate"",
                td.state_of_supply AS ""StateOfSupply"",
                td.phone_no AS ""PhoneNo"",
                td.po_no AS ""PONo"",
                td.po_date AS ""PODate"",
                td.eway_bill_no AS ""EWayBillNo"",
                td.transport_name AS ""TransportName"",
                td.delivery_location AS ""DeliveryLocation"",
                td.vehicle_number AS ""VehicleNumber"",
                td.delivery_date AS ""DeliveryDate"",
                td.payment_type AS ""PaymentType"",
                td.description AS ""Description"",
                td.image_path AS ""ImagePath"",
                td.round_off AS ""RoundOff"",
                td.total AS ""Total"",
                td.paidreciveamount AS ""paidReciveamount"",
                td.partyid AS ""PartyId"",
                pt.party_name as PartyName,
                td.final_amount as ""FinalAmount"",
                td.invoicenumber as ""InvoiceNumber"",
                td.IsCredit as ""IsCredit"",
                td.created_date as ""CreatedDate""
            FROM public.tradedocuments as td 
            LEFT JOIN parties as pt ON td.partyid = pt.id  
            WHERE td.TradeDocumentTypesid = @TradeDocumentTypesid 
            AND td.companyid = @p_companyid;
        ";

                var PurchaseBill = connection.QuerySql<PurchaseBill>(query,
                    new
                    {
                        TradeDocumentTypesid = (int)TradeDocumentTypes.PurchaseChallan,
                        p_companyid = companyId
                    }).ToList();

                if (PurchaseBill != null && PurchaseBill.Count > 0)
                {
                    // Handle potential null values with null-coalescing operator
                    var paidTotal = PurchaseBill.Any(x => x.paidReciveamount > 0) ? PurchaseBill.Sum(b => b.paidReciveamount) : decimal.Zero;
                    var unpaidTotal = PurchaseBill.Any(y => y.Total > 0) ? (PurchaseBill.Sum(x => (x.Total)) - paidTotal) : decimal.Zero;
                    var grandTotal = paidTotal + unpaidTotal;

                    ViewBag.PaidTotal = paidTotal;
                    ViewBag.UnpaidTotal = unpaidTotal;
                    ViewBag.GrandTotal = grandTotal;
                    ViewBag.StartDate = startDate.Value.ToString("dd/MM/yyyy");
                    ViewBag.EndDate = endDate.Value.ToString("dd/MM/yyyy");
                    ViewBag.FirmFilter = firmFilter;
                    ViewBag.VendorFilter = vendorFilter;
                    ViewBag.PartyList = PartyList;
                    ViewBag.GodawonList = godowns;
                    ViewBag.UserList = UserList;
                }
                else
                {
                    ViewBag.PaidTotal = 0m;
                    ViewBag.UnpaidTotal = 0m;
                    ViewBag.GrandTotal = 0m;
                    ViewBag.StartDate = startDate.Value.ToString("dd/MM/yyyy");
                    ViewBag.EndDate = endDate.Value.ToString("dd/MM/yyyy");
                    ViewBag.FirmFilter = firmFilter;
                    ViewBag.VendorFilter = vendorFilter;
                    ViewBag.PartyList = PartyList;
                    ViewBag.GodawonList = godowns;
                    ViewBag.UserList = UserList;


                    PurchaseBill = new List<PurchaseBill>();
                }

                return PartialView("purchase_Report", PurchaseBill);
            }
            catch (Exception ex)
            {
                ViewBag.PaidTotal = 0m;
                ViewBag.UnpaidTotal = 0m;
                ViewBag.GrandTotal = 0m;
                ViewBag.StartDate = DateTime.UtcNow.ToString("dd/MM/yyyy");
                ViewBag.EndDate = DateTime.UtcNow.ToString("dd/MM/yyyy");
                ViewBag.FirmFilter = "ALL FIRMS";
                ViewBag.VendorFilter = null;
                ViewBag.Error = "An error occurred while loading purchase bills.";

                return PartialView("purchase_Report", new List<PurchaseBill>());
            }
        }

        // Day Book Report - Returns Partial View
        public IActionResult DayBook()
        {

            return PartialView("Daybook_report");
        }

        // All Transactions Report - Returns Partial View
        public IActionResult AllTransactions()
        {

            return PartialView("AllTransactions_Report");
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

            return PartialView("Sale_Aging_Report");
        }

        // Cash Flow Report - Returns Partial View
        public IActionResult CashFlow()
        {

            return PartialView("CashFlow_report");
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

            return PartialView("Party_Report");
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
        public async Task<IActionResult> AllParties()
        {
            var companyId = _CompayTenancy.GetCurrentCompanyId();
            DateTime? startDate = DateTime.UtcNow;
            DateTime? endDate = DateTime.UtcNow;
            string firmFilter = "ALL FIRMS";
            var partyDropDowns = await partyController.GetPartyDropDownAsync(companyId);
            var godowns = await _godownService.GetAllGodownsAsync(companyId);
            var UserList = await _user.GetUserDropdown(companyId);
            var PartyList = await partyController.GetAllPartiesAsync(companyId);

            return PartialView("All_Party_Report", PartyList);
        }

        // Party Report By Item - Returns Partial View
        public async Task<IActionResult> PartyReportByItem()
        {
            var companyId = _CompayTenancy.GetCurrentCompanyId();
            List<PartyReportByItemModel> reportByItemModels = new List<PartyReportByItemModel>();
            var PartyList = await partyController.GetAllPartiesAsync(companyId);
            List<TradeDocumentReportModel> Traderecord = new List<TradeDocumentReportModel>();
            try
            {
                using (var Conn = new NpgsqlConnection(_connectionString))
                {
                    var QueryString = " SELECT  td.partyid, td.tradedocumenttypesid,  SUM(td.final_amount) AS amount, SUM(tdi.total_quantity) AS quantity FROM tradedocuments td" +
                        " LEFT JOIN ( SELECT   tradedocumentsid,  SUM(quantity) AS total_quantity   FROM tradedocumentitems " +
                        " GROUP BY tradedocumentsid ) tdi ON td.id = tdi.tradedocumentsid WHERE td.tradedocumenttypesid IN (4,5) GROUP BY td.partyid, td.tradedocumenttypesid;";


                    Traderecord = Conn.QuerySql<TradeDocumentReportModel>(QueryString).ToList();
                }

                if (PartyList != null && PartyList.Count() > 0)
                {
                    foreach (var item in PartyList)
                    {
                        PartyReportByItemModel partyReportByItemModel = new PartyReportByItemModel()
                        {
                            PartyId = item.Id,
                            PartyName = item.PartyName,
                            PurchaseAmount = Traderecord.Where(x => x.partyid == item.Id && x.TradedocumentTypesId == 4).Select(x => x.amount).FirstOrDefault(),
                            PurchaseQuantity = Traderecord.Where(x => x.partyid == item.Id && x.TradedocumentTypesId == 4).Select(x => x.quantity).FirstOrDefault(),
                            SaleAmount = Traderecord.Where(x => x.partyid == item.Id && x.TradedocumentTypesId == 5).Select(x => x.amount).FirstOrDefault(),
                            SaleQuantity = Traderecord.Where(x => x.partyid == item.Id && x.TradedocumentTypesId == 5).Select(x => x.quantity).FirstOrDefault(),
                        };

                        reportByItemModels.Add(partyReportByItemModel);
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return PartialView("Party_Report_by_Item",reportByItemModels);
        }

        // Sale Purchase By Party - Returns Partial View
        public IActionResult SalePurchaseByParty()
        {

            return PartialView("Sale_Purchase_by_Party");
        }

        // Sale Purchase By Party Group - Returns Partial View
        public IActionResult SalePurchaseByPartyGroup()
        {

            return PartialView("_SalePurchaseByPartyGroup");
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

        public async Task<IActionResult> OtherIncomeReport()
        {
            OtherIncomeReportModel Model = new OtherIncomeReportModel();
            try
            {
                using (var Conn = new NpgsqlConnection(_connectionString))
                {
                    var QueryString = $" select income_category as IncomeCategory , incomecategoryid, entry_date as EntryDate , total, amount from public.income_entries" +
                        $" left join income_entry_items on income_entry_items.entry_id = income_entries.id ";

                    Model.OtherIncomeEntries = Conn.QuerySql<OtherIncomeViewModel>(QueryString).ToList();
                }
                var companyId = _CompayTenancy.GetCurrentCompanyId();
                Model.OtherIncomeCategoryDropDown = await _otherIncome.GetAllOtherIncomeCategories();
                

            }
            catch(Exception ex)
            {

            }
            return PartialView("OtherIncome_Report", Model);

        }
        public async Task<IActionResult> OtherIncomeCategoryReport()
        {
            OtherIncomeReportModel Model = new OtherIncomeReportModel();
            try
            {
                using (var Conn = new NpgsqlConnection(_connectionString))
                {
                    var QueryString = $" select income_category as IncomeCategory , incomecategoryid, entry_date as EntryDate , total, amount from public.income_entries" +
                        $" left join income_entry_items on income_entry_items.entry_id = income_entries.id ";

                    Model.OtherIncomeEntries = Conn.QuerySql<OtherIncomeViewModel>(QueryString).ToList();
                }
                var companyId = _CompayTenancy.GetCurrentCompanyId();
                Model.OtherIncomeCategoryDropDown = await _otherIncome.GetAllOtherIncomeCategories();


            }
            catch (Exception ex)
            {

            }
            return PartialView("OtherIncome_Category_Report", Model);

        }
        public async Task<IActionResult> OtherIncomeItemReport()
        {
            OtherIncomeReportModel Model = new OtherIncomeReportModel();
            try
            {
                using (var Conn = new NpgsqlConnection(_connectionString))
                {
                    var QueryString = $" select item_name as ItemName , qty , amount from public.income_entry_items ";

                    Model.IncomeEntryItems = Conn.QuerySql<IncomeEntryItem>(QueryString).ToList();
                }
                var companyId = _CompayTenancy.GetCurrentCompanyId();
                Model.OtherIncomeCategoryDropDown = await _otherIncome.GetAllOtherIncomeCategories();


            }
            catch (Exception ex)
            {

            }
            return PartialView("OtherIncome_item_Report", Model);

        }

        public async Task<IActionResult>Sale_Purchase_Order_report()
        {
            var companyId = _CompayTenancy.GetCurrentCompanyId();
            List<PurchaseBill> Model = new List<PurchaseBill>(); 
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                string query = @"
            SELECT 
                td.id AS ""Id"",
                td.bill_number AS ""BillNumber"",
                td.bill_date AS ""BillDate"",
                td.state_of_supply AS ""StateOfSupply"",
                td.phone_no AS ""PhoneNo"",
                td.po_no AS ""PONo"",
                td.po_date AS ""PODate"",
                td.eway_bill_no AS ""EWayBillNo"",
                td.transport_name AS ""TransportName"",
                td.delivery_location AS ""DeliveryLocation"",
                td.vehicle_number AS ""VehicleNumber"",
                td.delivery_date AS ""DeliveryDate"",
                td.payment_type AS ""PaymentType"",
                td.description AS ""Description"",
                td.image_path AS ""ImagePath"",
                td.round_off AS ""RoundOff"",
                td.total AS ""Total"",
                td.paidreciveamount AS ""paidReciveamount"",
                td.partyid AS ""PartyId"",
                pt.party_name as PartyName,
                td.final_amount as ""FinalAmount"",
                td.invoicenumber as ""InvoiceNumber"",
                td.IsCredit as ""IsCredit""
            FROM public.tradedocuments as td 
            LEFT JOIN parties as pt ON td.partyid = pt.id  
            WHERE td.TradeDocumentTypesid in(1,2) 
            AND td.companyid = @p_companyid;
        ";

                Model = connection.QuerySql<PurchaseBill>(query,
                    new
                    {
                        p_companyid = companyId
                    }).ToList();

            }
            catch(Exception ex)
            {

            }
            return PartialView("Sale_Purchase_Order_report", Model);
        }
    }
}

