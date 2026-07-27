using Insight.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Repositories;
using MUNEEMJI.Services;
using Npgsql;
using static MUNEEMJI.Models.ItemModel;

namespace MUNEEMJI.Controllers
{
    [Authorize]
    public class UnitsController : Controller
    {
        [HttpPost]
        public IActionResult UnitCreate([FromBody] Unit model)
        {
            var _dbconnectionstrig = MUNEEMJI.DbConfig.ConnectionString;

            try
            {
                using (var Conn = new NpgsqlConnection(_dbconnectionstrig))
                {
                    Conn.Open();

                    // Check for duplicate unit name
                    string duplicateCheckQuery = model.Id > 0
                        ? "SELECT COUNT(*) FROM units WHERE LOWER(TRIM(fullname)) = LOWER(TRIM(@p_name)) AND id != @p_id"
                        : "SELECT COUNT(*) FROM units WHERE LOWER(TRIM(fullname)) = LOWER(TRIM(@p_name))";

                    using (var checkCmd = new NpgsqlCommand(duplicateCheckQuery, Conn))
                    {
                        checkCmd.Parameters.AddWithValue("p_name", model.FullName ?? "");
                        if (model.Id > 0)
                            checkCmd.Parameters.AddWithValue("p_id", model.Id);

                        var count = (long)(checkCmd.ExecuteScalar() ?? 0);
                        if (count > 0)
                        {
                            return Json(new { success = false, message = "A unit with this name already exists. Please use a different name." });
                        }
                    }

                    var insertquery = string.Empty;
                    if (model.Id > 0)
                    {
                        insertquery = "update units set name = @p_name , fullname = @p_name , shortname = @p_shortname  where id = @p_id ";

                    }
                    else
                    {
                        insertquery = "insert into units(name,fullname,shortname)values(@p_name,@p_name,@p_shortname) ";
                    }

                    Conn.ExecuteSql(insertquery, new { p_name = model.FullName, p_shortname = model.ShortName, p_id = model.Id });
                }

                return Json(new { success = true, message = "Unit created successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            try
            {
                await Task.Delay(1);
                using (var conn = new NpgsqlConnection(MUNEEMJI.DbConfig.ConnectionString))
                {

                    string query = " delete from units where  id =@p_id ";

                    conn.ExecuteSql(query, new { p_id = id });


                }
                return Json(new { success = true, message = "Unit has been deleted successfully!" });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });

            }
        }

        [HttpGet]
        public async Task<IActionResult> Unit()
        {
            ItemViewModel ViewModel = new ItemViewModel();
            ViewModel.Units = new List<UnitViewModel>();

            try
            {
                await Task.Delay(1);

                using (var conn = new NpgsqlConnection(MUNEEMJI.DbConfig.ConnectionString))
                {
                    conn.Open();
                    string query = @"SELECT id, fullname , shortname , name
                     FROM units ORDER BY fullname";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ViewModel.Units.Add(new UnitViewModel
                                {
                                    Id = reader.GetInt32(0),
                                    FullName = reader.GetString(1),
                                    ShortName = reader.GetString(2),
                                    Name   =  reader.GetString(3)
                                });
                            }
                        }
                    }
                }



            }
            catch (Exception ex)
            {

            }
            return View(ViewModel);

        }
    }
}
