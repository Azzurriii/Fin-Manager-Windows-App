using Fin_Manager_v2.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Controls;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Contracts.Services;
using System.Collections.ObjectModel;

namespace Fin_Manager_v2.Views;

public sealed partial class JobPage : Page
{
    private bool isUserSelection = false;
    private JobModel _currentEditingJob;
    public JobViewModel ViewModel { get; }

    private string _dialogTitle = "Thêm công việc mới";
    public string DialogTitle
    {
        get => _dialogTitle;
        set
        {
            if (_dialogTitle != value)
            {
                _dialogTitle = value;
            }
        }
    }

    private Visibility _isUpdateMode = Visibility.Collapsed;
    public Visibility IsUpdateMode
    {
        get => _isUpdateMode;
        set
        {
            if (_isUpdateMode != value)
            {
                _isUpdateMode = value;
            }
        }
    }

    public JobPage()
    {
        ViewModel = App.GetService<JobViewModel>();
        InitializeComponent();
        this.DataContext = ViewModel;
        this.Loaded += JobPage_Loaded;
    }

    private async void JobPage_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            isUserSelection = true;
            await ViewModel.InitializeAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"JobPage_Loaded Error: {ex.Message}");
            // Hiển thị dialog lỗi
            var dialog = new ContentDialog
            {
                Title = "Lỗi",
                Content = "Không thể tải danh sách công việc. Vui lòng thử lại sau.",
                CloseButtonText = "Đóng",
                XamlRoot = XamlRoot
            };
            await dialog.ShowAsync();
        }
    }

    private void OnJobSelected(object sender, SelectionChangedEventArgs e)
    {
        if (!isUserSelection) return;

        if (sender is ListView listView && listView.SelectedItem is JobModel selectedJob)
        {
            ViewModel.SelectedJob = selectedJob;
            DispatcherQueue.TryEnqueue(() =>
            {
                Frame.Navigate(typeof(JobDetailPage), selectedJob);
            });
        }
    }

    private async void OnAddJobClick(object sender, RoutedEventArgs e)
    {
        var dialog = new JobDialog
        {
            XamlRoot = XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            var jobDto = dialog.GetJobDto();
            await ViewModel.AddJobAsync(jobDto);
        }
    }

    private async void OnDeleteJobClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var job = button?.DataContext as JobModel;

        if (job != null)
        {
            var dialog = new ContentDialog
            {
                Title = "Xóa công việc",
                Content = $"B��n có chắc chắn muốn xóa công việc \"{job.JobName}\" không?",
                PrimaryButtonText = "Xóa",
                CloseButtonText = "Hủy",
                XamlRoot = button.XamlRoot
            };

            dialog.PrimaryButtonClick += async (_, _) =>
            {
                await ViewModel.DeleteJobAsync(job);
            };

            await dialog.ShowAsync();
        }
    }

    private void OnViewJobClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var job = button?.DataContext as JobModel;
        if (job != null)
        {
            Frame.Navigate(typeof(JobDetailPage), job);
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (ViewModel.IsInitialized)
        {
            _ = ViewModel.LoadJobsAsync();
        }
    }

    private Visibility CollectionVisibility(ObservableCollection<JobModel> jobs)
    {
        return ViewModel.CollectionVisibility(jobs);
    }

    private Visibility InverseCollectionVisibility(ObservableCollection<JobModel> jobs)
    {
        return ViewModel.InverseCollectionVisibility(jobs);
    }
}