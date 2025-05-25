using System.ComponentModel.DataAnnotations;

namespace MUNEEMJI.Models
{
    public class BusinessProfileModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Business Name is required")]
        public string BusinessName { get; set; }

        public string PhoneNumber { get; set; }
        public string Gstin { get; set; }
        public string Email { get; set; }

        // Foreign key IDs for dropdowns
        public int BusinessTypeId { get; set; }
        public int BusinessCategoryId { get; set; }
        public int StateId { get; set; }

        public string Pincode { get; set; }
        public string Address { get; set; }

        // Paths to uploaded images (stored in wwwroot/uploads)
        public string LogoPath { get; set; }
        public string SignaturePath { get; set; }
    }
}
