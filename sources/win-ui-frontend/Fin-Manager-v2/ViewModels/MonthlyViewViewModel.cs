using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fin_Manager_v2.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Fin_Manager_v2.Services;
using Fin_Manager_v2.Contracts.Services;
using Microsoft.UI.Xaml.Controls;

namespace Fin_Manager_v2.ViewModels;

public partial class MonthlyViewViewModel : ObservableRecipient
{
    private readonly ITransactionService _transactionService;
    private readonly ITagService _tagService;
    private readonly IAccountService _accountService;
    private readonly IAuthService _authService;
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly IDialogService _dialogService = new DialogService();

    [ObservableProperty]
    private ObservableCollection<TransactionModel> _transactions = new();

    [ObservableProperty]
    private ObservableCollection<AccountModel> _accounts = new();

    [ObservableProperty]
    private ObservableCollection<TagModel> _availableTags = new();

    [ObservableProperty]
    private AccountModel? _selectedAccount;

    [ObservableProperty]
    private List<TagModel> _selectedTags = new();

    [ObservableProperty]
    private DateTimeOffset _startDate = DateTimeOffset.Now.AddMonths(-1);

    [ObservableProperty]
    private DateTimeOffset _endDate = DateTimeOffset.Now;

    [ObservableProperty]
    private decimal _totalIncome;

    [ObservableProperty]
    private decimal _totalExpense;

    [ObservableProperty]
    private decimal _balance;

    public MonthlyViewViewModel(
        ITransactionService transactionService,
        ITagService tagService,
        IAccountService accountService,
        IAuthService authService)
    {
        _transactionService = transactionService;
        _tagService = tagService;
        _accountService = accountService;
        _authService = authService;
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        _ = InitializeAsync();
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        try
        {
            await Task.WhenAll(
                LoadAccountsAsync(),
                LoadTagsAsync()
            );
            await LoadTransactionsAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing: {ex.Message}");
            await _dialogService.ShowErrorAsync(
                "Initialization Error",
                "An error occurred while initializing the view.");
        }
    }

    [RelayCommand]
    public async Task LoadTransactionsAsync()
    {
        try
        {
            var query = new QueryDto
            {
                UserId = _authService.GetUserId() ?? 0,
                AccountId = SelectedAccount?.AccountId,
                StartDate = StartDate.DateTime,
                EndDate = EndDate.DateTime,
                TagIds = SelectedTags?.Select(t => t.Id).ToList()
            };

            var transactions = await _transactionService.GetTransactionsByQueryAsync(query);
            await LoadTagNamesForTransactions(transactions);

            _dispatcherQueue.TryEnqueue(() =>
            {
                Transactions.Clear();
                foreach (var transaction in transactions)
                {
                    Transactions.Add(transaction);
                }

                TotalIncome = transactions.Where(t => t.TransactionType == "INCOME")
                                        .Sum(t => t.Amount);
                TotalExpense = transactions.Where(t => t.TransactionType == "EXPENSE")
                                         .Sum(t => t.Amount);
                CalculateBalance();
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading transactions: {ex.Message}");
            await _dialogService.ShowErrorAsync(
                "Load Error",
                "Failed to load transactions.");
        }
    }

    private async Task LoadAccountsAsync()
    {
        try
        {
            var accounts = await _accountService.GetAccountsAsync();
            _dispatcherQueue.TryEnqueue(() =>
            {
                Accounts.Clear();
                if (accounts != null)
                {
                    foreach (var account in accounts)
                    {
                        Accounts.Add(account);
                    }
                }
                SelectedAccount = Accounts.FirstOrDefault();
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading accounts: {ex.Message}");
        }
    }

    private async Task LoadTagsAsync()
    {
        try
        {
            var tags = await _tagService.GetTagsAsync();
            _dispatcherQueue.TryEnqueue(() =>
            {
                AvailableTags.Clear();
                if (tags != null)
                {
                    foreach (var tag in tags)
                    {
                        AvailableTags.Add(tag);
                    }
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading tags: {ex.Message}");
        }
    }

    private async Task LoadTagNamesForTransactions(IList<TransactionModel> transactions)
    {
        var uniqueTagIds = transactions.Where(t => t.TagId > 0)
            .Select(t => t.TagId)
            .Distinct();

        var tagTasks = uniqueTagIds.Select(async tagId =>
        {
            var tag = await _tagService.GetTagAsync((int)tagId);
            return (tagId, tag?.TagName);
        });

        var tagResults = await Task.WhenAll(tagTasks);
        var tagDictionary = tagResults.ToDictionary(x => x.tagId, x => x.TagName);

        foreach (var transaction in transactions)
        {
            if (transaction.TagId > 0 && tagDictionary.TryGetValue(transaction.TagId, out var tagName))
            {
                transaction.TagName = tagName;
            }
        }
    }

    partial void OnStartDateChanged(DateTimeOffset value)
    {
        LoadTransactionsAsync().ConfigureAwait(false);
    }

    partial void OnEndDateChanged(DateTimeOffset value)
    {
        LoadTransactionsAsync().ConfigureAwait(false);
    }

    partial void OnSelectedAccountChanged(AccountModel? value)
    {
        LoadTransactionsAsync().ConfigureAwait(false);
    }

    partial void OnSelectedTagsChanged(List<TagModel> value)
    {
        LoadTransactionsAsync().ConfigureAwait(false);
    }

    private void CalculateBalance()
    {
        if (SelectedAccount?.AccountId != null && SelectedAccount.AccountName != "All Accounts")
        {
            Balance = SelectedAccount.InitialBalance + (TotalIncome - TotalExpense);
        }
        else
        {
            Balance = Accounts
                .Where(a => a.AccountName != "All Accounts")
                .Sum(a => a.CurrentBalance);
        }
    }

    public void OnTagSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox)
        {
            SelectedTags = listBox.SelectedItems.Cast<TagModel>().ToList();
            LoadTransactionsAsync().ConfigureAwait(false);
        }
    }
}
