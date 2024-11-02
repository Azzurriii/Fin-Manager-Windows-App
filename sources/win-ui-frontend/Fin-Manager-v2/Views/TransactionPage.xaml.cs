﻿using Fin_Manager_v2.ViewModels;
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
    }
}