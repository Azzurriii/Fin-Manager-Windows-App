using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Fin_Manager_v2.ViewModels;
using Fin_Manager_v2.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fin_Manager_v2.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountDetailPage : Page
    {
        public AccountDetailViewModel ViewModel { get; private set; }

        public AccountDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Receive the account object when navigating to this page
            if (e.Parameter is AccountModel account)
            {
                ViewModel = new AccountDetailViewModel(account);
                DataContext = ViewModel; // Make sure to set DataContext to ViewModel

                Console.WriteLine("Navigated to AccountDetailPage: " + account.AccountName);
            }
            base.OnNavigatedTo(e);
        }

        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
