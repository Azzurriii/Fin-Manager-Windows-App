using Fin_Manager_v2.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Fin_Manager_v2.Views;

public sealed partial class CurrencyPage : Page
{
    public CurrencyViewModel ViewModel
    {
        get;
    }

    public CurrencyPage()
    {
        ViewModel = App.GetService<CurrencyViewModel>();
        InitializeComponent();
    }
}
