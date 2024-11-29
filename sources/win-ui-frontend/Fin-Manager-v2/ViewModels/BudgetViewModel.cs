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

    public ObservableCollection<BudgetModel> Budgets { get; set; } = new ObservableCollection<BudgetModel>();
    public ObservableCollection<AccountModel> Accounts { get; set; } = new ObservableCollection<AccountModel>();

    public CreateBudgetDto NewBudget { get; set; } = new();

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

    public BudgetViewModel(IBudgetService budgetService, IAccountService accountService)
    {
        _budgetService = budgetService;
        _accountService = accountService;

        LoadBudgets();
        LoadAccounts();
    }

    private async void LoadAccounts()
    {
        var accounts = await _accountService.GetAccountsAsync();
        foreach (var account in accounts)
        {
            Accounts.Add(account);
        }
    }

    public void ShowAddBudget()
    {
        NewBudget = new CreateBudgetDto();
        IsAddingBudget = true;
    }

    public void CancelAddBudget()
    {
        IsAddingBudget = false;
        NewBudget = new CreateBudgetDto();
    }

    public async Task SaveBudget()
    {
        if (string.IsNullOrWhiteSpace(NewBudget.Category) || NewBudget.BudgetAmount <= 0)
        {
            return;
        }

        var createdBudget = await _budgetService.CreateBudgetAsync(NewBudget);
        Console.WriteLine($"Category: {NewBudget.Category}, BudgetAmount: {NewBudget.BudgetAmount}, StartDate: {NewBudget.StartDate}, EndDate: {NewBudget.EndDate}");
        if (createdBudget != null)
        {
            Budgets.Add(createdBudget);
            NewBudget = new CreateBudgetDto();
            CancelAddBudget();
        }
        else
        {
            // Xử lý trường hợp thêm không thành công
            //await _dialogService.ShowErrorDialog("Failed to add budget. Please try again.");
        }
    }

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
