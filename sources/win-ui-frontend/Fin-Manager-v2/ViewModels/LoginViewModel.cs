﻿using CommunityToolkit.Mvvm.ComponentModel;
using Fin_Manager_v2.Contracts.Services;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using Fin_Manager_v2;
using Microsoft.UI.Xaml;
using Fin_Manager_v2.Services.Interface;
using Fin_Manager_v2.ViewModels;
using Fin_Manager_v2.Views;


public class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private UIElement? _shell;

    public LoginViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
    }

    public async Task LoginAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ShowErrorDialog("Username and password cannot be empty.");
            return;
        }

        // Hardcoded login check
        if (username == "1" && password == "1")
        {
            _shell = App.GetService<ShellPage>();
            Fin_Manager_v2.App.MainWindow.Content = _shell ?? new Frame();
            return;
        }

        // Direct API call to AuthService for login
        var loginSuccessful = await _authService.LoginAsync(username, password);
        if (loginSuccessful)
        {
            _shell = App.GetService<ShellPage>();
            Fin_Manager_v2.App.MainWindow.Content = _shell ?? new Frame();
        }
        else
        {
            ShowErrorDialog("Invalid username or password.");
        }
    }

    private async void ShowErrorDialog(string message)
    {
        var dialog = new ContentDialog
        {
            Title = "Login Failed",
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = App.MainWindow.Content.XamlRoot
        };
        await dialog.ShowAsync();
    }

    public void NavigateToSignUp()
    {
        _navigationService.NavigateTo(typeof(SignUpViewModel).FullName!);
    }
}