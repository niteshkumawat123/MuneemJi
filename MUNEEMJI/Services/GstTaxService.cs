using Npgsql;

namespace MUNEEMJI.Services
{
    public class GstRateOption
    {
        public decimal TaxPercentage { get; set; }
        public decimal CgstRate { get; set; }
        public decimal SgstRate { get; set; }
        public decimal IgstRate { get; set; }
        public string DisplayText { get; set; } = string.Empty;
        public string TaxType { get; set; } = string.Empty;
        public bool IsSameState { get; set; }
    }

    public interface IGstTaxService
    {
        Task<List<GstRateOption>> GetGstRateOptionsAsync(int companyId, string? selectedStateOfSupply);
        Task<bool> IsSameStateAsync(int companyId, string? selectedStateOfSupply);
    }

    public class GstTaxService : IGstTaxService
    {
        private readonly string _connectionString = DbConfig.ConnectionString;

        public async Task<bool> IsSameStateAsync(int companyId, string? selectedStateOfSupply)
        {
            if (string.IsNullOrWhiteSpace(selectedStateOfSupply))
                return true;

            try
            {
                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                // Get business profile state name
                string bpQuery = @"SELECT s.name FROM business_profiles bp
                                   JOIN states s ON s.id = bp.state_id
                                   WHERE bp.businessesid = @cid LIMIT 1";
                using var bpCmd = new NpgsqlCommand(bpQuery, conn);
                bpCmd.Parameters.AddWithValue("@cid", companyId);
                var bpStateName = (await bpCmd.ExecuteScalarAsync())?.ToString() ?? "";

                // Compare with selected state of supply (plain state name string)
                return string.Equals(bpStateName.Trim(), selectedStateOfSupply.Trim(), StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return true;
            }
        }

        public async Task<List<GstRateOption>> GetGstRateOptionsAsync(int companyId, string? selectedStateOfSupply)
        {
            var options = new List<GstRateOption>
            {
                new GstRateOption
                {
                    TaxPercentage = 0, DisplayText = "NONE", TaxType = "NONE"
                }
            };

            try
            {
                bool isSameState = await IsSameStateAsync(companyId, selectedStateOfSupply);
                int firmId = 1; // TaxRates are stored per firmId

                using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();

                if (isSameState)
                {
                    // Fetch CGST rates — each represents half the total GST
                    string query = @"SELECT Rate, Name FROM TaxRates
                                     WHERE IsDeleted = false AND FirmId = @fid AND UPPER(TaxType) = 'CGST'
                                     ORDER BY Rate ASC";
                    using var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@fid", firmId);
                    using var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        decimal halfRate = reader.GetDecimal(reader.GetOrdinal("Rate"));
                        if (halfRate == 0) continue;
                        decimal totalRate = halfRate * 2;
                        options.Add(new GstRateOption
                        {
                            TaxPercentage = totalRate,
                            CgstRate = halfRate,
                            SgstRate = halfRate,
                            IgstRate = 0,
                            DisplayText = $"{totalRate}% (CGST {halfRate}% + SGST {halfRate}%)",
                            TaxType = "CGST_SGST",
                            IsSameState = true
                        });
                    }
                }
                else
                {
                    // Fetch IGST rates
                    string query = @"SELECT Rate, Name FROM TaxRates
                                     WHERE IsDeleted = false AND FirmId = @fid AND UPPER(TaxType) = 'IGST'
                                     ORDER BY Rate ASC";
                    using var cmd = new NpgsqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@fid", firmId);
                    using var reader = await cmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        decimal rate = reader.GetDecimal(reader.GetOrdinal("Rate"));
                        if (rate == 0) continue;
                        options.Add(new GstRateOption
                        {
                            TaxPercentage = rate,
                            CgstRate = 0,
                            SgstRate = 0,
                            IgstRate = rate,
                            DisplayText = $"{rate}% (IGST {rate}%)",
                            TaxType = "IGST",
                            IsSameState = false
                        });
                    }
                }

                // If no rates found from DB, use fallback
                if (options.Count <= 1)
                {
                    var fallbackRates = new[] { 5m, 12m, 18m, 28m };
                    foreach (var rate in fallbackRates)
                    {
                        if (isSameState)
                        {
                            options.Add(new GstRateOption
                            {
                                TaxPercentage = rate, CgstRate = rate / 2, SgstRate = rate / 2,
                                DisplayText = $"{rate}% (CGST {rate / 2}% + SGST {rate / 2}%)",
                                TaxType = "CGST_SGST", IsSameState = true
                            });
                        }
                        else
                        {
                            options.Add(new GstRateOption
                            {
                                TaxPercentage = rate, IgstRate = rate,
                                DisplayText = $"{rate}% (IGST {rate}%)",
                                TaxType = "IGST", IsSameState = false
                            });
                        }
                    }
                }
            }
            catch
            {
                // Fallback static
                foreach (var rate in new[] { 5m, 12m, 18m, 28m })
                {
                    options.Add(new GstRateOption
                    {
                        TaxPercentage = rate, DisplayText = $"{rate}%", TaxType = "NONE"
                    });
                }
            }

            return options;
        }
    }
}
