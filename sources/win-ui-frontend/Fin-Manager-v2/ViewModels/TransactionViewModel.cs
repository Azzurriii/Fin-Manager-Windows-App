using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fin_Manager_v2.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Fin_Manager_v2.Services.Interface;
using Microsoft.UI.Dispatching;

namespace Fin_Manager_v2.ViewModels
{
    public partial class TransactionViewModel : ObservableRecipient
    {
        private readonly ITransactionService _transactionService;
        private readonly ITagService _tagService;
        private readonly IAccountService _accountService;
        private readonly DispatcherQueue dispatcherQueue;

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
        private ObservableCollection<Account> _accounts = new();

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
        private TagModel _selectedTag;

        [ObservableProperty]
        private Account _selectedAccountObj;

        public TransactionViewModel(ITransactionService transactionService,
            ITagService tagService,
            IAccountService accountService)
        {
            _transactionService = transactionService;
            _tagService = tagService;
            _accountService = accountService;
            
            // Get the dispatcher queue for the current thread
            dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            
            SelectedDate = DateTimeOffset.Now;
            NewTransaction = new TransactionModel();
            
            // Initialize collections
            Transactions = new ObservableCollection<TransactionModel>();
            Accounts = new ObservableCollection<Account>();
            AvailableTags = new ObservableCollection<TagModel>();
            
            // Load data asynchronously
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
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
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

                var accountId = SelectedAccountObj?.AccountName == "All Accounts" ? null : SelectedAccountObj?.AccountId;

                var transactions = await _transactionService.GetUserTransactionsAsync(1);

                transactions = transactions
                    .Where(t => t.Date >= startDate && t.Date <= endDate)
                    .Where(t => accountId == null || t.AccountId == accountId)
                    .ToList();

                Transactions = new ObservableCollection<TransactionModel>(transactions);

                TotalIncome = await _transactionService.GetTotalAmountAsync(1, accountId, "INCOME", startDate, endDate);
                TotalExpense = await _transactionService.GetTotalAmountAsync(1, accountId, "EXPENSE", startDate, endDate);
                Balance = TotalIncome - TotalExpense;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading transactions: {ex.Message}");
            }
        }

        private async Task LoadAccountsAsync()
        {
            try
            {
                var accounts = await _accountService.GetAccountsAsync();
                System.Diagnostics.Debug.WriteLine($"Loaded {accounts?.Count() ?? 0} accounts");

                dispatcherQueue.TryEnqueue(() =>
                {
                    Accounts.Clear();
                    var allAccounts = new Account { AccountId = 0, AccountName = "All Accounts" };
                    Accounts.Add(allAccounts);

                    if (accounts != null)
                    {
                        foreach (var account in accounts)
                        {
                            Accounts.Add(account);
                        }
                    }

                    SelectedAccountObj = Accounts.FirstOrDefault();
                    System.Diagnostics.Debug.WriteLine($"Final accounts count: {Accounts.Count}");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading accounts: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private async Task LoadTagsAsync()
        {
            try
            {
                var tags = await _tagService.GetTagsAsync();
                System.Diagnostics.Debug.WriteLine($"Loaded {tags?.Count() ?? 0} tags");

                dispatcherQueue.TryEnqueue(() =>
                {
                    AvailableTags.Clear();
                    if (tags != null)
                    {
                        foreach (var tag in tags)
                        {
                            AvailableTags.Add(tag);
                        }
                    }
                    System.Diagnostics.Debug.WriteLine($"AvailableTags count: {AvailableTags.Count}");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading tags: {ex.Message}");
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
            
            // Ensure we select a valid account
            var firstRealAccount = Accounts.FirstOrDefault(a => a.AccountName != "All Accounts");
            if (firstRealAccount != null)
            {
                SelectedAccountObj = firstRealAccount;
            }
            
            SelectedTag = null;
            IsAddTransactionDialogOpen = true;
        }

        [RelayCommand]
        private async Task CreateTransactionAsync()
        {
            try
            {
                if (TransactionAmount <= 0 ||
                    string.IsNullOrEmpty(NewTransaction.Description) ||
                    SelectedAccountObj == null ||
                    SelectedAccountObj.AccountId == 0)
                {
                    return;
                }

                NewTransaction.AccountId = SelectedAccountObj.AccountId;
                NewTransaction.UserId = 1;
                NewTransaction.TransactionType = SelectedTransactionType;
                NewTransaction.Amount = Convert.ToDecimal(TransactionAmount);
                NewTransaction.TagId = SelectedTag?.Id ?? 0;
                NewTransaction.Description = NewTransaction.Description.Trim();
                NewTransaction.Date = TransactionDate.DateTime.ToUniversalTime();

                var result = await _transactionService.CreateTransactionAsync(NewTransaction);
                if (result)
                {
                    await LoadTransactionsAsync();
                    IsAddTransactionDialogOpen = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating transaction: {ex.Message}");
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

        partial void OnSelectedAccountObjChanged(Account value)
        {
            LoadTransactionsAsync().ConfigureAwait(false);
        }
    }
}
