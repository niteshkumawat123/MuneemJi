using System.ComponentModel.DataAnnotations;

namespace MUNEEMJI.Models
{
    public class Godown
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Godown Name is required")]
        [Display(Name = "Godown Name")]
        public string GodownName { get; set; }

        [Required(ErrorMessage = "Godown Type is required")]
        [Display(Name = "Godown Type")]
        public string GodownType { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [Display(Name = "Phone No")]
        public string PhoneNo { get; set; }

        [Display(Name = "Email ID")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string EmailId { get; set; }

        [Display(Name = "GSTIN")]
        public string GSTIN { get; set; }

        [Display(Name = "Godown Address")]
        public string GodownAddress { get; set; }

        [Display(Name = "Godown Pincode")]
        public string GodownPincode { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class GodownViewModel
    {
        public List<Godown> Godowns { get; set; }
        public string MainGodown { get; set; } = "MAIN GODOWN";
        public int TotalGodowns { get; set; }
    }
}


