using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services.Interface;
using Fin_Manager_v2.ViewModels;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;

public partial class SignUpViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    public SignUpViewModel(
        IAuthService authService,
        INavigationService navigationService,
        IDialogService dialogService)
    {
        _authService = authService;
        _navigationService = navigationService;
        _dialogService = dialogService;
    }

    [RelayCommand]
    public async Task SignUpAsync()
    {
        try
        {
            if (!await ValidateInputs())
            {
                return;
            }

            var user = new UserModel
            {
                Username = Username,
                Email = Email,
                Password = Password
            };

            var response = await _authService.SignUpAsync(user);
            if (response.IsSuccessStatusCode)
            {
                await _dialogService.ShowSuccessAsync(
                    "Success",
                    "Sign up successful. Please login now.");

                _navigationService.NavigateTo(typeof(LoginViewModel).FullName!);
            }
            else
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                if (errorResponse != null && errorResponse.TryGetValue("message", out var messageObj) && messageObj is JsonElement messageElement && messageElement.ValueKind == JsonValueKind.Array)
                {
                    var messages = messageElement.EnumerateArray().Select(m => m.GetString()).Where(m => !string.IsNullOrEmpty(m));
                    var errorMessage = string.Join("\n", messages);
                    await _dialogService.ShowErrorAsync(
                        "Sign Up Failed",
                        errorMessage);
                }
                else
                {
                    await _dialogService.ShowErrorAsync(
                        "Sign Up Failed",
                        "Sign up failed. Please try again.");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Sign up error: {ex}");
            await _dialogService.ShowErrorAsync(
                "Sign Up Failed",
                $"An error occurred: {ex.Message}");
        }
    }

    private async Task<bool> ValidateInputs()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await _dialogService.ShowErrorAsync(
                    "Sign Up Failed",
                    "All fields are required.");
                return false;
            }

            if (!IsValidUsername(Username))
            {
                await _dialogService.ShowErrorAsync(
                    "Sign Up Failed",
                    "Username can only contain letters, numbers, and underscores.");
                return false;
            }

            if (!IsValidEmail(Email))
            {
                await _dialogService.ShowErrorAsync(
                    "Sign Up Failed",
                    "Please enter a valid email address.");
                return false;
            }

            if (Password.Length < 6)
            {
                await _dialogService.ShowErrorAsync(
                    "Sign Up Failed",
                    "Password must be at least 6 characters long.");
                return false;
            }

            if (Password != ConfirmPassword)
            {
                await _dialogService.ShowErrorAsync(
                    "Sign Up Failed",
                    "Passwords do not match.");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Validation error: {ex}");
            await _dialogService.ShowErrorAsync(
                "Sign Up Failed",
                $"An error occurred during validation: {ex.Message}");
            return false;
        }
    }

    private bool IsValidEmail(string email)
    {
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return emailRegex.IsMatch(email);
    }

    private bool IsValidUsername(string username)
    {
        var usernameRegex = new Regex(@"^[a-zA-Z0-9_]+$");
        return usernameRegex.IsMatch(username);
    }
}