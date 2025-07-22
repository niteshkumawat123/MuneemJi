using MUNEEMJI.Models;
using Npgsql;
using System.Data;

namespace MUNEEMJI.Repositories
{
    public interface IEstimate_QuotationsRepository
    {
        Task<int> CreateBillAsync(PurchaseBill bill);
        Task<PurchaseBill?> GetBillByIdAsync(int id);
        Task<List<PurchaseBill>> GetAllBillsAsync();
        Task<bool> UpdateBillAsync(PurchaseBill bill);
        Task<bool> DeleteBillAsync(int id);
        string GenerateBillNumber();
        Task<int> UpdateEntries(PurchaseBill bill);
    }
    public class Estimate_QuotationsRepository : IEstimate_QuotationsRepository
    {
        private readonly string _connectionString;

        public Estimate_QuotationsRepository(IConfiguration configuration)
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
                                     total, created_date,paidReciveamount,TradeDocumentTypesid,PartyId,orderstatusid,OrderDate,OrderNo,duedate,ChallanDate,ChallanNo)
                    VALUES (@BillNumber, @BillDate, @StateOfSupply, @PhoneNo, @PONo, @PODate, 
                           @EWayBillNo, @TransportName, @DeliveryLocation, @VehicleNumber, 
                           @DeliveryDate, @PaymentType, @Description, @ImagePath, @RoundOff, 
                           @Total, @CreatedDate,@paidReciveamount,@TradeDocumentTypesid,@PartyId,@orderstatusid,@OrderDate,@OrderNo,@duedate,@ChallanDate,@ChallanNo)
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
                billCommand.Parameters.AddWithValue("@TradeDocumentTypesid", (int)TradeDocumentTypes.Estimation);
                billCommand.Parameters.AddWithValue("@PartyId", bill.PartyId);
                billCommand.Parameters.AddWithValue("@orderstatusid", (int)TradeDocumentStatusEnum.open);
                billCommand.Parameters.AddWithValue("@OrderDate", bill.OrderDate);
                billCommand.Parameters.AddWithValue("@OrderNo", bill.OrderNo ?? string.Empty);
                billCommand.Parameters.AddWithValue("@duedate", bill.DueDate);
                billCommand.Parameters.AddWithValue("@ChallanDate", bill.Challandate);
                billCommand.Parameters.AddWithValue("@ChallanNo", bill.ChallanNo);
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
                SELECT
                      td.id,
                      td.bill_number,
                      td.bill_date,
                      td.state_of_supply,
                      td.phone_no,
                      td.po_no,
                      td.po_date,
                      td.eway_bill_no,
                      td.transport_name,
                      td.delivery_location,
                      td.vehicle_number,
                      td.delivery_date,
                      td.payment_type,
                      td.description,
                      td.image_path,
                      td.round_off,
                      td.total,
                      td.paidreciveamount,
                      td.partyid,
                      pt.party_name,
                      td.created_date
                                FROM public.tradedocuments td
                                LEFT JOIN parties pt ON td.partyid = pt.id 
                                WHERE td.id = @Id";

            using var billCommand = new NpgsqlCommand(billQuery, connection);
            billCommand.Parameters.AddWithValue("@Id", id);

            using var billReader = await billCommand.ExecuteReaderAsync();

            if (!await billReader.ReadAsync())
                return null;




            var bill = new PurchaseBill
            {
                Id = billReader.IsDBNull(billReader.GetOrdinal("id"))
                  ? 0
                  : billReader.GetInt32(billReader.GetOrdinal("id")),

                BillNumber = billReader.IsDBNull(billReader.GetOrdinal("bill_number"))
                  ? string.Empty
                  : billReader.GetString(billReader.GetOrdinal("bill_number")),

                BillDate = billReader.IsDBNull(billReader.GetOrdinal("bill_date"))
                  ? DateTime.MinValue
                  : billReader.GetDateTime(billReader.GetOrdinal("bill_date")),

                StateOfSupply = billReader.IsDBNull(billReader.GetOrdinal("state_of_supply"))
                  ? string.Empty
                  : billReader.GetString(billReader.GetOrdinal("state_of_supply")),

                PhoneNo = billReader.IsDBNull(billReader.GetOrdinal("phone_no"))
                  ? string.Empty
                  : billReader.GetString(billReader.GetOrdinal("phone_no")),

                PONo = billReader.IsDBNull(billReader.GetOrdinal("po_no"))
                  ? string.Empty
                  : billReader.GetString(billReader.GetOrdinal("po_no")),

                PODate = billReader.IsDBNull(billReader.GetOrdinal("po_date"))
                  ? (DateTime?)null
                  : billReader.GetDateTime(billReader.GetOrdinal("po_date")),

                EWayBillNo = billReader.IsDBNull(billReader.GetOrdinal("eway_bill_no"))
                  ? string.Empty
                  : billReader.GetString(billReader.GetOrdinal("eway_bill_no")),

                TransportName = billReader.IsDBNull(billReader.GetOrdinal("transport_name"))
                  ? string.Empty
                  : billReader.GetString(billReader.GetOrdinal("transport_name")),

                DeliveryLocation = billReader.IsDBNull(billReader.GetOrdinal("delivery_location"))
                  ? string.Empty
                  : billReader.GetString(billReader.GetOrdinal("delivery_location")),

                VehicleNumber = billReader.IsDBNull(billReader.GetOrdinal("vehicle_number"))
                  ? string.Empty
                  : billReader.GetString(billReader.GetOrdinal("vehicle_number")),

                DeliveryDate = billReader.IsDBNull(billReader.GetOrdinal("delivery_date"))
                  ? (DateTime?)null
                  : billReader.GetDateTime(billReader.GetOrdinal("delivery_date")),

                PaymentType = billReader.IsDBNull(billReader.GetOrdinal("payment_type"))
                  ? string.Empty
                  : billReader.GetString(billReader.GetOrdinal("payment_type")),

                Description = billReader.IsDBNull(billReader.GetOrdinal("description"))
                  ? string.Empty
                  : billReader.GetString(billReader.GetOrdinal("description")),

                ImagePath = billReader.IsDBNull(billReader.GetOrdinal("image_path"))
                  ? string.Empty
                  : billReader.GetString(billReader.GetOrdinal("image_path")),

                RoundOffValue = billReader.IsDBNull(billReader.GetOrdinal("round_off"))
                  ? 0
                  : billReader.GetDecimal(billReader.GetOrdinal("round_off")),

                Total = billReader.IsDBNull(billReader.GetOrdinal("total"))
                  ? 0
                  : billReader.GetDecimal(billReader.GetOrdinal("total")),

                paidReciveamount = billReader.IsDBNull(billReader.GetOrdinal("paidreciveamount"))
                  ? 0
                  : billReader.GetDecimal(billReader.GetOrdinal("paidreciveamount")),

                PartyId = billReader.IsDBNull(billReader.GetOrdinal("partyid"))
                  ? 0
                  : billReader.GetInt32(billReader.GetOrdinal("partyid")),

                PartyName = billReader.IsDBNull(billReader.GetOrdinal("party_name"))
                  ? string.Empty
                  : billReader.GetString(billReader.GetOrdinal("party_name")),

                CreatedDate = billReader.IsDBNull(billReader.GetOrdinal("created_date"))
                  ? DateTime.MinValue
               : billReader.GetDateTime(billReader.GetOrdinal("created_date"))
            };

            await billReader.CloseAsync();

            // Get Bill Items
            var itemsQuery = @"
                SELECT id, tradedocumentsid, item, quantity, unit, price_per_unit, 
                       discount_percentage, discount_amount, tax, tax_amount, amount,itemid
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
                    Amount = itemsReader.GetDecimal("amount"),
                    ItemId = itemsReader.GetInt32("itemid"),
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



        public string GenerateBillNumber()
        {
            return $"BILL-{DateTime.Now:yyyyMMdd}-{DateTime.Now.Ticks.ToString().Substring(10)}";

        }
        #region Delete Update
        public async Task<bool> DeleteBillAsync(int id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Delete bill items first
                var deleteItemsQuery = "DELETE FROM TradeDocumentItems WHERE TradeDocumentsid = @BillId";
                using var deleteItemsCommand = new NpgsqlCommand(deleteItemsQuery, connection, transaction);
                deleteItemsCommand.Parameters.AddWithValue("@BillId", id);
                await deleteItemsCommand.ExecuteNonQueryAsync();

                // Delete bill
                var deleteBillQuery = "DELETE FROM TradeDocuments WHERE id = @Id";
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
        public async Task<int> UpdateEntries(PurchaseBill bill)
        {
            if (bill.Id <= 0)
                throw new ArgumentException("Invalid bill ID for update");

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // 🔁 Update existing bill
                var updateQuery = @"
            UPDATE TradeDocuments 
            SET bill_number = @BillNumber,
                bill_date = @BillDate,
                state_of_supply = @StateOfSupply,
                phone_no = @PhoneNo,
                po_no = @PONo,
                po_date = @PODate,
                eway_bill_no = @EWayBillNo,
                transport_name = @TransportName,
                delivery_location = @DeliveryLocation,
                vehicle_number = @VehicleNumber,
                delivery_date = @DeliveryDate,
                payment_type = @PaymentType,
                description = @Description,
                image_path = @ImagePath,
                round_off = @RoundOff,
                total = @Total,
                created_date = @CreatedDate,
                paidReciveamount = @paidReciveamount,
                PartyId = @PartyId
            WHERE id = @Id";

                using var updateCommand = new NpgsqlCommand(updateQuery, connection, transaction);
                updateCommand.Parameters.AddWithValue("@Id", bill.Id);
                updateCommand.Parameters.AddWithValue("@BillNumber", bill.BillNumber ?? string.Empty);
                updateCommand.Parameters.AddWithValue("@BillDate", bill.BillDate);
                updateCommand.Parameters.AddWithValue("@StateOfSupply", bill.StateOfSupply ?? string.Empty);
                updateCommand.Parameters.AddWithValue("@PhoneNo", bill.PhoneNo ?? string.Empty);
                updateCommand.Parameters.AddWithValue("@PONo", bill.PONo ?? string.Empty);
                updateCommand.Parameters.AddWithValue("@PODate", (object?)bill.PODate ?? DBNull.Value);
                updateCommand.Parameters.AddWithValue("@EWayBillNo", bill.EWayBillNo ?? string.Empty);
                updateCommand.Parameters.AddWithValue("@TransportName", bill.TransportName ?? string.Empty);
                updateCommand.Parameters.AddWithValue("@DeliveryLocation", bill.DeliveryLocation ?? string.Empty);
                updateCommand.Parameters.AddWithValue("@VehicleNumber", bill.VehicleNumber ?? string.Empty);
                updateCommand.Parameters.AddWithValue("@DeliveryDate", (object?)bill.DeliveryDate ?? DBNull.Value);
                updateCommand.Parameters.AddWithValue("@PaymentType", bill.PaymentType ?? string.Empty);
                updateCommand.Parameters.AddWithValue("@Description", bill.Description ?? string.Empty);
                updateCommand.Parameters.AddWithValue("@ImagePath", bill.ImagePath ?? string.Empty);
                updateCommand.Parameters.AddWithValue("@RoundOff", bill.RoundOffValue);
                updateCommand.Parameters.AddWithValue("@Total", bill.Total);
                updateCommand.Parameters.AddWithValue("@CreatedDate", bill.CreatedDate);
                updateCommand.Parameters.AddWithValue("@paidReciveamount", bill.paidReciveamount);
                updateCommand.Parameters.AddWithValue("@PartyId", bill.PartyId);

                await updateCommand.ExecuteNonQueryAsync();

                // ❌ Delete existing items for this bill (clean slate approach)
                var deleteItemsQuery = "DELETE FROM TradeDocumentItems WHERE TradeDocumentsid = @BillId";
                using var deleteCommand = new NpgsqlCommand(deleteItemsQuery, connection, transaction);
                deleteCommand.Parameters.AddWithValue("@BillId", bill.Id);
                await deleteCommand.ExecuteNonQueryAsync();

                // ➕ Re-insert bill items
                foreach (var item in bill.BillItems)
                {
                    if (item.ItemId > 0)
                    {
                        var itemQuery = @"
                    INSERT INTO TradeDocumentItems (TradeDocumentsid, itemid, serialno, batchno, modelno, expirydate, mfgdate, item, categoryid, quantity, unit, price_per_unit, 
                                                    discount_percentage, discount_amount, tax, tax_amount, amount)
                    VALUES (@TradeDocumentsid, @itemid, @serialno, @batchno, @modelno, @expirydate, @mfgdate, @item, @categoryid, @Quantity, @Unit, @PricePerUnit, 
                            @DiscountPercentage, @DiscountAmount, @Tax, @TaxAmount, @Amount)";

                        using var itemCommand = new NpgsqlCommand(itemQuery, connection, transaction);
                        itemCommand.Parameters.AddWithValue("@TradeDocumentsid", bill.Id);
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
                return bill.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        #endregion
    }
}

