using System.Text.Json.Serialization;

public class AnalysisModel
{
    [JsonPropertyName("time_period")]
    public TimePeriodModel TimePeriod { get; set; }

    [JsonPropertyName("spending_summary")]
    public SpendingSummaryModel SpendingSummary { get; set; }

    [JsonPropertyName("comparison_with_previous_period")]
    public ComparisonModel ComparisonWithPreviousPeriod { get; set; }

    [JsonPropertyName("top_categories")]
    public TopCategoriesModel TopCategories { get; set; }

    [JsonPropertyName("monthly_trend")]
    public MonthlyTrendModel MonthlyTrend { get; set; }

    [JsonPropertyName("category_details")]
    public CategoryDetailsModel CategoryDetails { get; set; }
}

public class TimePeriodModel
{
    [JsonPropertyName("start_date")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("end_date")]
    public DateTime EndDate { get; set; }

    [JsonPropertyName("duration_in_days")]
    public int DurationInDays { get; set; }
}

public class SpendingSummaryModel
{
    [JsonPropertyName("total_expense")]
    public decimal TotalExpense { get; set; }

    [JsonPropertyName("total_income")]
    public decimal TotalIncome { get; set; }

    [JsonPropertyName("net_change")]
    public decimal NetChange { get; set; }
}

public class ComparisonModel
{
    [JsonPropertyName("previous_start_date")]
    public DateTime PreviousStartDate { get; set; }

    [JsonPropertyName("previous_end_date")]
    public DateTime PreviousEndDate { get; set; }

    [JsonPropertyName("expense_change")]
    public ChangeModel ExpenseChange { get; set; }

    [JsonPropertyName("income_change")]
    public ChangeModel IncomeChange { get; set; }

    [JsonPropertyName("net_change")]
    public ChangeModel NetChange { get; set; }
}

public class ChangeModel
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("percentage")]
    public decimal Percentage { get; set; }
}

public class TopCategoriesModel
{
    [JsonPropertyName("most_expensive_category")]
    public CategoryModel MostExpensiveCategory { get; set; }

    [JsonPropertyName("most_income_category")]
    public CategoryModel MostIncomeCategory { get; set; }
}

public class CategoryModel
{
    [JsonPropertyName("category")]
    public string Name { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("percentage")]
    public decimal Percentage { get; set; }
}

public class MonthlyTrendModel
{
    [JsonPropertyName("expense_trend")]
    public List<MonthlyExpenseModel> ExpenseTrend { get; set; } = new();

    [JsonPropertyName("income_trend")]
    public List<MonthlyIncomeModel> IncomeTrend { get; set; } = new();
}

public class MonthlyExpenseModel
{
    [JsonPropertyName("month")]
    public string Month { get; set; }

    [JsonPropertyName("total_expense")]
    public decimal TotalExpense { get; set; }
}

public class MonthlyIncomeModel
{
    [JsonPropertyName("month")]
    public string Month { get; set; }

    [JsonPropertyName("total_income")]
    public decimal TotalIncome { get; set; }
}

public class CategoryDetailsModel
{
    [JsonPropertyName("expenses")]
    public List<CategoryDetailModel> Expenses { get; set; } = new();

    [JsonPropertyName("incomes")]
    public List<CategoryDetailModel> Incomes { get; set; } = new();
}

public class CategoryDetailModel
{
    [JsonPropertyName("category")]
    public string Category { get; set; }

    [JsonPropertyName("total")]
    public decimal Total { get; set; }
}