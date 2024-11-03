﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services.Interface;

namespace Fin_Manager_v2.ViewModels;

public partial class AccountViewModel : ObservableRecipient
{
    private readonly IAccountService _accountService;

    public ObservableCollection<Account> Accounts { get; private set; }

    [ObservableProperty]
    private Account _selectedAccount;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _errorMessage;

    public AccountViewModel(IAccountService accountService)
    {
        try
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            Accounts = new ObservableCollection<Account>();
            IsLoading = false;
            HasError = false;

            // Load accounts asynchronously
            _ = InitializeAsync();
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Initialization error: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"AccountViewModel initialization failed: {ex}");
        }
    }

    private async Task InitializeAsync()
    {
        await LoadAccountsAsync();
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

    public async Task UpdateAccount(Account account)
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