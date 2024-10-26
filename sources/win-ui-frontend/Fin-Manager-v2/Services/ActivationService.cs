using Fin_Manager_v2.Activation;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Services.Interface;
using Fin_Manager_v2.Views;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Fin_Manager_v2.Services;

public class ActivationService : IActivationService
{
    private readonly ActivationHandler<LaunchActivatedEventArgs> _defaultHandler;
    private readonly IEnumerable<IActivationHandler> _activationHandlers;
    private readonly IAuthService _authService;
    private UIElement? _shell = null;

    public ActivationService(
        ActivationHandler<LaunchActivatedEventArgs> defaultHandler,
        IEnumerable<IActivationHandler> activationHandlers,
        IAuthService authService)
    {
        _defaultHandler = defaultHandler;
        _activationHandlers = activationHandlers;
        _authService = authService;
    }

    public async Task ActivateAsync(object activationArgs)
    {
        await InitializeAsync();

        if (App.MainWindow.Content == null)
        {
            if (_authService.IsAuthenticated)
            {
                _shell = App.GetService<ShellPage>();
                App.MainWindow.Content = _shell ?? new Frame();
            }
            else
            {
                var frame = new Frame();
                frame.Navigate(typeof(LoginPage));
                App.MainWindow.Content = frame;
            }
        }

        await HandleActivationAsync(activationArgs);
        App.MainWindow.Activate();
        await StartupAsync();
    }

    private async Task HandleActivationAsync(object activationArgs)
    {
        var activationHandler = _activationHandlers.FirstOrDefault(h => h.CanHandle(activationArgs));

        if (activationHandler != null)
        {
            await activationHandler.HandleAsync(activationArgs);
        }

        if (_defaultHandler.CanHandle(activationArgs))
        {
            await _defaultHandler.HandleAsync(activationArgs);
        }
    }

    private async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    private async Task StartupAsync()
    {
        await Task.CompletedTask;
    }
}
