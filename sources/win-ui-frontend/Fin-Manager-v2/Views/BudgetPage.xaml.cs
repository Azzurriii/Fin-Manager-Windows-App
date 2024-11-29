using Fin_Manager_v2.ViewModels;

using Microsoft.UI.Xaml.Controls;

using Microsoft.UI.Xaml;

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
        SaveBudgetButton.Click += SaveBudget;

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

    private async void SaveBudget(object sender, RoutedEventArgs e)
    {
        await ViewModel.SaveBudget();
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
