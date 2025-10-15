using Dapper;
using Insight.Database;
using MUNEEMJI.Models;
using MUNEEMJI.Services;
using Npgsql;
using System.Linq;

namespace MUNEEMJI.Repositories
{
    public interface IParty
    {
        Task<List<PartyDropDownModel>> GetPartyDropDownAsync(int CompanyId);
        Task<List<PartyModel>> GetAllPartiesAsync(int companyId);
    }
    public class PartyRepository : IParty
    {
        private readonly string _connectionString = "Host=154.61.75.70;Port=5433;Database=MuneemJi;Username=betauser;Password=betauser";

        public async Task<List<PartyDropDownModel>> GetPartyDropDownAsync(int CompanyId)
        {
            List<PartyDropDownModel> model = new List<PartyDropDownModel>();

            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = "SELECT id, party_name AS PartyName, balance,phone_number as phonenumber FROM parties where companyid = @p_companyid ORDER BY party_name";

                    var result = await conn.QuerySqlAsync<PartyDropDownModel>(sql, new { p_companyid = CompanyId });
                    model = result?.ToList() ?? new List<PartyDropDownModel>();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw;
            }

            return model;
        }


        public async Task<List<PartyModel>> GetAllPartiesAsync(int companyId)
        {
            List<PartyModel> model = new List<PartyModel>();
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    string query = @"
                SELECT 
                    id AS Id,
                    party_name AS PartyName,
                    gstin AS GSTIN,
                    phone_number AS PhoneNumber,
                    gst_type AS GSTType,
                    state AS State,
                    email AS Email,
                    billing_address AS BillingAddress,
                    shipping_address AS ShippingAddress,
                    is_shipping_disabled AS IsShippingDisabled,
                    balance AS Balance,
                    opening_balance AS OpeningBalance,
                    as_of_date AS AsOfDate,
                    has_custom_credit_limit AS HasCustomCreditLimit,
                    credit_limit AS CreditLimit,
                    additional_field1_enabled AS AdditionalField1Enabled,
                    additional_field1_value AS AdditionalField1Value,
                    additional_field2_enabled AS AdditionalField2Enabled,
                    additional_field2_value AS AdditionalField2Value,
                    additional_field3_enabled AS AdditionalField3Enabled,
                    additional_field3_value AS AdditionalField3Value,
                    additional_field4_enabled AS AdditionalField4Enabled,
                    additional_field4_value AS AdditionalField4Value,
                    companyid AS CompanyId
                FROM public.parties
                WHERE companyid = @companyid
                ORDER BY id DESC;";

                    model = connection.QuerySql<PartyModel>(query, new { companyid = companyId }).ToList();

                }
            }
            catch(Exception ex)
            {

            }

            return model;
        }
    }
}
