using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Insight.Database;
using MUNEEMJI.Models;
using Dapper;
using Npgsql;

namespace MUNEEMJI.Controllers
{
    public class PaymentInController : Controller
    {
        private readonly string _connectionString;

        public PaymentInController(IConfiguration configuration)
        {
            _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
        }

        public async Task<IActionResult> Index()
        {
            using var connection = new NpgsqlConnection(_connectionString);

            var paymentInOuts =  connection.QuerySql<PaymentInOutViewModel>(@"
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
            catch(Exception ex)
            {

            }

                return Json(new { success = true, message = "Payment-In saved successfully!" });
            

           await  LoadViewBag();
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

            var parties =  connection.QuerySql<Party>(@"
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
    }

    public class Party
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}


