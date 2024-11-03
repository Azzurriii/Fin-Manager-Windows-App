using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fin_Manager_v2.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Fin_Manager_v2.Services.Interface;

namespace Fin_Manager_v2.ViewModels
{
    public partial class TransactionViewModel : ObservableRecipient
    {
        private readonly ITransactionService _transactionService;
        private readonly ITagService _tagService;
        private readonly IAccountService _accountService;

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
            SelectedDate = DateTimeOffset.Now;
            NewTransaction = new TransactionModel();
            LoadInitialData();
        }

        [RelayCommand]
        private async Task InitializeAsync()
        {
            try
            {
                await LoadTransactionsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing: {ex.Message}");
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

        [RelayCommand]
        private void LoadInitialData()
        {
            Task.Run(async () =>
            {
                await LoadAccountsAsync();
                await LoadTagsAsync();
            });
        }

        private async Task LoadAccountsAsync()
        {
            try
            {
                var accounts = await _accountService.GetAccountsAsync();
                var accountsList = new List<Account> { new Account { AccountName = "All Accounts" } };
                accountsList.AddRange(accounts);
                Accounts = new ObservableCollection<Account>(accountsList);
                SelectedAccountObj = Accounts.FirstOrDefault();
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
                AvailableTags = new ObservableCollection<TagModel>(tags);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading tags: {ex.Message}");
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
            SelectedAccountObj = Accounts.FirstOrDefault(a => a.AccountName != "All Accounts");
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
                    SelectedAccountObj.AccountName == "All Accounts")
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
