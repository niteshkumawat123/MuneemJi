using MUNEEMJI.Models.Setting;
using Npgsql;

namespace MUNEEMJI.Controllers
{
    public class TransactionSettingsController
    {
        public TransactionSettingsViewModel GetTransactionByFirmId(int firmId)
        {
            // default object with defaults already applied
            var settings = new TransactionSettingsViewModel
            {
                RoundOffType = "Nearest",
                RoundOffValue = 1.00m,
                BillingType = "Full Sale",
                SalePrefix = "None",
                CreditNotePrefix = "None",
                SaleOrderPrefix = "None",
                PurchaseOrderPrefix = "None",
                EstimatePrefix = "None",
                ProformaInvoicePrefix = "None",
                DeliveryChallanPrefix = "None",
                PaymentInPrefix = "None"
            };

            using (var connection = new NpgsqlConnection(MUNEEMJI.DbConfig.ConnectionString))
            {
                connection.Open();

                string sql = @"SELECT * 
                           FROM transaction_settings 
                           WHERE firm_id = @FirmId 
                           LIMIT 1;";

                using (var cmd = new NpgsqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@FirmId", firmId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            settings.Id = !reader.IsDBNull(reader.GetOrdinal("id")) ? reader.GetInt32(reader.GetOrdinal("id")) : 0;
                            settings.FirmId = !reader.IsDBNull(reader.GetOrdinal("firm_id")) ? reader.GetInt32(reader.GetOrdinal("firm_id")) : 0;
                            settings.InvoiceBillNo = !reader.IsDBNull(reader.GetOrdinal("invoice_bill_no")) && reader.GetBoolean(reader.GetOrdinal("invoice_bill_no"));
                            settings.AddTimeOnTransactions = !reader.IsDBNull(reader.GetOrdinal("add_time_on_transactions")) && reader.GetBoolean(reader.GetOrdinal("add_time_on_transactions"));
                            settings.PrintTimeOnInvoices = !reader.IsDBNull(reader.GetOrdinal("print_time_on_invoices")) && reader.GetBoolean(reader.GetOrdinal("print_time_on_invoices"));
                            settings.CashSaleByDefault = !reader.IsDBNull(reader.GetOrdinal("cash_sale_by_default")) && reader.GetBoolean(reader.GetOrdinal("cash_sale_by_default"));
                            settings.BillingNameOfParties = !reader.IsDBNull(reader.GetOrdinal("billing_name_of_parties")) && reader.GetBoolean(reader.GetOrdinal("billing_name_of_parties"));
                            settings.CustomerPODetails = !reader.IsDBNull(reader.GetOrdinal("customer_po_details")) && reader.GetBoolean(reader.GetOrdinal("customer_po_details"));
                            settings.EwayBillNo = !reader.IsDBNull(reader.GetOrdinal("eway_bill_no")) && reader.GetBoolean(reader.GetOrdinal("eway_bill_no"));
                            settings.QuickEntry = !reader.IsDBNull(reader.GetOrdinal("quick_entry")) && reader.GetBoolean(reader.GetOrdinal("quick_entry"));
                            settings.DoNotShowInvoicePreview = !reader.IsDBNull(reader.GetOrdinal("do_not_show_invoice_preview")) && reader.GetBoolean(reader.GetOrdinal("do_not_show_invoice_preview"));
                            settings.EnablePasscode = !reader.IsDBNull(reader.GetOrdinal("enable_passcode")) && reader.GetBoolean(reader.GetOrdinal("enable_passcode"));
                            settings.DiscountDuringPayments = !reader.IsDBNull(reader.GetOrdinal("discount_during_payments")) && reader.GetBoolean(reader.GetOrdinal("discount_during_payments"));
                            settings.LinkPaymentsToInvoices = !reader.IsDBNull(reader.GetOrdinal("link_payments_to_invoices")) && reader.GetBoolean(reader.GetOrdinal("link_payments_to_invoices"));
                            settings.DueDatesPaymentTerms = !reader.IsDBNull(reader.GetOrdinal("due_dates_payment_terms")) && reader.GetBoolean(reader.GetOrdinal("due_dates_payment_terms"));
                            settings.ShowProfitSaleInvoice = !reader.IsDBNull(reader.GetOrdinal("show_profit_sale_invoice")) && reader.GetBoolean(reader.GetOrdinal("show_profit_sale_invoice"));
                            settings.InclusiveExclusiveTax = !reader.IsDBNull(reader.GetOrdinal("inclusive_exclusive_tax")) && reader.GetBoolean(reader.GetOrdinal("inclusive_exclusive_tax"));
                            settings.DisplayPurchasePrice = !reader.IsDBNull(reader.GetOrdinal("display_purchase_price")) && reader.GetBoolean(reader.GetOrdinal("display_purchase_price"));
                            settings.ShowLast5SalePrice = !reader.IsDBNull(reader.GetOrdinal("show_last5_sale_price")) && reader.GetBoolean(reader.GetOrdinal("show_last5_sale_price"));
                            settings.FreeItemQuantity = !reader.IsDBNull(reader.GetOrdinal("free_item_quantity")) && reader.GetBoolean(reader.GetOrdinal("free_item_quantity"));
                            settings.CountEnabled = !reader.IsDBNull(reader.GetOrdinal("count_enabled")) && reader.GetBoolean(reader.GetOrdinal("count_enabled"));
                            settings.TransactionWiseTax = !reader.IsDBNull(reader.GetOrdinal("transaction_wise_tax")) && reader.GetBoolean(reader.GetOrdinal("transaction_wise_tax"));
                            settings.TransactionWiseDiscount = !reader.IsDBNull(reader.GetOrdinal("transaction_wise_discount")) && reader.GetBoolean(reader.GetOrdinal("transaction_wise_discount"));
                            settings.RoundOffTotal = !reader.IsDBNull(reader.GetOrdinal("round_off_total")) && reader.GetBoolean(reader.GetOrdinal("round_off_total"));
                            settings.RoundOffType = !reader.IsDBNull(reader.GetOrdinal("round_off_type")) ? reader.GetString(reader.GetOrdinal("round_off_type")) : settings.RoundOffType;
                            settings.RoundOffValue = !reader.IsDBNull(reader.GetOrdinal("round_off_value")) ? reader.GetDecimal(reader.GetOrdinal("round_off_value")) : settings.RoundOffValue;
                            settings.BillingType = !reader.IsDBNull(reader.GetOrdinal("billing_type")) ? reader.GetString(reader.GetOrdinal("billing_type")) : settings.BillingType;
                            settings.SalePrefix = !reader.IsDBNull(reader.GetOrdinal("sale_prefix")) ? reader.GetString(reader.GetOrdinal("sale_prefix")) : settings.SalePrefix;
                            settings.CreditNotePrefix = !reader.IsDBNull(reader.GetOrdinal("credit_note_prefix")) ? reader.GetString(reader.GetOrdinal("credit_note_prefix")) : settings.CreditNotePrefix;
                            settings.SaleOrderPrefix = !reader.IsDBNull(reader.GetOrdinal("sale_order_prefix")) ? reader.GetString(reader.GetOrdinal("sale_order_prefix")) : settings.SaleOrderPrefix;
                            settings.PurchaseOrderPrefix = !reader.IsDBNull(reader.GetOrdinal("purchase_order_prefix")) ? reader.GetString(reader.GetOrdinal("purchase_order_prefix")) : settings.PurchaseOrderPrefix;
                            settings.EstimatePrefix = !reader.IsDBNull(reader.GetOrdinal("estimate_prefix")) ? reader.GetString(reader.GetOrdinal("estimate_prefix")) : settings.EstimatePrefix;
                            settings.ProformaInvoicePrefix = !reader.IsDBNull(reader.GetOrdinal("proforma_invoice_prefix")) ? reader.GetString(reader.GetOrdinal("proforma_invoice_prefix")) : settings.ProformaInvoicePrefix;
                            settings.DeliveryChallanPrefix = !reader.IsDBNull(reader.GetOrdinal("delivery_challan_prefix")) ? reader.GetString(reader.GetOrdinal("delivery_challan_prefix")) : settings.DeliveryChallanPrefix;
                            settings.PaymentInPrefix = !reader.IsDBNull(reader.GetOrdinal("payment_in_prefix")) ? reader.GetString(reader.GetOrdinal("payment_in_prefix")) : settings.PaymentInPrefix;
                            settings.TransportName = !reader.IsDBNull(reader.GetOrdinal("transport_name"))? reader.GetBoolean(reader.GetOrdinal("transport_name")): settings.TransportName;
                            settings.VehicleNumber = !reader.IsDBNull(reader.GetOrdinal("vehicle_number"))
                                ? reader.GetBoolean(reader.GetOrdinal("vehicle_number"))
                                : settings.VehicleNumber;

                            settings.DeliveryDate = !reader.IsDBNull(reader.GetOrdinal("delivery_date"))
                                ? reader.GetBoolean(reader.GetOrdinal("delivery_date"))
                                : settings.DeliveryDate;

                            settings.DeliveryLocation = !reader.IsDBNull(reader.GetOrdinal("delivery_location"))
                                ? reader.GetBoolean(reader.GetOrdinal("delivery_location"))
                                : settings.DeliveryLocation;

                            settings.Field5 = !reader.IsDBNull(reader.GetOrdinal("field5"))
                                ? reader.GetBoolean(reader.GetOrdinal("field5"))
                                : settings.Field5;

                            settings.Field6 = !reader.IsDBNull(reader.GetOrdinal("field6"))
                                ? reader.GetBoolean(reader.GetOrdinal("field6"))
                                : settings.Field6;

                        }
                    }
                }
            }

            return settings;
        }

        public ItemSettingsViewModel GetItemSettings()
        {
            // default model values
            var settings = new ItemSettingsViewModel();

            using (var connection = new NpgsqlConnection(MUNEEMJI.DbConfig.ConnectionString))
            {
                connection.Open();

                string sql = @"SELECT * FROM item_settings LIMIT 1;";

                using (var cmd = new NpgsqlCommand(sql, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        settings.EnableItem = !reader.IsDBNull(reader.GetOrdinal("enable_item")) && reader.GetBoolean(reader.GetOrdinal("enable_item"));
                        settings.WhatDoYouSell = !reader.IsDBNull(reader.GetOrdinal("what_do_you_sell")) ? reader.GetString(reader.GetOrdinal("what_do_you_sell")) : settings.WhatDoYouSell;
                        settings.BarcodeScanning = !reader.IsDBNull(reader.GetOrdinal("barcode_scan")) && reader.GetBoolean(reader.GetOrdinal("barcode_scan"));
                        settings.DirectBarcodeScanning = !reader.IsDBNull(reader.GetOrdinal("direct_barcode_scan")) && reader.GetBoolean(reader.GetOrdinal("direct_barcode_scan"));
                        settings.StockMaintenance = !reader.IsDBNull(reader.GetOrdinal("stock_maintenance")) && reader.GetBoolean(reader.GetOrdinal("stock_maintenance"));
                        settings.Manufacturing = !reader.IsDBNull(reader.GetOrdinal("manufacturing")) && reader.GetBoolean(reader.GetOrdinal("manufacturing"));
                        settings.ShowLowStockDialog = !reader.IsDBNull(reader.GetOrdinal("show_low_stock_dialog")) && reader.GetBoolean(reader.GetOrdinal("show_low_stock_dialog"));
                        settings.ItemsUnit = !reader.IsDBNull(reader.GetOrdinal("items_unit")) && reader.GetBoolean(reader.GetOrdinal("items_unit"));
                        settings.DefaultUnit = !reader.IsDBNull(reader.GetOrdinal("default_unit")) ? reader.GetString(reader.GetOrdinal("default_unit")) : settings.DefaultUnit;
                        settings.ItemCategory = !reader.IsDBNull(reader.GetOrdinal("item_category")) && reader.GetBoolean(reader.GetOrdinal("item_category"));
                        settings.PartyWiseItemRate = !reader.IsDBNull(reader.GetOrdinal("party_wise_item_rate")) && reader.GetBoolean(reader.GetOrdinal("party_wise_item_rate"));
                        settings.Description = !reader.IsDBNull(reader.GetOrdinal("description")) && reader.GetBoolean(reader.GetOrdinal("description"));
                        settings.ItemWiseTax = !reader.IsDBNull(reader.GetOrdinal("item_wise_tax")) && reader.GetBoolean(reader.GetOrdinal("item_wise_tax"));
                        settings.ItemWiseDiscount = !reader.IsDBNull(reader.GetOrdinal("item_wise_discount")) && reader.GetBoolean(reader.GetOrdinal("item_wise_discount"));
                        settings.UpdateSalePriceFromTransaction = !reader.IsDBNull(reader.GetOrdinal("update_sale_price_from_transaction")) && reader.GetBoolean(reader.GetOrdinal("update_sale_price_from_transaction"));
                        settings.MrpEnabled = !reader.IsDBNull(reader.GetOrdinal("mrp_enabled")) && reader.GetBoolean(reader.GetOrdinal("mrp_enabled"));
                        settings.CalculateSalePriceFromMrp = !reader.IsDBNull(reader.GetOrdinal("calculate_sale_price_from_mrp")) && reader.GetBoolean(reader.GetOrdinal("calculate_sale_price_from_mrp"));
                        settings.UseMrpForBatchTracking = !reader.IsDBNull(reader.GetOrdinal("use_mrp_for_batch_tracking")) && reader.GetBoolean(reader.GetOrdinal("use_mrp_for_batch_tracking"));
                        settings.SerialNoTracking = !reader.IsDBNull(reader.GetOrdinal("serial_no_tracking")) && reader.GetBoolean(reader.GetOrdinal("serial_no_tracking"));
                        settings.BatchNoEnabled = !reader.IsDBNull(reader.GetOrdinal("batch_no_enabled")) && reader.GetBoolean(reader.GetOrdinal("batch_no_enabled"));
                        settings.ExpDateEnabled = !reader.IsDBNull(reader.GetOrdinal("exp_date_enabled")) && reader.GetBoolean(reader.GetOrdinal("exp_date_enabled"));
                        settings.MfgDateEnabled = !reader.IsDBNull(reader.GetOrdinal("mfg_date_enabled")) && reader.GetBoolean(reader.GetOrdinal("mfg_date_enabled"));
                        settings.ModelNoEnabled = !reader.IsDBNull(reader.GetOrdinal("model_no_enabled")) && reader.GetBoolean(reader.GetOrdinal("model_no_enabled"));
                        settings.SizeEnabled = !reader.IsDBNull(reader.GetOrdinal("size_enabled")) && reader.GetBoolean(reader.GetOrdinal("size_enabled"));
                        settings.ItemCode = !reader.IsDBNull(reader.GetOrdinal("item_code")) && reader.GetBoolean(reader.GetOrdinal("item_code"));
                        settings.HsnSacCode = !reader.IsDBNull(reader.GetOrdinal("hsn_sac_code")) && reader.GetBoolean(reader.GetOrdinal("hsn_sac_code"));

                    }
                }
            }

            return settings;
        }


    }
}
