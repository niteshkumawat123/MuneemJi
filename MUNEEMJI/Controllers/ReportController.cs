using Insight.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MUNEEMJI.Areas.Settings.Controllers;
using MUNEEMJI.Models;
using MUNEEMJI.Models.ReportModel;
using MUNEEMJI.Repositories;
using MUNEEMJI.Services;
using Npgsql;
using NuGet.Packaging.Signing;
using NuGet.Protocol.Plugins;
using System.ComponentModel.Design;
using System.Composition;
using static MUNEEMJI.Models.ItemModel;

namespace MUNEEMJI.Controllers
{
    public class ReportController : Controller
    {
        private readonly ICompanyTenancy _CompayTenancy;
        string _connectionString = MUNEEMJI.DbConfig.ConnectionString;
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


        public async Task<IActionResult> DayBook()
        {
            var companyId = _CompayTenancy.GetCurrentCompanyId();
            List<DayBookReportModel> reportByItemModels = new List<DayBookReportModel>();
            var PartyList = await partyController.GetAllPartiesAsync(companyId);
            List<DayBookReportModel> Traderecord = new List<DayBookReportModel>();
            try
            {
                using (var Conn = new NpgsqlConnection(_connectionString))
                {
                    var QueryString = " SELECT  td.partyid, td.tradedocumenttypesid,  td.final_amount as FinalAmount , td.total , td.invoicenumber FROM tradedocuments td " +
                                      " WHERE td.tradedocumenttypesid IN(4,5) ";


                    Traderecord = Conn.QuerySql<DayBookReportModel>(QueryString).ToList();
                }

                if (PartyList != null && PartyList.Count() > 0)
                {
                    foreach (var item in PartyList)
                    {
                        var TradeDocumentTypeId = Traderecord.Where(x => x.PartyId == item.Id).Select(x => x.tradedocumenttypesid).FirstOrDefault();
                        var TradeDocumentType = TradeDocumentTypeId == 4 ? "Purchase" : "Sales";
                        DayBookReportModel partyReportByItemModel = new DayBookReportModel()
                        {
                            PartyId = item.Id,
                            PartyName = item.PartyName,
                            FinalAmount = Traderecord.Where(x => x.PartyId == item.Id).Select(x => x.FinalAmount).FirstOrDefault(),
                            Total = Traderecord.Where(x => x.PartyId == item.Id).Select(x => x.Total).FirstOrDefault(),
                            tradedocumenttypesid = TradeDocumentTypeId,
                            TradeDocumentType = TradeDocumentType,
                            MoneyIn = TradeDocumentTypeId == 5 ? Traderecord.Where(x => x.PartyId == item.Id).Select(x => x.FinalAmount).FirstOrDefault() : 0,
                            MoneyOut = TradeDocumentTypeId == 4 ? Traderecord.Where(x => x.PartyId == item.Id).Select(x => x.FinalAmount).FirstOrDefault() : 0,
                            invoicenumber = Traderecord.Where(x => x.PartyId == item.Id).Select(x => x.invoicenumber).FirstOrDefault(),
                        };

                        reportByItemModels.Add(partyReportByItemModel);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return PartialView("Daybook_report", reportByItemModels);
        }

        // All Transactions Report - Returns Partial View
        public IActionResult AllTransactions()
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
            WHERE  td.companyid = @p_companyid;
        ";

                Model = connection.QuerySql<PurchaseBill>(query,
                    new
                    {
                        p_companyid = companyId
                    }).ToList();

            }
            catch (Exception ex)
            {

            }
            return PartialView("AllTransactions_Report", Model);

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
            WHERE  td.companyid = @p_companyid;
        ";

                Model = connection.QuerySql<PurchaseBill>(query,
                    new
                    {
                        p_companyid = companyId
                    }).ToList();

            }
            catch (Exception ex)
            {

            }
            return PartialView("Bill_Wise_Profit", Model);
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

        public async Task<IActionResult> Item_Report_By_Party()
        {
            return PartialView();
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
            catch (Exception ex)
            {

            }
            return PartialView("Party_Report_by_Item", reportByItemModels);
        }

        // Sale Purchase By Party - Returns Partial View
        public async Task<IActionResult> SalePurchaseByParty()
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
            catch (Exception ex)
            {

            }
            return PartialView("Sale_Purchase_by_Party", reportByItemModels);
        }

        // Sale Purchase By Party Group - Returns Partial View
        public async Task<IActionResult> SalePurchaseByPartyGroup()
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
            catch (Exception ex)
            {

            }

            return PartialView("_SalePurchaseByPartyGroup", reportByItemModels);
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

        public async Task<IActionResult> OtherIncomeReport(int categoryid)
        {
            OtherIncomeReportModel Model = new OtherIncomeReportModel();
            try
            {
                using (var Conn = new NpgsqlConnection(_connectionString))
                {
                    var QueryString = "SELECT income_category AS IncomeCategory, " +
                    "incomecategoryid, entry_date AS EntryDate, " +
                    "total, amount " +
                    "FROM public.income_entries " +
                    "LEFT JOIN income_entry_items ON income_entry_items.entry_id = income_entries.id";

                    if (categoryid > 0)
                    {
                        QueryString += " WHERE incomecategoryid = @p_categoryid";
                    }


                    Model.OtherIncomeEntries = Conn.QuerySql<OtherIncomeViewModel>(QueryString, new { p_categoryid = categoryid }).ToList();
                }
                var companyId = _CompayTenancy.GetCurrentCompanyId();
                Model.OtherIncomeCategoryDropDown = await _otherIncome.GetAllOtherIncomeCategories();


            }
            catch (Exception ex)
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

        public async Task<IActionResult> Sale_Purchase_Order_report()
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
            catch (Exception ex)
            {

            }
            return PartialView("Sale_Purchase_Order_report", Model);
        }

        public async Task<IActionResult> LoanReport()
        {
            List<LoanReportViewModel> transactions = new List<LoanReportViewModel>();

            try
            {
                await Task.Delay(1);

                var SqlAccount = " select id as AccountID , account_name as AccountName from loan_accounts ";


                string sql = @"
                                    SELECT 
                                        id,
                                        loanaccountid,
                                        transactiontype type,
                                        principalamount,
                                        interestamount,
                                        totalamount as amount,
                                        transactiondate,
                                        paymentmethod,
                                        interestrate,
                                        termduration,
                                        description,
                                        createddate as date
                                    FROM loantransactions
                                    ORDER BY transactiondate DESC, createddate DESC";


                using (var Conn = new NpgsqlConnection(_connectionString))
                {
                    transactions = Conn.QuerySql<LoanReportViewModel>(SqlAccount).ToList();

                    var loantransection = Conn.QuerySql<LoanTransectionReprotModel>(sql).ToList();

                    if(transactions!=null && transactions.Count()>0 && loantransection!=null && loantransection.Count()>0)
                    {
                        foreach (var item in transactions)
                        {
                            item.LoanTransections = loantransection.Where(x => x.loanaccountid == item.AccountID).ToList();
                        }
                    }


                }

            }
            catch (Exception ex)
            {
            }

            return View(transactions);
        }

        public async Task<IActionResult> ExpenseReport()
        {
            List<ExpenseItemTransection> itemTransections = new List<ExpenseItemTransection>();

            try
            {
                await Task.Delay(1);

                using (var Conn = new NpgsqlConnection(_connectionString))
                {
                    var SqlConnectionString = @" SELECT et.id, et.expenseid, et.itemid, et.quantity, et.price, et.amount , ess.expenseno , ess.expensedate , ec.category 
                                                 FROM expenseitemtransection as et
                                                 left join  expenses as ess  on et.expenseid = ess.id
                                                 left join expensecategory as ec on ess.categoryid = ec.id ";

                    itemTransections = Conn.QuerySql<ExpenseItemTransection>(SqlConnectionString).ToList();

                }
            }
            catch(Exception ex)
            {

            }
            return View(itemTransections);


        }
        public async Task<IActionResult> ExpenseCategoryReport()
        {
            return View();
        }

        public async Task<IActionResult> ExpenseItemReport()
        {
            List<ExpenseItemTransection> itemTransections = new List<ExpenseItemTransection>();
            try
            {
                await Task.Delay(1);
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    var QueryStrin = " select epit.*, item_name as itemname from expenseitemtransection as epit  left join  public.billitem as its on its.id = epit.itemid ";

                    itemTransections = conn.QuerySql<ExpenseItemTransection>(QueryStrin).ToList();
                }

            }
            catch(Exception ex)
            {
                
            }
            return View(itemTransections);
        }

        public async Task<IActionResult> GstReport()
        {
            return View();
        }
        public async Task<IActionResult> GSTRateReport()
        {
            return View();
        }
        public async Task<IActionResult> Form27EQ()
        {
            return View();
        }

        public async Task<IActionResult> TCSReceivable()
        {
            return View();
        }

        public async Task<IActionResult> TDSPayable()
        {
            return View();
        }
        public async Task<IActionResult> TDSReceivable()
        {
            return View();
        }

        public async Task<IActionResult> ItemWiseProfitAndLoss()
        {
            return View();
        }

        public async Task<IActionResult> LowStockSummary()
        {
            List<BillItem> bills = new List<BillItem>();
            try
            {
                await Task.Delay(1);
                using (var Conn = new NpgsqlConnection(_connectionString))
                {
                    var billItemSql = @"
                                    SELECT 
                                        id AS ""Id"",
                                        item_type AS ""ItemType"",
                                        item_name AS ""ItemName"",
                                        item_hsn AS ""ItemHsn"",
                                        item_code AS ""ItemCode"",
                                        category AS ""Category"",
                                        unit AS ""Unit"",
                                        item_image_url AS ""ItemImageUrl"",
                                        sale_price AS ""SalePrice"",
                                        sale_price_tax_type AS ""SalePriceTaxType"",
                                        discount_on_sale_price AS ""DiscountOnSalePrice"",
                                        discount_type AS ""DiscountType"",
                                        purchase_price AS ""PurchasePrice"",
                                        purchase_price_tax_type AS ""PurchasePriceTaxType"",
                                        tax_rate AS ""TaxRate"",
                                        wholesale_price AS ""WholesalePrice"",
                                        opening_quantity AS ""OpeningQuantity"",
                                        at_price AS ""AtPrice"",
                                        as_of_date AS ""AsOfDate"",
                                        location AS ""Location"",
                                        min_stock_to_maintain AS ""MinStockToMaintain"",
                                        online_store_price AS ""OnlineStorePrice"",
                                        description AS ""Description"",
                                        raw_materials AS ""RawMaterials"",
                                        additional_costs AS ""AdditionalCosts"",
                                        total_estimated_cost AS ""TotalEstimatedCost"",
                                        service_name AS ""ServiceName"",
                                        service_hsn AS ""ServiceHsn"",
                                        service_code AS ""ServiceCode"",
                                        created_at AS ""CreatedAt"",
                                        updated_at AS ""UpdatedAt""
                                    FROM billitem where  item_type = @p_itemtype 
                                    ORDER BY id;
                                    ";

                    // ? Fetch bill items
                    bills = Conn.QuerySql<BillItem>(billItemSql, new { p_itemtype = "product"}).ToList();
                }
            }
            catch(Exception ex)
            {

            }
            return View(bills);
        }

        public async Task<IActionResult> StockDetail()
        {
            List<BillItem> bills = new List<BillItem>();
            try
            {
                await Task.Delay(1);
                using (var Conn = new NpgsqlConnection(_connectionString))
                {
                    var billItemSql = @"
                                    SELECT 
                                        id AS ""Id"",
                                        item_type AS ""ItemType"",
                                        item_name AS ""ItemName"",
                                        item_hsn AS ""ItemHsn"",
                                        item_code AS ""ItemCode"",
                                        category AS ""Category"",
                                        unit AS ""Unit"",
                                        item_image_url AS ""ItemImageUrl"",
                                        sale_price AS ""SalePrice"",
                                        sale_price_tax_type AS ""SalePriceTaxType"",
                                        discount_on_sale_price AS ""DiscountOnSalePrice"",
                                        discount_type AS ""DiscountType"",
                                        purchase_price AS ""PurchasePrice"",
                                        purchase_price_tax_type AS ""PurchasePriceTaxType"",
                                        tax_rate AS ""TaxRate"",
                                        wholesale_price AS ""WholesalePrice"",
                                        opening_quantity AS ""OpeningQuantity"",
                                        at_price AS ""AtPrice"",
                                        as_of_date AS ""AsOfDate"",
                                        location AS ""Location"",
                                        min_stock_to_maintain AS ""MinStockToMaintain"",
                                        online_store_price AS ""OnlineStorePrice"",
                                        description AS ""Description"",
                                        raw_materials AS ""RawMaterials"",
                                        additional_costs AS ""AdditionalCosts"",
                                        total_estimated_cost AS ""TotalEstimatedCost"",
                                        service_name AS ""ServiceName"",
                                        service_hsn AS ""ServiceHsn"",
                                        service_code AS ""ServiceCode"",
                                        created_at AS ""CreatedAt"",
                                        updated_at AS ""UpdatedAt""
                                    FROM billitem where  item_type = @p_itemtype 
                                    ORDER BY id;
                                    ";

                    // ? Fetch bill items
                    bills = Conn.QuerySql<BillItem>(billItemSql, new { p_itemtype = "product" }).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return View(bills);
        }

        public async Task<IActionResult> ItemDetail()
        {
            List<BillItem> bills = new List<BillItem>();
            try
            {
                await Task.Delay(1);
                using (var Conn = new NpgsqlConnection(_connectionString))
                {
                    var billItemSql = @"
                                    SELECT 
                                        id AS ""Id"",
                                        item_type AS ""ItemType"",
                                        item_name AS ""ItemName"",
                                        item_hsn AS ""ItemHsn"",
                                        item_code AS ""ItemCode"",
                                        category AS ""Category"",
                                        unit AS ""Unit"",
                                        item_image_url AS ""ItemImageUrl"",
                                        sale_price AS ""SalePrice"",
                                        sale_price_tax_type AS ""SalePriceTaxType"",
                                        discount_on_sale_price AS ""DiscountOnSalePrice"",
                                        discount_type AS ""DiscountType"",
                                        purchase_price AS ""PurchasePrice"",
                                        purchase_price_tax_type AS ""PurchasePriceTaxType"",
                                        tax_rate AS ""TaxRate"",
                                        wholesale_price AS ""WholesalePrice"",
                                        opening_quantity AS ""OpeningQuantity"",
                                        at_price AS ""AtPrice"",
                                        as_of_date AS ""AsOfDate"",
                                        location AS ""Location"",
                                        min_stock_to_maintain AS ""MinStockToMaintain"",
                                        online_store_price AS ""OnlineStorePrice"",
                                        description AS ""Description"",
                                        raw_materials AS ""RawMaterials"",
                                        additional_costs AS ""AdditionalCosts"",
                                        total_estimated_cost AS ""TotalEstimatedCost"",
                                        service_name AS ""ServiceName"",
                                        service_hsn AS ""ServiceHsn"",
                                        service_code AS ""ServiceCode"",
                                        created_at AS ""CreatedAt"",
                                        updated_at AS ""UpdatedAt""
                                    FROM billitem where  item_type = @p_itemtype 
                                    ORDER BY id;
                                    ";

                    // ? Fetch bill items
                    bills = Conn.QuerySql<BillItem>(billItemSql, new { p_itemtype = "product" }).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return View(bills);
        }
    }
}

