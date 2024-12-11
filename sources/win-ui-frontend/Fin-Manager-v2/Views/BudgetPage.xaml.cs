using Fin_Manager_v2.ViewModels;

using Microsoft.UI.Xaml.Controls;

using Microsoft.UI.Xaml;
using Fin_Manager_v2.Models;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

namespace Fin_Manager_v2.Views;

public sealed partial class BudgetPage : Page
{
    public BudgetViewModel ViewModel
    {
        get;
    }

    public BudgetPage()
    {
        ViewModel = App.GetService<BudgetViewModel>();
        InitializeComponent();

        ShowAddBudgetButton.Click += ShowAddBudget;
        CancelAddBudgetButton.Click += CancelAddBudget;

        DataContext = ViewModel;
    }

    private void ShowAddBudget(object sender, RoutedEventArgs e)
    {
        ViewModel.ShowAddBudget();
    }

    private void CancelAddBudget(object sender, RoutedEventArgs e)
    {
        ViewModel.CancelAddBudget();
    }

    public async void DeleteBudget(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.CommandParameter is BudgetModel budgetToDelete)
        {
            var dialog = new ContentDialog
            {
                Title = "Confirm Deletion",
                Content = $"Are you sure you want to delete the budget '{budgetToDelete.Category}'?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                XamlRoot = this.Content.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await ViewModel.DeleteBudget(budgetToDelete);
            }
        }
    }

    private async void ShowSuccessDialog(string message)
    {
        var dialog = new ContentDialog
        {
            Title = "Success",
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.Content.XamlRoot
        };

        await dialog.ShowAsync();
    }
}
