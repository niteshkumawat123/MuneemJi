using static MUNEEMJI.Models.ItemModel;

namespace MUNEEMJI.Models
{
    public class UnitsViewModel
    {
        public List<Unit> Units { get; set; }
        public int SelectedUnitId { get; set; }
        public List<UnitConversion> Conversions { get; set; }
    }
}
