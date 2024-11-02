using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using Fin_Manager_v2.Model;
using Fin_Manager_v2.DTO;
using System.Net.Http.Json;
using Fin_Manager_v2.Services.Interface;
using Fin_Manager_v2.Services;
using System.Net.Http.Headers;

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

    public AccountViewModel()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:3000/") };
        Accounts = new ObservableCollection<Account>();

        _authService = new AuthService(_httpClient);

        var token = _authService.GetAccessToken();

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

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
            Console.WriteLine($"Request error: {e.Message}");
        }
    }

    public async Task AddAccountAsync(CreateFinanceAccountDto account)
    {
        try
        {
            var token = _authService.GetAccessToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync("finance-accounts", account);

            if (response.IsSuccessStatusCode)
            {
                await LoadAccountsAsync();
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {errorMessage}");
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
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

            OnPropertyChanged(nameof(Accounts));
        }
    }
}
