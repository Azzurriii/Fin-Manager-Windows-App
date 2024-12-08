using CommunityToolkit.Mvvm.ComponentModel;
using Fin_Manager_v2.Contracts.Services;
using Microsoft.UI.Xaml.Controls;
using Fin_Manager_v2;
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

    /// <summary>
    /// Asynchronously attempts to log in with the provided username and password.
    /// </summary>
    /// <param name="username">The username to log in with.</param>
    /// <param name="password">The password to log in with.</param>
    /// <returns>An asynchronous Task representing the login operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the username or password is empty.</exception>
    /// <exception cref="Exception">Thrown when an error occurs during the login process.</exception>
    public async Task LoginAsync(string username, string password)
    {
        try
        {
            // Check if the username or password is empty
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await _dialogService.ShowErrorAsync(
                    "Login Failed",
                    "Username and password cannot be empty.");
                return;
            }

            // Attempt to log in with the provided credentials
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

    /// <summary>
    /// Navigates to the sign-up page.
    /// </summary>
    public void NavigateToSignUp()
    {
        _navigationService.NavigateTo(typeof(SignUpViewModel).FullName!);
    }
}