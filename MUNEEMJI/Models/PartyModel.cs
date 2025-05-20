namespace MUNEEMJI.Models
{
    public class PartyModel
    {
        public string PartyName { get; set; }
        public string GSTIN { get; set; }
        public string PhoneNumber { get; set; }

        // GST & Address Tab
        public string GSTType { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public bool IsShippingDisabled { get; set; }

        
    }
}
