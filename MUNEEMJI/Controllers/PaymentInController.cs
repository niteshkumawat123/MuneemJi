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
    public class PaymentInController : Controller
    {
        private readonly string _connectionString;
        private readonly ICompanyTenancy companyTenancy;
        public PaymentInController(IConfiguration configuration, ICompanyTenancy tenancy)
        {
            _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
            companyTenancy = tenancy;
        }

        public async Task<IActionResult> Index()
        {
            var CompanyId = companyTenancy.GetCurrentCompanyId();
            using var connection = new NpgsqlConnection(_connectionString);

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
                    p.PrintShare,
                    p.PaymentType
                FROM PaymentInOut p
                LEFT JOIN parties pt ON p.PartyId = pt.Id where p.companyid = @p_companyid  and p.moduleid = @p_moduleid
                ORDER BY p.Date DESC
            ", new { p_companyid = CompanyId, p_moduleid = TradeDocumentTypes.PaymentIn }).ToList();

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
                var CompanyId = companyTenancy.GetCurrentCompanyId();

                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    string query = @"
                            INSERT INTO PaymentInOut 
                            (Date, RefNo, PartyId, CategoryName, Type, Total, ReceivedPaid, Balance, PrintShare, PaymentType, Description, CreatedDate,companyid,moduleid)
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
                        p_companyid = CompanyId,
                        p_moduleid = (int)TradeDocumentTypes.PaymentIn
                    });
                }

            }
            catch (Exception ex)
            {

            }
            //return Json(new { success = true, message = "Payment-In saved successfully!" });


            //await  LoadViewBag();
            return RedirectToAction(nameof(Index));
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
            paymentInOut.ViewTypeId = (int)ViewTypeEnum.Edit;
            await LoadViewBag();
            return PartialView("_Edit", paymentInOut);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var paymentInOut = await connection.QuerySingleOrDefaultAsync<PaymentInOutModel>(@"
                SELECT * FROM PaymentInOut WHERE Id = @Id
            ", new { Id = id });

            if (paymentInOut == null)
                return NotFound();
            paymentInOut.ViewTypeId = (int)ViewTypeEnum.View;
            await LoadViewBag();
            return PartialView("_Edit", paymentInOut);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PaymentInOutModel model)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {


                    var sql = @"
                                UPDATE paymentinout 
                                SET 
                                    date = @Date,
                                    refno = @RefNo,
                                    partyid = @PartyId,
                                    categoryname = @CategoryName,
                                    type = @Type,
                                    total = @Total,
                                    receivedpaid = @ReceivedPaid,
                                    balance = @Balance,
                                    printshare = @PrintShare,
                                    paymenttype = @PaymentType,
                                    description = @Description
                                WHERE id = @Id;
                                 ";

                    connection.ExecuteSql(sql, new
                    {
                        Id = model.Id,
                        Date = model.Date.ToUniversalTime(), // Ensures UTC
                        RefNo = model.RefNo,
                        PartyId = model.PartyId,
                        CategoryName = model.CategoryName,
                        Type = model.Type,
                        Total = model.Total,
                        ReceivedPaid = model.ReceivedPaid,
                        Balance = model.Balance,
                        PrintShare = model.PrintShare,
                        PaymentType = model.PaymentType,
                        Description = model.Description

                    });
                }

                return RedirectToAction(nameof(Index));
            }

            await LoadViewBag();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);

            await connection.ExecuteScalarAsync(@"
                DELETE FROM PaymentInOut WHERE Id = @Id
            ", new { Id = id });

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.ExecuteSql(@"
                                     DELETE FROM PaymentInOut WHERE Id = @Id
                                     ", new { Id = id });
                }



                return Json(new { success = true, message = "Record deleted successfully!" });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });

            }

        }

        private async Task LoadViewBag()
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var parties = connection.QuerySql<Party>(@"
                SELECT Id,party_name as Name FROM parties ORDER BY party_name
            ").ToList();

            ViewBag.Parties = parties.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            }).ToList();
        }
    }

    public class PaymentInOutViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string RefNo { get; set; }
        public string PartyName { get; set; }
        public string CategoryName { get; set; }
        public string Type { get; set; }
        public decimal Total { get; set; }
        public decimal ReceivedPaid { get; set; }
        public decimal Balance { get; set; }
        public string PrintShare { get; set; }
        public string paymenttype { get; set; }
    }

    public class Party
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}


