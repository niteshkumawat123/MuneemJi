using System.ComponentModel.DataAnnotations;

namespace MUNEEMJI.Models
{
    public class PaymentInOutModel
    {
      
        public int Id { get; set; }

        
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

      
        [Display(Name = "Reference No")]
        public string RefNo { get; set; }

      
        [Display(Name = "Party")]
        public int PartyId { get; set; }

        [Display(Name = "Party Name")]
        public string PartyName { get; set; }

      
        [Display(Name = "Category")]
        public string CategoryName { get; set; }

      
        [Display(Name = "Type")]
        public string Type { get; set; }

      
        [Display(Name = "Payment Type")]
        public string PaymentType { get; set; }

      
        [Display(Name = "Total")]
        public decimal Total { get; set; }

      
        [Display(Name = "Received/Paid")]
        public decimal ReceivedPaid { get; set; }

        [Display(Name = "Balance")]
        public decimal Balance { get; set; }

        [Display(Name = "Receipt No")]
        public string ReceiptNo { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Print/Share")]
        public string PrintShare { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}

    
