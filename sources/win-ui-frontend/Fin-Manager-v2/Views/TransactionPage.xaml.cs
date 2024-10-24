using Fin_Manager_v2.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Fin_Manager_v2.Views;

public sealed partial class TransactionPage : Page
{
    public TransactionViewModel ViewModel
    {
        get;
    }

    public TransactionPage()
    {
        ViewModel = App.GetService<TransactionViewModel>();
        InitializeComponent();
    }
}
