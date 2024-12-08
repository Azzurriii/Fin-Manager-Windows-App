namespace Fin_Manager_v2.Models;

public class SummaryModel
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance { get; set; }
}

public class OverviewModel
{
    public string? Month { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
}

public class CategoryReportModel
{
    public int TagId { get; set; }
    public string? TagName { get; set; }
    public decimal Amount { get; set; }
}