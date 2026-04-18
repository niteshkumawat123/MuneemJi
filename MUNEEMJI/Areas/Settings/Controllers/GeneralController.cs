using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MUNEEMJI.Models.Setting;
using Npgsql;

namespace MUNEEMJI.Areas.Settings.Controllers
{
    [Area("Settings")]

    public class GeneralController : Controller
    {
        private readonly string _connectionString = MUNEEMJI.DbConfig.ConnectionString;

        public IActionResult Index(int id)
        {
            GeneralSettingsViewModel model = new GeneralSettingsViewModel();
          
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string sql = @"
            SELECT 
                Id, StopSaleOnNegativeStock, BlockNewItemsFromTxn, 
                BlockNewPartiesFromTxn, GstinNumber, EstimateQuotation,
                ProformaInvoice, SalePurchaseOrder, OtherIncome,
                FixedAssets, DeliveryChallan, GoodsReturnOnDeliveryChallan,
                PrintAmountInDeliveryChallan, MultiFirm, GodownManagement,
                CreatedAt, UpdatedAt, CreatedBy, UpdatedBy
            FROM general_setting
            WHERE Id = 1;
        ";
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                model = new GeneralSettingsViewModel
                                {
                                    Id = reader.GetInt32(0),
                                    StopSaleOnNegativeStock = reader.GetBoolean(1),
                                    BlockNewItemsFromTxn = reader.GetBoolean(2),
                                    BlockNewPartiesFromTxn = reader.GetBoolean(3),
                                    GstinNumber = reader.GetBoolean(4),
                                    EstimateQuotation = reader.GetBoolean(5),
                                    ProformaInvoice = reader.GetBoolean(6),
                                    SalePurchaseOrder = reader.GetBoolean(7),
                                    OtherIncome = reader.GetBoolean(8),
                                    FixedAssets = reader.GetBoolean(9),
                                    DeliveryChallan = reader.GetBoolean(10),
                                    GoodsReturnOnDeliveryChallan = reader.GetBoolean(11),
                                    PrintAmountInDeliveryChallan = reader.GetBoolean(12),
                                    MultiFirm = reader.GetBoolean(13),
                                    GodownManagement = reader.GetBoolean(14),
                                    CreatedAt = reader.IsDBNull(15) ? DateTime.MinValue : reader.GetDateTime(15),
                                    UpdatedAt = reader.IsDBNull(16) ? DateTime.MinValue : reader.GetDateTime(16),
                                    CreatedBy = reader.IsDBNull(17) ? null : reader.GetInt32(17),
                                    UpdatedBy = reader.IsDBNull(18) ? null : reader.GetInt32(18)
                                };
                            }
                        }
                    }
                    conn.Close();
                }
            

            return View(model);
        }
        [HttpPost]
        public IActionResult SaveSettings([FromBody]GeneralSettingsViewModel model)
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    string sql;

                    if (model.Id > 0)
                    {
                        // UPDATE
                        sql = @"
                        UPDATE general_setting
                        SET
                            StopSaleOnNegativeStock = @StopSaleOnNegativeStock,
                            BlockNewItemsFromTxn = @BlockNewItemsFromTxn,
                            BlockNewPartiesFromTxn = @BlockNewPartiesFromTxn,
                            GstinNumber = @GstinNumber,
                            EstimateQuotation = @EstimateQuotation,
                            ProformaInvoice = @ProformaInvoice,
                            SalePurchaseOrder = @SalePurchaseOrder,
                            OtherIncome = @OtherIncome,
                            FixedAssets = @FixedAssets,
                            DeliveryChallan = @DeliveryChallan,
                            GoodsReturnOnDeliveryChallan = @GoodsReturnOnDeliveryChallan,
                            PrintAmountInDeliveryChallan = @PrintAmountInDeliveryChallan,
                            MultiFirm = @MultiFirm,
                            GodownManagement = @GodownManagement,
                            UpdatedAt = @UpdatedAt,
                            UpdatedBy = @UpdatedBy
                        WHERE Id = @Id;
                    ";
                    }
                    else
                    {
                        // INSERT
                        sql = @"
                        INSERT INTO general_setting
                        (
                            StopSaleOnNegativeStock, BlockNewItemsFromTxn, BlockNewPartiesFromTxn,
                            GstinNumber, EstimateQuotation, ProformaInvoice,
                            SalePurchaseOrder, OtherIncome, FixedAssets,
                            DeliveryChallan, GoodsReturnOnDeliveryChallan, PrintAmountInDeliveryChallan,
                            MultiFirm, GodownManagement,
                            CreatedAt, UpdatedAt, CreatedBy, UpdatedBy
                        )
                        VALUES
                        (
                            @StopSaleOnNegativeStock, @BlockNewItemsFromTxn, @BlockNewPartiesFromTxn,
                            @GstinNumber, @EstimateQuotation, @ProformaInvoice,
                            @SalePurchaseOrder, @OtherIncome, @FixedAssets,
                            @DeliveryChallan, @GoodsReturnOnDeliveryChallan, @PrintAmountInDeliveryChallan,
                            @MultiFirm, @GodownManagement,
                            @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy
                        );
                    ";
                    }

                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        if (model.Id > 0)
                        {
                            cmd.Parameters.AddWithValue("@Id", model.Id);
                        }

                        cmd.Parameters.AddWithValue("@StopSaleOnNegativeStock", model.StopSaleOnNegativeStock);
                        cmd.Parameters.AddWithValue("@BlockNewItemsFromTxn", model.BlockNewItemsFromTxn);
                        cmd.Parameters.AddWithValue("@BlockNewPartiesFromTxn", model.BlockNewPartiesFromTxn);
                        cmd.Parameters.AddWithValue("@GstinNumber", model.GstinNumber);
                        cmd.Parameters.AddWithValue("@EstimateQuotation", model.EstimateQuotation);
                        cmd.Parameters.AddWithValue("@ProformaInvoice", model.ProformaInvoice);
                        cmd.Parameters.AddWithValue("@SalePurchaseOrder", model.SalePurchaseOrder);
                        cmd.Parameters.AddWithValue("@OtherIncome", model.OtherIncome);
                        cmd.Parameters.AddWithValue("@FixedAssets", model.FixedAssets);
                        cmd.Parameters.AddWithValue("@DeliveryChallan", model.DeliveryChallan);
                        cmd.Parameters.AddWithValue("@GoodsReturnOnDeliveryChallan", model.GoodsReturnOnDeliveryChallan);
                        cmd.Parameters.AddWithValue("@PrintAmountInDeliveryChallan", model.PrintAmountInDeliveryChallan);
                        cmd.Parameters.AddWithValue("@MultiFirm", model.MultiFirm);
                        cmd.Parameters.AddWithValue("@GodownManagement", model.GodownManagement);
                        cmd.Parameters.AddWithValue("@CreatedAt", (object?)model.CreatedAt ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UpdatedAt", (object?)model.UpdatedAt ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", (object?)model.CreatedBy ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UpdatedBy", (object?)model.UpdatedBy ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }

                    conn.Close();
                }
                return Json(new
                {
                    success = true,
                    message = "Settings saved successfully!",
                    redirectUrl = Url.Action("Index", "ItemSettings", new { area = "Settings" })
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message,
                    redirectUrl = Url.Action("Index", "ItemSettings", new { area = "Settings" })
                });
            }
        }



        [HttpGet]
        public IActionResult GetSettings()
        {
            try
            {
                var model = new GeneralSettingsViewModel();
                // Load settings from database or configuration

                return Json(new { success = true, data = model });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error loading settings: " + ex.Message });
            }
        }
    }
}


