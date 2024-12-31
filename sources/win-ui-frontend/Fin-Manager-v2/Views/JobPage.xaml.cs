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

    private string _dialogTitle = "Add a new job";
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
                Title = "Error",
                Content = "Failed to load job list. Please try again later.",
                CloseButtonText = "Close",
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

    public async void OnNotifyJobClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var job = button.DataContext as JobModel;

        if (job != null)
        {
            await ShowMailerDialog(job);
        }
    }

    private async Task ShowMailerDialog(JobModel job)
    {
        // Lấy UserId từ LocalSettings
        //var userId = ;
        //var userEmail = GetUserEmail();
        var xamlRoot = this.XamlRoot;
        var dialog = new ContentDialog
        {
            XamlRoot = xamlRoot,
            Title = "Create Mailer Notification",
            Content = new StackPanel
            {
                Spacing = 10,
                Children =
            {
                new TextBox
                {
                    Header = "Title",
                    Text = job.JobName,
                    Name = "TitleTextBox"
                },
                new TextBox
                {
                    Header = "Description",
                    Text = job.Description ?? "",
                    Name = "DescriptionTextBox"
                },
                new NumberBox
                {
                    Header = "Amount",
                    Value = (double)job.Amount,
                    Name = "AmountNumberBox",
                    SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Inline
                },
                new ComboBox
                {
                    Header = "Period",
                    Items = { "monthly", "quarterly", "yearly" },
                    SelectedItem = job.RecurringType?.ToLower() ?? "monthly",
                    Name = "PeriodComboBox"
                },
                new CalendarDatePicker
                {
                    Header = "Start Date",
                    Date = DateTimeOffset.Now,
                    Name = "StartDatePicker"
                },
                new TextBox
                {
                    Header = "Payment Link (Optional)",
                    PlaceholderText = "https://example.com/payment",
                    Name = "PaymentLinkTextBox"
                }
            }
            },
            PrimaryButtonText = "Create Mailer",
            CloseButtonText = "Cancel"
        };

        dialog.PrimaryButtonClick += async (s, e) =>
        {
            var contentDialog = s as ContentDialog;

            // Lấy các giá trị từ form
            var titleBox = contentDialog.FindName("TitleTextBox") as TextBox;
            var descriptionBox = contentDialog.FindName("DescriptionTextBox") as TextBox;
            var amountBox = contentDialog.FindName("AmountNumberBox") as NumberBox;
            var periodCombo = contentDialog.FindName("PeriodComboBox") as ComboBox;
            var startDatePicker = contentDialog.FindName("StartDatePicker") as CalendarDatePicker;
            var paymentLinkBox = contentDialog.FindName("PaymentLinkTextBox") as TextBox;

            // Validate
            if (string.IsNullOrWhiteSpace(titleBox.Text) ||
                periodCombo.SelectedItem == null ||
                !startDatePicker.Date.HasValue)
            {
                await new ContentDialog
                {
                    Title = "Validation Error",
                    Content = "Please fill in all required fields",
                    PrimaryButtonText = "OK"
                }.ShowAsync();
                return;
            }

            // Tạo DTO
            var mailerDto = new CreateMailerDto
            {
                UserId = 0,
                UserEmail = "",
                Title = titleBox.Text,
                Description = descriptionBox.Text,
                Amount = amountBox.Value,
                Period = periodCombo.SelectedItem.ToString(),
                StartDate = startDatePicker.Date.Value.DateTime,
                PaymentLink = string.IsNullOrWhiteSpace(paymentLinkBox.Text) ? null : paymentLinkBox.Text
            };

            // Gọi service tạo mailer
            try
            {
                await ViewModel.CreateMailerAsync(mailerDto);

                // Thông báo thành công
                //await new ContentDialog
                //{
                //    Title = "Success",
                //    Content = "Mailer notification created successfully",
                //    PrimaryButtonText = "OK"
                //}.ShowAsync();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                await new ContentDialog
                {
                    Title = "Error",
                    Content = $"Failed to create mailer: {ex.Message}",
                    PrimaryButtonText = "OK"
                }.ShowAsync();
            }
        };

        await dialog.ShowAsync();
    }

    private async void OnDeleteJobClick(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var job = button?.DataContext as JobModel;

        if (job != null)
        {
            var dialog = new ContentDialog
            {
                Title = "Delete Job",
                Content = $"Are you sure you want to delete the job \"{job.JobName}\"?",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
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

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ViewModel.SelectedJob != null)
        {
            Frame.Navigate(typeof(JobDetailPage));
        }
    }

    private void OnJobItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is JobModel job)
        {
            Frame.Navigate(typeof(JobDetailPage), job);
        }
    }
}