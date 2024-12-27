using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Fin_Manager_v2.ViewModels;

public partial class AnalysisViewModel : ObservableRecipient
{
    private readonly IAnalysisService _analysisService;
    private readonly IAuthService _authService;
    private readonly IAccountService _accountService;

    [ObservableProperty]
    private DateTimeOffset _startDate = DateTimeOffset.Now.AddMonths(-1);

    [ObservableProperty]
    private DateTimeOffset _endDate = DateTimeOffset.Now;

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
    private string previousPeriod = "";

    [ObservableProperty]
    private string expenseChangeAmount = "0";

    [ObservableProperty]
    private string expenseChangePercentage = "0%";

    [ObservableProperty]
    private string incomeChangeAmount = "0";

    [ObservableProperty]
    private string incomeChangePercentage = "0%";

    [ObservableProperty]
    private string netChangeAmount = "0";

    [ObservableProperty]
    private string netChangePercentage = "0%";

    [ObservableProperty]
    private string durationInDays = "0";

    [ObservableProperty]
    private string mostExpensiveCategory = "N/A";

    [ObservableProperty]
    private string mostExpensiveAmount = "0";

    [ObservableProperty]
    private string mostIncomeCategory = "N/A";

    [ObservableProperty]
    private string mostIncomeAmount = "0";

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
            Debug.WriteLine($"Error initializing user and accounts: {ex.Message}");
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

    partial void OnStartDateChanged(DateTimeOffset value)
    {
        // Remove auto update
    }

    partial void OnEndDateChanged(DateTimeOffset value)
    {
        // Remove auto update
    }

    private async Task UpdateAnalysisData()
    {
        try
        {
            var analysis = await _analysisService.GetAnalysisAsync(
                UserId,
                AccountId,
                StartDate.Date,
                EndDate.Date);

            // Update Summary
            TotalIncome = analysis.SpendingSummary.TotalIncome.ToString("N");
            TotalExpense = analysis.SpendingSummary.TotalExpense.ToString("N");
            NetChange = analysis.SpendingSummary.NetChange.ToString("N");

            // Update Comparison
            PreviousPeriod = $"{analysis.ComparisonWithPreviousPeriod.PreviousStartDate:d} - {analysis.ComparisonWithPreviousPeriod.PreviousEndDate:d}";
            
            ExpenseChangeAmount = analysis.ComparisonWithPreviousPeriod.ExpenseChange.Amount.ToString("N");
            ExpenseChangePercentage = $"{analysis.ComparisonWithPreviousPeriod.ExpenseChange.Percentage:N1}%";
            
            IncomeChangeAmount = analysis.ComparisonWithPreviousPeriod.IncomeChange.Amount.ToString("N");
            IncomeChangePercentage = $"{analysis.ComparisonWithPreviousPeriod.IncomeChange.Percentage:N1}%";
            
            NetChangeAmount = analysis.ComparisonWithPreviousPeriod.NetChange.Amount.ToString("N");
            NetChangePercentage = $"{analysis.ComparisonWithPreviousPeriod.NetChange.Percentage:N1}%";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating analysis data: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task AnalyzeAsync()
    {
        try
        {
            var analysis = await _analysisService.GetAnalysisAsync(
                UserId,
                AccountId,
                StartDate.Date,
                EndDate.Date);

            Debug.WriteLine($"Analysis data: {analysis}");

            // Time Period
            DurationInDays = analysis.TimePeriod.DurationInDays.ToString();

            // Spending Summary
            TotalIncome = analysis.SpendingSummary.TotalIncome.ToString("N");
            TotalExpense = analysis.SpendingSummary.TotalExpense.ToString("N");
            NetChange = analysis.SpendingSummary.NetChange.ToString("N");

            // Comparison
            PreviousPeriod = $"{analysis.ComparisonWithPreviousPeriod.PreviousStartDate:d} - {analysis.ComparisonWithPreviousPeriod.PreviousEndDate:d}";
            
            ExpenseChangeAmount = analysis.ComparisonWithPreviousPeriod.ExpenseChange.Amount.ToString("N");
            ExpenseChangePercentage = $"{analysis.ComparisonWithPreviousPeriod.ExpenseChange.Percentage:N1}%";
            
            IncomeChangeAmount = analysis.ComparisonWithPreviousPeriod.IncomeChange.Amount.ToString("N");
            IncomeChangePercentage = $"{analysis.ComparisonWithPreviousPeriod.IncomeChange.Percentage:N1}%";
            
            NetChangeAmount = analysis.ComparisonWithPreviousPeriod.NetChange.Amount.ToString("N");
            NetChangePercentage = $"{analysis.ComparisonWithPreviousPeriod.NetChange.Percentage:N1}%";

            // Top Categories
            if (analysis.TopCategories.MostExpensiveCategory != null)
            {
                MostExpensiveCategory = analysis.TopCategories.MostExpensiveCategory.Name;
                MostExpensiveAmount = analysis.TopCategories.MostExpensiveCategory.Amount.ToString("N");
            }

            if (analysis.TopCategories.MostIncomeCategory != null)
            {
                MostIncomeCategory = analysis.TopCategories.MostIncomeCategory.Name;
                MostIncomeAmount = analysis.TopCategories.MostIncomeCategory.Amount.ToString("N");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error analyzing data: {ex.Message}");
        }
    }
}
