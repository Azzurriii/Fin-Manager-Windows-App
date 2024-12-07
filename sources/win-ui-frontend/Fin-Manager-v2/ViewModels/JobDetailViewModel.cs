using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.ViewModels;

public partial class JobDetailViewModel : ObservableRecipient
{
    private readonly IJobService _jobService;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private JobModel _job;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public JobDetailViewModel(
        IJobService jobService,
        IDialogService dialogService)
    {
        _jobService = jobService;
        _dialogService = dialogService;
    }

    public void Initialize(JobModel job)
    {
        Job = job;
    }

    public async Task DeleteJobAsync()
    {
        IsLoading = true;
        HasError = false;

        try
        {
            var success = await _jobService.DeleteJobAsync(Job.JobId);
            if (!success)
            {
                HasError = true;
                ErrorMessage = "Không thể xóa công việc";
                await _dialogService.ShowErrorAsync("Lỗi", "Không thể xóa công việc");
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Không thể xóa công việc";
            await _dialogService.ShowErrorAsync("Lỗi", "Không thể xóa công việc: " + ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }
}