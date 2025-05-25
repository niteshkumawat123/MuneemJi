namespace MUNEEMJI.Models
{
    public class UnitConversion
    {
        public int Id { get; set; }
        public int FromUnitId { get; set; }
        public int ToUnitId { get; set; }
        public decimal Factor { get; set; }
    }
}
