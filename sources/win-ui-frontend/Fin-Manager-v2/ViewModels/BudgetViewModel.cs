using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Services;
using Fin_Manager_v2.Contracts.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Fin_Manager_v2.ViewModels;

public partial class BudgetViewModel : ObservableRecipient
{
    private readonly IBudgetService _budgetService;
    private readonly IAccountService _accountService;
    private readonly IDialogService _dialogService;
    private readonly ITagService _tagService;
    private readonly ITransactionService _transactionService;
    private readonly IAuthService _authService;

    public ObservableCollection<BudgetModel> Budgets { get; set; } = new ObservableCollection<BudgetModel>();
    public ObservableCollection<AccountModel> Accounts { get; set; } = new ObservableCollection<AccountModel>();
    public ObservableCollection<TagModel> AvailableTags { get; set; } = new();

    private CreateBudgetDto _newBudget;
    public CreateBudgetDto NewBudget
    {
        get => _newBudget;
        set => SetProperty(ref _newBudget, value);
    }

    private bool _isAddingBudget;
    public bool IsAddingBudget
    {
        get => _isAddingBudget;
        set => SetProperty(ref _isAddingBudget, value);
    }

    private BudgetModel _selectedBudget;
    public BudgetModel SelectedBudget
    {
        get => _selectedBudget;
        set => SetProperty(ref _selectedBudget, value);
    }

    private TagModel _selectedTag;
    public TagModel SelectedTag
    {
        get => _selectedTag;
        set => SetProperty(ref _selectedTag, value);
    }

    public BudgetViewModel(
        IBudgetService budgetService, 
        IAccountService accountService, 
        IDialogService dialogService,
        ITagService tagService,
        ITransactionService transactionService,
        IAuthService authService)
    {
        _budgetService = budgetService;
        _accountService = accountService;
        _dialogService = dialogService;
        _tagService = tagService;
        _transactionService = transactionService;
        _authService = authService;

        LoadBudgets();
        LoadAccounts();
        LoadTags();
    }

    private async void LoadAccounts()
    {
        var accounts = await _accountService.GetAccountsAsync();
        foreach (var account in accounts)
        {
            Accounts.Add(account);
        }
    }

    private async void LoadTags()
    {
        try 
        {
            var tags = await _tagService.GetTagsByTypeAsync("EXPENSE");
            AvailableTags.Clear();
            foreach (var tag in tags)
            {
                AvailableTags.Add(tag);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading tags: {ex.Message}");
        }
    }

    [RelayCommand]
    public void ShowAddBudget()
    {
        NewBudget = new CreateBudgetDto
        {
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(1)
        };
        SelectedTag = null;
        IsAddingBudget = true;
    }

    [RelayCommand]
    public void CancelAddBudget()
    {
        IsAddingBudget = false;
        NewBudget = new CreateBudgetDto();
    }

    [RelayCommand]
    public async Task SaveBudget()
    {
        if (NewBudget == null || SelectedTag == null)
        {
            await _dialogService.ShowErrorAsync("Error", "Please select a category");
            return;
        }

        if (NewBudget.BudgetAmount <= 0)
        {
            await _dialogService.ShowErrorAsync("Error", "Budget amount must be greater than 0");
            return;
        }

        NewBudget.Category = SelectedTag.TagName;

        int userId = _authService.GetUserId() ?? 0;
        DateTimeOffset? startDate = NewBudget.StartDate;
        DateTimeOffset? endDate = NewBudget.EndDate;
        var totalAmount = await _transactionService.GetTotalAmountAsync(userId, NewBudget.AccountId, "EXPENSE", startDate?.DateTime ?? DateTime.MinValue,
    endDate?.DateTime ?? DateTime.MaxValue);

        NewBudget.SpentAmount = totalAmount;

        var createdBudget = await _budgetService.CreateBudgetAsync(NewBudget);
        if (createdBudget != null)
        {
            Budgets.Add(createdBudget);
            NewBudget = new CreateBudgetDto();
            SelectedTag = null;
            CancelAddBudget();
        }
        else
        {
            await _dialogService.ShowErrorAsync("Error", "Cannot Add Budget");
        }
    }

    [RelayCommand]
    public async Task DeleteBudget(BudgetModel budget)
    {
        try
        {
            bool isDeleted = await _budgetService.DeleteBudgetAsync(budget.BudgetId);

            if (isDeleted)
            {
                Budgets.Remove(budget);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting budget: {ex.Message}");
        }
    }

    /// <summary>
    /// Load the budgets.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async void LoadBudgets()
    {
        var budgets = await _budgetService.GetBudgetsAsync();
        Budgets.Clear();
        foreach (var budget in budgets)
        {
            Budgets.Add(budget);
        }
    }

}
