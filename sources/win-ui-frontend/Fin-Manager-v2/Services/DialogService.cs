using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Fin_Manager_v2.Contracts.Services;

namespace Fin_Manager_v2.Services;

public class DialogService : IDialogService
{
    private async Task ShowDialog(string title, string message, string buttonText = "OK")
    {
        try
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = buttonText,
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = App.MainWindow.Content.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style
            };

            await dialog.ShowAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error showing dialog: {ex}");
        }
    }

    public async Task ShowErrorAsync(string title, string message)
    {
        await ShowDialog(title, message);
    }

    public async Task ShowWarningAsync(string title, string message)
    {
        await ShowDialog(title, message);
    }

    public async Task ShowSuccessAsync(string title, string message)
    {
        await ShowDialog(title, message);
    }

    public async Task<bool> ShowConfirmAsync(string title, string message)
    {
        try
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = App.MainWindow.Content.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style
            };

            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error showing confirm dialog: {ex}");
            return false;
        }
    }
}