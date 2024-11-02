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
        private string _selectedAccount;

        [ObservableProperty]
        private ObservableCollection<string> _accounts = new() { "All Accounts", "Account 1", "Account 2" };

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

        public TransactionViewModel(ITransactionService transactionService, ITagService tagService)
        {
            _transactionService = transactionService;
            _tagService = tagService;
            SelectedDate = DateTimeOffset.Now;
            SelectedAccount = "All Accounts";
            NewTransaction = new TransactionModel();
            LoadTagsAsync();
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
                var transactions = await _transactionService.GetUserTransactionsAsync(1);
                Transactions = new ObservableCollection<TransactionModel>(transactions);

                // Calculate totals
                var startDate = new DateTime(SelectedDate?.Year ?? DateTime.Now.Year, 
                                           SelectedDate?.Month ?? DateTime.Now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                var accountId = SelectedAccount == "All Accounts" ? null : GetAccountId(SelectedAccount);

                TotalIncome = await _transactionService.GetTotalAmountAsync(1, accountId, "INCOME", startDate, endDate);
                TotalExpense = await _transactionService.GetTotalAmountAsync(1, accountId, "EXPENSE", startDate, endDate);
                Balance = TotalIncome - TotalExpense;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading transactions: {ex.Message}");
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
            SelectedAccount = "Account 1";
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
                    SelectedAccount == null || 
                    SelectedAccount == "All Accounts")
                {
                    return;
                }

                NewTransaction.AccountId = GetAccountId(SelectedAccount) ?? 1;
                NewTransaction.UserId = 1;
                NewTransaction.TransactionType = SelectedTransactionType;
                NewTransaction.Amount = Convert.ToDecimal(TransactionAmount);
                NewTransaction.TagId = SelectedTag?.Id;
                NewTransaction.Description = NewTransaction.Description.Trim();
                NewTransaction.Date = TransactionDate.DateTime.ToUniversalTime();

                System.Diagnostics.Debug.WriteLine($"Creating transaction with TagId: {NewTransaction.TagId}");

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

        private int? GetAccountId(string accountName)
        {
            return accountName switch
            {
                "Account 1" => 1,
                "Account 2" => 2,
                _ => null
            };
        }

        partial void OnTransactionAmountChanged(double value)
        {
            if (NewTransaction != null)
            {
                NewTransaction.Amount = Convert.ToDecimal(value);
            }
        }
    }
}
