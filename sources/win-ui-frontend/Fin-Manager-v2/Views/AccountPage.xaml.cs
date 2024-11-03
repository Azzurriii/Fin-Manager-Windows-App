using Fin_Manager_v2.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Security.Principal;
using Fin_Manager_v2.Model;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Services.Interface;
using Fin_Manager_v2.Services;

namespace Fin_Manager_v2.Views;

public sealed partial class AccountPage : Page
{
    public AccountViewModel ViewModel { get; set; }

    private bool isUserSelection = false;

    public AccountPage()
    {
        this.InitializeComponent();
        ViewModel = new AccountViewModel();
        this.DataContext = ViewModel;

        this.Loaded += OnPageLoaded;
    }

    private void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        isUserSelection = true;
    }

    private void OnAccountSelected(object sender, SelectionChangedEventArgs e)
    {
        if (isUserSelection) // Kiểm tra xem người dùng có thực sự chọn mục hay không
        {
            var selectedAccount = (Account)((ListView)sender).SelectedItem;
            if (selectedAccount != null)
            {
                ViewModel.SelectedAccount = selectedAccount;
                DispatcherQueue.TryEnqueue(() =>
                {
                    Frame.Navigate(typeof(AccountDetailPage), selectedAccount);
                });
            }
        }
    }


    private async void OnAddAccountClick(object sender, RoutedEventArgs e)
    {
        await AddAccountDialog.ShowAsync();
    }

    private async void OnAddAccountDialogPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var accountName = AccountNameInput.Text;
        var accountType = AccountTypeInput.Text;
        var currency = CurrencyInput.Text;

        if (decimal.TryParse(InitialBalanceInput.Text, out decimal initialBalance))
        {

            var newAccount = new CreateFinanceAccountDto
            {
                account_name = accountName,
                account_type = accountType,
                initial_balance = initialBalance,
                current_balance = initialBalance,
                currency = currency,
            };

            try
            {
                await ViewModel.AddAccountAsync(newAccount);

                ViewModel.SelectedAccount = null;
            }
            catch (Exception ex)
            {
                await ShowErrorDialog("Failed to add account. Please try again.");
                Console.WriteLine($"Error adding account: {ex.Message}");
            }
        }
        else
        {
            await ShowErrorDialog("Please enter a valid initial balance.");
        }
    }

    private async Task ShowErrorDialog(string message)
    {
        var errorDialog = new ContentDialog
        {
            Title = "Error",
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
        };
        await errorDialog.ShowAsync();
    }

}
