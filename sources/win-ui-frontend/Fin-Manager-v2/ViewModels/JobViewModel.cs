using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Contracts.Services;
using Microsoft.UI.Xaml;

namespace Fin_Manager_v2.ViewModels;

public partial class JobViewModel : ObservableRecipient
{
    private readonly IJobService _jobService;
    private readonly IDialogService _dialogService;
    private readonly IAuthService _authService;

    [ObservableProperty]
    private ObservableCollection<JobModel> _jobs;

    [ObservableProperty]
    private JobModel _selectedJob;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string _errorMessage;

    [ObservableProperty]
    private bool _isInitialized;

    public JobViewModel(IJobService jobService, IDialogService dialogService, IAuthService authService)
    {
        _jobService = jobService;
        _dialogService = dialogService;
        _authService = authService;
        _jobs = new ObservableCollection<JobModel>();
        IsLoading = false;
        HasError = false;
        IsInitialized = false;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
            return;

        try
        {
            IsLoading = true;
            
            // Kiểm tra token
            var token = _authService.GetAccessToken();
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("Chưa đăng nhập");
            }

            await LoadJobsAsync();
            IsInitialized = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Initialize Error: {ex.Message}");
            HasError = true;
            ErrorMessage = "Không thể khởi tạo: " + ex.Message;
            await _dialogService.ShowErrorAsync("Lỗi khởi tạo", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task LoadJobsAsync()
    {
        try
        {
            var jobs = await _jobService.GetJobsAsync();
            
            // Debug
            System.Diagnostics.Debug.WriteLine($"Loaded {jobs.Count} jobs");
            
            Jobs.Clear();
            foreach (var job in jobs)
            {
                Jobs.Add(job);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadJobs Error: {ex.Message}");
            throw;
        }
    }

    public async Task AddJobAsync(CreateJobDto jobDto)
    {
        try
        {
            IsLoading = true;
            HasError = false;

            if (!ValidateJobDto(jobDto))
                return;

            var success = await _jobService.CreateJobAsync(jobDto);
            if (success)
            {
                await LoadJobsAsync();
            }
            else
            {
                HasError = true;
                ErrorMessage = "Không thể thêm công việc mới";
                await _dialogService.ShowErrorAsync("Lỗi", ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Không thể thêm công việc mới";
            await _dialogService.ShowErrorAsync("Lỗi", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task DeleteJobAsync(JobModel job)
    {
        try
        {
            IsLoading = true;
            HasError = false;

            var success = await _jobService.DeleteJobAsync(job.JobId);
            if (success)
            {
                Jobs.Remove(job);
            }
            else
            {
                HasError = true;
                ErrorMessage = "Không thể xóa công việc";
                await _dialogService.ShowErrorAsync("Lỗi", ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Không thể xóa công việc";
            await _dialogService.ShowErrorAsync("Lỗi", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task UpdateJobAsync(UpdateJobDto jobDto)
    {
        try
        {
            IsLoading = true;
            HasError = false;

            if (!ValidateJobDto(jobDto))
                return;

            var success = await _jobService.UpdateJobAsync(SelectedJob.JobId, jobDto);
            if (success)
            {
                await LoadJobsAsync();
                await _dialogService.ShowSuccessAsync("Thành công", "Cập nhật công việc thành công");
            }
            else
            {
                HasError = true;
                ErrorMessage = "Không thể cập nhật công việc";
                await _dialogService.ShowErrorAsync("Lỗi", ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Không thể cập nhật công việc";
            await _dialogService.ShowErrorAsync("Lỗi", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool ValidateJobDto(UpdateJobDto jobDto)
    {
        if (string.IsNullOrWhiteSpace(jobDto.JobName))
        {
            SetError("Lỗi", "Vui lòng nhập tên công việc");
            return false;
        }

        if (jobDto.Amount <= 0)
        {
            SetError("Lỗi", "Số tiền phải lớn hơn 0");
            return false;
        }

        return true;
    }

    private bool ValidateJobDto(CreateJobDto jobDto){
        if (string.IsNullOrWhiteSpace(jobDto.JobName))
        {
            SetError("Lỗi", "Vui lòng nhập tên công việc");
            return false;
        }

        if (jobDto.Amount <= 0)
        {
            SetError("Lỗi", "Số tiền phải lớn hơn 0");
            return false;
        }

        return true;
    }

    private void SetError(string title, string message)
    {
        HasError = true;
        ErrorMessage = $"{title}: {message}";
    }

    public Visibility CollectionVisibility(ObservableCollection<JobModel> jobs)
    {
        return (jobs == null || jobs.Count == 0) ? Visibility.Visible : Visibility.Collapsed;
    }

    public Visibility InverseCollectionVisibility(ObservableCollection<JobModel> jobs)
    {
        return (jobs == null || jobs.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
    }
}
