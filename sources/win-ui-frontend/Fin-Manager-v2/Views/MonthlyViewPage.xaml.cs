using Fin_Manager_v2.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Fin_Manager_v2.Views;

public sealed partial class MonthlyViewPage : Page
{
    public MonthlyViewViewModel ViewModel
    {
        get;
    }

    public MonthlyViewPage()
    {
        ViewModel = App.GetService<MonthlyViewViewModel>();
        InitializeComponent();
    }
}
