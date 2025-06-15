using MUNEEMJI.Models;
using Newtonsoft.Json;
using Npgsql;
using System.Data;

namespace MUNEEMJI.Repositories
{
    public interface IBillItemService
    {
        Task<bool> SaveBillItemAsync(BillItemViewModel model);
        Task<List<string>> GetCategoriesAsync();
        Task<List<string>> GetUnitsAsync();
        Task<List<string>> GetTaxRatesAsync();
    }
    public class BillItemService : IBillItemService
    {
        private readonly string _connectionString;

        public BillItemService(IConfiguration configuration)
        {
            _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
        }

        public async Task<bool> SaveBillItemAsync(BillItemViewModel model)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                // Serialize raw materials and additional costs to JSON
                string rawMaterialsJson = Newtonsoft.Json.JsonConvert.SerializeObject(model.RawMaterials);
                string additionalCostsJson = Newtonsoft.Json.JsonConvert.SerializeObject(model.AdditionalCosts);

                // Calculate total estimated cost
                decimal totalRawMaterialCost = model.RawMaterials.Sum(rm => rm.EstimatedCost);
                decimal totalAdditionalCost = model.AdditionalCosts.Sum(ac => ac.Cost);
                decimal totalEstimatedCost = totalRawMaterialCost + totalAdditionalCost;

                string sql = @"
                    INSERT INTO billitem (
                        item_type, item_name, item_hsn, item_code, category, unit, item_image_url,
                        sale_price, sale_price_tax_type, discount_on_sale_price, discount_type,
                        purchase_price, purchase_price_tax_type, tax_rate, wholesale_price,
                        opening_quantity, at_price, as_of_date, location, min_stock_to_maintain,
                        online_store_price, description,
                        raw_materials, additional_costs, total_estimated_cost,
                        service_name, service_hsn, service_code,
                        created_at, updated_at
                    ) VALUES (
                        @item_type, @item_name, @item_hsn, @item_code, @category, @unit, @item_image_url,
                        @sale_price, @sale_price_tax_type, @discount_on_sale_price, @discount_type,
                        @purchase_price, @purchase_price_tax_type, @tax_rate, @wholesale_price,
                        @opening_quantity, @at_price, @as_of_date, @location, @min_stock_to_maintain,
                        @online_store_price, @description,
                        @raw_materials::jsonb, @additional_costs::jsonb, @total_estimated_cost,
                        @service_name, @service_hsn, @service_code,
                        @created_at, @updated_at
                    )";

                using var command = new NpgsqlCommand(sql, connection);

                // Basic information parameters
                command.Parameters.AddWithValue("@item_type", model.BillItem.ItemType);
                command.Parameters.AddWithValue("@item_name", model.BillItem.ItemName ?? string.Empty);
                command.Parameters.AddWithValue("@item_hsn", (object?)model.BillItem.ItemHsn ?? DBNull.Value);
                command.Parameters.AddWithValue("@item_code", (object?)model.BillItem.ItemCode ?? DBNull.Value);
                command.Parameters.AddWithValue("@category", (object?)model.BillItem.Category ?? DBNull.Value);
                command.Parameters.AddWithValue("@unit", (object?)model.BillItem.Unit ?? DBNull.Value);
                command.Parameters.AddWithValue("@item_image_url", (object?)model.BillItem.ItemImageUrl ?? DBNull.Value);

                // Pricing parameters
                command.Parameters.AddWithValue("@sale_price", (object?)model.BillItem.SalePrice ?? DBNull.Value);
                command.Parameters.AddWithValue("@sale_price_tax_type", model.BillItem.SalePriceTaxType);
                command.Parameters.AddWithValue("@discount_on_sale_price", (object?)model.BillItem.DiscountOnSalePrice ?? DBNull.Value);
                command.Parameters.AddWithValue("@discount_type", model.BillItem.DiscountType);
                command.Parameters.AddWithValue("@purchase_price", (object?)model.BillItem.PurchasePrice ?? DBNull.Value);
                command.Parameters.AddWithValue("@purchase_price_tax_type", model.BillItem.PurchasePriceTaxType);
                command.Parameters.AddWithValue("@tax_rate", model.BillItem.TaxRate);
                command.Parameters.AddWithValue("@wholesale_price", (object?)model.BillItem.WholesalePrice ?? DBNull.Value);

                // Stock parameters
                command.Parameters.AddWithValue("@opening_quantity", model.BillItem.OpeningQuantity);
                command.Parameters.AddWithValue("@at_price", (object?)model.BillItem.AtPrice ?? DBNull.Value);
                command.Parameters.AddWithValue("@as_of_date", (object?)model.BillItem.AsOfDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@location", (object?)model.BillItem.Location ?? DBNull.Value);
                command.Parameters.AddWithValue("@min_stock_to_maintain", model.BillItem.MinStockToMaintain);

                // Online store parameters
                command.Parameters.AddWithValue("@online_store_price", (object?)model.BillItem.OnlineStorePrice ?? DBNull.Value);
                command.Parameters.AddWithValue("@description", (object?)model.BillItem.Description ?? DBNull.Value);

                // Manufacturing parameters
                command.Parameters.AddWithValue("@raw_materials", rawMaterialsJson);
                command.Parameters.AddWithValue("@additional_costs", additionalCostsJson);
                command.Parameters.AddWithValue("@total_estimated_cost", totalEstimatedCost);

                // Service parameters
                command.Parameters.AddWithValue("@service_name", (object?)model.BillItem.ServiceName ?? DBNull.Value);
                command.Parameters.AddWithValue("@service_hsn", (object?)model.BillItem.ServiceHsn ?? DBNull.Value);
                command.Parameters.AddWithValue("@service_code", (object?)model.BillItem.ServiceCode ?? DBNull.Value);

                // Audit parameters
                command.Parameters.AddWithValue("@created_at", DateTime.UtcNow);
                command.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);

                int result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                // Log the exception (you can use ILogger here)
                Console.WriteLine($"Error saving bill item: {ex.Message}");
                throw;
            }
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                string sql = "SELECT DISTINCT category FROM billitem WHERE category IS NOT NULL ORDER BY category";

                using var command = new NpgsqlCommand(sql, connection);
                using var reader = await command.ExecuteReaderAsync();

                var categories = new List<string>();
                while (await reader.ReadAsync())
                {
                    categories.Add(reader.GetString("category"));
                }

                return categories;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting categories: {ex.Message}");
                return new List<string>();
            }
        }

        public async Task<List<string>> GetUnitsAsync()
        {
            try
            {
                // Return predefined units
                await Task.Delay(1); // Simulate async operation
                return new List<string>
                {
                    "Pieces", "Kg", "Grams", "Liters", "Meters", "Boxes", "Packets",
                    "Hours", "Days", "Months", "Years", "Square Feet", "Square Meters"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting units: {ex.Message}");
                return new List<string>();
            }
        }

        public async Task<List<string>> GetTaxRatesAsync()
        {
            try
            {
                // Return predefined tax rates
                await Task.Delay(1); // Simulate async operation
                return new List<string>
                {
                    "None", "0%", "5%", "12%", "18%", "28%"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting tax rates: {ex.Message}");
                return new List<string>();
            }
        }
    }
}
