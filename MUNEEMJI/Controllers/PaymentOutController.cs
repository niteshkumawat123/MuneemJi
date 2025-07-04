using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MUNEEMJI.Models;
using Npgsql;
using Insight.Database;
using Dapper;

namespace MUNEEMJI.Controllers
{
    public class PaymentOutController: Controller
    {
        private readonly string _connectionString;

        public PaymentOutController(IConfiguration configuration)
        {
            _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
        }

        public async Task<IActionResult> Index()
        {
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
                    p.PrintShare
                FROM PaymentInOut p
                LEFT JOIN parties pt ON p.PartyId = pt.Id
                ORDER BY p.Date DESC
            ").ToList();

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
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    string query = @"
                            INSERT INTO PaymentInOut 
                            (Date, RefNo, PartyId, CategoryName, Type, Total, ReceivedPaid, Balance, PrintShare, PaymentType, Description, CreatedDate)
                            VALUES 
                            (@Date, @RefNo, @PartyId, @CategoryName, @Type, @Total, @ReceivedPaid, @Balance, @PrintShare, @PaymentType, @Description, @CreatedDate)
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
                        CreatedDate = model.CreatedDate
                    });
                }

            }
            catch (Exception ex)
            {

            }

            return Json(new { success = true, message = "Payment-In saved successfully!" });


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

   

}

