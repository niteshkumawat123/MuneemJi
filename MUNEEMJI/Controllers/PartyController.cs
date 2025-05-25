using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using Npgsql;
using System.IO;

namespace MUNEEMJI.Controllers
{
  
        public class PartyController : Controller
        {


        private readonly string _connectionString = "Host=13.232.132.81;Port=5432;Database=MyDatabase;Username=betauser;Password=sdmVertex+beta@2022";

            [HttpGet]
            public IActionResult Add()
            {
                return View(new PartyModel());
            }

        [HttpPost]
        public IActionResult Add(PartyModel model, string save)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var conn = new NpgsqlConnection(_connectionString))
                    {
                        conn.Open();

                        string query = @"
                            INSERT INTO parties 
                                (party_name, gstin, phone_number, gst_type, state, email, billing_address, shipping_address, is_shipping_disabled)
                            VALUES 
                                (@party_name, @gstin, @phone_number, @gst_type, @state, @email, @billing_address, @shipping_address, @is_shipping_disabled);";

                        using (var cmd = new NpgsqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("party_name", model.PartyName ?? "");
                            cmd.Parameters.AddWithValue("gstin", model.GSTIN ?? "");
                            cmd.Parameters.AddWithValue("phone_number", model.PhoneNumber ?? "");
                            cmd.Parameters.AddWithValue("gst_type", model.GSTType ?? "");
                            cmd.Parameters.AddWithValue("state", model.State ?? "");
                            cmd.Parameters.AddWithValue("email", model.Email ?? "");
                            cmd.Parameters.AddWithValue("billing_address", model.BillingAddress ?? "");
                            cmd.Parameters.AddWithValue("shipping_address", model.ShippingAddress ?? "");
                            cmd.Parameters.AddWithValue("is_shipping_disabled", model.IsShippingDisabled);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    if (save == "new")
                    {
                        TempData["Message"] = "Party saved. Ready to add new.";
                        return RedirectToAction("Add");
                    }

                    TempData["Message"] = "Party saved successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Database error: " + ex.Message);
                }
            }

            return View(model);
        }

        public IActionResult Index(int? id)
        {
            var model = new PartyViewModel();
            model.Parties = new List<PartyModel>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();  // establish PostgreSQL connection:contentReference[oaicite:3]{index=3}
                              // 1) Query all parties
                string sql = "SELECT id, party_name, balance FROM parties ORDER BY party_name";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.Parties.Add(new PartyModel
                            {
                                Id = reader.GetInt32(0),
                                PartyName = reader.GetString(1),
                                Balance = 100 /*reader.GetDecimal(2)*/
                            });
                        }
                    }
                }

                id = model.Parties.Select(x => x.Id).FirstOrDefault();
                // 2) If a party is selected (id passed), fetch its details
                if (id.HasValue)
                {
                    string detailSql = "SELECT phone_number, email, gstin, billing_address FROM parties WHERE id = @id";
                    using (var cmd2 = new NpgsqlCommand(detailSql, conn))
                    {
                        cmd2.Parameters.AddWithValue("id", id.Value);
                        using (var reader2 = cmd2.ExecuteReader())
                        {
                            if (reader2.Read())
                            {
                                model.SelectedParty = new PartyModel
                                {
                                    Id = id.Value,
                                    PhoneNumber = reader2.GetString(0),
                                    Email = reader2.GetString(1),
                                    GSTIN = reader2.GetString(2),
                                    BillingAddress = reader2.GetString(3)
                                };
                            }
                        }
                    }
                }
            }
            return View(model);
        }

    }

}
