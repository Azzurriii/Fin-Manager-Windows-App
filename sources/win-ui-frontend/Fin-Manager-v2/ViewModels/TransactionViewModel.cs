using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fin_Manager_v2.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Fin_Manager_v2.Services;
using Fin_Manager_v2.Contracts.Services;
using Microsoft.UI.Xaml.Controls;

namespace Fin_Manager_v2.ViewModels
{
    public partial class TransactionViewModel : ObservableRecipient
    {
        private readonly ITransactionService _transactionService;
        private readonly ITagService _tagService;
        private readonly IAccountService _accountService;
        private readonly DispatcherQueue _dispatcherQueue;
        private readonly IAuthService _authService;
        private readonly IDialogService _dialogService = new DialogService();

        [ObservableProperty]
        private ObservableCollection<TransactionModel> _transactions = new();

        [ObservableProperty]
        private TransactionModel _newTransaction = new();

        [ObservableProperty]
        private decimal _totalIncome;

        [ObservableProperty]
        private decimal _totalExpense;

        [ObservableProperty]
        private decimal _balance;

        [ObservableProperty]
        private DateTimeOffset? _selectedDate;

        [ObservableProperty]
        private ObservableCollection<AccountModel> _accounts = new();

        [ObservableProperty]
        private bool _isAddTransactionDialogOpen;

        [ObservableProperty]
        private DateTimeOffset _transactionDate = DateTimeOffset.Now;

        [ObservableProperty]
        private string _selectedTransactionType = "INCOME";

        [ObservableProperty]
        private double _transactionAmount;

        [ObservableProperty]
        private ObservableCollection<TagModel> _availableTags = new();

        [ObservableProperty]
        private TagModel? _selectedTag;

        [ObservableProperty]
        private AccountModel _selectedAccountObj;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private int _editingTransactionId;

        public string DialogTitle => IsEditMode ? "Edit Transaction" : "Add New Transaction";

        public TransactionViewModel(ITransactionService transactionService,
            ITagService tagService,
            IAccountService accountService,
            IAuthService authService
            )
        {
            _transactionService = transactionService;
            _tagService = tagService;
            _accountService = accountService;
            _authService = authService;

            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            SelectedDate = DateTimeOffset.Now;
            NewTransaction = new TransactionModel();

            Transactions = new ObservableCollection<TransactionModel>();
            Accounts = new ObservableCollection<AccountModel>();
            AvailableTags = new ObservableCollection<TagModel>();

            _ = InitializeAsync();
        }

        [RelayCommand]
        private async Task InitializeAsync()
        {
            try
            {
                await Task.WhenAll(
                    LoadAccountsAsync(),
                    LoadTagsByTypeAsync("INCOME")
                );
                await LoadTransactionsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                await _dialogService.ShowErrorAsync(
                    "Initialization Error",
                    "An error occurred while initializing the application. Please try again later.");
            }
        }

        [RelayCommand]
        private async Task LoadTransactionsAsync()
        {
            try
            {
                var startDate = new DateTime(SelectedDate?.Year ?? DateTime.Now.Year,
                    SelectedDate?.Month ?? DateTime.Now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                var userId = _authService.GetUserId() ?? 0;
                var accountId = SelectedAccountObj?.AccountName == "All Accounts" ? null : SelectedAccountObj?.AccountId;

                // Load transactions
                var transactions = await _transactionService.GetUserTransactionsAsync(userId);
                var filteredTransactions = FilterTransactions(transactions, accountId, startDate, endDate);

                // Load tag names for all transactions in parallel
                await LoadTagNamesForTransactions(filteredTransactions);

                Transactions = new ObservableCollection<TransactionModel>(filteredTransactions);

                // Calculate totals
                await CalculateTotals(userId, accountId, startDate, endDate);

                // Calculate balance
                CalculateBalance();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading transactions: {ex.Message}");
                await _dialogService.ShowErrorAsync(
                    "Load Transactions Error",
                    "An error occurred while loading the transactions. Please try again later.");
            }
        }

        private IList<TransactionModel> FilterTransactions(IList<TransactionModel> transactions, int? accountId, DateTime startDate, DateTime endDate)
        {
            if (accountId.HasValue)
            {
                return transactions.Where(t => t.AccountId == accountId).ToList();
            }
            return transactions.Where(t => t.Date >= startDate && t.Date <= endDate).ToList();
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

        private async Task CalculateTotals(int userId, int? accountId, DateTime startDate, DateTime endDate)
        {
            var tasks = new[]
            {
                _transactionService.GetTotalAmountAsync(userId, accountId, "INCOME", startDate, endDate),
                _transactionService.GetTotalAmountAsync(userId, accountId, "EXPENSE", startDate, endDate)
            };

            var results = await Task.WhenAll(tasks);
            TotalIncome = results[0];
            TotalExpense = results[1];
        }

        private void CalculateBalance()
        {
            if (SelectedAccountObj?.AccountId != null && SelectedAccountObj.AccountName != "All Accounts")
            {
                Balance = SelectedAccountObj.InitialBalance + (TotalIncome - TotalExpense);
            }
            else if (SelectedAccountObj?.AccountName == "All Accounts")
            {
                Balance = Accounts
                    .Where(a => a.AccountName != "All Accounts")
                    .Sum(a => a.CurrentBalance);
            }
        }

        private async Task LoadAccountsAsync()
        {
            try
            {
                var accounts = await _accountService.GetAccountsAsync();
                System.Diagnostics.Debug.WriteLine($"Loaded {accounts?.Count() ?? 0} accounts");

                _dispatcherQueue.TryEnqueue((DispatcherQueueHandler)(() =>
                {
                    Accounts.Clear();
                    var allAccounts = new AccountModel { AccountId = 0, AccountName = "All Accounts" };
                    Accounts.Add(allAccounts);

                    if (accounts != null)
                    {
                        foreach (var account in accounts)
                        {
                            Accounts.Add(account);
                        }
                    }

                    SelectedAccountObj = Enumerable.FirstOrDefault<AccountModel>(Accounts);
                    System.Diagnostics.Debug.WriteLine($"Final accounts count: {Accounts.Count}");
                }));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading accounts: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        [RelayCommand]
        private void OpenAddTransaction()
        {
            NewTransaction = new TransactionModel
            {
                Date = DateTime.Now,
                TransactionType = "INCOME",
                Amount = 0
            };
            TransactionAmount = 0;
            TransactionDate = DateTimeOffset.Now;
            SelectedTransactionType = "INCOME";

            var firstRealAccount = Accounts.FirstOrDefault(a => a.AccountName != "All Accounts");
            if (firstRealAccount != null)
            {
                SelectedAccountObj = firstRealAccount;
            }

            LoadTagsByTypeAsync("INCOME").ConfigureAwait(false);
            SelectedTag = null;
            IsAddTransactionDialogOpen = true;
        }

        [RelayCommand]
        private async Task SaveTransactionAsync()
        {
            try
            {
                if (TransactionAmount <= 0)
                {
                    await _dialogService.ShowErrorAsync(
                        "Invalid Amount",
                        "Transaction amount must be greater than zero.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(NewTransaction.Description))
                {
                    await _dialogService.ShowErrorAsync(
                        "Invalid Description",
                        "Please provide a valid description for the transaction.");
                    return;
                }

                NewTransaction.AccountId = SelectedAccountObj.AccountId;
                NewTransaction.UserId = _authService.GetUserId() ?? 0;
                NewTransaction.TransactionType = SelectedTransactionType;
                NewTransaction.Amount = Convert.ToDecimal(TransactionAmount);
                NewTransaction.TagId = SelectedTag?.Id ?? 0;
                NewTransaction.Description = NewTransaction.Description.Trim();
                NewTransaction.Date = TransactionDate.DateTime.ToUniversalTime();

                bool success;
                if (IsEditMode)
                {
                    // Edit existing transaction
                    success = await _transactionService.UpdateTransactionAsync(EditingTransactionId, NewTransaction);
                }
                else
                {
                    // Create new transaction
                    success = await _transactionService.CreateTransactionAsync(NewTransaction);
                }

                if (success)
                {
                    await LoadTransactionsAsync();
                    IsAddTransactionDialogOpen = false;
                    IsEditMode = false;
                    // Reset form
                    NewTransaction = new TransactionModel();
                    TransactionAmount = 0;
                    TransactionDate = DateTimeOffset.Now;
                    SelectedTransactionType = "INCOME";
                    SelectedTag = null;
                }
                else
                {
                    if(SelectedAccountObj.AccountId == 0)
                    {
                        await _dialogService.ShowErrorAsync(
                            "Create Failed",
                            "Please select an account to add a transaction.");
                    }
                    else if (SelectedTag == null)
                    {
                        await _dialogService.ShowErrorAsync(
                            "Create Failed",
                            "Please select a tag to add a transaction.");
                    }
                    else
                    {
                        await _dialogService.ShowErrorAsync(
                            IsEditMode ? "Update Failed" : "Create Failed",
                            "An error occurred. Please try again.");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving transaction: {ex.Message}");
                await _dialogService.ShowErrorAsync(
                    IsEditMode ? "Update Failed" : "Create Failed",
                    "An unexpected error occurred. Please try again later.");
            }
        }

        partial void OnTransactionAmountChanged(double value)
        {
            if (NewTransaction != null)
            {
                NewTransaction.Amount = Convert.ToDecimal(value);
            }
        }

        partial void OnSelectedDateChanged(DateTimeOffset? value)
        {
            LoadTransactionsAsync().ConfigureAwait(false);
        }

        partial void OnSelectedAccountObjChanged(AccountModel value)
        {
            LoadTransactionsAsync().ConfigureAwait(false);
        }

        [RelayCommand]
        public void EditTransaction(TransactionModel transaction)
        {
            IsEditMode = true;
            EditingTransactionId = transaction.TransactionId;
            
            // Populate the dialog with existing values
            NewTransaction = new TransactionModel
            {
                Description = transaction.Description,
                Amount = transaction.Amount,
                TransactionType = transaction.TransactionType,
                TagId = transaction.TagId,
                AccountId = transaction.AccountId,
                UserId = transaction.UserId,
                Date = transaction.Date
            };
            
            TransactionAmount = Convert.ToDouble(transaction.Amount);
            TransactionDate = new DateTimeOffset(transaction.Date);
            SelectedTransactionType = transaction.TransactionType;
            SelectedAccountObj = Accounts.FirstOrDefault(a => a.AccountId == transaction.AccountId);
            SelectedTag = AvailableTags.FirstOrDefault(t => t.Id == transaction.TagId);
            
            IsAddTransactionDialogOpen = true;
        }

        [RelayCommand]
        public async Task DeleteTransactionAsync(TransactionModel transaction)
        {
            var dialog = new ContentDialog
            {
                Title = "Confirm Delete",
                Content = "Are you sure you want to delete this transaction?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = App.MainWindow.Content.XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var success = await _transactionService.DeleteTransactionAsync(transaction.TransactionId);
                if (success)
                {
                    await LoadTransactionsAsync();
                }
                else
                {
                    await _dialogService.ShowErrorAsync(
                        "Delete Failed",
                        "Failed to delete the transaction. Please try again.");
                }
            }
        }

        partial void OnIsEditModeChanged(bool value)
        {
            OnPropertyChanged(nameof(DialogTitle));
        }

        private async Task LoadTagsByTypeAsync(string type)
        {
            try
            {
                var tags = await _tagService.GetTagsByTypeAsync(type);
                _dispatcherQueue.TryEnqueue(() =>
                {
                    AvailableTags.Clear();
                    foreach (var tag in tags)
                    {
                        AvailableTags.Add(tag);
                    }
                    SelectedTag = null;
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading tags by type: {ex.Message}");
            }
        }

        partial void OnSelectedTransactionTypeChanged(string value)
        {
            LoadTagsByTypeAsync(value).ConfigureAwait(false);
        }
    }
}
