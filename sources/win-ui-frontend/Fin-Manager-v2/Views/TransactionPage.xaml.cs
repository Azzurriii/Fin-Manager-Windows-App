using Fin_Manager_v2.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Fin_Manager_v2.Views
{
    public sealed partial class TransactionPage : Page
    {
        public TransactionViewModel ViewModel { get; }

        public TransactionPage()
        {
            ViewModel = App.GetService<TransactionViewModel>();
            InitializeComponent();
            Loaded += TransactionPage_Loaded;
        }

        private async void TransactionPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Load transactions when the page is loaded
            if (ViewModel.LoadTransactionsCommand.CanExecute(null))
            {
                await ViewModel.LoadTransactionsCommand.ExecuteAsync(null);
            }
        }

        private void AddTransactionDialog_Closing(TeachingTip sender, TeachingTipClosingEventArgs args)
        {
            // Close the dialog and reset necessary properties if required
            ViewModel.IsAddTransactionDialogOpen = false;
        }
    }
}