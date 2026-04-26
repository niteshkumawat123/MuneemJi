using System;

namespace MUNEEMJI.Models.Setting
{
    public class GstSettingsModel
    {
        public int Id { get; set; }
        public int FirmId { get; set; }
        public bool EnableGST { get; set; } = true;
        public bool EnableHSNSACCode { get; set; } = true;
        public bool AdditionalCessOnItem { get; set; } = true;
        public bool ReverseCharge { get; set; } = true;
        public bool EnablePlaceOfSupply { get; set; } = true;
        public bool CompositeScheme { get; set; } = true;
        public string CompositeSchemeType { get; set; } = "Restaurant 5.0%";
        public bool EnableTCS { get; set; } = true;
        public bool EnableTDS { get; set; } = true;
    }

    public class TaxRateModel
    {
        public int Id { get; set; }
        public int FirmId { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
        public string TaxType { get; set; } = "Other";
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }

    public class TaxGroupModel
    {
        public int Id { get; set; }
        public int FirmId { get; set; }
        public string GroupName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string MemberTaxes { get; set; } // Only for display read
        public int[] TaxRateIds { get; set; } // Used for insert
    }

    public class TaxGroupItemModel
    {
        public int Id { get; set; }
        public int TaxGroupId { get; set; }
        public int TaxRateId { get; set; }
    }
}
