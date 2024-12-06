using CommunityToolkit.Mvvm.ComponentModel;
using Fin_Manager_v2.Contracts.Services;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Diagnostics;

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
    private IEnumerable<ICartesianAxis> xAxes = new[] { new Axis { Labels = Array.Empty<string>() } };

    [ObservableProperty]
    private int userId = 1;

    [ObservableProperty]
    private int accountId = 1;

    public bool IsDayPeriod => SelectedTimePeriod == "Day";
    public bool IsMonthPeriod => SelectedTimePeriod == "Month";
    public bool IsQuarterPeriod => SelectedTimePeriod == "Quarter";
    public bool IsYearPeriod => SelectedTimePeriod == "Year";

    public ReportViewModel(IReportService reportService)
    {
        _reportService = reportService;
        UpdateChartData();
    }

    partial void OnSelectedTimePeriodChanged(string value)
    {
        OnPropertyChanged(nameof(IsDayPeriod));
        OnPropertyChanged(nameof(IsMonthPeriod));
        OnPropertyChanged(nameof(IsQuarterPeriod));
        OnPropertyChanged(nameof(IsYearPeriod));
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

            // 1. Update Summary - Wait for completion
            Debug.WriteLine("Fetching summary data...");
            var summary = await _reportService.GetSummaryAsync(UserId, AccountId, startDate, endDate);
            Debug.WriteLine("Summary data received");
            TotalIncome = summary.TotalIncome.ToString("N");
            TotalExpense = summary.TotalExpense.ToString("N");
            Balance = summary.Balance.ToString("N");

            await Task.Delay(100); // Wait before next API call

            // 2. Update Overview Chart - Wait for completion
            Debug.WriteLine("Fetching overview data...");
            var overviewData = await _reportService.GetOverviewAsync(UserId, AccountId, startDate, endDate);
            Debug.WriteLine($"Received {overviewData.Count} overview records");

            var months = overviewData.Select(x => x.Month ?? "").ToArray();
            var incomeSeries = overviewData.Select(x => x.TotalIncome).ToArray();
            var expenseSeries = overviewData.Select(x => x.TotalExpense).ToArray();

            XAxes = new[] 
            { 
                new Axis 
                { 
                    Labels = months 
                } 
            };

            OverviewSeries = new ISeries[]
            {
                new LineSeries<decimal>
                {
                    Values = incomeSeries,
                    Name = "Income",
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Green, 2),
                    GeometryStroke = new SolidColorPaint(SKColors.Green, 2),
                    GeometrySize = 8
                },
                new LineSeries<decimal>
                {
                    Values = expenseSeries,
                    Name = "Expense",
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Red, 2),
                    GeometryStroke = new SolidColorPaint(SKColors.Red, 2),
                    GeometrySize = 8
                }
            }.ToList(); // Force evaluation

            await Task.Delay(100); // Wait before next API call

            // 3. Update Income Pie Chart - Wait for completion
            Debug.WriteLine("Fetching income categories...");
            var incomeCategories = await _reportService.GetCategoryReportAsync(UserId, AccountId, "INCOME", startDate, endDate);
            Debug.WriteLine($"Received {incomeCategories.Count} income categories");
            
            var incomePieSeries = incomeCategories
                .Select(x => new PieSeries<double>
                {
                    Values = new[] { (double)x.Amount },
                    Name = x.TagName,
                    Fill = new SolidColorPaint(SKColors.Green.WithAlpha((byte)(155 + Random.Shared.Next(100))))
                })
                .ToList(); // Force evaluation

            IncomeSeries = incomePieSeries;

            await Task.Delay(100); // Wait before next API call

            // 4. Update Expense Pie Chart - Wait for completion
            Debug.WriteLine("Fetching expense categories...");
            var expenseCategories = await _reportService.GetCategoryReportAsync(UserId, AccountId, "EXPENSE", startDate, endDate);
            Debug.WriteLine($"Received {expenseCategories.Count} expense categories");
            
            var expensePieSeries = expenseCategories
                .Select(x => new PieSeries<double>
                {
                    Values = new[] { (double)x.Amount },
                    Name = x.TagName,
                    Fill = new SolidColorPaint(SKColors.Red.WithAlpha((byte)(155 + Random.Shared.Next(100))))
                })
                .ToList(); // Force evaluation

            ExpenseSeries = expensePieSeries;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating chart data: {ex.Message}");
        }
    }
}