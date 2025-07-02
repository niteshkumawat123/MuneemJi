using MUNEEMJI.Models;
using Newtonsoft.Json;
using Npgsql;
using System.Data;

namespace MUNEEMJI.Repositories
{
    public interface IBillItemService
    {
        Task<bool> SaveBillItemAsync(BillItem model);
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

        public async Task<bool> SaveBillItemAsync(BillItem model)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                using var transaction = await connection.BeginTransactionAsync();

                // Calculate total estimated cost
                decimal totalRawMaterialCost = model.TotalEstimatedCost;
                decimal totalAdditionalCost = Convert.ToDecimal(model.AdditionalCosts);
                decimal totalEstimatedCost = totalRawMaterialCost + totalAdditionalCost;

                string sql = @"
            INSERT INTO billitem (
                item_type, item_name, item_hsn, item_code, category, unit, item_image_url,
                sale_price, sale_price_tax_type, discount_on_sale_price, discount_type,
                purchase_price, purchase_price_tax_type, tax_rate, wholesale_price,
                opening_quantity, at_price, as_of_date, location, min_stock_to_maintain,
                online_store_price, description, total_estimated_cost,
                service_name, service_hsn, service_code,
                created_at, updated_at
            )
            VALUES (
                @item_type, @item_name, @item_hsn, @item_code, @category, @unit, @item_image_url,
                @sale_price, @sale_price_tax_type, @discount_on_sale_price, @discount_type,
                @purchase_price, @purchase_price_tax_type, @tax_rate, @wholesale_price,
                @opening_quantity, @at_price, @as_of_date, @location, @min_stock_to_maintain,
                @online_store_price, @description, @total_estimated_cost,
                @service_name, @service_hsn, @service_code,
                @created_at, @updated_at
            )
            RETURNING id;";   // ✅ This will return the generated primary key

                using var command = new NpgsqlCommand(sql, connection, transaction);

                // Add parameters (same as your code)
                command.Parameters.AddWithValue("@item_type", model.ItemType);
                command.Parameters.AddWithValue("@item_name", model.ItemName ?? string.Empty);
                command.Parameters.AddWithValue("@item_hsn", (object?)model.ItemHsn ?? DBNull.Value);
                command.Parameters.AddWithValue("@item_code", (object?)model.ItemCode ?? DBNull.Value);
                command.Parameters.AddWithValue("@category", (object?)model.Category ?? DBNull.Value);
                command.Parameters.AddWithValue("@unit", (object?)model.Unit ?? DBNull.Value);
                command.Parameters.AddWithValue("@item_image_url", (object?)model.ItemImageUrl ?? DBNull.Value);

                command.Parameters.AddWithValue("@sale_price", (object?)model.SalePrice ?? DBNull.Value);
                command.Parameters.AddWithValue("@sale_price_tax_type", model.SalePriceTaxType);
                command.Parameters.AddWithValue("@discount_on_sale_price", (object?)model.DiscountOnSalePrice ?? DBNull.Value);
                command.Parameters.AddWithValue("@discount_type", model.DiscountType);
                command.Parameters.AddWithValue("@purchase_price", (object?)model.PurchasePrice ?? DBNull.Value);
                command.Parameters.AddWithValue("@purchase_price_tax_type", model.PurchasePriceTaxType);
                command.Parameters.AddWithValue("@tax_rate", model.TaxRate);
                command.Parameters.AddWithValue("@wholesale_price", (object?)model.WholesalePrice ?? DBNull.Value);

                command.Parameters.AddWithValue("@opening_quantity", model.OpeningQuantity);
                command.Parameters.AddWithValue("@at_price", (object?)model.AtPrice ?? DBNull.Value);
                command.Parameters.AddWithValue("@as_of_date", (object?)model.AsOfDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@location", (object?)model.Location ?? DBNull.Value);
                command.Parameters.AddWithValue("@min_stock_to_maintain", model.MinStockToMaintain);

                command.Parameters.AddWithValue("@online_store_price", (object?)model.OnlineStorePrice ?? DBNull.Value);
                command.Parameters.AddWithValue("@description", (object?)model.Description ?? DBNull.Value);

                command.Parameters.AddWithValue("@total_estimated_cost", totalEstimatedCost);

                command.Parameters.AddWithValue("@service_name", (object?)model.ServiceName ?? DBNull.Value);
                command.Parameters.AddWithValue("@service_hsn", (object?)model.ServiceHsn ?? DBNull.Value);
                command.Parameters.AddWithValue("@service_code", (object?)model.ServiceCode ?? DBNull.Value);

                command.Parameters.AddWithValue("@created_at", DateTime.UtcNow);
                command.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);

                // ✅ Get the inserted BillItem Id
                var itemBillingId = (int)(await command.ExecuteScalarAsync())!;

                // ✅ Insert into Manufacturing table if data exists
                if (model.Manufacturing != null && model.Manufacturing.Any())
                {
                    foreach (var item in model.Manufacturing)
                    {
                        string manufacturingSql = @"
                    INSERT INTO manufacturing (
                        itembillingid, name, quantity, unit, purchasepriceperunit, estimatedcost
                    )
                    VALUES (
                        @itembillingid, @name, @quantity, @unit, @purchasepriceperunit, @estimatedcost
                    )";

                        using var manufacturingCmd = new NpgsqlCommand(manufacturingSql, connection, transaction);

                        manufacturingCmd.Parameters.AddWithValue("@itembillingid", itemBillingId);
                        manufacturingCmd.Parameters.AddWithValue("@name", item.Name);
                        manufacturingCmd.Parameters.AddWithValue("@quantity", item.Quantity);
                        manufacturingCmd.Parameters.AddWithValue("@unit", item.Unit);
                        manufacturingCmd.Parameters.AddWithValue("@purchasepriceperunit", item.PurchasePricePerUnit);
                        manufacturingCmd.Parameters.AddWithValue("@estimatedcost", item.EstimatedCost);

                        await manufacturingCmd.ExecuteNonQueryAsync();
                    }
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
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
