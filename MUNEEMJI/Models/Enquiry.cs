using System.ComponentModel.DataAnnotations;

namespace MUNEEMJI.Models
{
    // Entity matching DB table
    public class Enquiry
    {
        public int EnquiryId { get; set; }
        public int CompanyId { get; set; }
        public string EnquirySource { get; set; } = "manual";
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string? CustomerEmail { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string Status { get; set; } = "new";
        public string? Reason { get; set; }
        public int? AssignedTo { get; set; }
        public string? AssignedToName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public string SectionType { get; set; } = "enquiry";
    }

    // ViewModel for Create/Edit forms
    public class EnquiryViewModel
    {
        public int EnquiryId { get; set; }

        [Required(ErrorMessage = "Customer name is required")]
        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(20)]
        public string CustomerPhone { get; set; } = string.Empty;

        [MaxLength(200)]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? CustomerEmail { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        [MaxLength(300)]
        public string Subject { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Message { get; set; }

        public string EnquirySource { get; set; } = "manual";
        public int? AssignedTo { get; set; }
        public string SectionType { get; set; } = "enquiry";
    }

    // DTO for status update AJAX call
    public class EnquiryStatusUpdateDto
    {
        [Required]
        public int EnquiryId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;

        public string? Reason { get; set; }
    }
}
