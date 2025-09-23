using Dapper;
using Insight.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using MUNEEMJI.Models;
using MUNEEMJI.Services;
using Npgsql;

namespace MUNEEMJI.Controllers
{
    [Authorize]
    public class PaymentOutController: Controller
    {
        private readonly string _connectionString;
        private readonly ICompanyTenancy _tenancy;
        public PaymentOutController(IConfiguration configuration, ICompanyTenancy company)
        {
            _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
            _tenancy = company;
        }

        public async Task<IActionResult> Index()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var CompanyId = _tenancy.GetCurrentCompanyId();

            var paymentInOuts = connection.QuerySql<PaymentInOutViewModel>(@"
                SELECT 
                    p.Id,
                    p.Date,
                    p.RefNo,
                    pt.party_name as PartyName,
                    p.CategoryName,
                    p.Type,
                    p.Total,
                    p.ReceivedPaid,
                    p.Balance,
                    p.PrintShare
                FROM PaymentInOut p
                LEFT JOIN parties pt ON p.PartyId = pt.Id where p.companyid =  @p_companyid and moduleid = @p_moduleid
                ORDER BY p.Date DESC
            ",new { p_companyid   = CompanyId , p_moduleid  = (int)TradeDocumentTypes.PaymentOut }).ToList();

            return View(paymentInOuts);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadViewBag();
            return PartialView("_Create");
        }

        [HttpPost]
        public async Task<IActionResult> Create(PaymentInOutModel model)
        {
            try
            {
                var CompanyId = _tenancy.GetCurrentCompanyId();
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    string query = @"
                            INSERT INTO PaymentInOut 
                            (Date, RefNo, PartyId, CategoryName, Type, Total, ReceivedPaid, Balance, PrintShare, PaymentType, Description, CreatedDate,CompanyId,moduleid)
                            VALUES 
                            (@Date, @RefNo, @PartyId, @CategoryName, @Type, @Total, @ReceivedPaid, @Balance, @PrintShare, @PaymentType, @Description, @CreatedDate,@p_companyid,@p_moduleid)
                                ";

                    connection.ExecuteSql(query, new
                    {
                        Date = model.Date.ToUniversalTime(),
                        RefNo = model.RefNo,
                        PartyId = model.PartyId,
                        CategoryName = model.CategoryName,
                        Type = model.Type,
                        Total = model.Total,
                        ReceivedPaid = model.ReceivedPaid,
                        Balance = model.Balance,
                        PrintShare = model.PrintShare,
                        PaymentType = model.PaymentType,
                        Description = model.Description,
                        CreatedDate = model.CreatedDate,
                        p_companyid  = CompanyId,
                        p_moduleid = (int)TradeDocumentTypes.PaymentOut
                    });
                }

            }
            catch (Exception ex)
            {

            }

            //return Json(new { success = true, message = "Payment-In saved successfully!" });


            await LoadViewBag();
            return PartialView("_Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var paymentInOut = await connection.QuerySingleOrDefaultAsync<PaymentInOutModel>(@"
                SELECT * FROM PaymentInOut WHERE Id = @Id
            ", new { Id = id });

            if (paymentInOut == null)
                return NotFound();

            await LoadViewBag();
            return PartialView("_Edit", paymentInOut);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PaymentInOutModel model)
        {
            if (ModelState.IsValid)
            {
                using var connection = new SqlConnection(_connectionString);

                await connection.ExecuteSqlAsync(@"
                    UPDATE PaymentInOut 
                    SET Date = @Date, RefNo = @RefNo, PartyId = @PartyId, CategoryName = @CategoryName, 
                        Type = @Type, Total = @Total, ReceivedPaid = @ReceivedPaid, Balance = @Balance, 
                        PrintShare = @PrintShare, PaymentType = @PaymentType, Description = @Description
                    WHERE Id = @Id
                ", model);

                return Json(new { success = true, message = "Payment-In updated successfully!" });
            }

            await LoadViewBag();
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            await connection.ExecuteScalarAsync(@"
                DELETE FROM PaymentInOut WHERE Id = @Id
            ", new { Id = id });

            return Json(new { success = true, message = "Payment-In deleted successfully!" });
        }

        private async Task LoadViewBag()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var Companyid = _tenancy.GetCurrentCompanyId();
            var parties = connection.QuerySql<Party>(@"
                SELECT Id,party_name as Name FROM parties where companyid = @Companyid   ORDER BY party_name 
            ", new { Companyid  =  Companyid}).ToList();

            ViewBag.Parties = parties.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToList();
        }
    }

   

}

