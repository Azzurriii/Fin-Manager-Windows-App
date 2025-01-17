using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Fin_Manager_v2.ViewModels;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Contracts.Services;

namespace Fin_Manager_v2.Views;

public sealed partial class JobDetailPage : Page
{
    public JobDetailViewModel ViewModel { get; private set; }
    private readonly IDialogService _dialogService;

    public JobDetailPage()
    {
        InitializeComponent();
        _dialogService = App.GetService<IDialogService>();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is JobModel job)
        {
            ViewModel = new JobDetailViewModel(
                App.GetService<IJobService>(),
                _dialogService);
            ViewModel.Initialize(job);
        }
    }

    private async void OnSaveButtonClick(object sender, RoutedEventArgs e)
    {
        var result = await _dialogService.ShowConfirmAsync(
            "Update Job",
            "Are you sure you want to save the changes?");

        if (result)
        {
            var success = await ViewModel.UpdateJobAsync();
            if (success)
            {
                Frame.GoBack();
            }
        }
    }

    private void OnBackButtonClick(object sender, RoutedEventArgs e)
    {
        Frame.GoBack();
    }

    private async void OnDeleteButtonClick(object sender, RoutedEventArgs e)
    {
        var result = await _dialogService.ShowConfirmAsync(
            "Delete Job",
            "Are you sure you want to delete this job? This action cannot be undone.");

        if (result)
        {
            await ViewModel.DeleteJobAsync();
            Frame.GoBack();
        }
    }
}