﻿using Fin_Manager_v2.Activation;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Core.Contracts.Services;
using Fin_Manager_v2.Core.Services;
using Fin_Manager_v2.Helpers;
using Fin_Manager_v2.Services;
using Fin_Manager_v2.Services.Interface;
using Fin_Manager_v2.ViewModels;
using Fin_Manager_v2.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Fin_Manager_v2;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar { get; set; }

    public IServiceProvider Services { get; private set; }

    public App()
    {
        Services = ConfigureServices();
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Services
            services.AddTransient<INavigationViewService, NavigationViewService>();
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IAuthService, AuthService>();

            // HTTP and Currency Services
            services.AddHttpClient<ICurrencyService, CurrencyService>();
            services.AddSingleton<HttpClient>();

            // Views and ViewModels
            services.AddTransient<MonthlyViewViewModel>();
            services.AddTransient<MonthlyViewPage>();
            services.AddTransient<CurrencyViewModel>();
            services.AddTransient<CurrencyPage>();
            services.AddTransient<TransactionViewModel>();
            services.AddTransient<TransactionPage>();
            services.AddTransient<AccountViewModel>();
            services.AddTransient<AccountPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<LoginPage>();
            services.AddTransient<SignUpViewModel>();
            services.AddTransient<SignUpPage>();
            services.AddSingleton<ITagService, TagService>();
            services.AddSingleton<ITransactionService, TransactionService>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IDialogService, DialogService>();
        }).
        Build();

        UnhandledException += App_UnhandledException;
    }
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddHttpClient<ICurrencyService, CurrencyService>();
        services.AddTransient<CurrencyViewModel>();
        services.AddTransient<CurrencyPage>();
        services.Configure<PageService>(options =>
        {
            options.Configure<CurrencyViewModel, CurrencyPage>();
        });
        return services.BuildServiceProvider();
    }


    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
        System.Diagnostics.Debug.WriteLine($"Unhandled exception: {e.Message}\n{e.Exception.StackTrace}");

    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        await App.GetService<IActivationService>().ActivateAsync(args);
    }
}
