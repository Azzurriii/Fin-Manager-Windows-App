using Fin_Manager_v2.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services;
using System.Collections.ObjectModel;

namespace Fin_Manager_v2.Views;

public sealed partial class AccountPage : Page
{
    private bool isUserSelection = false;

    private AccountModel _currentEditingAccount;

    public AccountViewModel ViewModel { get; }

    private string _dialogTitle = "Add New Account";
    public string DialogTitle
    {
        get => _dialogTitle;
        set
        {
            if (_dialogTitle != value)
            {
                _dialogTitle = value;
                //OnPropertyChanged(nameof(DialogTitle));
            }
        }
    }

    private Visibility _isUpdateMode = Visibility.Collapsed;
    public Visibility IsUpdateMode
    {
        get => _isUpdateMode;
        set
        {
            if (_isUpdateMode != value)
            {
                _isUpdateMode = value;
                //OnPropertyChanged(nameof(IsUpdateMode));
            }
        }
    }

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

        _currentEditingAccount = null;
        DialogTitle = "Add New Account";
        IsUpdateMode = Visibility.Collapsed;
        PopulateDialogFields(null);
        AddAccountDialog.ShowAsync();
    }

    private async void OnAddAccountDialogPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var accountName = AccountNameInput.Text;
        var accountTypeItem = AccountTypeInput.SelectedItem as ComboBoxItem;
        var accountType = accountTypeItem?.Content as string;
        var initialBalance = InitialBalanceInput.Value;
        var currencyItem = CurrencyInput.SelectedItem as ComboBoxItem;
        var currency = currencyItem?.Content as string;

        if (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(accountType) || string.IsNullOrEmpty(currency))
        {
            ErrorTextBlock.Visibility = Visibility.Visible;
            return;
        }

        if (_currentEditingAccount != null)
        {
            var account = new UpdateFinanceAccountDto
            {
                account_id = _currentEditingAccount.AccountId,
                account_name = accountName,
                account_type = accountType,
                initial_balance = (decimal)initialBalance,
                current_balance = _currentEditingAccount.CurrentBalance,
                currency = currency,
            };

            await ViewModel.UpdateAccountAsync(account);
        }
        else
        {
            var newAccount = new CreateFinanceAccountDto
            {
                account_name = accountName,
                account_type = accountType,
                initial_balance = (decimal)initialBalance,
                currency = currency,
                current_balance = (decimal)InitialBalanceInput.Value,
            };

            await ViewModel.AddAccountAsync(newAccount);
        }

        AddAccountDialog.Hide();
    }


    private async void OnDeleteAccountClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var account = button?.DataContext as AccountModel;

        if (account != null)
        {
            var dialog = new ContentDialog
            {
                Title = "Delete Account",
                Content = $"Are you sure you want to delete the account \"{account.AccountName}\"?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = button.XamlRoot
            };

            dialog.PrimaryButtonClick += async (_, _) =>
            {
                await ViewModel.DeleteAccountAsync(account);
            };

            await dialog.ShowAsync();
        }
    }

    private async void OnUpdateAccountClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var account = button?.DataContext as AccountModel;
        if (account != null)
        {
            _currentEditingAccount = account;
            DialogTitle = "Update Account";
            IsUpdateMode = Visibility.Visible;
            PopulateDialogFields(account);
            AddAccountDialog.ShowAsync();
        }
    }

    private void PopulateDialogFields(AccountModel? account)
    {
        if (account != null)
        {
            IsUpdateMode = Visibility.Visible;
            AccountNameInput.Text = account.AccountName;
            AccountTypeInput.SelectedItem = AccountTypeInput.Items
                .OfType<ComboBoxItem>()
                .FirstOrDefault(item => item.Content as string == account.AccountType);
            InitialBalanceInput.Value = (double)account.InitialBalance;
            CurrencyInput.SelectedItem = CurrencyInput.Items
                .OfType<ComboBoxItem>()
                .FirstOrDefault(item => item.Content as string == account.Currency);
            CurrentBalanceInput.Value = (double)account.CurrentBalance;
        }
        else
        {
            IsUpdateMode = Visibility.Collapsed;
            AccountNameInput.Text = string.Empty;
            AccountTypeInput.SelectedItem = null;
            InitialBalanceInput.Value = 0;
            CurrencyInput.SelectedItem = null;
            CurrentBalanceInput.Value = 0;
        }

        ErrorTextBlock.Visibility = Visibility.Collapsed;
    }


    private async Task ShowErrorDialog(string message)
    {
        var errorDialog = new DialogService();

        await errorDialog.ShowErrorAsync("Error", message);
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
        return ViewModel.CollectionVisibility(accounts);
    }

    private Visibility InverseCollectionVisibility(ObservableCollection<AccountModel> accounts)
    {
        return ViewModel.InverseCollectionVisibility(accounts);
    }
}