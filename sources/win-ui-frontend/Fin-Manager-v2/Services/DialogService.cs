using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Fin_Manager_v2.Contracts.Services;

namespace Fin_Manager_v2.Services;

public class DialogService : IDialogService
{
    /// <summary>Displays a dialog with the specified title and message.</summary>
    /// <param name="title">The title of the dialog.</param>
    /// <param name="message">The message to display in the dialog.</param>
    /// <param name="buttonText">The text to display on the button (default is "OK").</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>If an error occurs while showing the dialog, it will be caught and logged.</remarks>
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

    /// <summary>Displays a confirmation dialog with the specified title and message.</summary>
    /// <param name="title">The title of the confirmation dialog.</param>
    /// <param name="message">The message displayed in the confirmation dialog.</param>
    /// <returns>True if the user clicks "Yes", false if the user clicks "No" or an error occurs.</returns>
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