using Fin_Manager_v2.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Fin_Manager_v2.Views;

public sealed partial class AccountPage : Page
{
    public AccountViewModel ViewModel
    {
        get;
    }

    public AccountPage()
    {
        ViewModel = App.GetService<AccountViewModel>();
        InitializeComponent();
    }
}
