using CommunityToolkit.Mvvm.ComponentModel;

using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.ViewModels;
using Fin_Manager_v2.Views;

using Microsoft.UI.Xaml.Controls;

namespace Fin_Manager_v2.Services;

public class PageService : IPageService
{
    private readonly Dictionary<string, Type> _pages = new();

    public PageService()
    {
        Configure<AccountViewModel, AccountPage>();
        Configure<TransactionViewModel, TransactionPage>();
        Configure<CurrencyViewModel, CurrencyPage>();
        Configure<MonthlyViewViewModel, MonthlyViewPage>();
        Configure<ShellViewModel, ShellPage>();
        Configure<LoginViewModel, LoginPage>();
        Configure<SignUpViewModel, SignUpPage>();
        Configure<ReportViewModel, ReportPage>();
        Configure<BudgetViewModel, BudgetPage>();
        Configure<JobViewModel, JobPage>();
        Configure<FinancialGoalViewModel, FinancialGoalPage>();
    }

    public Type GetPageType(string key)
    {
        Type? pageType;
        lock (_pages)
        {
            if (!_pages.TryGetValue(key, out pageType))
            {
                throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
            }
        }

        return pageType;
    }

   public void Configure<VM, V>()
        where VM : ObservableObject
        where V : Page
    {
        lock (_pages)
        {
            var key = typeof(VM).FullName!;
            if (_pages.ContainsKey(key))
            {
                throw new ArgumentException($"The key {key} is already configured in PageService");
            }

            var type = typeof(V);
            if (_pages.ContainsValue(type))
            {
                throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == type).Key}");
            }

            _pages.Add(key, type);
        }
    }
}
