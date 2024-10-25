using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FinManager.Models;
using Microsoft.UI.Xaml;

namespace FinManager.ViewModels;

public class AccountViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Account> Accounts { get; set; }

    private Account _selectedAccount;
    public Account SelectedAccount
    {
        get { return _selectedAccount; }
        set
        {
            if (_selectedAccount != value)  // Add this check to prevent redundant updates
            {
                _selectedAccount = value;
                OnPropertyChanged(nameof(SelectedAccount));
            }
        }
    }

    public static readonly DependencyProperty SelectedAccountProperty =
            DependencyProperty.Register("SelectedAccount", typeof(Account), typeof(AccountViewModel), new PropertyMetadata(null));

    public AccountViewModel()
    {
        Accounts = new ObservableCollection<Account>
        {
            new Account { AccountName = "Checking", AccountType = "Bank", CurrentBalance = 1000 },
            new Account { AccountName = "Savings", AccountType = "Bank", CurrentBalance = 5000 }
        };

        // Initialize SelectedAccount to avoid null reference issues
        //SelectedAccount = Accounts[0];
    }

    public void AddAccount(Account account)
    {
        Accounts.Add(account);
    }

    public void UpdateAccount(Account account)
    {
        var existingAccount = Accounts.FirstOrDefault(a => a.AccountId == account.AccountId);
        if (existingAccount != null)
        {
            existingAccount.AccountName = account.AccountName;
            existingAccount.AccountType = account.AccountType;
            existingAccount.InitialBalance = account.InitialBalance;
            existingAccount.CurrentBalance = account.CurrentBalance;
            existingAccount.UpdateAt = DateTime.Now;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}