using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Fin_Manager_v2.Views
{
    public sealed partial class SignUpPage : Page
    {
        public SignUpViewModel ViewModel
        {
            get;
        }

        public SignUpPage()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<SignUpViewModel>();
        }

        private async void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Username = UsernameBox.Text;
            ViewModel.Email = EmailBox.Text;
            ViewModel.Password = PasswordBox.Password;
            ViewModel.ConfirmPassword = ConfirmPasswordBox.Password;
            await ViewModel.SignUpAsync();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var navigationService = App.GetService<INavigationService>();
            navigationService.NavigateTo(typeof(LoginViewModel).FullName!);
        }
    }
}