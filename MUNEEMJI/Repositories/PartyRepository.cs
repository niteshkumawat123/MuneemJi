using Insight.Database;
using MUNEEMJI.Models;
using MUNEEMJI.Services;
using Npgsql;

namespace MUNEEMJI.Repositories
{
    public interface IParty 
    {
        Task<List<PartyDropDownModel>> GetPartyDropDownAsync(int CompanyId);
    }
    public class PartyRepository: IParty
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
    }
}
