using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;
using Microsoft.UI.Xaml;
using Fin_Manager_v2.Contracts.Services;
using Windows.Services.Maps;

namespace Fin_Manager_v2.ViewModels;

public partial class AccountViewModel : ObservableRecipient
{
    private readonly IAccountService _accountService;

    private readonly IDialogService _dialogService;

    // Observable properties: An observable property is a property that notifies the UI when its value changes.
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

    /// <summary>Initializes a new instance of the AccountViewModel class.</summary>
    /// <param name="accountService">The account service to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when the account service is null.</exception>
    public AccountViewModel(IAccountService accountService)
    {
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        _dialogService = App.GetService<IDialogService>();
        _accounts = new ObservableCollection<AccountModel>();
        IsLoading = false;
        HasError = false;
        IsInitialized = false;
    }

    /// <summary>Initializes the object asynchronously.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method loads accounts asynchronously and sets the IsInitialized flag to true upon successful initialization.
    /// If an exception occurs during initialization, HasError is set to true and an error message is displayed.
    /// </remarks>
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
            await _dialogService.ShowErrorAsync("Initialization Error", ex.Message);
        }
    }

    /// <summary>
    /// Asynchronously loads accounts from the account service.
    /// </summary>
    /// <remarks>
    /// This method sets the loading state to true, clears any previous error message,
    /// retrieves accounts from the account service, populates the Accounts collection,
    /// handles any exceptions by displaying an error message, and sets the loading state back to false.
    /// </remarks>
    /// <returns>A task representing the asynchronous operation.</returns>
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
            await _dialogService.ShowErrorAsync("Failed to load accounts", e.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ResetError()
    {
        _errorMessage = null;
    }

    private void SetError(string title, string message)
    {
        HasError = true;
        ErrorMessage = $"{title}: {message}";
    }

    private string _errorText;
    public string ErrorText
    {
        get => _errorText;
        set
        {
            if (_errorText != value)
            {
                _errorText = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Add or update an account.
    /// </summary>
    /// <param name="accountName">The name of the account.</param>
    /// <param name="accountType">The type of the account.</param>
    /// <param name="initialBalance">The initial balance of the account.</param>
    /// <param name="currency">The currency of the account.</param>
    /// <param name="currentEditingAccount">The current editing account.</param>
    public async Task<bool> AddOrUpdateAccountAsync(string accountName, string accountType, decimal initialBalance, string currency, AccountModel currentEditingAccount)
    {
        if (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(accountType) || string.IsNullOrEmpty(currency))
        {
            ErrorText = "Please fill in all required fields.";
            return false;
        }

        if (currentEditingAccount != null)
        {
            var account = new UpdateFinanceAccountDto
            {
                account_id = currentEditingAccount.AccountId,
                account_name = accountName,
                account_type = accountType,
                initial_balance = initialBalance,
                current_balance = currentEditingAccount.CurrentBalance,
                currency = currency,
            };

            await UpdateAccountAsync(account);
        }
        else
        {
            var newAccount = new CreateFinanceAccountDto
            {
                account_name = accountName,
                account_type = accountType,
                initial_balance = initialBalance,
                currency = currency,
                current_balance = initialBalance,
            };

            await AddAccountAsync(newAccount);
        }

        return true;
    }

    /// <summary>
    /// Add a new account.
    /// </summary>
    /// <param name="accountDto">The account data transfer object.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<bool> AddAccountAsync(CreateFinanceAccountDto accountDto)
    {
        ResetError();

        if (!ValidateAccountDto(accountDto))
        {
            return false;
        }

        try
        {
            IsLoading = true;
            var success = await _accountService.CreateAccountAsync(accountDto);
            if (success)
            {
                await LoadAccountsAsync();
                return true;
            }
            else
            {
                await _dialogService.ShowErrorAsync("Failed to add account", "An error occurred during account creation.");
                return false;
            }
        }
        catch (Exception e)
        {
            await _dialogService.ShowErrorAsync("Failed to add account", e.Message);
            return false;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Delete an account.
    /// </summary>
    /// <param name="account">The account to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteAccountAsync(AccountModel account)
    {
        if (Accounts.Contains(account))
        {
            Accounts.Remove(account);

            await _accountService.DeleteAccountAsync(account.AccountId);
        }
    }

    /// <summary>
    /// Validate the account data transfer object.
    /// </summary>
    /// <param name="accountDto">The account data transfer object.</param>
    /// <returns>A boolean indicating whether the account data transfer object is valid.</returns>
    private bool ValidateAccountDto(CreateFinanceAccountDto accountDto)
    {
        if (string.IsNullOrWhiteSpace(accountDto.account_name))
        {
            SetError("Validation Error", "Please enter an account name.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(accountDto.account_type))
        {
            SetError("Validation Error", "Please select an account type.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(accountDto.currency))
        {
            SetError("Validation Error", "Please select a currency.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Update an account.
    /// </summary>
    /// <param name="account">The account data transfer object.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateAccountAsync(UpdateFinanceAccountDto account)
    {
        try
        {
            var success = await _accountService.UpdateAccountAsync(account);
            if (success)
            {
                LoadAccountsAsync();
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

    /// <summary>
    /// Determine the visibility of the collection.
    /// </summary>
    /// <param name="accounts">The observable collection of accounts.</param>
    /// <returns>A Visibility value indicating whether the collection is visible or collapsed.</returns>
    public Visibility CollectionVisibility(ObservableCollection<AccountModel> accounts)
    {
        return (accounts == null || accounts.Count == 0) ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// Determine the inverse visibility of the collection.
    /// </summary>
    /// <param name="accounts">The observable collection of accounts.</param>
    /// <returns>A Visibility value indicating whether the collection is visible or collapsed.</returns>
    public Visibility InverseCollectionVisibility(ObservableCollection<AccountModel> accounts)
    {
        return (accounts == null || accounts.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
    }
}