using Fin_Manager_v2.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Fin_Manager_v2.Views;

public sealed partial class AnalysisPage : Page
{
    public AnalysisViewModel ViewModel
    {
        get;
    }

    public AnalysisPage()
    {
        ViewModel = App.GetService<AnalysisViewModel>();
        InitializeComponent();
    }
}
