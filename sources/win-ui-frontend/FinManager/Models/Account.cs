using System.ComponentModel;

namespace FinManager.Models;

public class Account : INotifyPropertyChanged
{
    public int AccountId { get; set; }
    public int UserId { get; set; }

    private string? _accountName; // Make this nullable
    public string AccountName
    {
        get { return _accountName ?? string.Empty; } // Return an empty string if null
        set
        {
            if (_accountName != value)
            {
                _accountName = value;
                OnPropertyChanged(nameof(AccountName));
            }
        }
    }

    private string? _accountType;
    public string AccountType
    {
        get { return _accountType ?? string.Empty; }
        set
        {
            if (_accountType != value)
            {
                _accountType = value;
                OnPropertyChanged(nameof(AccountType));
            }
        }
    }

    private decimal _initialBalance;
    public decimal InitialBalance
    {
        get { return _initialBalance; }
        set
        {
            if (_initialBalance != value)
            {
                _initialBalance = value;
                OnPropertyChanged(nameof(InitialBalance));
            }
        }
    }

    private decimal _currentBalance;
    public decimal CurrentBalance
    {
        get { return _currentBalance; }
        set
        {
            if (_currentBalance != value)
            {
                _currentBalance = value;
                OnPropertyChanged(nameof(CurrentBalance));
            }
        }
    }

    public string Currency { get; set; } = string.Empty; // Initialize to empty string to avoid null
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public DateTime UpdateAt { get; set; } = DateTime.Now;

    public event PropertyChangedEventHandler? PropertyChanged; // Make this nullable
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}