using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Kernel.Sketches;
using SkiaSharp;
using Fin_Manager_v2.Contracts.Services;

namespace Fin_Manager_v2.ViewModels;

public partial class ReportViewModel : ObservableRecipient
{
    private readonly IReportService _reportService;
    
    [ObservableProperty]
    private string _selectedTimePeriod = "Year";

    [ObservableProperty]
    private string _selectedMonth = DateTime.Now.ToString("MMMM");

    [ObservableProperty]
    private string _selectedQuarter = $"Q{(DateTime.Now.Month + 2) / 3}";

    [ObservableProperty]
    private int _selectedYear = DateTime.Now.Year;

    [ObservableProperty]
    private DateTimeOffset _selectedDate = DateTimeOffset.Now;

    // Visibility control properties
    public bool IsDayPeriod => SelectedTimePeriod == "Day";
    public bool IsMonthPeriod => SelectedTimePeriod == "Month";
    public bool IsQuarterPeriod => SelectedTimePeriod == "Quarter";
    public bool IsYearPeriod => SelectedTimePeriod == "Year";

    partial void OnSelectedTimePeriodChanged(string value)
    {
        OnPropertyChanged(nameof(IsDayPeriod));
        OnPropertyChanged(nameof(IsMonthPeriod));
        OnPropertyChanged(nameof(IsQuarterPeriod));
        OnPropertyChanged(nameof(IsYearPeriod));
        UpdateChartData();
    }

    [ObservableProperty]
    private string totalIncome = "0";

    [ObservableProperty]
    private string totalExpense = "0";

    [ObservableProperty]
    private string balance = "0";

    [ObservableProperty]
    private IEnumerable<ISeries> overviewSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private IEnumerable<ISeries> incomeSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private IEnumerable<ISeries> expenseSeries = Array.Empty<ISeries>();

    [ObservableProperty]
    private IEnumerable<ICartesianAxis> xAxes = Array.Empty<ICartesianAxis>();

    [ObservableProperty]
    private int userId = 1;

    [ObservableProperty]
    private int accountId = 1;

    public ReportViewModel(IReportService reportService)
    {
        _reportService = reportService;
        UpdateChartData();
    }

    private async void UpdateChartData()
    {
        try
        {
            var (startDate, endDate) = _reportService.GetDateRangeFromPeriod(
                SelectedTimePeriod, 
                SelectedDate, 
                SelectedMonth, 
                SelectedQuarter, 
                SelectedYear);

            // Update Summary
            var summary = await _reportService.GetSummaryAsync(UserId, AccountId, startDate, endDate);
            TotalIncome = summary.TotalIncome.ToString("C");
            TotalExpense = summary.TotalExpense.ToString("C");
            Balance = summary.Balance.ToString("C");

            // Update Overview Chart
            var overview = await _reportService.GetOverviewAsync(UserId, AccountId, startDate, endDate);
            var months = overview.Select(x => x.Month).ToArray();
            var incomeValues = overview.Select(x => (double)x.TotalIncome).ToArray();
            var expenseValues = overview.Select(x => (double)x.TotalExpense).ToArray();

            OverviewSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "Income",
                    Values = incomeValues,
                    Fill = new SolidColorPaint(SKColors.Green),
                },
                new ColumnSeries<double>
                {
                    Name = "Expense",
                    Values = expenseValues,
                    Fill = new SolidColorPaint(SKColors.Red),
                }
            };

            XAxes = new ICartesianAxis[]
            {
                new Axis
                {
                    Labels = months,
                    LabelsRotation = 0,
                    ForceStepToMin = true,
                    MinStep = 1,
                    TextSize = 12,
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 }
                }
            };

            // Update Income Pie Chart
            var incomeCategories = await _reportService.GetCategoryReportAsync(UserId, AccountId, "income", startDate, endDate);
            IncomeSeries = incomeCategories.Select(x => new PieSeries<double>
            {
                Values = new[] { (double)x.Amount },
                Name = x.Tag,
                Fill = new SolidColorPaint(SKColors.Green.WithAlpha((byte)(155 + Random.Shared.Next(100))))
            });

            // Update Expense Pie Chart
            var expenseCategories = await _reportService.GetCategoryReportAsync(UserId, AccountId, "expense", startDate, endDate);
            ExpenseSeries = expenseCategories.Select(x => new PieSeries<double>
            {
                Values = new[] { (double)x.Amount },
                Name = x.Tag,
                Fill = new SolidColorPaint(SKColors.Red.WithAlpha((byte)(155 + Random.Shared.Next(100))))
            });
        }
        catch (Exception ex)
        {
            // Handle error appropriately
            Debug.WriteLine($"Error updating chart data: {ex.Message}");
        }
    }
}