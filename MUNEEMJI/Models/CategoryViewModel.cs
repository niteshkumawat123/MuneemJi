using static MUNEEMJI.Models.ItemModel;

namespace MUNEEMJI.Models
{
    public class CategoryViewModel
    {
        public List<Category> Categories { get; set; }
        public int SelectedCategoryId { get; set; }
        public List<Product> Items { get; set; }
    }
}
