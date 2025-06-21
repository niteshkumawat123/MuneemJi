using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MUNEEMJI.Models
{
    [Table("templates")]
    public class Template
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        [Column("image_url")]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("category")]
        public string Category { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("type")]
        public string Type { get; set; } = string.Empty;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }

        [MaxLength(1000)]
        [Column("description")]
        public string? Description { get; set; }

        [Column("view_count")]
        public int ViewCount { get; set; } = 0;

        [Column("download_count")]
        public int DownloadCount { get; set; } = 0;
    }

    [Table("template_customizations")]
    public class TemplateCustomization
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("template_id")]
        public int TemplateId { get; set; }

        [ForeignKey("TemplateId")]
        public Template Template { get; set; } = null!;

        [MaxLength(200)]
        [Column("business_name")]
        public string BusinessName { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("contact_person")]
        public string ContactPerson { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("contact_number")]
        public string ContactNumber { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("additional_text")]
        public string AdditionalText { get; set; } = string.Empty;

        [MaxLength(1000)]
        [Column("whatsapp_text")]
        public string WhatsappText { get; set; } = string.Empty;

        [MaxLength(500)]
        [Column("business_logo_url")]
        public string? BusinessLogoUrl { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [MaxLength(500)]
        [Column("generated_image_url")]
        public string? GeneratedImageUrl { get; set; }

        [Column("is_downloaded")]
        public bool IsDownloaded { get; set; } = false;

        [Column("download_date")]
        public DateTime? DownloadDate { get; set; }
    }

    [Table("categories")]
    public class Category
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("type")]
        public string Type { get; set; } = string.Empty; // Greetings, Trending, Business, Offers

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("sort_order")]
        public int SortOrder { get; set; } = 0;

        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

