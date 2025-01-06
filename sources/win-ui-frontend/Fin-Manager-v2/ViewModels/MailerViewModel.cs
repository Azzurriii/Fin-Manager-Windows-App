using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.DTO;
using System.Text.RegularExpressions;

namespace Fin_Manager_v2.ViewModels;

public partial class MailerViewModel : ObservableRecipient
{
    private readonly IMailerService _mailerService;
    private static readonly Regex EmailRegex = new Regex(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    [ObservableProperty]
    private string _email;

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _description;

    [ObservableProperty]
    private string _paymentLink;

    [ObservableProperty]
    private string _errorMessage;

    [ObservableProperty]
    private bool _hasError;

    private JobModel _job;

    public MailerViewModel(IMailerService mailerService)
    {
        _mailerService = mailerService;
    }

    public void Initialize(JobModel job)
    {
        _job = job;
        Title = job.JobName;
        Description = job.Description;
    }

    private bool ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            ErrorMessage = "Please enter your email";
            HasError = true;
            return false;
        }

        if (!EmailRegex.IsMatch(email))
        {
            ErrorMessage = "Please enter a valid email address";
            HasError = true;
            return false;
        }

        return true;
    }

    public async Task<bool> SendMailAsync()
    {
        try
        {
            if (!ValidateEmail(Email))
            {
                return false;
            }

            var mailerDto = new MailerDto
            {
                UserId = _job.JobId,
                UserEmail = Email,
                Title = Title,
                Description = Description,
                Amount = _job.Amount,
                Period = _job.RecurringType.ToString().ToLower(),
                StartDate = _job.ScheduleDate.DateTime,
                PaymentLink = PaymentLink
            };

            return await _mailerService.CreateMailerAsync(mailerDto);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to setup email reminder";
            HasError = true;
            return false;
        }
    }

    public async Task<bool> DeleteMailAsync()
    {
        try
        {
            return await _mailerService.DeleteMailerAsync(_job.JobId);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to delete email reminder";
            HasError = true;
            return false;
        }
    }
} 