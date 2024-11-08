﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;
using Microsoft.UI.Xaml;
using Fin_Manager_v2.Contracts.Services;

namespace Fin_Manager_v2.ViewModels;

public partial class AccountViewModel : ObservableRecipient
{
    private readonly IAccountService _accountService;

    private readonly IDialogService _dialogService;

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
        _dialogService = App.GetService<IDialogService>();
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
            await _dialogService.ShowErrorAsync("Initialization Error", ex.Message);
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

    public Visibility CollectionVisibility(ObservableCollection<AccountModel> accounts)
    {
        return (accounts == null || accounts.Count == 0) ? Visibility.Visible : Visibility.Collapsed;
    }

    public Visibility InverseCollectionVisibility(ObservableCollection<AccountModel> accounts)
    {
        return (accounts == null || accounts.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
    }
}