using MUNEEMJI.Models.Setting;
using System.Collections.Generic;

namespace MUNEEMJI.Services
{
    public interface IGstSettingsService
    {
        GstSettingsModel GetGstSettings(int firmId);
        bool SaveGstSettings(GstSettingsModel model);

        List<TaxRateModel> GetTaxRates(int firmId);
        bool SaveTaxRate(TaxRateModel model);
        bool DeleteTaxRate(int id);

        List<TaxGroupModel> GetTaxGroups(int firmId);
        int SaveTaxGroup(TaxGroupModel model);
        bool DeleteTaxGroup(int id);
    }
}
