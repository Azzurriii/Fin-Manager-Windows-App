using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fin_Manager_v2.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Fin_Manager_v2.ViewModels
{
    public partial class TransactionViewModel : ObservableRecipient
    {
        private readonly ITransactionService _transactionService;

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

        public TransactionViewModel(ITransactionService transactionService)
        {
            _transactionService = transactionService;
            SelectedDate = DateTimeOffset.Now;
            SelectedAccount = "All Accounts";
        }

        partial void OnSelectedDateChanged(DateTimeOffset? value)
        {
            LoadTransactionsCommand.ExecuteAsync(null);
        }

        partial void OnSelectedAccountChanged(string value)
        {
            LoadTransactionsCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private async Task LoadTransactionsAsync()
        {
            try
            {
                var transactions = await _transactionService.GetUserTransactionsAsync(1);

                // Filter transactions by date
                if (SelectedDate.HasValue)
                {
                    transactions = transactions.Where(t =>
                        t.Date.Month == SelectedDate.Value.Month &&
                        t.Date.Year == SelectedDate.Value.Year).ToList();
                }

                // Filter transactions by account
                if (SelectedAccount != "All Accounts")
                {
                    transactions = transactions.Where(t => t.Account == SelectedAccount).ToList();
                }

                Transactions = new ObservableCollection<TransactionModel>(transactions);

                // Calculate totals
                TotalIncome = transactions.Where(t => t.TransactionType == "INCOME").Sum(t => t.Amount);
                TotalExpense = transactions.Where(t => t.TransactionType == "EXPENSE").Sum(t => t.Amount);
                Balance = TotalIncome - TotalExpense;
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log them or display a user message
            }
        }

        [RelayCommand]
        private async Task CreateTransactionAsync()
        {
            if (NewTransaction == null || NewTransaction.Amount <= 0)
            {
                // Validate amount or other fields as needed
                return;
            }

            NewTransaction.UserId = 1; // User ID hard-coded, adjust as needed
            NewTransaction.Date = DateTime.Now;

            try
            {
                var result = await _transactionService.CreateTransactionAsync(NewTransaction);
                if (result)
                {
                    await LoadTransactionsAsync();
                    NewTransaction = new TransactionModel(); // Reset for next input
                    IsAddTransactionDialogOpen = false;
                }
            }
            catch (Exception ex)
            {
                // Handle exception, e.g., log or show a message
            }
        }

        [RelayCommand]
        private void OpenAddTransaction()
        {
            NewTransaction = new TransactionModel();
            IsAddTransactionDialogOpen = true;
        }
    }
}
