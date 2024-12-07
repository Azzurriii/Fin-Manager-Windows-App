using CommunityToolkit.Mvvm.ComponentModel;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Diagnostics;
using System.Collections.ObjectModel;
namespace Fin_Manager_v2.ViewModels;

public partial class ReportViewModel : ObservableRecipient
{
    private readonly IReportService _reportService;
    private readonly IAuthService _authService;
    private readonly IAccountService _accountService;

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
    private int userId;

    [ObservableProperty]
    private int? accountId;

    [ObservableProperty]
    private ObservableCollection<AccountModel> accounts = new();

    [ObservableProperty]
    private AccountModel selectedAccountObj;

    [ObservableProperty]
    private SolidColorPaint _legendTextPaint;

    [ObservableProperty]
    private bool _isMonthPeriod;

    [ObservableProperty]
    private bool _isQuarterPeriod;

    [ObservableProperty]
    private bool _isYearPeriod;

    public ReportViewModel(
        IReportService reportService,
        IAuthService authService,
        IAccountService accountService)
    {
        _reportService = reportService;
        _authService = authService;
        _accountService = accountService;
        
        InitializeUserAndAccountsAsync();
        LegendTextPaint = new SolidColorPaint(new SKColor(0, 120, 215));
    }

    private async void InitializeUserAndAccountsAsync()
    {
        try
        {
            // Get current user ID from stored value
            var storedUserId = _authService.GetUserId();
            if (!storedUserId.HasValue)
            {
                await _authService.FetchUserIdAsync();
                storedUserId = _authService.GetUserId();
            }

            if (storedUserId.HasValue)
            {
                UserId = storedUserId.Value;
                await LoadAccountsAsync();
                UpdateChartData();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error initializing user and accounts: {ex.Message}");
        }
    }

    private async Task LoadAccountsAsync()
    {
        try
        {
            var accountsList = await _accountService.GetAccountsAsync();
            
            Accounts.Clear();

            // Add "All Accounts" option
            var allAccounts = new AccountModel { AccountId = 0, AccountName = "All Accounts" };
            Accounts.Add(allAccounts);

            if (accountsList != null)
            {
                foreach (var account in accountsList)
                {
                    Accounts.Add(account);
                }
            }

            SelectedAccountObj = Accounts.FirstOrDefault();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading accounts: {ex.Message}");
        }
    }

    partial void OnSelectedAccountObjChanged(AccountModel value)
    {
        AccountId = value?.AccountName == "All Accounts" ? null : value?.AccountId;
        UpdateChartData();
    }

    partial void OnSelectedTimePeriodChanged(string value)
    {
        IsMonthPeriod = value == "Month";
        IsQuarterPeriod = value == "Quarter";
        IsYearPeriod = value == "Year";
        UpdateChartData();
    }

    partial void OnSelectedMonthChanged(string value)
    {
        if (IsMonthPeriod)
        {
            UpdateChartData();
        }
    }

    partial void OnSelectedQuarterChanged(string value)
    {
        if (IsQuarterPeriod)
        {
            UpdateChartData();
        }
    }

    partial void OnSelectedYearChanged(int value)
    {
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

            // API calls separately
            var summaryTask = FetchSummaryAsync(startDate, endDate);
            var overviewTask = FetchOverviewAsync(startDate, endDate);
            var incomeTask = FetchCategoryReportAsync("INCOME", startDate, endDate);
            var expenseTask = FetchCategoryReportAsync("EXPENSE", startDate, endDate);

            // Wait for all tasks to complete
            await Task.WhenAll(summaryTask, overviewTask, incomeTask, expenseTask);

            // Update UI data
            TotalIncome = (await summaryTask).TotalIncome.ToString("N");
            TotalExpense = (await summaryTask).TotalExpense.ToString("N");
            Balance = (await summaryTask).Balance.ToString("N");

            var overviewData = await overviewTask;
            UpdateOverviewChart(overviewData);

            var incomeCategories = await incomeTask;
            IncomeSeries = ConvertToPieSeries(incomeCategories, SKColors.Green);

            var expenseCategories = await expenseTask;
            ExpenseSeries = ConvertToPieSeries(expenseCategories, SKColors.Red);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating chart data: {ex.Message}");
        }
    }

    private async Task<SummaryModel> FetchSummaryAsync(DateTime startDate, DateTime endDate)
    {
        Debug.WriteLine($"Fetching summary data for account {AccountId}...");
        var summary = await _reportService.GetSummaryAsync(UserId, AccountId, startDate, endDate);
        Debug.WriteLine("Summary data received");
        return summary;
    }

    private async Task<List<OverviewModel>> FetchOverviewAsync(DateTime startDate, DateTime endDate)
    {
        Debug.WriteLine($"Fetching overview data for account {AccountId}...");
        var overviewData = await _reportService.GetOverviewAsync(UserId, AccountId, startDate, endDate);
        Debug.WriteLine($"Received {overviewData.Count} overview records");
        return overviewData;
    }

    private async Task<List<CategoryReportModel>> FetchCategoryReportAsync(string type, DateTime startDate, DateTime endDate)
    {
        Debug.WriteLine($"Fetching {type.ToLower()} categories for account {AccountId}...");
        var categories = await _reportService.GetCategoryReportAsync(UserId, AccountId, type, startDate, endDate);
        Debug.WriteLine($"Received {categories.Count} {type.ToLower()} categories");
        return categories;
    }

    private void UpdateOverviewChart(List<OverviewModel> overviewData)
    {
        var months = overviewData.Select(x => x.Month ?? "").ToArray();
        var incomeSeries = overviewData.Select(x => x.TotalIncome).ToArray();
        var expenseSeries = overviewData.Select(x => x.TotalExpense).ToArray();

        XAxes = new[]
        {
            new Axis
            {
                Labels = months,
                LabelsPaint = new SolidColorPaint(SKColors.Gray),
                TextSize = 14,
                LabelsRotation = 0,
                SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
                SeparatorsAtCenter = false,
                TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
                TicksAtCenter = true,
                ForceStepToMin = true,
                MinStep = 1
            }
        };

        OverviewSeries = new ISeries[]
        {
            new ColumnSeries<decimal>
            {
                Values = incomeSeries,
                Name = "Income",
                Fill = new SolidColorPaint(SKColors.MediumSeaGreen),
                DataLabelsFormatter = (chartPoint) => $"Income: {chartPoint.Model:C2}",
            },
            new ColumnSeries<decimal>
            {
                Values = expenseSeries,
                Name = "Expense",
                Fill = new SolidColorPaint(SKColors.Crimson),
                DataLabelsFormatter = (chartPoint) => $"Expense: {chartPoint.Model:C2}",
            }
        }.ToList();
    }

    private List<PieSeries<double>> ConvertToPieSeries(List<CategoryReportModel> categories, SKColor baseColor)
    {
        // Tag color 
        var colors = new SKColor[]
        {
            // Basic colors
            SKColors.MediumSeaGreen,
            SKColors.Crimson,
            SKColors.DodgerBlue,
            SKColors.Orange,
            SKColors.Purple,
            SKColors.Gold,
            
            // Subtle colors
            SKColors.DeepPink,
            SKColors.Teal,
            SKColors.Brown,
            SKColors.SlateBlue,
            SKColors.OrangeRed,
            SKColors.ForestGreen,
            
            // Supplemental colors
            SKColors.MediumVioletRed,
            SKColors.Chocolate,
            SKColors.DarkCyan,
            SKColors.Tomato,
            SKColors.DarkOliveGreen,
            
            // Special colors
            SKColors.Indigo,
            SKColors.Sienna,
            SKColors.DarkOrchid,
            SKColors.SteelBlue,
            SKColors.Maroon,
            SKColors.DarkGoldenrod,
            
            // Neutral colors
            SKColors.DimGray,
            SKColors.RosyBrown,
            SKColors.CadetBlue,
            SKColors.Peru,
            SKColors.MediumPurple,
            SKColors.DarkKhaki
        };

        return categories.Select((x, index) => new PieSeries<double>
        {
            Values = new[] { (double)x.Amount },
            Name = x.TagName,
            Fill = new SolidColorPaint(
                colors[index % colors.Length]
                .WithAlpha((byte)230)
            )
        }).ToList();
    }
}