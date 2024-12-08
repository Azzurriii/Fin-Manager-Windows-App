using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.DTO;

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
        Job = new JobModel
        {
            JobId = job.JobId,
            JobName = job.JobName,
            Description = job.Description,
            TagId = job.TagId,
            AccountId = job.AccountId,
            UserId = job.UserId,
            Amount = job.Amount,
            RecurringType = job.RecurringType,
            TransactionType = job.TransactionType,
            NextRunDate = DateTimeOffset.Parse(job.NextRunDate.DateTime.ToString("yyyy-MM-dd"))
        };
    }

    /// <summary>
    /// Update the job.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<bool> UpdateJobAsync()
    {
        IsLoading = true;
        HasError = false;

        try
        {
            var updateDto = new UpdateJobDto
            {
                JobName = Job.JobName,
                Description = Job.Description,
                TagId = Job.TagId,
                AccountId = Job.AccountId,
                UserId = Job.UserId,
                ScheduleDate = Job.NextRunDate.DateTime.ToString("yyyy-MM-dd"),
                Amount = Job.Amount,
                RecurringType = Job.RecurringType,
                TransactionType = Job.TransactionType
            };

            var success = await _jobService.UpdateJobAsync(Job.JobId, updateDto);
            if (!success)
            {
                HasError = true;
                ErrorMessage = "Cannot update job";
                await _dialogService.ShowErrorAsync("Error", ErrorMessage);
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Cannot update job";
            await _dialogService.ShowErrorAsync("Error", ErrorMessage);
            return false;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Delete the job.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
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
                ErrorMessage = "Cannot delete job";
                await _dialogService.ShowErrorAsync("Error", ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Cannot delete job";
            await _dialogService.ShowErrorAsync("Error", ErrorMessage);
        }
        finally
        {
            IsLoading = false;
        }
    }
}