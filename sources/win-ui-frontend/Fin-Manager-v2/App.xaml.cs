using System.Net.Http;

using Fin_Manager_v2.Activation;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Core.Contracts.Services;
using Fin_Manager_v2.Core.Services;
using Fin_Manager_v2.Helpers;
using Fin_Manager_v2.Services;
using Fin_Manager_v2.ViewModels;
using Fin_Manager_v2.Views;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

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
            services.AddSingleton<IApiConfiguration, ApiConfiguration>();
            services.AddSingleton<ITagService, TagService>();
            services.AddSingleton<ITransactionService, TransactionService>();
            services.AddSingleton<IAccountService, AccountService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<HttpClient>();
            services.AddSingleton<IReportService, ReportService>();

            // HTTP and Currency Services
            services.AddHttpClient<ITransactionService, TransactionService>((provider, client) =>
            {
                var config = provider.GetRequiredService<IApiConfiguration>();
                client.BaseAddress = new Uri(config.BaseUrl);
            });

            services.AddHttpClient<ITagService, TagService>((provider, client) =>
            {
                var config = provider.GetRequiredService<IApiConfiguration>();
                client.BaseAddress = new Uri(config.BaseUrl);
            });

            services.AddHttpClient<IAccountService, AccountService>((provider, client) =>
            {
                var config = provider.GetRequiredService<IApiConfiguration>();
                client.BaseAddress = new Uri(config.BaseUrl);
            });

            services.AddHttpClient<ICurrencyService, CurrencyService>((provider, client) =>
            {
                var config = provider.GetRequiredService<IApiConfiguration>();
                client.BaseAddress = new Uri(config.BaseUrl);
            });

            services.AddHttpClient<IAuthService, AuthService>((provider, client) =>
            {
                var config = provider.GetRequiredService<IApiConfiguration>();
                client.BaseAddress = new Uri(config.BaseUrl);
            });
            services.AddHttpClient<IReportService, ReportService>((provider, client) =>
            {
                var config = provider.GetRequiredService<IApiConfiguration>();
                client.BaseAddress = new Uri(config.BaseUrl);
            });

            // Views and ViewModels
            services.AddTransient<ReportViewModel>();
            services.AddTransient<ReportPage>();
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
