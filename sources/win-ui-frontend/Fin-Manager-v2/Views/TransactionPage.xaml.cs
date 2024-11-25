using Fin_Manager_v2.Models;
using Fin_Manager_v2.ViewModels;
using Microsoft.UI.Xaml;
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
            try
            {
                await ViewModel.InitializeCommand.ExecuteAsync(null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in TransactionPage_Loaded: {ex.Message}");
            }
        }

        private void AddTransactionDialog_Closing(TeachingTip sender, TeachingTipClosingEventArgs args)
        {
            ViewModel.IsAddTransactionDialogOpen = false;
        }

        private void Transaction_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (sender is FrameworkElement element && 
                element.DataContext is TransactionModel transaction)
            {
                ViewModel.EditTransaction(transaction);
            }
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem && 
                menuItem.DataContext is TransactionModel transaction)
            {
                ViewModel.EditTransaction(transaction);
            }
        }

        private async void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuFlyoutItem menuItem && 
                menuItem.DataContext is TransactionModel transaction)
            {
                await ViewModel.DeleteTransactionCommand.ExecuteAsync(transaction);
            }
        }
    }
}