using FinManager.Helpers;
using Windows.UI.ViewManagement;
using Microsoft.UI.Xaml.Controls;
using FinManager.Views;
using Microsoft.UI.Xaml;
using FinManager.Contracts.Services;
namespace FinManager;

public sealed partial class MainWindow : WindowEx
{
    private Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue;

    private UISettings settings;

    public MainWindow()
    {
        InitializeComponent();

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        //Content = null;
        Title = "AppDisplayName".GetLocalized();

        // Theme change code picked from https://github.com/microsoft/WinUI-Gallery/pull/1239
        dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        settings = new UISettings();
        settings.ColorValuesChanged += Settings_ColorValuesChanged; // cannot use FrameworkElement.ActualThemeChanged event

        //MainFrame.Navigate(typeof(Views.AccountPage));
    }

    // this handles updating the caption button colors correctly when indows system theme is changed
    // while the app is open
    private void Settings_ColorValuesChanged(UISettings sender, object args)
    {
        // This calls comes off-thread, hence we will need to dispatch it to current app's thread
        dispatcherQueue.TryEnqueue(() =>
        {
            TitleBarHelper.ApplySystemThemeToCaptionButtons();
        });
    }

    private void OnAccountsButtonClick(object sender, RoutedEventArgs e)
    {
        if (MainFrame != null)
        {
            // Navigate to the AccountPage when button is clicked
            MainFrame.Navigate(typeof(AccountPage));
        }
        else
        {
            // Debugging output to check if the frame is null
            System.Diagnostics.Debug.WriteLine("MainFrame is null");
        }
        //var navigationService = App.GetService<INavigationService>();
        //navigationService.NavigateTo("AccountPage");
    }
}

