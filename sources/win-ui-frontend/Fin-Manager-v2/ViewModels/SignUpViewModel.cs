using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services.Interface;
using Microsoft.UI.Xaml.Controls;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fin_Manager_v2.ViewModels
{
    public partial class SignUpViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string confirmPassword;

        public SignUpViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
        }

        [RelayCommand]
        public async Task SignUpAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ShowErrorDialog("All fields are required.");
                return;
            }

            if (!IsValidEmail(Email))
            {
                ShowErrorDialog("Please enter a valid email address.");
                return;
            }

            if (Password != ConfirmPassword)
            {
                ShowErrorDialog("Passwords do not match.");
                return;
            }

            // Tạo đối tượng UserModel từ các thông tin nhập vào
            var user = new UserModel
            {
                Username = Username,
                Email = Email,
                Password = Password
            };

            var signUpSuccessful = await _authService.SignUpAsync(user);
            if (signUpSuccessful)
            {
                await ShowSuccessDialog("Sign up successful. Please login now.");
                _navigationService.NavigateTo(typeof(LoginViewModel).FullName!);
            }
            else
            {
                ShowErrorDialog("Sign up failed. Please try again.");
            }
        }

        private async void ShowErrorDialog(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Sign Up Error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = App.MainWindow.Content.XamlRoot
            };
            await dialog.ShowAsync();
        }

        private async Task ShowSuccessDialog(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Sign Up Successful.",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = App.MainWindow.Content.XamlRoot
            };
            await dialog.ShowAsync();
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }
    }
}
