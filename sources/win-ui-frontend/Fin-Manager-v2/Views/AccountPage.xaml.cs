using Fin_Manager_v2.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;
using System.Collections.ObjectModel;

namespace Fin_Manager_v2.Views;

public sealed partial class AccountPage : Page
{
    private bool isUserSelection = false;
    public AccountViewModel ViewModel { get; }

    public AccountPage()
    {
        ViewModel = App.GetService<AccountViewModel>();
        this.InitializeComponent();
        this.DataContext = ViewModel;
        this.Loaded += AccountPage_Loaded;
    }

    private async void AccountPage_Loaded(object sender, RoutedEventArgs e)
    {
        isUserSelection = true;
        await ViewModel.InitializeAsync();
    }

    private void OnAccountSelected(object sender, SelectionChangedEventArgs e)
    {
        if (!isUserSelection) return;

        if (sender is ListView listView && listView.SelectedItem is AccountModel selectedAccount)
        {
            ViewModel.SelectedAccount = selectedAccount;
            DispatcherQueue.TryEnqueue(() =>
            {
                Frame.Navigate(typeof(AccountDetailPage), selectedAccount);
            });
        }
    }

    private async void OnAddAccountClick(object sender, RoutedEventArgs e)
    {
        AccountNameInput.Text = string.Empty;
        AccountTypeInput.SelectedItem = null;
        InitialBalanceInput.Value = 0;
        CurrencyInput.SelectedItem = null;

        await AddAccountDialog.ShowAsync();
    }

    private async void OnAddAccountDialogPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();

        try
        {
            if (string.IsNullOrWhiteSpace(AccountNameInput.Text))
            {
                await ShowErrorDialog("Please enter an account name.");
                args.Cancel = true;
                return;
            }

            if (AccountTypeInput.SelectedItem == null)
            {
                await ShowErrorDialog("Please select an account type.");
                args.Cancel = true;
                return;
            }

            if (CurrencyInput.SelectedItem == null)
            {
                await ShowErrorDialog("Please select a currency.");
                args.Cancel = true;
                return;
            }

            var accountType = (AccountTypeInput.SelectedItem as ComboBoxItem)?.Content.ToString();
            var currency = (CurrencyInput.SelectedItem as ComboBoxItem)?.Content.ToString();
            var initialBalance = InitialBalanceInput.Value;

            var newAccount = new CreateFinanceAccountDto
            {
                account_name = AccountNameInput.Text.Trim(),
                account_type = accountType?.Trim(),
                initial_balance = (decimal)initialBalance,
                current_balance = (decimal)initialBalance,
                currency = currency?.Split('-')[0].Trim(),
            };

            sender.IsPrimaryButtonEnabled = false;
            sender.IsSecondaryButtonEnabled = false;

            await ViewModel.AddAccountAsync(newAccount);

            if (ViewModel.HasError)
            {
                await ShowErrorDialog(ViewModel.ErrorMessage);
                args.Cancel = true;
            }
            else
            {
                ViewModel.SelectedAccount = null;
            }
        }
        catch (Exception ex)
        {
            await ShowErrorDialog($"An unexpected error occurred: {ex.Message}");
            args.Cancel = true;
        }
        finally
        {
            sender.IsPrimaryButtonEnabled = true;
            sender.IsSecondaryButtonEnabled = true;
            deferral.Complete();
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

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (ViewModel.IsInitialized)
        {
            _ = ViewModel.LoadAccountsAsync();
        }
    }

    private Visibility CollectionVisibility(ObservableCollection<AccountModel> accounts)
    {
        return (accounts == null || accounts.Count == 0) ? Visibility.Visible : Visibility.Collapsed;
    }

    private Visibility InverseCollectionVisibility(ObservableCollection<AccountModel> accounts)
    {
        return (accounts == null || accounts.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
    }
}