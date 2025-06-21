using System.ComponentModel.DataAnnotations;

namespace MUNEEMJI.Models
{
    public class TemplateViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public class TemplateEditorViewModel
    {
        public int TemplateId { get; set; }

        [Display(Name = "Business Name")]
        public string BusinessName { get; set; } = string.Empty;

        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; } = string.Empty;

        [Display(Name = "Contact Number")]
        [Phone]
        public string ContactNumber { get; set; } = string.Empty;

        [Display(Name = "Additional Text")]
        public string AdditionalText { get; set; } = string.Empty;

        [Display(Name = "WhatsApp Text")]
        public string WhatsappText { get; set; } = string.Empty;

        public string BusinessLogo { get; set; } = string.Empty;
        public string TemplateImageUrl { get; set; } = string.Empty;
        public string Category { get; set; } = "Motivation";
    }

    public class GenerateImageRequest
    {
        public int TemplateId { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string ContactPerson { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string AdditionalText { get; set; } = string.Empty;
        public string WhatsappText { get; set; } = string.Empty;
    }

    public class TemplateListViewModel
    {
        public List<TemplateViewModel> Templates { get; set; } = new List<TemplateViewModel>();
        public string SelectedCategory { get; set; } = "Greetings";
        public string SearchTerm { get; set; } = string.Empty;

        public List<string> Categories { get; set; } = new List<string>
        {
            "All",
            "Latest Update",
            "Gupt Navratri",
            "Kamakhya Ambubachi Mela",
            "Good Morning",
            "Good Thoughts (Suvichar)",
            "Good Night",
            "Motivation"
        };

        public List<string> TabCategories { get; set; } = new List<string>
        {
            "Greetings",
            "Trending",
            "Business",
            "Offers"
        };
    }
}
