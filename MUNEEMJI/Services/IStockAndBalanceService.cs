using MUNEEMJI.Models;
using MUNEEMJI.Models;
using Npgsql;

namespace MUNEEMJI.Services
{
    public interface IStockAndBalanceService
    {
        /// <summary>
        /// Validates that all items in the bill have sufficient stock for a sale.
        /// Returns null if valid, or an error message string if insufficient stock.
        /// </summary>
        Task<string?> ValidateStockForSaleAsync(List<PurchaseBillItem> billItems, int companyId);

        /// <summary>
        /// After a successful sale: decreases item stock and increases party balance.
        /// </summary>
        Task UpdateStockAndBalanceForSaleAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, List<PurchaseBillItem> billItems, int partyId, decimal invoiceTotal, int companyId);

        /// <summary>
        /// After a successful purchase: increases item stock and decreases party balance.
        /// </summary>
        Task UpdateStockAndBalanceForPurchaseAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, List<PurchaseBillItem> billItems, int partyId, decimal billTotal, int companyId);
    }

    public class StockAndBalanceService : IStockAndBalanceService
    {
        private readonly string _connectionString;

        public StockAndBalanceService()
        {
            _connectionString = MUNEEMJI.DbConfig.ConnectionString;
        }

        public async Task<string?> ValidateStockForSaleAsync(List<PurchaseBillItem> billItems, int companyId)
        {
            if (billItems == null || billItems.Count == 0)
                return null;

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            foreach (var item in billItems)
            {
                if (item.ItemId <= 0 || item.Quantity <= 0)
                    continue;

                var query = @"SELECT item_name, opening_quantity FROM billitem WHERE id = @ItemId AND companyid = @CompanyId";
                using var cmd = new NpgsqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ItemId", item.ItemId);
                cmd.Parameters.AddWithValue("@CompanyId", companyId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    var itemName = reader.GetString(0);
                    var availableQty = reader.GetInt32(1);

                    if (item.Quantity > availableQty)
                    {
                        return $"Insufficient stock for item '{itemName}'. Available: {availableQty}, Requested: {item.Quantity}";
                    }
                }
                else
                {
                    return $"Item with ID {item.ItemId} not found.";
                }
            }

            return null; // All items have sufficient stock
        }

        public async Task UpdateStockAndBalanceForSaleAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, List<PurchaseBillItem> billItems, int partyId, decimal invoiceTotal, int companyId)
        {
            // Decrease stock for each item sold
            foreach (var item in billItems)
            {
                if (item.ItemId <= 0 || item.Quantity <= 0)
                    continue;

                var updateStockQuery = @"UPDATE billitem SET opening_quantity = opening_quantity - @Quantity WHERE id = @ItemId AND companyid = @CompanyId";
                using var stockCmd = new NpgsqlCommand(updateStockQuery, connection, transaction);
                stockCmd.Parameters.AddWithValue("@Quantity", (int)item.Quantity);
                stockCmd.Parameters.AddWithValue("@ItemId", item.ItemId);
                stockCmd.Parameters.AddWithValue("@CompanyId", companyId);
                await stockCmd.ExecuteNonQueryAsync();
            }

            // If invoiceTotal is 0, calculate from item amounts as fallback
            decimal effectiveTotal = invoiceTotal;
            if (effectiveTotal == 0 && billItems != null)
            {
                foreach (var item in billItems)
                {
                    if (item.ItemId <= 0 || item.Quantity <= 0)
                        continue;
                    var itemAmount = item.Amount > 0 ? item.Amount : item.Quantity * item.PricePerUnit;
                    effectiveTotal += itemAmount;
                }
            }

            // Increase party balance (party owes more money after a sale)
            if (partyId > 0 && effectiveTotal != 0)
            {
                var updatePartyQuery = @"UPDATE parties SET balance = COALESCE(balance, 0) + @Amount WHERE id = @PartyId";
                using var partyCmd = new NpgsqlCommand(updatePartyQuery, connection, transaction);
                partyCmd.Parameters.Add(new NpgsqlParameter("@Amount", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = effectiveTotal });
                partyCmd.Parameters.Add(new NpgsqlParameter("@PartyId", NpgsqlTypes.NpgsqlDbType.Integer) { Value = partyId });
                await partyCmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateStockAndBalanceForPurchaseAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, List<PurchaseBillItem> billItems, int partyId, decimal billTotal, int companyId)
        {
            // Increase stock for each item purchased
            foreach (var item in billItems)
            {
                if (item.ItemId <= 0 || item.Quantity <= 0)
                    continue;

                var updateStockQuery = @"UPDATE billitem SET opening_quantity = opening_quantity + @Quantity WHERE id = @ItemId AND companyid = @CompanyId";
                using var stockCmd = new NpgsqlCommand(updateStockQuery, connection, transaction);
                stockCmd.Parameters.AddWithValue("@Quantity", (int)item.Quantity);
                stockCmd.Parameters.AddWithValue("@ItemId", item.ItemId);
                stockCmd.Parameters.AddWithValue("@CompanyId", companyId);
                await stockCmd.ExecuteNonQueryAsync();
            }

            // If billTotal is 0, calculate from item amounts as fallback
            decimal effectiveTotal = billTotal;
            if (effectiveTotal == 0 && billItems != null)
            {
                foreach (var item in billItems)
                {
                    if (item.ItemId <= 0 || item.Quantity <= 0)
                        continue;
                    var itemAmount = item.Amount > 0 ? item.Amount : item.Quantity * item.PricePerUnit;
                    effectiveTotal += itemAmount;
                }
            }

            // Decrease party balance (you owe the supplier after a purchase)
            if (partyId > 0 && effectiveTotal != 0)
            {
                var updatePartyQuery = @"UPDATE parties SET balance = COALESCE(balance, 0) - @Amount WHERE id = @PartyId";
                using var partyCmd = new NpgsqlCommand(updatePartyQuery, connection, transaction);
                partyCmd.Parameters.Add(new NpgsqlParameter("@Amount", NpgsqlTypes.NpgsqlDbType.Numeric) { Value = effectiveTotal });
                partyCmd.Parameters.Add(new NpgsqlParameter("@PartyId", NpgsqlTypes.NpgsqlDbType.Integer) { Value = partyId });
                await partyCmd.ExecuteNonQueryAsync();
            }
        }
    }
}
