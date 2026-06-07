using Dapper;
using Insight.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models;
using MUNEEMJI.Repositories;
using MUNEEMJI.Services;
using Npgsql;
using Npgsql.Replication.PgOutput.Messages;
using NuGet.Protocol.Plugins;
using SkiaSharp;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace MUNEEMJI.Controllers
{
    [Authorize]
    public class PartyController : Controller
    {


        private readonly string _connectionString = MUNEEMJI.DbConfig.ConnectionString;
        private readonly ICompanyTenancy _CompayTenancy;
        public IParty _party;
        private readonly IErrorLogService _errorLogService;
        public PartyController(ICompanyTenancy companyTenancy, IParty party, IErrorLogService errorLogService)
        {
            _CompayTenancy = companyTenancy;
            _party = party;
            _errorLogService = errorLogService;
        }

        [HttpGet]
        public IActionResult Add(int id = 0)
        {
            StateController stateObj = new StateController();

            var data = new PartyModel();
            data.States = stateObj.StateDropDown();
            if (id > 0)
            {
                data = PartGetById(id);
                data.States = stateObj.StateDropDown();
            }


            return View(data);
        }

        [HttpPost]
        public IActionResult Add(PartyModel model, string save)
        {
            try
            {
                var companyId = _CompayTenancy.GetCurrentCompanyId();

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query;

                    if (model.Id > 0)
                    {
                        // UPDATE query
                        query = @"
                                  UPDATE parties SET 
                                    party_name = @party_name,
                                    gstin = @gstin,
                                    phone_number = @phone_number,
                                    gst_type = @gst_type,
                                    state = @state,
                                    email = @email,
                                    billing_address = @billing_address,
                                    shipping_address = @shipping_address,
                                    is_shipping_disabled = @is_shipping_disabled,
                                    opening_balance = @opening_balance,
                                    as_of_date = @as_of_date,
                                    has_custom_credit_limit = @has_custom_credit_limit,
                                    credit_limit = @credit_limit,
                                    additional_field1_enabled = @additional_field1_enabled,
                                    additional_field1_value = @additional_field1_value,
                                    additional_field2_enabled = @additional_field2_enabled,
                                    additional_field2_value = @additional_field2_value,
                                    additional_field3_enabled = @additional_field3_enabled,
                                    additional_field3_value = @additional_field3_value,
                                    additional_field4_enabled = @additional_field4_enabled,
                                    additional_field4_value = @additional_field4_value,
                                    PartyGroupId            = @p_PartyGroupId,
                                    PartyGroup              = @p_PartyGroup
                                  WHERE id = @id;";
                    }
                    else
                    {
                        // INSERT query
                        query = @"
                                    INSERT INTO parties (
                                    party_name, gstin, phone_number, gst_type, state, email, billing_address, shipping_address, is_shipping_disabled,
                                    opening_balance, as_of_date, has_custom_credit_limit, credit_limit,
                                    additional_field1_enabled, additional_field1_value,
                                    additional_field2_enabled, additional_field2_value,
                                    additional_field3_enabled, additional_field3_value,
                                    additional_field4_enabled, additional_field4_value,
                                    companyid,
                                    PartyGroupId,
                                    PartyGroup
                                     ) VALUES (
                                    @party_name, @gstin, @phone_number, @gst_type, @state, @email, @billing_address, @shipping_address, @is_shipping_disabled,
                                    @opening_balance, @as_of_date, @has_custom_credit_limit, @credit_limit,
                                    @additional_field1_enabled, @additional_field1_value,
                                    @additional_field2_enabled, @additional_field2_value,
                                    @additional_field3_enabled, @additional_field3_value,
                                    @additional_field4_enabled, @additional_field4_value,
                                    @p_companyid,
                                    @p_PartyGroupId,
                                    @p_PartyGroup
                                  );";
                    }

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        // Common parameters
                        cmd.Parameters.AddWithValue("party_name", model.PartyName ?? "");
                        cmd.Parameters.AddWithValue("gstin", model.GSTIN ?? "");
                        cmd.Parameters.AddWithValue("phone_number", model.PhoneNumber ?? "");
                        cmd.Parameters.AddWithValue("gst_type", model.GSTType ?? "");
                        cmd.Parameters.AddWithValue("state", model.State ?? "");
                        cmd.Parameters.AddWithValue("email", model.Email ?? "");
                        cmd.Parameters.AddWithValue("billing_address", model.BillingAddress ?? "");
                        cmd.Parameters.AddWithValue("shipping_address", model.ShippingAddress ?? "");
                        cmd.Parameters.AddWithValue("is_shipping_disabled", model.IsShippingDisabled);
                        cmd.Parameters.AddWithValue("opening_balance", (object)model.OpeningBalance ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("as_of_date", (object)model.AsOfDate ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("has_custom_credit_limit", model.HasCustomCreditLimit);
                        cmd.Parameters.AddWithValue("credit_limit", model.HasCustomCreditLimit && model.CreditLimit.HasValue
                            ? (object)model.CreditLimit.Value
                            : DBNull.Value);
                        cmd.Parameters.AddWithValue("additional_field1_enabled", model.AdditionalField1Enabled);
                        cmd.Parameters.AddWithValue("additional_field1_value", model.AdditionalField1Enabled
                            ? (object)(model.AdditionalField1Value ?? "")
                            : DBNull.Value);
                        cmd.Parameters.AddWithValue("additional_field2_enabled", model.AdditionalField2Enabled);
                        cmd.Parameters.AddWithValue("additional_field2_value", model.AdditionalField2Enabled
                            ? (object)(model.AdditionalField2Value ?? "")
                            : DBNull.Value);
                        cmd.Parameters.AddWithValue("additional_field3_enabled", model.AdditionalField3Enabled);
                        cmd.Parameters.AddWithValue("additional_field3_value", model.AdditionalField3Enabled
                            ? (object)(model.AdditionalField3Value ?? "")
                            : DBNull.Value);
                        cmd.Parameters.AddWithValue("additional_field4_enabled", model.AdditionalField4Enabled);
                        cmd.Parameters.AddWithValue("additional_field4_value", model.AdditionalField4Enabled && model.AdditionalField4Value.HasValue
                            ? (object)model.AdditionalField4Value.Value
                            : DBNull.Value);
                        cmd.Parameters.AddWithValue("p_companyid", companyId);
                        cmd.Parameters.AddWithValue("p_PartyGroupId", model.PartyGroupId);
                        cmd.Parameters.AddWithValue("p_PartyGroup", model.PartyGroup!=null ? model.PartyGroup:"");

                        if (model.Id > 0)
                        {
                            cmd.Parameters.AddWithValue("id", model.Id);
                        }

                        cmd.ExecuteNonQuery();
                    }
                }

                if (save == "new")
                {
                    return Json(new
                    {
                        success = true,
                        IsSaveAgain = true,
                        message = model.Id > 0 ? "Party updated. Ready to add new." : "Party saved. Ready to add new.",
                        id = model.Id,
                        name = model.PartyName
                    });
                }

                return Json(new
                {
                    success = true,
                    IsSaveAgain = false,
                    message = model.Id > 0 ? "Party updated successfully." : "Party saved successfully.",
                    id = model.Id,
                    name = model.PartyName
                });
            }
            catch (Exception ex)
            {
                _errorLogService.LogErrorAsync($"Party Add Error: {ex.Message}", ex.StackTrace).Wait();
                // ? RETURN JSON ERROR
                return Json(new
                {
                    success = false,
                    message = "Database error: " + ex.Message
                });
            }

        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            PartyModel model = null;

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT * FROM parties WHERE id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new PartyModel
                            {
                                Id = id,
                                PartyName = reader["party_name"].ToString(),
                                GSTIN = reader["gstin"].ToString(),
                                PhoneNumber = reader["phone_number"].ToString(),
                                GSTType = reader["gst_type"].ToString(),
                                State = reader["state"].ToString(),
                                Email = reader["email"].ToString(),
                                BillingAddress = reader["billing_address"].ToString(),
                                ShippingAddress = reader["shipping_address"].ToString(),
                                IsShippingDisabled = Convert.ToBoolean(reader["is_shipping_disabled"]),
                                OpeningBalance = reader["opening_balance"] != DBNull.Value ? Convert.ToDecimal(reader["opening_balance"]) : (decimal?)null,
                                AsOfDate = reader["as_of_date"] != DBNull.Value ? Convert.ToDateTime(reader["as_of_date"]) : (DateTime?)null,
                                HasCustomCreditLimit = Convert.ToBoolean(reader["has_custom_credit_limit"]),
                                CreditLimit = reader["credit_limit"] != DBNull.Value ? Convert.ToDecimal(reader["credit_limit"]) : (decimal?)null,
                                AdditionalField1Enabled = Convert.ToBoolean(reader["additional_field1_enabled"]),
                                AdditionalField1Value = reader["additional_field1_value"]?.ToString(),
                                AdditionalField2Enabled = Convert.ToBoolean(reader["additional_field2_enabled"]),
                                AdditionalField2Value = reader["additional_field2_value"]?.ToString(),
                                AdditionalField3Enabled = Convert.ToBoolean(reader["additional_field3_enabled"]),
                                AdditionalField3Value = reader["additional_field3_value"]?.ToString(),
                                AdditionalField4Enabled = Convert.ToBoolean(reader["additional_field4_enabled"]),
                                AdditionalField4Value = reader["additional_field4_value"] != DBNull.Value ? Convert.ToDateTime(reader["additional_field4_value"]) : (DateTime?)null
                            };
                        }
                    }
                }
            }

            if (model == null)
            {
                TempData["Error"] = "Party not found.";
                return RedirectToAction("Index");
            }

            return View("Add", model); // You can reuse the Add.cshtml for both Add and Edit
        }

        [HttpPost]
        public IActionResult Edit(PartyModel model)
        {
            if (!ModelState.IsValid)
                return View("Add", model); // Reuse Add.cshtml

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
                UPDATE parties SET
                    party_name = @party_name,
                    gstin = @gstin,
                    phone_number = @phone_number,
                    gst_type = @gst_type,
                    state = @state,
                    email = @email,
                    billing_address = @billing_address,
                    shipping_address = @shipping_address,
                    is_shipping_disabled = @is_shipping_disabled,
                    opening_balance = @opening_balance,
                    as_of_date = @as_of_date,
                    has_custom_credit_limit = @has_custom_credit_limit,
                    credit_limit = @credit_limit,
                    additional_field1_enabled = @additional_field1_enabled,
                    additional_field1_value = @additional_field1_value,
                    additional_field2_enabled = @additional_field2_enabled,
                    additional_field2_value = @additional_field2_value,
                    additional_field3_enabled = @additional_field3_enabled,
                    additional_field3_value = @additional_field3_value,
                    additional_field4_enabled = @additional_field4_enabled,
                    additional_field4_value = @additional_field4_value
                WHERE party_id = @party_id;";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("party_id", model.Id);
                        cmd.Parameters.AddWithValue("party_name", model.PartyName ?? "");
                        cmd.Parameters.AddWithValue("gstin", model.GSTIN ?? "");
                        cmd.Parameters.AddWithValue("phone_number", model.PhoneNumber ?? "");
                        cmd.Parameters.AddWithValue("gst_type", model.GSTType ?? "");
                        cmd.Parameters.AddWithValue("state", model.State ?? "");
                        cmd.Parameters.AddWithValue("email", model.Email ?? "");
                        cmd.Parameters.AddWithValue("billing_address", model.BillingAddress ?? "");
                        cmd.Parameters.AddWithValue("shipping_address", model.ShippingAddress ?? "");
                        cmd.Parameters.AddWithValue("is_shipping_disabled", model.IsShippingDisabled);

                        cmd.Parameters.AddWithValue("opening_balance", model.OpeningBalance.HasValue ? (object)model.OpeningBalance.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("as_of_date", model.AsOfDate.HasValue ? (object)model.AsOfDate.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("has_custom_credit_limit", model.HasCustomCreditLimit);
                        cmd.Parameters.AddWithValue("credit_limit", model.HasCustomCreditLimit && model.CreditLimit.HasValue
                            ? (object)model.CreditLimit.Value
                            : DBNull.Value);

                        cmd.Parameters.AddWithValue("additional_field1_enabled", model.AdditionalField1Enabled);
                        cmd.Parameters.AddWithValue("additional_field1_value", model.AdditionalField1Enabled
                            ? (object)(model.AdditionalField1Value ?? "")
                            : DBNull.Value);

                        cmd.Parameters.AddWithValue("additional_field2_enabled", model.AdditionalField2Enabled);
                        cmd.Parameters.AddWithValue("additional_field2_value", model.AdditionalField2Enabled
                            ? (object)(model.AdditionalField2Value ?? "")
                            : DBNull.Value);

                        cmd.Parameters.AddWithValue("additional_field3_enabled", model.AdditionalField3Enabled);
                        cmd.Parameters.AddWithValue("additional_field3_value", model.AdditionalField3Enabled
                            ? (object)(model.AdditionalField3Value ?? "")
                            : DBNull.Value);

                        cmd.Parameters.AddWithValue("additional_field4_enabled", model.AdditionalField4Enabled);
                        cmd.Parameters.AddWithValue("additional_field4_value", model.AdditionalField4Enabled && model.AdditionalField4Value.HasValue
                            ? (object)model.AdditionalField4Value.Value
                            : DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["Message"] = "Party updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _errorLogService.LogErrorAsync($"Party Edit Error: {ex.Message}", ex.StackTrace).Wait();
                ModelState.AddModelError("", "Database error: " + ex.Message);
                return View("Add", model);
            }
        }


        public IActionResult Index(int? id)
        {
            var model = new PartyViewModel();
            model.Parties = new List<PartyModel>();
            var companyId = _CompayTenancy.GetCurrentCompanyId();
            List<PurchaseBill> TransectionList = new List<PurchaseBill>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();  // establish PostgreSQL connection:contentReference[oaicite:3]{index=3}
                              // 1) Query all parties
                string sql = "SELECT id, party_name,opening_balance FROM parties where companyid = @p_companyid ORDER BY party_name";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("p_companyid", companyId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.Parties.Add(new PartyModel
                            {
                                Id = reader.GetInt32(0),
                                PartyName = reader.GetString(1),
                                Balance = reader.IsDBNull(2) ? (decimal?)null : reader.GetDecimal(2),
                                OpeningBalance = reader.IsDBNull(2) ? (decimal?)null : reader.GetDecimal(2),
                            });
                        }
                    }
                }

                if (id == 0 || id == null)
                {
                    id = model.Parties.Select(x => x.Id).FirstOrDefault();
                }
                // 2) If a party is selected (id passed), fetch its details
                if (id > 0)
                {
                    string detailSql = "SELECT phone_number, email, gstin, billing_address,party_name,opening_balance FROM parties WHERE id = @id";
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
                                    BillingAddress = reader2.GetString(3),
                                    PartyName = reader2.GetString(4),
                                    OpeningBalance = reader2.IsDBNull(5) ? (decimal?)null : reader2.GetDecimal(5)

                                };
                            }
                        }
                    }

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
                                         td.tradedocumenttypesid
                                     FROM public.tradedocuments as td 
                                     LEFT JOIN parties as pt ON td.partyid = pt.id  
                                     WHERE  td.companyid = @p_companyid and td.partyid = @p_partyid;";
                    TransectionList = conn.QuerySql<PurchaseBill>(query,
                        new
                        {
                            p_companyid = companyId,
                            p_partyid =  id
                        }).ToList();

                    model.PartyTransection = TransectionList;

                }
            }

            return View(model);
        }


        public PartyModel PartGetById(int id)
        {
            PartyModel model = null;

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT * FROM parties WHERE id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model = new PartyModel
                            {
                                Id = id,
                                PartyName = reader["party_name"].ToString(),
                                GSTIN = reader["gstin"].ToString(),
                                PhoneNumber = reader["phone_number"].ToString(),
                                GSTType = reader["gst_type"].ToString(),
                                State = reader["state"].ToString(),
                                Email = reader["email"].ToString(),
                                BillingAddress = reader["billing_address"].ToString(),
                                ShippingAddress = reader["shipping_address"].ToString(),
                                IsShippingDisabled = Convert.ToBoolean(reader["is_shipping_disabled"]),
                                OpeningBalance = reader["opening_balance"] != DBNull.Value ? Convert.ToDecimal(reader["opening_balance"]) : (decimal?)null,
                                AsOfDate = reader["as_of_date"] != DBNull.Value ? Convert.ToDateTime(reader["as_of_date"]) : (DateTime?)null,
                                HasCustomCreditLimit = Convert.ToBoolean(reader["has_custom_credit_limit"]),
                                CreditLimit = reader["credit_limit"] != DBNull.Value ? Convert.ToDecimal(reader["credit_limit"]) : (decimal?)null,
                                AdditionalField1Enabled = Convert.ToBoolean(reader["additional_field1_enabled"]),
                                AdditionalField1Value = reader["additional_field1_value"]?.ToString(),
                                AdditionalField2Enabled = Convert.ToBoolean(reader["additional_field2_enabled"]),
                                AdditionalField2Value = reader["additional_field2_value"]?.ToString(),
                                AdditionalField3Enabled = Convert.ToBoolean(reader["additional_field3_enabled"]),
                                AdditionalField3Value = reader["additional_field3_value"]?.ToString(),
                                AdditionalField4Enabled = Convert.ToBoolean(reader["additional_field4_enabled"]),
                                AdditionalField4Value = reader["additional_field4_value"] != DBNull.Value ? Convert.ToDateTime(reader["additional_field4_value"]) : (DateTime?)null,
                                PartyGroup = reader["partygroup"] != DBNull.Value ? Convert.ToString(reader["partygroup"]) : string.Empty,
                                PartyGroupId = reader["partygroupid"] != DBNull.Value ? Convert.ToInt32(reader["partygroupid"]) : 0
                            };
                        }
                    }
                }
            }

            return model;
        }


        public async Task<IActionResult> GetPartyDropDownAsync()
        {
            var companyId = _CompayTenancy.GetCurrentCompanyId();
            try
            {
                var Record = await _party.GetPartyDropDownAsync(companyId);
                return Ok(Record);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public IActionResult GetPartyDetailsById(int id)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"SELECT ps.party_name, ps.billing_address, ps.shipping_address, ps.phone_number, ps.gstin,
                                            ps.stateid, ss.name as state_name, ss.code as state_code
                                     FROM parties ps
                                     LEFT JOIN states ss ON ss.id = ps.stateid
                                     WHERE ps.id = @p_id";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("p_id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return Json(new
                                {
                                    success = true,
                                    partyName = reader["party_name"]?.ToString() ?? "",
                                    billingAddress = reader["billing_address"]?.ToString() ?? "",
                                    shippingAddress = reader["shipping_address"]?.ToString() ?? "",
                                    phoneNumber = reader["phone_number"]?.ToString() ?? "",
                                    gstin = reader["gstin"]?.ToString() ?? "",
                                    stateId = reader["stateid"] != DBNull.Value ? Convert.ToInt32(reader["stateid"]) : 0,
                                    stateName = reader["state_name"]?.ToString() ?? "",
                                    stateCode = reader["state_code"]?.ToString() ?? "",
                                    stateOfSupply = (reader["state_code"]?.ToString() ?? "") + "-" + (reader["state_name"]?.ToString() ?? "")
                                });
                            }
                        }
                    }
                }
                return Json(new { success = false, message = "Party not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        public void DeleteParty(int id = 0)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                string query = "DELETE FROM parties WHERE id = @p_id";
                conn.QuerySql(query, new { p_id = id });
            }
        }

        [HttpGet]
        public async Task<List<PartyGroupModel>> GetPartyGroups()
        {

            List<PartyGroupModel> Model = new List<PartyGroupModel>();
            try
            {
                using (var Conn = new NpgsqlConnection(_connectionString))
                {
                    var QueryString = "select * from partygroup ";
                    Model = Conn.QuerySql<PartyGroupModel>(QueryString).ToList();
                }
            }
            catch (Exception ex)
            {

            }

            return Model;
        }

        [HttpPost]
        public JsonResult CreatePartyGroup(string name)
        {
            try
            {


                using (var Conn = new NpgsqlConnection(_connectionString))
                {
                    var SaveQuery = " insert into partygroup(groupname)VALUES(@p_partygroup) ";

                    Conn.ExecuteSql(SaveQuery, new { p_partygroup = name });
                }
            }
            catch (Exception ex)
            {

            }
            return Json(new { success = true });
        }

        // ?? GSTIN Lookup: check cache first, then call external API ??
        [HttpGet]
        public async Task<IActionResult> LookupGstin(string gstin)
        {
            if (string.IsNullOrWhiteSpace(gstin) || gstin.Length != 15)
                return Json(new { success = false, message = "Invalid GSTIN format. Must be 15 characters." });

            gstin = gstin.Trim().ToUpper();

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                // Step 1: Check cache
                var cacheQuery = @"SELECT legal_name, trade_name, status, full_address, state_name, city, pincode, district
                                   FROM gstin_cache WHERE gstin = @gstin LIMIT 1";
                using var cacheCmd = new NpgsqlCommand(cacheQuery, conn);
                cacheCmd.Parameters.AddWithValue("gstin", gstin);

                using var reader = await cacheCmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var cached = new
                    {
                        success = true,
                        source = "cache",
                        legalName = reader["legal_name"]?.ToString() ?? "",
                        tradeName = reader["trade_name"]?.ToString() ?? "",
                        status = reader["status"]?.ToString() ?? "",
                        address = reader["full_address"]?.ToString() ?? "",
                        state = reader["state_name"]?.ToString() ?? "",
                        city = reader["city"]?.ToString() ?? "",
                        pincode = reader["pincode"]?.ToString() ?? "",
                        district = reader["district"]?.ToString() ?? ""
                    };
                    return Json(cached);
                }
                await reader.CloseAsync();

                // Step 2: Call external API
                //  https://sheet.gstincheck.co.in/user-dashboard
                //  email :- niteshkumawat0004@gmail.com
                // password:-  Nitesh@123

                const string apiKey = "750dd0eec74dcc8a48fea526ccb1239a";
                var apiUrl = $"http://sheet.gstincheck.co.in/check/{apiKey}/{gstin}";

                using var http = new System.Net.Http.HttpClient();
                http.Timeout = TimeSpan.FromSeconds(15);
                var response = await http.GetStringAsync(apiUrl);

                var json = System.Text.Json.JsonDocument.Parse(response);
                var root = json.RootElement;

                var flag = root.GetProperty("flag").GetBoolean();
                if (!flag)
                {
                    var msg = root.TryGetProperty("message", out var m) ? m.GetString() : "GSTIN not found";
                    return Json(new { success = false, message = msg });
                }

                var data = root.GetProperty("data");
                var legalName = data.TryGetProperty("lgnm", out var ln) ? ln.GetString() ?? "" : "";
                var tradeName = data.TryGetProperty("tradeNam", out var tn) ? tn.GetString() ?? "" : "";
                var sts = data.TryGetProperty("sts", out var s) ? s.GetString() ?? "" : "";
                var dty = data.TryGetProperty("dty", out var dt) ? dt.GetString() ?? "" : "";
                var ctb = data.TryGetProperty("ctb", out var cb) ? cb.GetString() ?? "" : "";
                var rgdt = data.TryGetProperty("rgdt", out var rd) ? rd.GetString() ?? "" : "";
                var cxdt = data.TryGetProperty("cxdt", out var cd) ? cd.GetString() ?? "" : "";
                var einv = data.TryGetProperty("einvoiceStatus", out var ei) ? ei.GetString() ?? "" : "";

                var fullAddress = "";
                var floorNo = ""; var bldgName = ""; var bldgNo = "";
                var street = ""; var city = ""; var district = ""; var stateName = ""; var pincode = "";

                if (data.TryGetProperty("pradr", out var pradr))
                {
                    fullAddress = pradr.TryGetProperty("adr", out var adr) ? adr.GetString() ?? "" : "";
                    if (pradr.TryGetProperty("addr", out var addr))
                    {
                        floorNo = addr.TryGetProperty("flno", out var f) ? f.GetString() ?? "" : "";
                        bldgName = addr.TryGetProperty("bnm", out var b) ? b.GetString() ?? "" : "";
                        bldgNo = addr.TryGetProperty("bno", out var bn) ? bn.GetString() ?? "" : "";
                        street = addr.TryGetProperty("st", out var st) ? st.GetString() ?? "" : "";
                        city = addr.TryGetProperty("loc", out var l) ? l.GetString() ?? "" : "";
                        district = addr.TryGetProperty("dst", out var d) ? d.GetString() ?? "" : "";
                        stateName = addr.TryGetProperty("stcd", out var sc) ? sc.GetString() ?? "" : "";
                        pincode = addr.TryGetProperty("pncd", out var p) ? p.GetString() ?? "" : "";
                    }
                }

                // Step 3: Save to cache
                var insertCache = @"INSERT INTO gstin_cache 
                    (gstin, legal_name, trade_name, status, dealer_type, constitution, reg_date, cancel_date,
                     full_address, floor_no, building_name, building_no, street, city, district, state_name, pincode, 
                     einvoice_status, raw_json)
                    VALUES (@gstin, @legal_name, @trade_name, @status, @dealer_type, @constitution, @reg_date, @cancel_date,
                     @full_address, @floor_no, @building_name, @building_no, @street, @city, @district, @state_name, @pincode,
                     @einvoice_status, @raw_json)
                    ON CONFLICT (gstin) DO UPDATE SET
                     legal_name=EXCLUDED.legal_name, trade_name=EXCLUDED.trade_name, status=EXCLUDED.status,
                     full_address=EXCLUDED.full_address, state_name=EXCLUDED.state_name, city=EXCLUDED.city,
                     pincode=EXCLUDED.pincode, district=EXCLUDED.district, raw_json=EXCLUDED.raw_json, updated_at=NOW()";

                using var insCmd = new NpgsqlCommand(insertCache, conn);
                insCmd.Parameters.AddWithValue("gstin", gstin);
                insCmd.Parameters.AddWithValue("legal_name", legalName);
                insCmd.Parameters.AddWithValue("trade_name", tradeName);
                insCmd.Parameters.AddWithValue("status", sts);
                insCmd.Parameters.AddWithValue("dealer_type", dty);
                insCmd.Parameters.AddWithValue("constitution", ctb);
                insCmd.Parameters.AddWithValue("reg_date", rgdt);
                insCmd.Parameters.AddWithValue("cancel_date", cxdt);
                insCmd.Parameters.AddWithValue("full_address", fullAddress);
                insCmd.Parameters.AddWithValue("floor_no", floorNo);
                insCmd.Parameters.AddWithValue("building_name", bldgName);
                insCmd.Parameters.AddWithValue("building_no", bldgNo);
                insCmd.Parameters.AddWithValue("street", street);
                insCmd.Parameters.AddWithValue("city", city);
                insCmd.Parameters.AddWithValue("district", district);
                insCmd.Parameters.AddWithValue("state_name", stateName);
                insCmd.Parameters.AddWithValue("pincode", pincode);
                insCmd.Parameters.AddWithValue("einvoice_status", einv);
                insCmd.Parameters.AddWithValue("raw_json", response);
                await insCmd.ExecuteNonQueryAsync();

                return Json(new
                {
                    success = true,
                    source = "api",
                    legalName,
                    tradeName,
                    status = sts,
                    address = fullAddress,
                    state = stateName,
                    city,
                    pincode,
                    district
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error looking up GSTIN: " + ex.Message });
            }
        }

    }

}
