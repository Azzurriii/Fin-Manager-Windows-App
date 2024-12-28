using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System.Text.Json;

namespace Fin_Manager_v2.ViewModels;

public partial class AnalysisViewModel : ObservableRecipient
{
    private readonly IAnalysisService _analysisService;
    private readonly IAuthService _authService;
    private readonly IAccountService _accountService;
    private AnalysisModel? _lastAnalysis;

    [ObservableProperty]
    private DateTimeOffset? startDate = DateTimeOffset.Now.AddMonths(-1);

    [ObservableProperty]
    private DateTimeOffset? endDate = DateTimeOffset.Now;

    [ObservableProperty]
    private int userId;

    [ObservableProperty]
    private int? accountId;

    [ObservableProperty]
    private ObservableCollection<AccountModel> accounts = new();

    [ObservableProperty]
    private AccountModel selectedAccountObj;

    [ObservableProperty]
    private string totalIncome = "0";

    [ObservableProperty]
    private string totalExpense = "0";

    [ObservableProperty]
    private string netChange = "0";

    [ObservableProperty]
    private string previousPeriodIncome = "0";

    [ObservableProperty]
    private string previousPeriodExpense = "0";

    [ObservableProperty]
    private string previousPeriodNetChange = "0";

    [ObservableProperty]
    private string previousPeriodDates = "";

    [ObservableProperty]
    private string durationInDays = "0";

    [ObservableProperty]
    private string incomeChangePercentage = "0%";

    [ObservableProperty]
    private string expenseChangePercentage = "0%";

    [ObservableProperty]
    private string netChangePercentage = "0%";

    [ObservableProperty]
    private string mostExpensiveCategory = "N/A";

    [ObservableProperty]
    private string mostExpensiveAmount = "0";

    [ObservableProperty]
    private string mostExpensivePercentage = "0%";

    [ObservableProperty]
    private string mostIncomeCategory = "N/A";

    [ObservableProperty]
    private string mostIncomeAmount = "0";

    [ObservableProperty]
    private string mostIncomePercentage = "0%";


    // Change Amounts
    [ObservableProperty]
    private string incomeChangeAmount = "0";

    [ObservableProperty]
    private string expenseChangeAmount = "0";

    [ObservableProperty]
    private string netChangeAmount = "0";

    // Change Direction Properties
    public bool IsIncomeIncreased => _lastAnalysis?.ComparisonWithPreviousPeriod?.IncomeChange.Amount > 0;
    public bool IsIncomeDecreased => _lastAnalysis?.ComparisonWithPreviousPeriod?.IncomeChange.Amount < 0;
    public bool IsExpenseIncreased => _lastAnalysis?.ComparisonWithPreviousPeriod?.ExpenseChange.Amount > 0;
    public bool IsExpenseDecreased => _lastAnalysis?.ComparisonWithPreviousPeriod?.ExpenseChange.Amount < 0;

    // Colors for changes
    public SolidColorBrush IncomeChangeColor => 
        IsIncomeIncreased ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
    public SolidColorBrush ExpenseChangeColor => 
        IsExpenseIncreased ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);


    public AnalysisViewModel(
        IAnalysisService analysisService,
        IAuthService authService,
        IAccountService accountService)
    {
        _analysisService = analysisService;
        _authService = authService;
        _accountService = accountService;

        InitializeUserAndAccountsAsync();
    }

    private async void InitializeUserAndAccountsAsync()
    {
        try
        {
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
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error initializing: {ex.Message}");
        }
    }

    private async Task LoadAccountsAsync()
    {
        try
        {
            var accountsList = await _accountService.GetAccountsAsync();
            
            Accounts.Clear();
            Accounts.Add(new AccountModel { AccountId = 0, AccountName = "All Accounts" });

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
    }

    [RelayCommand]
    private async Task AnalyzeAsync()
    {
        try
        {
            if (UserId == 0 || StartDate == null || EndDate == null)
            {
                Debug.WriteLine("Missing required data for analysis");
                return;
            }

            var analysis = await _analysisService.GetAnalysisAsync(
                UserId,
                SelectedAccountObj?.AccountId,
                StartDate?.DateTime.Date ?? DateTime.Now.AddMonths(-1).Date,
                EndDate?.DateTime.Date ?? DateTime.Now.Date);

            Debug.WriteLine($"API Response: {JsonSerializer.Serialize(analysis)}");

            // Current Period Values
            TotalIncome = analysis.SpendingSummary.TotalIncome.ToString("N2");
            TotalExpense = analysis.SpendingSummary.TotalExpense.ToString("N2");
            NetChange = analysis.SpendingSummary.NetChange.ToString("N2");

            // Previous Period Values - Tính từ current và change amount
            decimal prevIncome = analysis.SpendingSummary.TotalIncome - analysis.ComparisonWithPreviousPeriod.IncomeChange.Amount;
            decimal prevExpense = analysis.SpendingSummary.TotalExpense - analysis.ComparisonWithPreviousPeriod.ExpenseChange.Amount;
            decimal prevNetChange = analysis.SpendingSummary.NetChange - analysis.ComparisonWithPreviousPeriod.NetChange.Amount;

            PreviousPeriodIncome = prevIncome.ToString("N2");
            PreviousPeriodExpense = prevExpense.ToString("N2");
            PreviousPeriodNetChange = prevNetChange.ToString("N2");

            // Change Percentages - Nhân với 100 vì API trả về dạng decimal
            IncomeChangePercentage = $"{(analysis.ComparisonWithPreviousPeriod.IncomeChange.Percentage * 100):N1}%";
            ExpenseChangePercentage = $"{(analysis.ComparisonWithPreviousPeriod.ExpenseChange.Percentage * 100):N1}%";
            NetChangePercentage = $"{(analysis.ComparisonWithPreviousPeriod.NetChange.Percentage * 100):N1}%";

            // Change Amounts
            IncomeChangeAmount = analysis.ComparisonWithPreviousPeriod.IncomeChange.Amount.ToString("N2");
            ExpenseChangeAmount = analysis.ComparisonWithPreviousPeriod.ExpenseChange.Amount.ToString("N2");
            NetChangeAmount = analysis.ComparisonWithPreviousPeriod.NetChange.Amount.ToString("N2");

            // Top Categories
            if (analysis.TopCategories?.MostExpensiveCategory != null)
            {
                MostExpensiveCategory = analysis.TopCategories.MostExpensiveCategory.Name;
                MostExpensiveAmount = analysis.TopCategories.MostExpensiveCategory.Amount.ToString("N2");
                MostExpensivePercentage = $"{(analysis.TopCategories.MostExpensiveCategory.Percentage * 100):N1}%";
            }

            if (analysis.TopCategories?.MostIncomeCategory != null)
            {
                MostIncomeCategory = analysis.TopCategories.MostIncomeCategory.Name;
                MostIncomeAmount = analysis.TopCategories.MostIncomeCategory.Amount.ToString("N2");
                MostIncomePercentage = $"{(analysis.TopCategories.MostIncomeCategory.Percentage * 100):N1}%";
            }

            // Period Info
            DurationInDays = analysis.TimePeriod.DurationInDays.ToString();
            PreviousPeriodDates = $"{analysis.ComparisonWithPreviousPeriod.PreviousStartDate:d} - {analysis.ComparisonWithPreviousPeriod.PreviousEndDate:d}";

            // Notify property changes
            OnPropertyChanged(nameof(IsIncomeIncreased));
            OnPropertyChanged(nameof(IsIncomeDecreased));
            OnPropertyChanged(nameof(IsExpenseIncreased));
            OnPropertyChanged(nameof(IsExpenseDecreased));
            OnPropertyChanged(nameof(IncomeChangeColor));
            OnPropertyChanged(nameof(ExpenseChangeColor));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error analyzing data: {ex.Message}");
        }
    }
}
