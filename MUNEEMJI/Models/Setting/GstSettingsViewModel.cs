namespace MUNEEMJI.Models.Setting
{
    public class GstSettingsViewModel
    {
        public bool EnableGst { get; set; }
        public bool EnableHsnSacCode { get; set; }
        public bool AdditionalCessOnItem { get; set; }
        public bool ReverseCharge { get; set; }
        public bool EnablePlaceOfSupply { get; set; }
        public bool CompositeScheme { get; set; }
        public string CompositeSchemeType { get; set; } = "Manufacturer 1.0%";
        public bool EnableTcs { get; set; }
        public bool EnableTds { get; set; }
        public int FirmId { get; set; }
    }
}
