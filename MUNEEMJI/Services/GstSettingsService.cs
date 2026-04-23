using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Npgsql;
using Insight.Database;
using MUNEEMJI.Models.Setting;
using Microsoft.Extensions.Configuration;

namespace MUNEEMJI.Services
{
    public class GstSettingsService : IGstSettingsService
    {
        private string connectionString;

        public GstSettingsService(IConfiguration configuration)
        {
            connectionString = MUNEEMJI.DbConfig.ConnectionString;
        }

        private NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }

        public GstSettingsModel GetGstSettings(int firmId)
        {
            using (var c = GetConnection())
            {
                return c.QuerySql<GstSettingsModel>(
                    "SELECT * FROM GstSettings WHERE FirmId = @firmId LIMIT 1", 
                    new { firmId }).FirstOrDefault();
            }
        }

        public bool SaveGstSettings(GstSettingsModel model)
        {
            using (var c = GetConnection())
            {
                var existing = GetGstSettings(model.FirmId);
                if (existing != null)
                {
                    c.ExecuteSql(@"UPDATE GstSettings SET 
                        EnableGST=@EnableGST, EnableHSNSACCode=@EnableHSNSACCode, AdditionalCessOnItem=@AdditionalCessOnItem, 
                        ReverseCharge=@ReverseCharge, EnablePlaceOfSupply=@EnablePlaceOfSupply, CompositeScheme=@CompositeScheme, 
                        CompositeSchemeType=@CompositeSchemeType, EnableTCS=@EnableTCS, EnableTDS=@EnableTDS 
                        WHERE FirmId=@FirmId", model);
                }
                else
                {
                    c.ExecuteSql(@"INSERT INTO GstSettings (FirmId, EnableGST, EnableHSNSACCode, AdditionalCessOnItem, 
                        ReverseCharge, EnablePlaceOfSupply, CompositeScheme, CompositeSchemeType, EnableTCS, EnableTDS)
                        VALUES (@FirmId, @EnableGST, @EnableHSNSACCode, @AdditionalCessOnItem, @ReverseCharge, 
                        @EnablePlaceOfSupply, @CompositeScheme, @CompositeSchemeType, @EnableTCS, @EnableTDS)", model);
                }
                return true;
            }
        }

        public List<TaxRateModel> GetTaxRates(int firmId)
        {
            using (var c = GetConnection())
            {
                return c.QuerySql<TaxRateModel>(
                    "SELECT * FROM TaxRates WHERE IsDeleted = false AND FirmId = @firmId ORDER BY Rate ASC", new { firmId }).ToList();
            }
        }

        public bool SaveTaxRate(TaxRateModel model)
        {
            using (var c = GetConnection())
            {
                if (model.Id > 0)
                {
                    c.ExecuteSql("UPDATE TaxRates SET Name=@Name, Rate=@Rate, TaxType=@TaxType WHERE Id=@Id", model);
                }
                else
                {
                    c.ExecuteSql("INSERT INTO TaxRates (FirmId, Name, Rate, TaxType, IsDeleted, CreatedOn) VALUES (@FirmId, @Name, @Rate, @TaxType, false, current_timestamp)", model);
                }
                return true;
            }
        }

        public bool DeleteTaxRate(int id)
        {
            using (var c = GetConnection())
            {
                c.ExecuteSql("UPDATE TaxRates SET IsDeleted = true WHERE Id = @id", new { id });
                return true;
            }
        }

        public List<TaxGroupModel> GetTaxGroups(int firmId)
        {
            using (var c = GetConnection())
            {
                var groups = c.QuerySql<TaxGroupModel>("SELECT * FROM TaxGroups WHERE IsDeleted = false AND FirmId = @firmId", new { firmId }).ToList();
                foreach(var g in groups)
                {
                    var items = c.QuerySql<TaxGroupItemModel>("SELECT * FROM TaxGroupItems WHERE TaxGroupId = @id", new { id = g.Id }).ToList();
                    g.TaxRateIds = items.Select(i => i.TaxRateId).ToArray();

                    var taxes = c.QuerySql<string>(@"SELECT tr.Name FROM TaxRates tr 
                        INNER JOIN TaxGroupItems tgi ON tr.Id = tgi.TaxRateId 
                        WHERE tgi.TaxGroupId = @id", new { id = g.Id }).ToList();
                    g.MemberTaxes = string.Join("  ", taxes);
                }
                return groups;
            }
        }

        public int SaveTaxGroup(TaxGroupModel model)
        {
            using (var c = GetConnection())
            {
                int newId = model.Id;
                if (model.Id > 0)
                {
                    c.ExecuteSql("UPDATE TaxGroups SET GroupName=@GroupName WHERE Id=@Id", model);
                    c.ExecuteSql("DELETE FROM TaxGroupItems WHERE TaxGroupId=@Id", new { model.Id });
                }
                else
                {
                    newId = c.QuerySql<int>("INSERT INTO TaxGroups (FirmId, GroupName, IsDeleted, CreatedOn) VALUES (@FirmId, @GroupName, false, current_timestamp) RETURNING Id", model).FirstOrDefault();
                }

                if (model.TaxRateIds != null && newId > 0)
                {
                    foreach(var rateId in model.TaxRateIds)
                    {
                        c.ExecuteSql("INSERT INTO TaxGroupItems (TaxGroupId, TaxRateId) VALUES (@newId, @rateId)", new { newId, rateId });
                    }
                }
                return newId;
            }
        }

        public bool DeleteTaxGroup(int id)
        {
            using (var c = GetConnection())
            {
                c.ExecuteSql("UPDATE TaxGroups SET IsDeleted = true WHERE Id = @id", new { id });
                c.ExecuteSql("DELETE FROM TaxGroupItems WHERE TaxGroupId = @id", new { id });
                return true;
            }
        }
    }
}
