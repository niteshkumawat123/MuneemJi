namespace MUNEEMJI.Models.Setting
{
    public class PartySettingsViewModel
    {
        // Party Settings
        public bool PartyGrouping { get; set; }
        public bool ShippingAddress { get; set; }
        public bool PrintShippingAddress { get; set; }
        public bool ManagePartyStatus { get; set; }
        public bool EnablePaymentReminder { get; set; }
        public int PaymentReminderDays { get; set; } = 1;

        // Additional Fields
        public bool AdditionalField1Enabled { get; set; }
        public string AdditionalField1 { get; set; } = "";
        public bool AdditionalField1ShowInPrint { get; set; }

        public bool AdditionalField2Enabled { get; set; }
        public string AdditionalField2 { get; set; } = "";
        public bool AdditionalField2ShowInPrint { get; set; }

        public bool AdditionalField3Enabled { get; set; }
        public string AdditionalField3 { get; set; } = "";
        public bool AdditionalField3ShowInPrint { get; set; }

        public bool AdditionalField4Enabled { get; set; }
        public string AdditionalField4 { get; set; } = "";
        public string AdditionalField4Type { get; set; } = "dd/mm/yy";
        public bool AdditionalField4ShowInPrint { get; set; }

        // Loyalty Point
        public bool EnableLoyaltyPoint { get; set; }

        // System
        public int FirmId { get; set; }
    }
}
