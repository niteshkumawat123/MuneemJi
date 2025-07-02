using System.IO;

namespace MUNEEMJI.Models
{
    public class PartyModel
    {
        // Existing properties

        public string? BalanceDirection { get; set; } // "ToPay" or "ToReceive"
        public bool EnableOpeningBalance { get; set; }

        public int Id { get; set; }
        public decimal? Balance { get; set; }
        public string PartyName { get; set; }
        public string GSTIN { get; set; }
        public string PhoneNumber { get; set; }
        public string PartyGroup { get; set; }
        public string GSTType { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public bool IsShippingDisabled { get; set; }
        // New Credit & Balance properties
        public decimal? OpeningBalance { get; set; }
        public DateTime? AsOfDate { get; set; }
        public bool HasCustomCreditLimit { get; set; }
        public decimal? CreditLimit { get; set; }
        // New Additional Fields
        public bool AdditionalField1Enabled { get; set; }
        public string AdditionalField1Value { get; set; }
        public bool AdditionalField2Enabled { get; set; }
        public string AdditionalField2Value { get; set; }
        public bool AdditionalField3Enabled { get; set; }
        public string AdditionalField3Value { get; set; }
        public bool AdditionalField4Enabled { get; set; }
        public DateTime? AdditionalField4Value { get; set; }
    }

    public class PartyViewModel
    {
        public List<PartyModel> Parties { get; set; }
        public PartyModel SelectedParty { get; set; }
    }
    public class PartyDropDownModel
    {
        public int id { get; set; }
        public string partyname { get; set; }
        public decimal balance { get; set; }
        public string phonenumber { get; set; }


    }

}
