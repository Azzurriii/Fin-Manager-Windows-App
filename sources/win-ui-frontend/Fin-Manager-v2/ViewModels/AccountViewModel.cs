using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using Fin_Manager_v2.Model;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Fin_Manager_v2.Services.Interface;
using Fin_Manager_v2.Services;

namespace Fin_Manager_v2.ViewModels;

public partial class AccountViewModel : ObservableRecipient
{
    //public ObservableCollection<Account> Accounts { get; set; }

    //private Account _selectedAccount;
    //public Account SelectedAccount
    //{
    //    get { return _selectedAccount; }
    //    set
    //    {
    //        if (_selectedAccount != value)  // Add this check to prevent redundant updates
    //        {
    //            _selectedAccount = value;
    //            OnPropertyChanged(nameof(SelectedAccount));
    //        }
    //    }
    //}

    //public static readonly DependencyProperty SelectedAccountProperty =
    //        DependencyProperty.Register("SelectedAccount", typeof(Account), typeof(AccountViewModel), new PropertyMetadata(null));

    //public AccountViewModel()
    //{
    //    Accounts = new ObservableCollection<Account>
    //    {
    //        new Account { AccountName = "Checking", AccountType = "Bank", CurrentBalance = 1000 },
    //        new Account { AccountName = "Savings", AccountType = "Bank", CurrentBalance = 5000 }
    //    };

    //    // Initialize SelectedAccount to avoid null reference issues
    //    //SelectedAccount = Accounts[0];
    //}

    //public void AddAccount(Account account)
    //{
    //    Accounts.Add(account);
    //}

    //public void UpdateAccount(Account account)
    //{
    //    var existingAccount = Accounts.FirstOrDefault(a => a.AccountId == account.AccountId);
    //    if (existingAccount != null)
    //    {
    //        existingAccount.AccountName = account.AccountName;
    //        existingAccount.AccountType = account.AccountType;
    //        existingAccount.InitialBalance = account.InitialBalance;
    //        existingAccount.CurrentBalance = account.CurrentBalance;
    //        existingAccount.UpdateAt = DateTime.Now;
    //    }
    //}

    //public event PropertyChangedEventHandler PropertyChanged;
    //protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    //{
    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //}

    private readonly HttpClient _httpClient;

    private readonly IAuthService _authService;

    public ObservableCollection<Account> Accounts { get; private set; }

    [ObservableProperty]
    private Account _selectedAccount;

    private string RetrieveTokenFromLocalStorage()
    {
        // Replace with your actual retrieval logic, e.g., reading from local storage
        // This is just a placeholder
        return Windows.Storage.ApplicationData.Current.LocalSettings.Values["AuthToken"] as string;
    }

    //public AccountViewModel() { }

    public AccountViewModel()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:3000/") }; // Replace with your API base URL
        Accounts = new ObservableCollection<Account>();

        _authService = new AuthService(_httpClient);

        var token = _authService.GetAccessToken(); // You need to implement this function

        // Set Authorization header
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // Load accounts asynchronously
        LoadAccountsAsync().ConfigureAwait(true);
    }

    public async Task LoadAccountsAsync()
    {
        try
        {
            var accounts = await _httpClient.GetFromJsonAsync<List<Account>>("finance-accounts/me"); // Replace with your endpoint
            if (accounts != null)
            {
                Accounts.Clear();
                foreach (var account in accounts)
                {
                    Accounts.Add(account);
                }
            }
        }
        catch (HttpRequestException e)
        {
            // Handle the error (e.g., log it or show a message to the user)
            Console.WriteLine($"Request error: {e.Message}");
        }
    }

    public async Task AddAccountAsync(Account account)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("finance-accounts", account); // Replace with your endpoint
            if (response.IsSuccessStatusCode)
            {
                // Optionally, reload accounts or add the new account to the collection
                // Option 1: Reload all accounts
                await LoadAccountsAsync();

                // Option 2: Add the new account directly to the collection
                // Accounts.Add(account); 
            }
            else
            {
                // Handle error response
            }
        }
        catch (HttpRequestException e)
        {
            // Handle the error
        }
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

            // Notify property change if needed
            OnPropertyChanged(nameof(Accounts));
        }
    }
}
