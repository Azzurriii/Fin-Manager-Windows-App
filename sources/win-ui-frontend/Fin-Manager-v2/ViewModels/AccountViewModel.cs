using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services.Interface;

namespace Fin_Manager_v2.ViewModels;

public partial class AccountViewModel : ObservableRecipient
{
    private readonly IAccountService _accountService;

    [ObservableProperty]
    private ObservableCollection<AccountModel> _accounts;

    [ObservableProperty]
    private AccountModel _selectedAccount;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _errorMessage;

    [ObservableProperty]
    private bool _isInitialized;

    public AccountViewModel(IAccountService accountService)
    {
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        _accounts = new ObservableCollection<AccountModel>();
        IsLoading = false;
        HasError = false;
        IsInitialized = false;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized) return;

        try
        {
            await LoadAccountsAsync();
            IsInitialized = true;
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Initialization error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"AccountViewModel initialization failed: {ex}");
        }
    }

    public async Task LoadAccountsAsync()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;

            var accounts = await _accountService.GetAccountsAsync();
            Accounts.Clear();
            foreach (var account in accounts)
            {
                Accounts.Add(account);
            }
        }
        catch (Exception e)
        {
            HasError = true;
            ErrorMessage = "Failed to load accounts. Please try again.";
            System.Diagnostics.Debug.WriteLine($"Request error: {e.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task AddAccountAsync(CreateFinanceAccountDto account)
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = string.Empty;

            var success = await _accountService.CreateAccountAsync(account);
            if (success)
            {
                await LoadAccountsAsync();
            }
            else
            {
                HasError = true;
                ErrorMessage = "Failed to add account. Please try again.";
            }
        }
        catch (Exception e)
        {
            HasError = true;
            ErrorMessage = "Failed to add account. Please try again.";
            System.Diagnostics.Debug.WriteLine($"Request error: {e.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task UpdateAccount(AccountModel account)
    {
        try
        {
            var success = await _accountService.UpdateAccountAsync(account);
            if (success)
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
            else
            {
                HasError = true;
                ErrorMessage = "Failed to update account. Please try again.";
            }
        }
        catch (Exception e)
        {
            HasError = true;
            ErrorMessage = "Failed to update account. Please try again.";
            System.Diagnostics.Debug.WriteLine($"Request error: {e.Message}");
        }
    }
}