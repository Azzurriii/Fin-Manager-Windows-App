using Fin_Manager_v2.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace Fin_Manager_v2.Views
{
    public sealed partial class CurrencyPage : Page
    {
        public CurrencyViewModel ViewModel { get; }

        public CurrencyPage()
        {
            try
            {
                ViewModel = App.GetService<CurrencyViewModel>();
                this.InitializeComponent();
                DataContext = ViewModel;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing CurrencyPage: {ex.Message}");
                throw;
            }
        }
    }
}
