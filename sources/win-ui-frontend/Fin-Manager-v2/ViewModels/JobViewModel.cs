using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Fin_Manager_v2.ViewModels;

public partial class JobViewModel : ObservableRecipient
{
    private readonly IJobService _jobService;
    private readonly IDialogService _dialogService;
    private readonly IAuthService _authService;
    private readonly IMailerService _mailerService;

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

    public JobViewModel(IJobService jobService, IDialogService dialogService, IAuthService authService, IMailerService mailerService)
    {
        _jobService = jobService;
        _dialogService = dialogService;
        _authService = authService;
        _mailerService = mailerService;
        _jobs = new ObservableCollection<JobModel>();
        IsLoading = false;
        HasError = false;
        IsInitialized = false;
    }

    /// <summary>
    /// Initializes the application asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the user is not logged in.</exception>
    /// <exception cref="Exception">Thrown when an error occurs during initialization.</exception>
    public async Task InitializeAsync()
    {
        if (IsInitialized)
            return;

        try
        {
            IsLoading = true;
            
            // Check token
            var token = _authService.GetAccessToken();
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("Not logged in");
            }

            await LoadJobsAsync();
            IsInitialized = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Initialize Error: {ex.Message}");
            HasError = true;
            ErrorMessage = "Cannot initialize: " + ex.Message;
            await _dialogService.ShowErrorAsync("Error", ErrorMessage);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Asynchronously loads jobs from the job service and updates the collection of jobs.
    /// </summary>
    /// <exception cref="Exception">Thrown when an error occurs during the loading process.</exception>
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

    /// <summary>
    /// Asynchronously adds a new job using the provided job data transfer object.
    /// </summary>
    /// <param name="jobDto">The data transfer object containing information about the job to be added.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when an error occurs while adding the job.</exception>
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
                ErrorMessage = "Cannot add new job";
                await _dialogService.ShowErrorAsync("Error", ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Cannot add new job";
            await _dialogService.ShowErrorAsync("Error", ErrorMessage);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Asynchronously deletes a job from the system.
    /// </summary>
    /// <param name="job">The job model to be deleted.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when an error occurs during the deletion process.</exception>
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

    /// <summary>
    /// Asynchronously updates a job using the provided job data transfer object.
    /// </summary>
    /// <param name="jobDto">The data transfer object containing information about the job to be updated.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Thrown when an error occurs while updating the job.</exception>
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
                await _dialogService.ShowSuccessAsync("Success", "Update job successfully");
            }
            else
            {
                HasError = true;
                ErrorMessage = "Cannot update job";
                await _dialogService.ShowErrorAsync("Error", ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = "Cannot update job";
            await _dialogService.ShowErrorAsync("Error", ErrorMessage);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Validates the properties of an UpdateJobDto object.
    /// </summary>
    /// <param name="jobDto">The UpdateJobDto object to validate.</param>
    /// <returns>True if the UpdateJobDto object is valid; otherwise, false.</returns>
    private bool ValidateJobDto(UpdateJobDto jobDto)
    {
        if (string.IsNullOrWhiteSpace(jobDto.JobName))
        {
            SetError("Error", "Please enter the job name");
            return false;
        }

        if (jobDto.Amount <= 0)
        {
            SetError("Error", "Amount must be greater than 0");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates the provided CreateJobDto object.
    /// </summary>
    /// <param name="jobDto">The CreateJobDto object to be validated.</param>
    /// <returns>True if the object is valid, false otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown when the job name is null or whitespace, or the amount is less than or equal to 0.</exception>
    private bool ValidateJobDto(CreateJobDto jobDto){
        if (string.IsNullOrWhiteSpace(jobDto.JobName))
        {
            SetError("Error", "Please enter the job name");
            return false;
        }

        if (jobDto.Amount <= 0)
        {
            SetError("Error", "Amount must be greater than 0");
            return false;
        }

        return true;
    }

    /// <summary>Sets an error message.</summary>
    /// <param name="title">The title of the error message.</param>
    /// <param name="message">The content of the error message.</param>
    private void SetError(string title, string message)
    {
        HasError = true;
        ErrorMessage = $"{title}: {message}";
    }

    /// <summary>
    /// Determines the visibility of a collection based on its content.
    /// </summary>
    /// <param name="jobs">The ObservableCollection to check for content.</param>
    /// <returns>Visibility.Visible if the collection is null or empty, otherwise Visibility.Collapsed.</returns>
    public Visibility CollectionVisibility(ObservableCollection<JobModel> jobs)
    {
        return (jobs == null || jobs.Count == 0) ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// Determines the visibility of a collection based on its content.
    /// </summary>
    /// <param name="jobs">The ObservableCollection to check for content.</param>
    /// <returns>Visibility.Visible if the collection is null or empty, otherwise Visibility.Collapsed.</returns>
    public Visibility InverseCollectionVisibility(ObservableCollection<JobModel> jobs)
    {
        return (jobs == null || jobs.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
    }

    public async Task ShowMailerDialogAsync(JobModel job, XamlRoot xamlRoot)
    {
        try
        {
            var mailerViewModel = App.GetService<MailerViewModel>();
            mailerViewModel.Initialize(job);

            var dialog = new MailerDialog()
            {
                XamlRoot = xamlRoot,
                DataContext = mailerViewModel
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var success = await mailerViewModel.SendMailAsync();
                if (success)
                {
                    await _dialogService.ShowSuccessAsync("Success", "Email reminder setup successfully");
                }
                else
                {
                    await _dialogService.ShowErrorAsync("Error", mailerViewModel.ErrorMessage);
                }
            }
            else if (result == ContentDialogResult.Secondary)
            {
                var success = await mailerViewModel.DeleteMailAsync();
                if (success)
                {
                    await _dialogService.ShowSuccessAsync("Success", "Email reminder deleted successfully");
                }
                else
                {
                    await _dialogService.ShowErrorAsync("Error", mailerViewModel.ErrorMessage);
                }
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync("Error", "Failed to process email reminder");
        }
    }

    public string GetMailerTooltip(bool hasMailer)
    {
        return hasMailer ? "Email Reminder Active" : "Setup Email Reminder";
    }
}
