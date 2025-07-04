using MUNEEMJI.Models;
using Npgsql;
using System.Data;

namespace MUNEEMJI.Repositories
{

    public interface ISalesBillService
    {
        Task<int> CreateBillAsync(PurchaseBill bill);
        Task<PurchaseBill?> GetBillByIdAsync(int id);
        Task<List<PurchaseBill>> GetAllBillsAsync();
        Task<bool> UpdateBillAsync(PurchaseBill bill);
        Task<bool> DeleteBillAsync(int id);
        string GenerateBillNumber();
    }
    public class SalesBillService : ISalesBillService
    {
        private readonly string _connectionString;

        public SalesBillService(IConfiguration configuration)
        {
            _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
        }

        public async Task<int> CreateBillAsync(PurchaseBill bill)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Insert Bill
                var billQuery = @"
                    INSERT INTO TradeDocuments (bill_number, bill_date, state_of_supply, phone_no, po_no, po_date, 
                                     eway_bill_no, transport_name, delivery_location, vehicle_number, 
                                     delivery_date, payment_type, description, image_path, round_off, 
                                     total, created_date,paidReciveamount,TradeDocumentTypesid,PartyId)
                    VALUES (@BillNumber, @BillDate, @StateOfSupply, @PhoneNo, @PONo, @PODate, 
                           @EWayBillNo, @TransportName, @DeliveryLocation, @VehicleNumber, 
                           @DeliveryDate, @PaymentType, @Description, @ImagePath, @RoundOff, 
                           @Total, @CreatedDate,@paidReciveamount,@TradeDocumentTypesid,@PartyId)
                    RETURNING id";

                using var billCommand = new NpgsqlCommand(billQuery, connection, transaction);
                billCommand.Parameters.AddWithValue("@BillNumber", bill.BillNumber ?? string.Empty);
                billCommand.Parameters.AddWithValue("@BillDate", bill.BillDate); // assuming DateTime (not nullable)
                billCommand.Parameters.AddWithValue("@StateOfSupply", bill.StateOfSupply ?? string.Empty);
                billCommand.Parameters.AddWithValue("@PhoneNo", bill.PhoneNo ?? string.Empty);
                billCommand.Parameters.AddWithValue("@PONo", bill.PONo ?? string.Empty);
                billCommand.Parameters.AddWithValue("@PODate", (object?)bill.PODate ?? DBNull.Value);
                billCommand.Parameters.AddWithValue("@EWayBillNo", bill.EWayBillNo ?? string.Empty);
                billCommand.Parameters.AddWithValue("@TransportName", bill.TransportName ?? string.Empty);
                billCommand.Parameters.AddWithValue("@DeliveryLocation", bill.DeliveryLocation ?? string.Empty);
                billCommand.Parameters.AddWithValue("@VehicleNumber", bill.VehicleNumber ?? string.Empty);
                billCommand.Parameters.AddWithValue("@DeliveryDate", (object?)bill.DeliveryDate ?? DBNull.Value);
                billCommand.Parameters.AddWithValue("@PaymentType", bill.PaymentType ?? string.Empty);
                billCommand.Parameters.AddWithValue("@Description", bill.Description ?? string.Empty);
                billCommand.Parameters.AddWithValue("@ImagePath", bill.ImagePath ?? string.Empty);
                billCommand.Parameters.AddWithValue("@RoundOff", bill.RoundOffValue);
                billCommand.Parameters.AddWithValue("@Total", bill.Total);
                billCommand.Parameters.AddWithValue("@CreatedDate", bill.CreatedDate); // assuming DateTime (not nullable)
                billCommand.Parameters.AddWithValue("@paidReciveamount", bill.paidReciveamount);
                billCommand.Parameters.AddWithValue("@TradeDocumentTypesid", (int)TradeDocumentTypes.SalesChallan);
                billCommand.Parameters.AddWithValue("@PartyId", bill.PartyId);



                var billId = (int)(await billCommand.ExecuteScalarAsync() ?? 0);

                // Insert Bill Items
                foreach (var item in bill.BillItems)
                {
                    if (item.ItemId > 0)
                    {
                        var itemQuery = @"
                        INSERT INTO TradeDocumentItems (TradeDocumentsid, itemid,serialno,batchno,modelno,expirydate,mfgdate,item,categoryid, quantity, unit, price_per_unit, 
                                              discount_percentage, discount_amount, tax, tax_amount, amount)
                        VALUES (@TradeDocumentsid, @itemid,@serialno,@batchno,@modelno,@expirydate,@mfgdate,@item,@categoryid, @Quantity, @Unit, @PricePerUnit, 
                               @DiscountPercentage, @DiscountAmount, @Tax, @TaxAmount, @Amount)";

                        using var itemCommand = new NpgsqlCommand(itemQuery, connection, transaction);
                        itemCommand.Parameters.AddWithValue("@TradeDocumentsid", billId);
                        itemCommand.Parameters.AddWithValue("@itemid", item.ItemId);
                        itemCommand.Parameters.AddWithValue("@serialno", item.serialno ?? string.Empty);
                        itemCommand.Parameters.AddWithValue("@batchno", item.batchno ?? string.Empty);
                        itemCommand.Parameters.AddWithValue("@modelno", item.modelno ?? string.Empty);
                        itemCommand.Parameters.AddWithValue("@expirydate", item.expirydate ?? (object)DBNull.Value);
                        itemCommand.Parameters.AddWithValue("@mfgdate", item.mfgdate ?? (object)DBNull.Value);
                        itemCommand.Parameters.AddWithValue("@item", item.Item ?? string.Empty);
                        itemCommand.Parameters.AddWithValue("@categoryid", item.categoryid);
                        itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                        itemCommand.Parameters.AddWithValue("@Unit", item.Unit ?? string.Empty);
                        itemCommand.Parameters.AddWithValue("@PricePerUnit", item.PricePerUnit);
                        itemCommand.Parameters.AddWithValue("@DiscountPercentage", item.DiscountPercentage);
                        itemCommand.Parameters.AddWithValue("@DiscountAmount", item.DiscountAmount);
                        itemCommand.Parameters.AddWithValue("@Tax", item.Tax);
                        itemCommand.Parameters.AddWithValue("@TaxAmount", item.TaxAmount);
                        itemCommand.Parameters.AddWithValue("@Amount", item.Amount);


                        await itemCommand.ExecuteNonQueryAsync();
                    }
                }

                await transaction.CommitAsync();
                return billId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PurchaseBill?> GetBillByIdAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var billQuery = @"
                SELECT id, bill_number, bill_date, state_of_supply, phone_no, po_no, po_date, 
                       eway_bill_no, transport_name, delivery_location, vehicle_number, 
                       delivery_date, payment_type, description, image_path, round_off, 
                       total, created_date
                FROM TradeDocuments 
                WHERE id = @Id";

            using var billCommand = new NpgsqlCommand(billQuery, connection);
            billCommand.Parameters.AddWithValue("@Id", id);

            using var billReader = await billCommand.ExecuteReaderAsync();

            if (!await billReader.ReadAsync())
                return null;

            var bill = new PurchaseBill
            {
                Id = billReader.GetInt32("id"),
                BillNumber = billReader.GetString("bill_number"),
                BillDate = billReader.GetDateTime("bill_date"),
                StateOfSupply = billReader.GetString("state_of_supply"),
                PhoneNo = billReader.GetString("phone_no"),
                PONo = billReader.GetString("po_no"),
                PODate = billReader.IsDBNull("po_date") ? null : billReader.GetDateTime("po_date"),
                EWayBillNo = billReader.GetString("eway_bill_no"),
                TransportName = billReader.GetString("transport_name"),
                DeliveryLocation = billReader.GetString("delivery_location"),
                VehicleNumber = billReader.GetString("vehicle_number"),
                DeliveryDate = billReader.IsDBNull("delivery_date") ? null : billReader.GetDateTime("delivery_date"),
                PaymentType = billReader.GetString("payment_type"),
                Description = billReader.GetString("description"),
                ImagePath = billReader.GetString("image_path"),
                RoundOffValue = billReader.GetDecimal("round_off"),
                Total = billReader.GetDecimal("total"),
                CreatedDate = billReader.GetDateTime("created_date")
            };

            await billReader.CloseAsync();

            // Get Bill Items
            var itemsQuery = @"
                SELECT id, tradedocumentsid, item, quantity, unit, price_per_unit, 
                       discount_percentage, discount_amount, tax, tax_amount, amount
                FROM TradeDocumentItems 
                WHERE tradedocumentsid = @BillId";

            using var itemsCommand = new NpgsqlCommand(itemsQuery, connection);
            itemsCommand.Parameters.AddWithValue("@BillId", id);

            using var itemsReader = await itemsCommand.ExecuteReaderAsync();

            while (await itemsReader.ReadAsync())
            {
                bill.BillItems.Add(new PurchaseBillItem
                {
                    Id = itemsReader.GetInt32("id"),
                    BillId = itemsReader.GetInt32("tradedocumentsid"),
                    Item = itemsReader.GetString("item"),
                    Quantity = itemsReader.GetDecimal("quantity"),
                    Unit = itemsReader.GetString("unit"),
                    PricePerUnit = itemsReader.GetDecimal("price_per_unit"),
                    DiscountPercentage = itemsReader.GetDecimal("discount_percentage"),
                    DiscountAmount = itemsReader.GetDecimal("discount_amount"),
                    Tax = itemsReader.GetString("tax"),
                    TaxAmount = itemsReader.GetDecimal("tax_amount"),
                    Amount = itemsReader.GetDecimal("amount")
                });
            }

            return bill;
        }

        public async Task<List<PurchaseBill>> GetAllBillsAsync()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT id, bill_number, bill_date, state_of_supply, phone_no, po_no, po_date, 
                       eway_bill_no, transport_name, delivery_location, vehicle_number, 
                       delivery_date, payment_type, description, image_path, round_off, 
                       total, created_date
                FROM bills 
                ORDER BY created_date DESC";

            using var command = new NpgsqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            var bills = new List<PurchaseBill>();

            while (await reader.ReadAsync())
            {
                bills.Add(new PurchaseBill
                {
                    Id = reader.GetInt32("id"),
                    BillNumber = reader.GetString("bill_number"),
                    BillDate = reader.GetDateTime("bill_date"),
                    StateOfSupply = reader.GetString("state_of_supply"),
                    PhoneNo = reader.GetString("phone_no"),
                    PONo = reader.GetString("po_no"),
                    PODate = reader.IsDBNull("po_date") ? null : reader.GetDateTime("po_date"),
                    EWayBillNo = reader.GetString("eway_bill_no"),
                    TransportName = reader.GetString("transport_name"),
                    DeliveryLocation = reader.GetString("delivery_location"),
                    VehicleNumber = reader.GetString("vehicle_number"),
                    DeliveryDate = reader.IsDBNull("delivery_date") ? null : reader.GetDateTime("delivery_date"),
                    PaymentType = reader.GetString("payment_type"),
                    Description = reader.GetString("description"),
                    ImagePath = reader.GetString("image_path"),
                    RoundOff = reader.GetBoolean("round_off"),
                    Total = reader.GetDecimal("total"),
                    CreatedDate = reader.GetDateTime("created_date")
                });
            }

            return bills;
        }

        public async Task<bool> UpdateBillAsync(PurchaseBill bill)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Update Bill
                var billQuery = @"
                    UPDATE bills SET 
                        bill_number = @BillNumber, bill_date = @BillDate, state_of_supply = @StateOfSupply, 
                        phone_no = @PhoneNo, po_no = @PONo, po_date = @PODate, eway_bill_no = @EWayBillNo, 
                        transport_name = @TransportName, delivery_location = @DeliveryLocation, 
                        vehicle_number = @VehicleNumber, delivery_date = @DeliveryDate, 
                        payment_type = @PaymentType, description = @Description, image_path = @ImagePath, 
                        round_off = @RoundOff, total = @Total
                    WHERE id = @Id";

                using var billCommand = new NpgsqlCommand(billQuery, connection, transaction);
                billCommand.Parameters.AddWithValue("@Id", bill.Id);
                billCommand.Parameters.AddWithValue("@BillNumber", bill.BillNumber);
                billCommand.Parameters.AddWithValue("@BillDate", bill.BillDate);
                billCommand.Parameters.AddWithValue("@StateOfSupply", bill.StateOfSupply);
                billCommand.Parameters.AddWithValue("@PhoneNo", bill.PhoneNo);
                billCommand.Parameters.AddWithValue("@PONo", bill.PONo);
                billCommand.Parameters.AddWithValue("@PODate", (object?)bill.PODate ?? DBNull.Value);
                billCommand.Parameters.AddWithValue("@EWayBillNo", bill.EWayBillNo);
                billCommand.Parameters.AddWithValue("@TransportName", bill.TransportName);
                billCommand.Parameters.AddWithValue("@DeliveryLocation", bill.DeliveryLocation);
                billCommand.Parameters.AddWithValue("@VehicleNumber", bill.VehicleNumber);
                billCommand.Parameters.AddWithValue("@DeliveryDate", (object?)bill.DeliveryDate ?? DBNull.Value);
                billCommand.Parameters.AddWithValue("@PaymentType", bill.PaymentType);
                billCommand.Parameters.AddWithValue("@Description", bill.Description);
                billCommand.Parameters.AddWithValue("@ImagePath", bill.ImagePath);
                billCommand.Parameters.AddWithValue("@RoundOff", bill.RoundOff);
                billCommand.Parameters.AddWithValue("@Total", bill.Total);

                await billCommand.ExecuteNonQueryAsync();

                // Delete existing items
                var deleteItemsQuery = "DELETE FROM bill_items WHERE bill_id = @BillId";
                using var deleteCommand = new NpgsqlCommand(deleteItemsQuery, connection, transaction);
                deleteCommand.Parameters.AddWithValue("@BillId", bill.Id);
                await deleteCommand.ExecuteNonQueryAsync();

                // Insert updated items
                foreach (var item in bill.BillItems)
                {
                    var itemQuery = @"
                        INSERT INTO bill_items (bill_id, item, quantity, unit, price_per_unit, 
                                              discount_percentage, discount_amount, tax, tax_amount, amount)
                        VALUES (@BillId, @Item, @Quantity, @Unit, @PricePerUnit, 
                               @DiscountPercentage, @DiscountAmount, @Tax, @TaxAmount, @Amount)";

                    using var itemCommand = new NpgsqlCommand(itemQuery, connection, transaction);
                    itemCommand.Parameters.AddWithValue("@BillId", bill.Id);
                    itemCommand.Parameters.AddWithValue("@Item", item.Item);
                    itemCommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                    itemCommand.Parameters.AddWithValue("@Unit", item.Unit);
                    itemCommand.Parameters.AddWithValue("@PricePerUnit", item.PricePerUnit);
                    itemCommand.Parameters.AddWithValue("@DiscountPercentage", item.DiscountPercentage);
                    itemCommand.Parameters.AddWithValue("@DiscountAmount", item.DiscountAmount);
                    itemCommand.Parameters.AddWithValue("@Tax", item.Tax);
                    itemCommand.Parameters.AddWithValue("@TaxAmount", item.TaxAmount);
                    itemCommand.Parameters.AddWithValue("@Amount", item.Amount);

                    await itemCommand.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteBillAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Delete bill items first
                var deleteItemsQuery = "DELETE FROM bill_items WHERE bill_id = @BillId";
                using var deleteItemsCommand = new NpgsqlCommand(deleteItemsQuery, connection, transaction);
                deleteItemsCommand.Parameters.AddWithValue("@BillId", id);
                await deleteItemsCommand.ExecuteNonQueryAsync();

                // Delete bill
                var deleteBillQuery = "DELETE FROM bills WHERE id = @Id";
                using var deleteBillCommand = new NpgsqlCommand(deleteBillQuery, connection, transaction);
                deleteBillCommand.Parameters.AddWithValue("@Id", id);
                var rowsAffected = await deleteBillCommand.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
                return rowsAffected > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public string GenerateBillNumber()
        {
            return $"BILL-{DateTime.Now:yyyyMMdd}-{DateTime.Now.Ticks.ToString().Substring(10)}";
        }
    }


}
