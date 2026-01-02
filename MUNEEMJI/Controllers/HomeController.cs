using Insight.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MUNEEMJI.Models;
using MUNEEMJI.Services;
using Npgsql;
using Npgsql.Internal.Postgres;
using System.Diagnostics;

namespace MUNEEMJI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        string _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
        private readonly ICompanyTenancy _comapnytenancy;

        public HomeController(ILogger<HomeController> logger, ICompanyTenancy tenancy)
        {
            _logger = logger;
            _comapnytenancy = tenancy;
        }

        // Main dashboard action
        public async Task<IActionResult> Index()
        {
            try
            {
                var CompanyId = _comapnytenancy.GetCurrentCompanyId();

                var dashboardData = await GetDashboardDataAsync(CompanyId);
                ViewBag.TotalReceivable=  dashboardData.TotalReceivable;
                ViewBag.TotalPayable = dashboardData.TotalPayable;
                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                return View(new DashboardViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AllTransection()
        {
            try
            {
                await Task.Delay(1);
              
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                return View(new DashboardViewModel());
            }
        }

        // Search transactions
        [HttpGet]
        public async Task<IActionResult> SearchTransactions(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return Json(new { success = false, message = "Search query cannot be empty" });
                }

                var transactions = await SearchTransactionsAsync(query);
                return Json(new { success = true, data = transactions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching transactions for query: {Query}", query);
                return Json(new { success = false, message = "Error occurred while searching transactions" });
            }
        }

        // Add Sale action
        [HttpGet]
        public IActionResult AddSale()
        {
            var model = new SaleViewModel
            {
                Date = DateTime.Now,
                Items = new List<SaleItemViewModel>()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSale(SaleViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = await CreateSaleAsync(model);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Sale added successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", result.ErrorMessage);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding sale");
                ModelState.AddModelError("", "An error occurred while adding the sale");
                return View(model);
            }
        }

        // Add Purchase action
        [HttpGet]
        public IActionResult AddPurchase()
        {
            var model = new PurchaseViewModel
            {
                Date = DateTime.Now,
                Items = new List<PurchaseItemViewModel>()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPurchase(PurchaseViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var result = await CreatePurchaseAsync(model);
                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Purchase added successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", result.ErrorMessage);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding purchase");
                ModelState.AddModelError("", "An error occurred while adding the purchase");
                return View(model);
            }
        }

        // Get sales chart data
        [HttpGet]
        public async Task<IActionResult> GetSalesChartData(string period = "month")
        {
            try
            {
                var chartData = await GetSalesChartDataAsync(period);
                return Json(new { success = true, data = chartData });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales chart data for period: {Period}", period);
                return Json(new { success = false, message = "Error loading chart data" });
            }
        }

        // Reports actions
        [HttpGet]
        public async Task<IActionResult> SaleReport(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var reportData = await GetSaleReportAsync(startDate, endDate);
                return View(reportData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading sale report");
                return View(new SaleReportViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AllTransactions(int page = 1, int pageSize = 50)
        {
            try
            {
                var transactions = await GetAllTransactionsAsync(page, pageSize);
                return View(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading all transactions");
                return View(new TransactionListViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> DaybookReport(DateTime? date = null)
        {
            try
            {
                var reportDate = date ?? DateTime.Now.Date;
                var daybookData = await GetDaybookReportAsync(reportDate);
                return View(daybookData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading daybook report");
                return View(new DaybookReportViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> PartyStatement(int? partyId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var statementData = await GetPartyStatementAsync(partyId, startDate, endDate);
                return View(statementData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading party statement");
                return View(new PartyStatementViewModel());
            }
        }

        // Widget management
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddWidget(string widgetType)
        {
            try
            {
                var result = await AddWidgetAsync(widgetType);
                if (result.Success)
                {
                    return Json(new { success = true, message = "Widget added successfully" });
                }
                else
                {
                    return Json(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding widget: {WidgetType}", widgetType);
                return Json(new { success = false, message = "Error adding widget" });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveWidget(int widgetId)
        {
            try
            {
                var result = await RemoveWidgetAsync(widgetId);
                if (result.Success)
                {
                    return Json(new { success = true, message = "Widget removed successfully" });
                }
                else
                {
                    return Json(new { success = false, message = result.ErrorMessage });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing widget: {WidgetId}", widgetId);
                return Json(new { success = false, message = "Error removing widget" });
            }
        }

        // AJAX endpoint for updating dashboard data
        [HttpGet]
        public async Task<IActionResult> GetDashboardSummary()
        {
            try
            {
                var summary = await GetDashboardSummaryAsync();
                return Json(new { success = true, data = summary });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting dashboard summary");
                return Json(new { success = false, message = "Error loading summary data" });
            }
        }

        // Private helper methods
        private async Task<DashboardViewModel> GetDashboardDataAsync(int CompanyId)
        {
            // Simulate async data loading
            await Task.Delay(100);
            decimal TotalReceivable = 0;
            decimal TotalPayable = 0;
            decimal ExpenseAmount = 0;
            decimal CashInHand = 0;
            decimal BanckAmount = 0;
            decimal StockAmount = 0;

            using (var conn = new NpgsqlConnection(_connectionString))
            {

                string QueryString = "SELECT SUM(CAST(final_amount AS DECIMAL(18,2))) FROM TradeDocuments WHERE TradeDocumentTypesId = @P_typeid and companyid = @p_companyid ";

                 TotalReceivable = conn.QuerySql<decimal?>(QueryString, new { P_typeid = (int)TradeDocumentTypes.SalesChallan , p_companyid  = CompanyId}).FirstOrDefault() ?? 0;

                string QueryString1 = "SELECT SUM(CAST(final_amount AS DECIMAL(18,2))) FROM TradeDocuments WHERE TradeDocumentTypesId = @P_typeid and companyid = @p_companyid ";

                 TotalPayable = conn.QuerySql<decimal?>(QueryString1, new { P_typeid = (int)TradeDocumentTypes.PurchaseChallan, p_companyid = CompanyId }).FirstOrDefault() ?? 0;

                string QueryString2 = " SELECT SUM(CAST(amount AS DECIMAL(18,2))) FROM expenseitemtransection  ";

                ExpenseAmount = conn.QuerySql<decimal?>(QueryString2).FirstOrDefault() ?? 0;

                string QueryString3 = @" SELECT
                                                COALESCE(SUM(
                                                    CASE 
                                                        WHEN adjusttypeid = 1 THEN CAST(amount AS DECIMAL(18,2))
                                                        WHEN adjusttypeid = 2 THEN -CAST(amount AS DECIMAL(18,2))
                                                        ELSE 0
                                                    END
                                                ), 0) AS balance
                                        FROM public.bank_cash ; " ;

                CashInHand = conn.QuerySql<decimal?>(QueryString3).FirstOrDefault() ?? 0;


                string QueryString4 = " select sum(cast(opening_balance as decimal(18,2))) from extended_bank_accounts ";
                BanckAmount = conn.QuerySql<decimal?>(QueryString4).FirstOrDefault() ?? 0;

                string QueryString5 = " select coalesce(sum(cast((opening_quantity*sale_price)as decimal(18,2) )),0) from  billitem where  item_type = 'product' " +
                                        " and companyid = @p_companyid";
                StockAmount = conn.QuerySql<decimal?>(QueryString5,new { p_companyid  = CompanyId}).FirstOrDefault() ?? 0;


            }

            return new DashboardViewModel
            {
                TotalReceivable = TotalReceivable,
                TotalPayable = TotalPayable,
                PayablePartyCount = 1,
                HasReceivables = false,
                TotalSalesThisMonth = 0,
                SalesChartData = GetSampleChartData(),
                MostUsedReports = GetMostUsedReports(),
                Widgets = new List<WidgetViewModel>(),
                ExpenseAmount = ExpenseAmount,
                BanckAmount = BanckAmount,
                CashInHand = CashInHand,
                StockAmount= StockAmount,

            };
        }

        private async Task<List<TransactionSearchResult>> SearchTransactionsAsync(string query)
        {
            await Task.Delay(50); // Simulate database query

            // Return empty results for demo
            return new List<TransactionSearchResult>();
        }

        private async Task<OperationResult> CreateSaleAsync(SaleViewModel model)
        {
            await Task.Delay(100); // Simulate database operation

            // Simulate successful creation
            return new OperationResult { Success = true };
        }

        private async Task<OperationResult> CreatePurchaseAsync(PurchaseViewModel model)
        {
            await Task.Delay(100); // Simulate database operation

            // Simulate successful creation
            return new OperationResult { Success = true };
        }

        private async Task<SalesChartData> GetSalesChartDataAsync(string period)
        {
            await Task.Delay(50);
            return GetSampleChartData();
        }

        private async Task<SaleReportViewModel> GetSaleReportAsync(DateTime? startDate, DateTime? endDate)
        {
            await Task.Delay(100);
            return new SaleReportViewModel
            {
                StartDate = startDate ?? DateTime.Now.AddMonths(-1),
                EndDate = endDate ?? DateTime.Now,
                Sales = new List<SaleReportItem>()
            };
        }

        private async Task<TransactionListViewModel> GetAllTransactionsAsync(int page, int pageSize)
        {
            await Task.Delay(100);
            return new TransactionListViewModel
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = 0,
                Transactions = new List<TransactionViewModel>()
            };
        }

        private async Task<DaybookReportViewModel> GetDaybookReportAsync(DateTime date)
        {
            await Task.Delay(100);
            return new DaybookReportViewModel
            {
                Date = date,
                Entries = new List<DaybookEntryViewModel>()
            };
        }

        private async Task<PartyStatementViewModel> GetPartyStatementAsync(int? partyId, DateTime? startDate, DateTime? endDate)
        {
            await Task.Delay(100);
            return new PartyStatementViewModel
            {
                PartyId = partyId,
                StartDate = startDate ?? DateTime.Now.AddMonths(-1),
                EndDate = endDate ?? DateTime.Now,
                Statements = new List<PartyStatementItem>()
            };
        }

        private async Task<OperationResult> AddWidgetAsync(string widgetType)
        {
            await Task.Delay(50);
            return new OperationResult { Success = true };
        }

        private async Task<OperationResult> RemoveWidgetAsync(int widgetId)
        {
            await Task.Delay(50);
            return new OperationResult { Success = true };
        }

        private async Task<DashboardSummaryViewModel> GetDashboardSummaryAsync()
        {
            await Task.Delay(50);
            return new DashboardSummaryViewModel
            {
                TotalReceivable = 0,
                TotalPayable = 1221,
                TotalSalesToday = 0,
                TotalPurchasesToday = 0
            };
        }

        private SalesChartData GetSampleChartData()
        {
            return new SalesChartData
            {
                Labels = new List<string> { "1 Jun", "4 Jun", "7 Jun", "10 Jun", "13 Jun", "16 Jun", "19 Jun", "22 Jun", "25 Jun", "28 Jun" },
                Values = new List<decimal> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                Period = "This Month"
            };
        }

        private List<ReportViewModel> GetMostUsedReports()
        {
            return new List<ReportViewModel>
            {
                new ReportViewModel { Name = "Sale Report", Action = "SaleReport", Controller = "Home" },
                new ReportViewModel { Name = "All Transactions", Action = "AllTransactions", Controller = "Home" },
                new ReportViewModel { Name = "Daybook Report", Action = "DaybookReport", Controller = "Home" },
                new ReportViewModel { Name = "Party Statement", Action = "PartyStatement", Controller = "Home" }
            };
        }

        // Error handling
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
