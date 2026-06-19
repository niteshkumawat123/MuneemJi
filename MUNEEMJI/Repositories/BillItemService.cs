using Insight.Database;
using MUNEEMJI.Models;
using Newtonsoft.Json;
using Npgsql;
using System.Data;

namespace MUNEEMJI.Repositories
{
    public interface IBillItemService
    {
        Task<bool> SaveBillItemAsync(BillItem model,int companyId);
        Task<List<string>> GetCategoriesAsync(int companyId = 0);
        Task<List<string>> GetUnitsAsync();
        Task<List<string>> GetTaxRatesAsync();
        Task<List<BillItem>> GetItems(int companyid );
    }
    public class BillItemService : IBillItemService
    {
        private readonly string _connectionString;

        public BillItemService(IConfiguration configuration)
        {
            _connectionString = MUNEEMJI.DbConfig.ConnectionString;
        }

       

        public async Task<bool> SaveBillItemAsync(BillItem model, int companyId)
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

                int itemBillingId = 0;

                if (model.Id == 0)
                {
                    // INSERT
                    string insertSql = @"
            INSERT INTO billitem (
                item_type, item_name, item_hsn, item_code, category, unit, item_image_url,
                sale_price, sale_price_tax_type, discount_on_sale_price, discount_type,
                purchase_price, purchase_price_tax_type, tax_rate, wholesale_price,
                wholesale_price_tax_type, min_wholesale_qty, disc_on_mrp_wholesale, additional_cess,
                opening_quantity, at_price, as_of_date, location, min_stock_to_maintain,
                online_store_price, description, total_estimated_cost,
                service_name, service_hsn, service_code,
                colour, material, mfg_date, exp_date, size, brand,
                created_at, updated_at,companyid
            )
            VALUES (
                @item_type, @item_name, @item_hsn, @item_code, @category, @unit, @item_image_url,
                @sale_price, @sale_price_tax_type, @discount_on_sale_price, @discount_type,
                @purchase_price, @purchase_price_tax_type, @tax_rate, @wholesale_price,
                @wholesale_price_tax_type, @min_wholesale_qty, @disc_on_mrp_wholesale, @additional_cess,
                @opening_quantity, @at_price, @as_of_date, @location, @min_stock_to_maintain,
                @online_store_price, @description, @total_estimated_cost,
                @service_name, @service_hsn, @service_code,
                @colour, @material, @mfg_date, @exp_date, @size, @brand,
                @created_at, @updated_at,@p_companyid
            )
            RETURNING id;";

                    using var insertCommand = new NpgsqlCommand(insertSql, connection, transaction);
                    AddBillItemParameters(insertCommand, model, totalEstimatedCost, companyId);
                    itemBillingId = (int)(await insertCommand.ExecuteScalarAsync())!;
                }
                else
                {
                    // UPDATE
                    string updateSql = @"
            UPDATE billitem SET
                item_type = @item_type,
                item_name = @item_name,
                item_hsn = @item_hsn,
                item_code = @item_code,
                category = @category,
                unit = @unit,
                item_image_url = @item_image_url,
                sale_price = @sale_price,
                sale_price_tax_type = @sale_price_tax_type,
                discount_on_sale_price = @discount_on_sale_price,
                discount_type = @discount_type,
                purchase_price = @purchase_price,
                purchase_price_tax_type = @purchase_price_tax_type,
                tax_rate = @tax_rate,
                wholesale_price = @wholesale_price,
                wholesale_price_tax_type = @wholesale_price_tax_type,
                min_wholesale_qty = @min_wholesale_qty,
                disc_on_mrp_wholesale = @disc_on_mrp_wholesale,
                additional_cess = @additional_cess,
                opening_quantity = @opening_quantity,
                at_price = @at_price,
                as_of_date = @as_of_date,
                location = @location,
                min_stock_to_maintain = @min_stock_to_maintain,
                online_store_price = @online_store_price,
                description = @description,
                total_estimated_cost = @total_estimated_cost,
                service_name = @service_name,
                service_hsn = @service_hsn,
                service_code = @service_code,
                colour = @colour,
                material = @material,
                mfg_date = @mfg_date,
                exp_date = @exp_date,
                size = @size,
                brand = @brand,
                updated_at = @updated_at
            WHERE id = @id;";

                    using var updateCommand = new NpgsqlCommand(updateSql, connection, transaction);
                    AddBillItemParameters(updateCommand, model, totalEstimatedCost, companyId);
                    updateCommand.Parameters.AddWithValue("@id", model.Id);
                    await updateCommand.ExecuteNonQueryAsync();
                    itemBillingId = model.Id;

                    // Delete existing manufacturing rows for update scenario
                    string deleteManufacturingSql = "DELETE FROM manufacturing WHERE itembillingid = @itembillingid";
                    using var deleteCmd = new NpgsqlCommand(deleteManufacturingSql, connection, transaction);
                    deleteCmd.Parameters.AddWithValue("@itembillingid", model.Id);
                    await deleteCmd.ExecuteNonQueryAsync();
                }

                // Insert into manufacturing table (both insert & update)
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

        private void AddBillItemParameters(NpgsqlCommand command, BillItem model, decimal totalEstimatedCost,int CompanyId)
        {
            command.Parameters.AddWithValue("@item_type", model.ItemType);
            command.Parameters.AddWithValue("@item_name", model.ItemName ?? string.Empty);
            command.Parameters.AddWithValue("@item_hsn", (object?)model.ItemHsn ?? DBNull.Value);
            command.Parameters.AddWithValue("@item_code", (object?)model.ItemCode ?? DBNull.Value);
            command.Parameters.AddWithValue("@category", (object?)model.Category ?? DBNull.Value);
            command.Parameters.AddWithValue("@unit", (object?)model.Unit ?? DBNull.Value);
            command.Parameters.AddWithValue("@item_image_url", (object?)model.ImageUrl ?? DBNull.Value);
            command.Parameters.AddWithValue("@sale_price", (object?)model.SalePrice ?? DBNull.Value);
            command.Parameters.AddWithValue("@sale_price_tax_type", model.SalePriceTaxType);
            command.Parameters.AddWithValue("@discount_on_sale_price", (object?)model.DiscountOnSalePrice ?? DBNull.Value);
            command.Parameters.AddWithValue("@discount_type", model.DiscountType);
            command.Parameters.AddWithValue("@purchase_price", (object?)model.PurchasePrice ?? DBNull.Value);
            command.Parameters.AddWithValue("@purchase_price_tax_type", model.PurchasePriceTaxType);
            command.Parameters.AddWithValue("@tax_rate", model.TaxRate);
            command.Parameters.AddWithValue("@wholesale_price", (object?)model.WholesalePrice ?? DBNull.Value);
            command.Parameters.AddWithValue("@wholesale_price_tax_type", model.WholesalePriceTaxType ?? "Without Tax");
            command.Parameters.AddWithValue("@min_wholesale_qty", model.MinWholesaleQty);
            command.Parameters.AddWithValue("@disc_on_mrp_wholesale", (object?)model.DiscOnMrpWholesale ?? DBNull.Value);
            command.Parameters.AddWithValue("@additional_cess", (object?)model.AdditionalCess ?? DBNull.Value);
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
            command.Parameters.AddWithValue("@colour", (object?)model.Colour ?? DBNull.Value);
            command.Parameters.AddWithValue("@material", (object?)model.Material ?? DBNull.Value);
            command.Parameters.AddWithValue("@mfg_date", (object?)model.MfgDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@exp_date", (object?)model.ExpDate ?? DBNull.Value);
            command.Parameters.AddWithValue("@size", (object?)model.Size ?? DBNull.Value);
            command.Parameters.AddWithValue("@brand", (object?)model.Brand ?? DBNull.Value);
            command.Parameters.AddWithValue("@created_at", DateTime.UtcNow);
            command.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);
            command.Parameters.AddWithValue("@p_companyid", CompanyId);
        }

        public async Task<List<string>> GetCategoriesAsync(int companyId = 0)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();

                string sql = "SELECT name FROM categorieses WHERE companyid = @p_companyid ORDER BY name";

                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.AddWithValue("@p_companyid", companyId);
                using var reader = await command.ExecuteReaderAsync();

                var categories = new List<string>();
                while (await reader.ReadAsync())
                {
                    categories.Add(reader.GetString(0));
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

        public async Task<List<BillItem>> GetItems(int Companyid)
        {
            List<BillItem> items = new List<BillItem>();
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            // ? Query to get all bill items
            var billItemSql = @"
                               SELECT 
                                   id AS ""Id"",
                                   item_type AS ""ItemType"",
                                   item_name AS ""ItemName"",
                                   item_hsn AS ""ItemHsn"",
                                   item_code AS ""ItemCode"",
                                   category AS ""Category"",
                                   unit AS ""Unit"",
                                   item_image_url AS ""ItemImageUrl"",
                                   sale_price AS ""SalePrice"",
                                   sale_price_tax_type AS ""SalePriceTaxType"",
                                   discount_on_sale_price AS ""DiscountOnSalePrice"",
                                   discount_type AS ""DiscountType"",
                                   purchase_price AS ""PurchasePrice"",
                                   purchase_price_tax_type AS ""PurchasePriceTaxType"",
                                   tax_rate AS ""TaxRate"",
                                   wholesale_price AS ""WholesalePrice"",
                                   opening_quantity AS ""OpeningQuantity"",
                                   at_price AS ""AtPrice"",
                                   as_of_date AS ""AsOfDate"",
                                   location AS ""Location"",
                                   min_stock_to_maintain AS ""MinStockToMaintain"",
                                   online_store_price AS ""OnlineStorePrice"",
                                   description AS ""Description"",
                                   raw_materials AS ""RawMaterials"",
                                   additional_costs AS ""AdditionalCosts"",
                                   total_estimated_cost AS ""TotalEstimatedCost"",
                                   service_name AS ""ServiceName"",
                                   service_hsn AS ""ServiceHsn"",
                                   service_code AS ""ServiceCode"",
                                   colour AS ""Colour"",
                                   material AS ""Material"",
                                   mfg_date AS ""MfgDate"",
                                   exp_date AS ""ExpDate"",
                                   size AS ""Size"",
                                   brand AS ""Brand"",
                                   created_at AS ""CreatedAt"",
                                   updated_at AS ""UpdatedAt"",
                                   mrp
                               FROM billitem where companyid = @p_companyid
                               ORDER BY id;
";

            // ? Fetch bill items
            items = connection.QuerySql<BillItem>(billItemSql,new { p_companyid = Companyid }).ToList();
            return items;
        }
    }
}

