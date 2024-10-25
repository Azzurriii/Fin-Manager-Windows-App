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
using FinManager.ViewModels;
using FinManager.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FinManager.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
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
        // Khi trang được nạp xong, cho phép chọn mục
        isUserSelection = true;
    }

    //private void OnAddAccountClick(object sender, RoutedEventArgs e)
    //{
    //    // Make sure SelectedAccount is not null
    //    if (ViewModel.SelectedAccount == null)
    //    {
    //        ViewModel.SelectedAccount = new Account(); // Initialize if null
    //    }

    //    var newAccount = new Account
    //    {
    //        AccountName = ViewModel.SelectedAccount.AccountName,
    //        AccountType = ViewModel.SelectedAccount.AccountType,
    //        InitialBalance = ViewModel.SelectedAccount.InitialBalance,
    //        CurrentBalance = ViewModel.SelectedAccount.CurrentBalance,
    //        Currency = "USD",
    //        UserId = 1 // For demo purposes, hardcode UserId
    //    };

    //    ViewModel.AddAccount(newAccount);
    //}

    //private void OnAccountSelected(object sender, SelectionChangedEventArgs e)
    //{
    //    if (ViewModel.SelectedAccount != null)
    //    {
    //        Frame.Navigate(typeof(AccountDetailPage), ViewModel.SelectedAccount);
    //    }
    //}

    //private void OnAccountSelected(object sender, SelectionChangedEventArgs e)
    //{
    //    var selectedAccount = (Account)((ListView)sender).SelectedItem;
    //    if (selectedAccount != null)
    //    {
    //        isUserSelection = false;
    //        // Sử dụng DispatcherQueue để thực hiện điều hướng sau khi UI hoàn thành
    //        DispatcherQueue.TryEnqueue(() =>
    //        {
    //            Frame.Navigate(typeof(AccountDetailPage), selectedAccount);
    //        });

    //        ((ListView)sender).SelectedItem = null;

    //        // Kích hoạt lại isSelectionEnabled sau khi xử lý
    //        isUserSelection = true;
    //    }
    //}

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

    private void OnAddAccountDialogPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var accountName = AccountNameInput.Text;
        var accountType = AccountTypeInput.Text;
        var currency = CurrencyInput.Text;
        if (decimal.TryParse(InitialBalanceInput.Text, out decimal initialBalance))
        {
            var newAccount = new Account
            {
                AccountName = accountName,
                AccountType = accountType,
                InitialBalance = initialBalance,
                CurrentBalance = initialBalance,
                Currency = currency,
                UserId = 1,
                CreateAt = DateTime.Now,
            };

            ViewModel.AddAccount(newAccount);

            ViewModel.SelectedAccount = null;
        }
    }

}
