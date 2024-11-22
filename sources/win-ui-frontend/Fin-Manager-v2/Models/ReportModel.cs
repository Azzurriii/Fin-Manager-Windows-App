namespace Fin_Manager_v2.Models;

public class ReportModel
{
    public class SummaryModel
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
    }

    public class OverviewModel
    {
        public string Month { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
    }

    public class TagAmountModel
    {
        public string Tag { get; set; }
        public decimal Amount { get; set; }
    }

    public class CategoryReportModel
    {
        public decimal Total { get; set; }
        public List<TagAmountModel> TagAmounts { get; set; }
    }
}