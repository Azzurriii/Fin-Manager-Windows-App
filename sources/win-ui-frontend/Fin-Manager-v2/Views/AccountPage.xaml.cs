using Fin_Manager_v2.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.Views;

public sealed partial class AccountPage : Page
{
    private bool isUserSelection = false;

    // Make ViewModel read-only since we'll set it once in constructor
    public AccountViewModel ViewModel { get; }

    public AccountPage()
    {
        // Get ViewModel from DI container
        ViewModel = App.GetService<AccountViewModel>();

        this.InitializeComponent();
        this.DataContext = ViewModel;
        this.Loaded += OnPageLoaded;
    }

    private void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        isUserSelection = true;
    }

    private void OnAccountSelected(object sender, SelectionChangedEventArgs e)
    {
        if (!isUserSelection) return;

        if (sender is ListView listView && listView.SelectedItem is Account selectedAccount)
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
        // Clear previous input values
        AccountNameInput.Text = string.Empty;
        AccountTypeInput.Text = string.Empty;
        InitialBalanceInput.Text = string.Empty;
        CurrencyInput.Text = string.Empty;

        await AddAccountDialog.ShowAsync();
    }

    private async void OnAddAccountDialogPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Defer the closing of the dialog until we validate
        var deferral = args.GetDeferral();

        try
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(AccountNameInput.Text))
            {
                await ShowErrorDialog("Please enter an account name.");
                args.Cancel = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(AccountTypeInput.Text))
            {
                await ShowErrorDialog("Please enter an account type.");
                args.Cancel = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(CurrencyInput.Text))
            {
                await ShowErrorDialog("Please enter a currency.");
                args.Cancel = true;
                return;
            }

            if (!decimal.TryParse(InitialBalanceInput.Text, out decimal initialBalance))
            {
                await ShowErrorDialog("Please enter a valid initial balance.");
                args.Cancel = true;
                return;
            }

            var newAccount = new CreateFinanceAccountDto
            {
                account_name = AccountNameInput.Text.Trim(),
                account_type = AccountTypeInput.Text.Trim(),
                initial_balance = initialBalance,
                current_balance = initialBalance,
                currency = CurrencyInput.Text.Trim(),
            };

            // Show loading indicator if you have one
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
                // Clear selection only if add was successful
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
            // Re-enable dialog buttons
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

        // Refresh accounts when navigating to this page
        _ = ViewModel.LoadAccountsAsync();
    }

    public Visibility CollectionVisibility(ICollection<Account> accounts)
    {
        return (accounts == null || !accounts.Any()) ? Visibility.Visible : Visibility.Collapsed;
    }

    public Visibility InverseCollectionVisibility(ICollection<Account> accounts)
    {
        return (accounts == null || !accounts.Any()) ? Visibility.Collapsed : Visibility.Visible;
    }
}