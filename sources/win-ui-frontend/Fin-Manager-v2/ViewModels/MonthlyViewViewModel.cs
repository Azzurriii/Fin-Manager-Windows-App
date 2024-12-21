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
using System.Diagnostics;

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

    /// <summary>Initializes a new instance of the MonthlyViewViewModel class.</summary>
    /// <param name="transactionService">The transaction service to use.</param>
    /// <param name="tagService">The tag service to use.</param>
    /// <param name="accountService">The account service to use.</param>
    /// <param name="authService">The authentication service to use.</param>
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

    /// <summary>
    /// Initializes the view asynchronously by loading accounts, tags, and transactions.
    /// </summary>
    /// <remarks>
    /// This method first loads accounts and tags asynchronously using <see cref="LoadAccountsAsync"/> and <see cref="LoadTagsAsync"/>.
    /// Then, it proceeds to load transactions asynchronously using <see cref="LoadTransactionsAsync"/>.
    /// If any exception occurs during the initialization process, an error message is displayed.
    /// </remarks>
    /// <returns>A task representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Asynchronously loads transactions based on the specified query parameters and updates the UI.
    /// </summary>
    /// <remarks>
    /// This method retrieves transactions based on the provided query parameters, including user ID, account ID, date range, and selected tags.
    /// It then updates the UI with the loaded transactions, calculates total income, total expense, and the balance.
    /// </remarks>
    /// <exception cref="Exception">Thrown when an error occurs during the loading process.</exception>
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

    [RelayCommand]
private async Task ExportTransactionsAsync()
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

        string fileName = $"transactions_{StartDate:yyyy-MM-dd}_to_{EndDate:yyyy-MM-dd}.csv";
        var result = await _transactionService.ExportTransactionsAsync(query, fileName);

        if (result)
        {
            await _dialogService.ShowSuccessAsync(
                "Export Successful",
                "Transactions have been exported successfully.");
        }
        else
        {
            await _dialogService.ShowErrorAsync(
                "Export Failed",
                "Failed to export transactions.");
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Error exporting transactions: {ex.Message}");
        await _dialogService.ShowErrorAsync(
            "Export Error",
                "An error occurred while exporting transactions.");
        }
    }

    /// <summary>
    /// Asynchronously loads accounts from the account service and updates the collection of accounts.
    /// </summary>
    /// <remarks>
    /// This method retrieves accounts from the account service and updates the Accounts collection with the retrieved accounts.
    /// If an exception occurs during the process, it is caught and a debug message is written.
    /// </remarks>
    /// <returns>A task representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Asynchronously loads tags from the tag service and updates the AvailableTags collection.
    /// </summary>
    /// <remarks>
    /// This method retrieves tags from the tag service and updates the AvailableTags collection with the retrieved tags.
    /// </remarks>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when an error occurs while loading tags.</exception>
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

    /// <summary>
    /// Loads tag names for a list of transactions asynchronously.
    /// </summary>
    /// <param name="transactions">The list of transactions to load tag names for.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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

    /// <summary>
    /// Asynchronously loads transactions when the start date, end date, selected account, or selected tags are changed.
    /// </summary>
    /// <remarks>
    /// This method is called when the start date, end date, selected account, or selected tags are changed.
    /// It triggers the asynchronous loading of transactions without capturing the context.
    /// </remarks>
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
         _dispatcherQueue.TryEnqueue(() =>
    {
        LoadTransactionsAsync().ConfigureAwait(false);
    });
    }

    /// <summary>
    /// Calculates the balance based on the selected account and transaction totals.
    /// </summary>
    /// <remarks>
    /// If a specific account is selected (not "All Accounts"), the balance is calculated as the initial balance of the selected account
    /// plus the difference between total income and total expenses.
    /// If "All Accounts" is selected or no specific account is chosen, the balance is calculated as the sum of current balances of all accounts
    /// excluding the "All Accounts" account.
    /// </remarks>
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

    /// <summary>
    /// Handles the event when the selection of tags in a ListBox changes.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event data.</param>
    /// <remarks>
    /// This method updates the SelectedTags property with the selected items in the ListBox.
    /// It then asynchronously loads transactions based on the selected tags.
    /// </remarks>
    public void OnTagSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox)
        {
            SelectedTags = listBox.SelectedItems.Cast<TagModel>().ToList();
            LoadTransactionsAsync().ConfigureAwait(false);
        }
    }
}
