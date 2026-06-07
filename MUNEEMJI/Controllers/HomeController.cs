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
        string _connectionString = MUNEEMJI.DbConfig.ConnectionString;
        private readonly ICompanyTenancy _comapnytenancy;
        private readonly IErrorLogService _errorLogService;

        public HomeController(ILogger<HomeController> logger, ICompanyTenancy tenancy, IErrorLogService errorLogService)
        {
            _logger = logger;
            _comapnytenancy = tenancy;
            _errorLogService = errorLogService;
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
                await _errorLogService.LogErrorAsync($"Home Index Error: {ex.Message}", ex.StackTrace);
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
                await _errorLogService.LogErrorAsync($"Home AllTransection Error: {ex.Message}", ex.StackTrace);
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
                await _errorLogService.LogErrorAsync($"Home SearchTransactions Error: {ex.Message}", ex.StackTrace);
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
                await _errorLogService.LogErrorAsync($"Home AddSale Error: {ex.Message}", ex.StackTrace);
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
                await _errorLogService.LogErrorAsync($"Home AddPurchase Error: {ex.Message}", ex.StackTrace);
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
                await _errorLogService.LogErrorAsync($"Home GetSalesChartData Error: {ex.Message}", ex.StackTrace);
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
                await _errorLogService.LogErrorAsync($"Home SaleReport Error: {ex.Message}", ex.StackTrace);
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
                await _errorLogService.LogErrorAsync($"Home AllTransactions Error: {ex.Message}", ex.StackTrace);
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
                await _errorLogService.LogErrorAsync($"Home DaybookReport Error: {ex.Message}", ex.StackTrace);
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
                await _errorLogService.LogErrorAsync($"Home PartyStatement Error: {ex.Message}", ex.StackTrace);
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
                await _errorLogService.LogErrorAsync($"Home AddWidget Error: {ex.Message}", ex.StackTrace);
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
                await _errorLogService.LogErrorAsync($"Home RemoveWidget Error: {ex.Message}", ex.StackTrace);
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
                await _errorLogService.LogErrorAsync($"Home GetDashboardSummary Error: {ex.Message}", ex.StackTrace);
                _logger.LogError(ex, "Error getting dashboard summary");
                return Json(new { success = false, message = "Error loading summary data" });
            }
        }

        // Private helper methods
        private async Task<DashboardViewModel> GetDashboardDataAsync(int CompanyId)
        {
            decimal TotalReceivable = 0, TotalPayable = 0, ExpenseAmount = 0, CashInHand = 0, BanckAmount = 0, StockAmount = 0;
            int receivablePartyCount = 0, payablePartyCount = 0;
            // Slider data
            int todayInvoices = 0, weekInvoices = 0, monthInvoices = 0;
            decimal todayRevenue = 0, weekRevenue = 0, monthRevenue = 0;
            int todayPending = 0, weekPending = 0, monthPending = 0;
            int todayParties = 0, weekParties = 0, monthParties = 0;
            // Chart data
            var chartLabels = new List<string>();
            var chartValues = new List<decimal>();
            // Low stock
            var lowStockItems = new List<LowStockItem>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Total Receivable (Sales)
                TotalReceivable = conn.QuerySql<decimal?>(
                    "SELECT SUM(CAST(final_amount AS DECIMAL(18,2))) FROM TradeDocuments WHERE TradeDocumentTypesId = @P_typeid AND companyid = @p_companyid",
                    new { P_typeid = (int)TradeDocumentTypes.SalesChallan, p_companyid = CompanyId }).FirstOrDefault() ?? 0;

                // Total Payable (Purchases)
                TotalPayable = conn.QuerySql<decimal?>(
                    "SELECT SUM(CAST(final_amount AS DECIMAL(18,2))) FROM TradeDocuments WHERE TradeDocumentTypesId = @P_typeid AND companyid = @p_companyid",
                    new { P_typeid = (int)TradeDocumentTypes.PurchaseChallan, p_companyid = CompanyId }).FirstOrDefault() ?? 0;

                // Party counts
                receivablePartyCount = conn.QuerySql<int?>(
                    "SELECT COUNT(DISTINCT party_id) FROM TradeDocuments WHERE TradeDocumentTypesId = @P_typeid AND companyid = @p_companyid AND CAST(final_amount AS DECIMAL(18,2)) > 0",
                    new { P_typeid = (int)TradeDocumentTypes.SalesChallan, p_companyid = CompanyId }).FirstOrDefault() ?? 0;

                payablePartyCount = conn.QuerySql<int?>(
                    "SELECT COUNT(DISTINCT party_id) FROM TradeDocuments WHERE TradeDocumentTypesId = @P_typeid AND companyid = @p_companyid AND CAST(final_amount AS DECIMAL(18,2)) > 0",
                    new { P_typeid = (int)TradeDocumentTypes.PurchaseChallan, p_companyid = CompanyId }).FirstOrDefault() ?? 0;

                // Expenses
                ExpenseAmount = conn.QuerySql<decimal?>("SELECT SUM(CAST(amount AS DECIMAL(18,2))) FROM expenseitemtransection WHERE companyid = @p_companyid",
                    new { p_companyid = CompanyId }).FirstOrDefault() ?? 0;

                // Cash in hand
                CashInHand = conn.QuerySql<decimal?>(
                    @"SELECT COALESCE(SUM(CASE WHEN adjusttypeid = 1 THEN CAST(amount AS DECIMAL(18,2))
                                                WHEN adjusttypeid = 2 THEN -CAST(amount AS DECIMAL(18,2)) ELSE 0 END), 0)
                      FROM public.bank_cash WHERE companyid = @p_companyid",
                    new { p_companyid = CompanyId }).FirstOrDefault() ?? 0;

                // Bank balance
                BanckAmount = conn.QuerySql<decimal?>(
                    "SELECT SUM(CAST(opening_balance AS DECIMAL(18,2))) FROM extended_bank_accounts WHERE companyid = @p_companyid",
                    new { p_companyid = CompanyId }).FirstOrDefault() ?? 0;

                // Stock value
                StockAmount = conn.QuerySql<decimal?>(
                    "SELECT COALESCE(SUM(CAST((opening_quantity * sale_price) AS DECIMAL(18,2))), 0) FROM billitem WHERE item_type = 'product' AND companyid = @p_companyid",
                    new { p_companyid = CompanyId }).FirstOrDefault() ?? 0;

                // === TODAY stats ===
                var todayStart = DateTime.UtcNow.Date;
                todayInvoices = conn.QuerySql<int?>(
                    "SELECT COUNT(*) FROM TradeDocuments WHERE companyid = @cid AND TradeDocumentTypesId = @tid AND invoice_date >= @d",
                    new { cid = CompanyId, tid = (int)TradeDocumentTypes.SalesChallan, d = todayStart }).FirstOrDefault() ?? 0;
                todayRevenue = conn.QuerySql<decimal?>(
                    "SELECT COALESCE(SUM(CAST(final_amount AS DECIMAL(18,2))), 0) FROM TradeDocuments WHERE companyid = @cid AND TradeDocumentTypesId = @tid AND invoice_date >= @d",
                    new { cid = CompanyId, tid = (int)TradeDocumentTypes.SalesChallan, d = todayStart }).FirstOrDefault() ?? 0;
                todayPending = conn.QuerySql<int?>(
                    "SELECT COUNT(*) FROM TradeDocuments WHERE companyid = @cid AND TradeDocumentTypesId = @tid AND invoice_date >= @d AND orderstatusid != 1",
                    new { cid = CompanyId, tid = (int)TradeDocumentTypes.SalesChallan, d = todayStart }).FirstOrDefault() ?? 0;
                todayParties = conn.QuerySql<int?>(
                    "SELECT COUNT(DISTINCT party_id) FROM TradeDocuments WHERE companyid = @cid AND TradeDocumentTypesId = @tid AND invoice_date >= @d",
                    new { cid = CompanyId, tid = (int)TradeDocumentTypes.SalesChallan, d = todayStart }).FirstOrDefault() ?? 0;

                // === THIS WEEK stats ===
                var weekStart = todayStart.AddDays(-(int)todayStart.DayOfWeek);
                weekInvoices = conn.QuerySql<int?>(
                    "SELECT COUNT(*) FROM TradeDocuments WHERE companyid = @cid AND TradeDocumentTypesId = @tid AND invoice_date >= @d",
                    new { cid = CompanyId, tid = (int)TradeDocumentTypes.SalesChallan, d = weekStart }).FirstOrDefault() ?? 0;
                weekRevenue = conn.QuerySql<decimal?>(
                    "SELECT COALESCE(SUM(CAST(final_amount AS DECIMAL(18,2))), 0) FROM TradeDocuments WHERE companyid = @cid AND TradeDocumentTypesId = @tid AND invoice_date >= @d",
                    new { cid = CompanyId, tid = (int)TradeDocumentTypes.SalesChallan, d = weekStart }).FirstOrDefault() ?? 0;
                weekPending = conn.QuerySql<int?>(
                    "SELECT COUNT(*) FROM TradeDocuments WHERE companyid = @cid AND TradeDocumentTypesId = @tid AND invoice_date >= @d AND orderstatusid != 1",
                    new { cid = CompanyId, tid = (int)TradeDocumentTypes.SalesChallan, d = weekStart }).FirstOrDefault() ?? 0;
                weekParties = conn.QuerySql<int?>(
                    "SELECT COUNT(DISTINCT party_id) FROM TradeDocuments WHERE companyid = @cid AND TradeDocumentTypesId = @tid AND invoice_date >= @d",
                    new { cid = CompanyId, tid = (int)TradeDocumentTypes.SalesChallan, d = weekStart }).FirstOrDefault() ?? 0;

                // === THIS MONTH stats ===
                var monthStart = new DateTime(todayStart.Year, todayStart.Month, 1);
                monthInvoices = conn.QuerySql<int?>(
                    "SELECT COUNT(*) FROM TradeDocuments WHERE companyid = @cid AND TradeDocumentTypesId = @tid AND invoice_date >= @d",
                    new { cid = CompanyId, tid = (int)TradeDocumentTypes.SalesChallan, d = monthStart }).FirstOrDefault() ?? 0;
                monthRevenue = conn.QuerySql<decimal?>(
                    "SELECT COALESCE(SUM(CAST(final_amount AS DECIMAL(18,2))), 0) FROM TradeDocuments WHERE companyid = @cid AND TradeDocumentTypesId = @tid AND invoice_date >= @d",
                    new { cid = CompanyId, tid = (int)TradeDocumentTypes.SalesChallan, d = monthStart }).FirstOrDefault() ?? 0;
                monthPending = conn.QuerySql<int?>(
                    "SELECT COUNT(*) FROM TradeDocuments WHERE companyid = @cid AND TradeDocumentTypesId = @tid AND invoice_date >= @d AND orderstatusid != 1",
                    new { cid = CompanyId, tid = (int)TradeDocumentTypes.SalesChallan, d = monthStart }).FirstOrDefault() ?? 0;
                monthParties = conn.QuerySql<int?>(
                    "SELECT COUNT(DISTINCT party_id) FROM TradeDocuments WHERE companyid = @cid AND TradeDocumentTypesId = @tid AND invoice_date >= @d",
                    new { cid = CompanyId, tid = (int)TradeDocumentTypes.SalesChallan, d = monthStart }).FirstOrDefault() ?? 0;

                // === CHART: daily sales for current month ===
                var daysInMonth = DateTime.DaysInMonth(todayStart.Year, todayStart.Month);
                var dailySales = conn.QuerySql<DailyChartRow>(
                    @"SELECT DATE(invoice_date) AS day, COALESCE(SUM(CAST(final_amount AS DECIMAL(18,2))), 0) AS total
                      FROM TradeDocuments
                      WHERE companyid = @cid AND TradeDocumentTypesId = @tid AND invoice_date >= @ms AND invoice_date < @me
                      GROUP BY DATE(invoice_date) ORDER BY DATE(invoice_date)",
                    new { cid = CompanyId, tid = (int)TradeDocumentTypes.SalesChallan, ms = monthStart, me = monthStart.AddMonths(1) }).ToList();

                var salesByDay = dailySales.ToDictionary(d => d.Day.Date, d => d.Total);
                // Build ~10 data points spread across the month
                int step = Math.Max(daysInMonth / 10, 1);
                for (int day = 1; day <= daysInMonth; day += step)
                {
                    var dt = new DateTime(todayStart.Year, todayStart.Month, Math.Min(day, daysInMonth));
                    // Sum from this day to next step
                    decimal sum = 0;
                    for (int d = day; d < day + step && d <= daysInMonth; d++)
                    {
                        var dd = new DateTime(todayStart.Year, todayStart.Month, d);
                        if (salesByDay.TryGetValue(dd, out var v)) sum += v;
                    }
                    chartLabels.Add(dt.ToString("d MMM"));
                    chartValues.Add(sum);
                }

                // === LOW STOCK items ===
                lowStockItems = conn.QuerySql<LowStockItem>(
                    @"SELECT item_name AS Name, COALESCE(opening_quantity, 0) AS Quantity
                      FROM billitem WHERE item_type = 'product' AND companyid = @cid AND COALESCE(opening_quantity, 0) <= 5
                      ORDER BY opening_quantity ASC LIMIT 10",
                    new { cid = CompanyId }).ToList();
            }

            return new DashboardViewModel
            {
                TotalReceivable = TotalReceivable,
                TotalPayable = TotalPayable,
                ReceivablePartyCount = receivablePartyCount,
                PayablePartyCount = payablePartyCount,
                HasReceivables = TotalReceivable > 0,
                TotalSalesThisMonth = monthRevenue,
                SalesChartData = GetSampleChartData(),
                MostUsedReports = GetMostUsedReports(),
                Widgets = new List<WidgetViewModel>(),
                ExpenseAmount = ExpenseAmount,
                BanckAmount = BanckAmount,
                CashInHand = CashInHand,
                StockAmount = StockAmount,
                ChartLabels = chartLabels,
                ChartValues = chartValues,
                TodayInvoices = todayInvoices, TodayRevenue = todayRevenue, TodayPending = todayPending, TodayParties = todayParties,
                WeekInvoices = weekInvoices, WeekRevenue = weekRevenue, WeekPending = weekPending, WeekParties = weekParties,
                MonthInvoices = monthInvoices, MonthRevenue = monthRevenue, MonthPending = monthPending, MonthParties = monthParties,
                LowStockItems = lowStockItems,
            };
        }

        // Helper class for chart query
        private class DailyChartRow
        {
            public DateTime Day { get; set; }
            public decimal Total { get; set; }
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

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CheckPermissions()
        {
            var roleId = HttpContext.Session.GetString("RoleId");
            var isOwner = HttpContext.Session.GetString("IsOwner");
            var businessId = HttpContext.Session.GetString("BusinessId");
            var email = HttpContext.Session.GetString("Email");

            return Json(new
            {
                sessionRoleId = roleId,
                sessionIsOwner = isOwner,
                sessionBusinessId = businessId,
                sessionEmail = email,
                isAuthenticated = User.Identity?.IsAuthenticated,
                message = "If IsOwner is 'True', permissions are bypassed. Log in as a non-owner user to test."
            });
        }
    }
}
