namespace MUNEEMJI.Models.ReportModel
{
    public class OtherIncomeReportModel
    {
        public List<OtherIncomeCategory> OtherIncomeCategoryDropDown { get; set; }
        public List<OtherIncomeViewModel> OtherIncomeEntries { get; set; }
        public List<IncomeEntryItem> IncomeEntryItems { get; set; }
    }
}
