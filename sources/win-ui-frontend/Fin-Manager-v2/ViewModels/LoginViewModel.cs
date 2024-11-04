using CommunityToolkit.Mvvm.ComponentModel;
using Fin_Manager_v2.Contracts.Services;
using Microsoft.UI.Xaml.Controls;
using Fin_Manager_v2;
using Fin_Manager_v2.Services.Interface;
using Fin_Manager_v2.ViewModels;
using Fin_Manager_v2.Views;

namespace Fin_Manager_v2.ViewModels;

public class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    public LoginViewModel(IAuthService authService, INavigationService navigationService, IDialogService dialogService)
    {
        _authService = authService;
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    public async Task LoginAsync(string username, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await _dialogService.ShowErrorAsync(
                    "Login Failed",
                    "Username and password cannot be empty.");
                return;
            }

            var loginSuccessful = await _authService.LoginAsync(username, password);
            if (loginSuccessful)
            {
                await _authService.FetchUserIdAsync();
                var shell = App.GetService<ShellPage>();
                App.MainWindow.Content = shell;

                if (shell.FindName("NavigationFrame") is Frame shellFrame)
                {
                    _navigationService.Frame = shellFrame;
                    _navigationService.NavigateTo(typeof(AccountViewModel).FullName!);
                }

                await _dialogService.ShowSuccessAsync(
                    "Success",
                    "Login successful!");
            }
            else
            {
                await _dialogService.ShowErrorAsync(
                    "Login Failed",
                    "Invalid username or password.");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Login error: {ex}");
            await _dialogService.ShowErrorAsync(
                "Login Failed",
                $"An error occurred: {ex.Message}");
        }
    }

    public void NavigateToSignUp()
    {
        _navigationService.NavigateTo(typeof(SignUpViewModel).FullName!);
    }
}