using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MUNEEMJI.Models.Setting;
using Npgsql;

namespace MUNEEMJI.Areas.Settings.Controllers
{
    [Area("Settings")]
    public class TransactionController : Controller
    {
        public IActionResult Index()
        {
            var connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
            TransactionSettingsViewModel settings = null;

            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
            SELECT 
                firm_id AS ""FirmId"",
                invoice_bill_no AS ""InvoiceBillNo"",
                add_time_on_transactions AS ""AddTimeOnTransactions"",
                print_time_on_invoices AS ""PrintTimeOnInvoices"",
                cash_sale_by_default AS ""CashSaleByDefault"",
                billing_name_of_parties AS ""BillingNameOfParties"",
                customer_po_details AS ""CustomerPODetails"",
                eway_bill_no AS ""EwayBillNo"",
                quick_entry AS ""QuickEntry"",
                do_not_show_invoice_preview AS ""DoNotShowInvoicePreview"",
                enable_passcode AS ""EnablePasscode"",
                discount_during_payments AS ""DiscountDuringPayments"",
                link_payments_to_invoices AS ""LinkPaymentsToInvoices"",
                due_dates_payment_terms AS ""DueDatesPaymentTerms"",
                show_profit_sale_invoice AS ""ShowProfitSaleInvoice"",
                inclusive_exclusive_tax AS ""InclusiveExclusiveTax"",
                display_purchase_price AS ""DisplayPurchasePrice"",
                show_last5_sale_price AS ""ShowLast5SalePrice"",
                free_item_quantity AS ""FreeItemQuantity"",
                count_enabled AS ""CountEnabled"",
                transaction_wise_tax AS ""TransactionWiseTax"",
                transaction_wise_discount AS ""TransactionWiseDiscount"",
                round_off_total AS ""RoundOffTotal"",
                round_off_type AS ""RoundOffType"",
                round_off_value AS ""RoundOffValue"",
                billing_type AS ""BillingType"",
                sale_prefix AS ""SalePrefix"",
                credit_note_prefix AS ""CreditNotePrefix"",
                sale_order_prefix AS ""SaleOrderPrefix"",
                purchase_order_prefix AS ""PurchaseOrderPrefix"",
                estimate_prefix AS ""EstimatePrefix"",
                proforma_invoice_prefix AS ""ProformaInvoicePrefix"",
                delivery_challan_prefix AS ""DeliveryChallanPrefix"",
                payment_in_prefix AS ""PaymentInPrefix""
            FROM transaction_settings
            LIMIT 1;   -- ✅ Only first record
        ";

                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) // ✅ FirstOrDefault
                    {
                        settings = new TransactionSettingsViewModel
                        {
                            FirmId = reader["FirmId"] as int? ?? 0,
                            InvoiceBillNo = reader["InvoiceBillNo"] as bool? ?? false,
                            AddTimeOnTransactions = reader["AddTimeOnTransactions"] as bool? ?? false,
                            PrintTimeOnInvoices = reader["PrintTimeOnInvoices"] as bool? ?? false,
                            CashSaleByDefault = reader["CashSaleByDefault"] as bool? ?? false,
                            BillingNameOfParties = reader["BillingNameOfParties"] as bool? ?? false,
                            CustomerPODetails = reader["CustomerPODetails"] as bool? ?? false,
                            EwayBillNo = reader["EwayBillNo"] as bool? ?? false,
                            QuickEntry = reader["QuickEntry"] as bool? ?? false,
                            DoNotShowInvoicePreview = reader["DoNotShowInvoicePreview"] as bool? ?? false,
                            EnablePasscode = reader["EnablePasscode"] as bool? ?? false,
                            DiscountDuringPayments = reader["DiscountDuringPayments"] as bool? ?? false,
                            LinkPaymentsToInvoices = reader["LinkPaymentsToInvoices"] as bool? ?? false,
                            DueDatesPaymentTerms = reader["DueDatesPaymentTerms"] as bool? ?? false,
                            ShowProfitSaleInvoice = reader["ShowProfitSaleInvoice"] as bool? ?? false,
                            InclusiveExclusiveTax = reader["InclusiveExclusiveTax"] as bool? ?? false,
                            DisplayPurchasePrice = reader["DisplayPurchasePrice"] as bool? ?? false,
                            ShowLast5SalePrice = reader["ShowLast5SalePrice"] as bool? ?? false,
                            FreeItemQuantity = reader["FreeItemQuantity"] as bool? ?? false,
                            CountEnabled = reader["CountEnabled"] as bool? ?? false,
                            TransactionWiseTax = reader["TransactionWiseTax"] as bool? ?? false,
                            TransactionWiseDiscount = reader["TransactionWiseDiscount"] as bool? ?? false,
                            RoundOffTotal = reader["RoundOffTotal"] as bool? ?? false,
                            RoundOffType = reader["RoundOffType"] as string ?? "Nearest",
                            RoundOffValue = reader["RoundOffValue"] as decimal? ?? 1,
                            BillingType = reader["BillingType"] as string ?? "Full Sale",
                            SalePrefix = reader["SalePrefix"] as string ?? "None",
                            CreditNotePrefix = reader["CreditNotePrefix"] as string ?? "None",
                            SaleOrderPrefix = reader["SaleOrderPrefix"] as string ?? "None",
                            PurchaseOrderPrefix = reader["PurchaseOrderPrefix"] as string ?? "None",
                            EstimatePrefix = reader["EstimatePrefix"] as string ?? "None",
                            ProformaInvoicePrefix = reader["ProformaInvoicePrefix"] as string ?? "None",
                            DeliveryChallanPrefix = reader["DeliveryChallanPrefix"] as string ?? "None",
                            PaymentInPrefix = reader["PaymentInPrefix"] as string ?? "None"
                        };
                    }
                }
            }

            return View(settings);
        }



        [HttpPost]
        public async Task<IActionResult> SaveTransactionSettings([FromBody]TransactionSettingsViewModel model)
        {
            try
            {
                var connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";
                using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                // Check if settings already exist for this firm
                var existingSettingsQuery = @"
                                            SELECT COUNT(*) 
                                            FROM transaction_settings 
                                            WHERE firm_id = @FirmId";

                var existsCount = await connection.QuerySingleAsync<int>(existingSettingsQuery, new { FirmId = model.FirmId });
                bool isUpdate = existsCount > 0;

                string query;

                if (isUpdate)
                {
                    // Update existing record
                    query = @"
                                UPDATE transaction_settings 
                                SET 
                                    invoice_bill_no = @InvoiceBillNo,
                                    add_time_on_transactions = @AddTimeOnTransactions,
                                    print_time_on_invoices = @PrintTimeOnInvoices,
                                    cash_sale_by_default = @CashSaleByDefault,
                                    billing_name_of_parties = @BillingNameOfParties,
                                    customer_po_details = @CustomerPODetails,
                                    eway_bill_no = @EwayBillNo,
                                    quick_entry = @QuickEntry,
                                    do_not_show_invoice_preview = @DoNotShowInvoicePreview,
                                    enable_passcode = @EnablePasscode,
                                    discount_during_payments = @DiscountDuringPayments,
                                    link_payments_to_invoices = @LinkPaymentsToInvoices,
                                    due_dates_payment_terms = @DueDatesPaymentTerms,
                                    show_profit_sale_invoice = @ShowProfitSaleInvoice,
                                    inclusive_exclusive_tax = @InclusiveExclusiveTax,
                                    display_purchase_price = @DisplayPurchasePrice,
                                    show_last5_sale_price = @ShowLast5SalePrice,
                                    free_item_quantity = @FreeItemQuantity,
                                    count_enabled = @CountEnabled,
                                    transaction_wise_tax = @TransactionWiseTax,
                                    transaction_wise_discount = @TransactionWiseDiscount,
                                    round_off_total = @RoundOffTotal,
                                    round_off_type = @RoundOffType,
                                    round_off_value = @RoundOffValue,
                                    billing_type = @BillingType,
                                    sale_prefix = @SalePrefix,
                                    credit_note_prefix = @CreditNotePrefix,
                                    sale_order_prefix = @SaleOrderPrefix,
                                    purchase_order_prefix = @PurchaseOrderPrefix,
                                    estimate_prefix = @EstimatePrefix,
                                    proforma_invoice_prefix = @ProformaInvoicePrefix,
                                    delivery_challan_prefix = @DeliveryChallanPrefix,
                                    payment_in_prefix = @PaymentInPrefix,
                                    updated_at = @UpdatedAt,
                                    updated_by = @UpdatedBy
                                WHERE firm_id = @FirmId";
                }
                else
                {
                    // Insert new record
                    query = @"
                              INSERT INTO transaction_settings (
                                  firm_id, invoice_bill_no, add_time_on_transactions, print_time_on_invoices,
                                  cash_sale_by_default, billing_name_of_parties, customer_po_details,
                                  eway_bill_no, quick_entry, do_not_show_invoice_preview, enable_passcode,
                                  discount_during_payments, link_payments_to_invoices, due_dates_payment_terms,
                                  show_profit_sale_invoice, inclusive_exclusive_tax, display_purchase_price,
                                  show_last5_sale_price, free_item_quantity, count_enabled, transaction_wise_tax,
                                  transaction_wise_discount, round_off_total, round_off_type, round_off_value,
                                  billing_type, sale_prefix, credit_note_prefix, sale_order_prefix,
                                  purchase_order_prefix, estimate_prefix, proforma_invoice_prefix,
                                  delivery_challan_prefix, payment_in_prefix, created_at, created_by
                              ) VALUES (
                                  @FirmId, @InvoiceBillNo, @AddTimeOnTransactions, @PrintTimeOnInvoices,
                                  @CashSaleByDefault, @BillingNameOfParties, @CustomerPODetails,
                                  @EwayBillNo, @QuickEntry, @DoNotShowInvoicePreview, @EnablePasscode,
                                  @DiscountDuringPayments, @LinkPaymentsToInvoices, @DueDatesPaymentTerms,
                                  @ShowProfitSaleInvoice, @InclusiveExclusiveTax, @DisplayPurchasePrice,
                                  @ShowLast5SalePrice, @FreeItemQuantity, @CountEnabled, @TransactionWiseTax,
                                  @TransactionWiseDiscount, @RoundOffTotal, @RoundOffType, @RoundOffValue,
                                  @BillingType, @SalePrefix, @CreditNotePrefix, @SaleOrderPrefix,
                                  @PurchaseOrderPrefix, @EstimatePrefix, @ProformaInvoicePrefix,
                                  @DeliveryChallanPrefix, @PaymentInPrefix, @CreatedAt, @CreatedBy
                              )";
                }

                var parameters = new
                {
                    FirmId = model.FirmId,
                    InvoiceBillNo = model.InvoiceBillNo,
                    AddTimeOnTransactions = model.AddTimeOnTransactions,
                    PrintTimeOnInvoices = model.PrintTimeOnInvoices,
                    CashSaleByDefault = model.CashSaleByDefault,
                    BillingNameOfParties = model.BillingNameOfParties,
                    CustomerPODetails = model.CustomerPODetails,
                    EwayBillNo = model.EwayBillNo,
                    QuickEntry = model.QuickEntry,
                    DoNotShowInvoicePreview = model.DoNotShowInvoicePreview,
                    EnablePasscode = model.EnablePasscode,
                    DiscountDuringPayments = model.DiscountDuringPayments,
                    LinkPaymentsToInvoices = model.LinkPaymentsToInvoices,
                    DueDatesPaymentTerms = model.DueDatesPaymentTerms,
                    ShowProfitSaleInvoice = model.ShowProfitSaleInvoice,
                    InclusiveExclusiveTax = model.InclusiveExclusiveTax,
                    DisplayPurchasePrice = model.DisplayPurchasePrice,
                    ShowLast5SalePrice = model.ShowLast5SalePrice,
                    FreeItemQuantity = model.FreeItemQuantity,
                    CountEnabled = model.CountEnabled,
                    TransactionWiseTax = model.TransactionWiseTax,
                    TransactionWiseDiscount = model.TransactionWiseDiscount,
                    RoundOffTotal = model.RoundOffTotal,
                    RoundOffType = model.RoundOffType,
                    RoundOffValue = model.RoundOffValue,
                    BillingType = model.BillingType,
                    SalePrefix = model.SalePrefix,
                    CreditNotePrefix = model.CreditNotePrefix,
                    SaleOrderPrefix = model.SaleOrderPrefix,
                    PurchaseOrderPrefix = model.PurchaseOrderPrefix,
                    EstimatePrefix = model.EstimatePrefix,
                    ProformaInvoicePrefix = model.ProformaInvoicePrefix,
                    DeliveryChallanPrefix = model.DeliveryChallanPrefix,
                    PaymentInPrefix = model.PaymentInPrefix,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.Identity?.Name ?? "System",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedBy = User.Identity?.Name ?? "System"
                };

                await connection.ExecuteAsync(query, parameters);

                return Json(new { success = true, message = isUpdate ? "Settings updated successfully!" : "Settings saved successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while saving settings: " + ex.Message });
            }
        }
    }
}
