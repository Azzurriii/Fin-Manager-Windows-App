using Microsoft.UI.Xaml.Controls;
using Fin_Manager_v2.DTO;
using System;
using Fin_Manager_v2.Contracts.Services;
namespace Fin_Manager_v2.Controls;

public sealed partial class JobDialog : ContentDialog
{
    public JobDialog()
    {
        InitializeComponent();
        ScheduleDatePicker.Date = DateTime.Now;
    }

    private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(JobNameBox.Text))
        {
            ErrorTextBlock.Text = "Job name is required";
            ErrorTextBlock.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            args.Cancel = true;
            return;
        }

        if (!decimal.TryParse(AmountBox.Text, out decimal amount) || amount <= 0)
        {
            ErrorTextBlock.Text = "Please enter a valid amount";
            ErrorTextBlock.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            args.Cancel = true;
            return;
        }

        if (RecurringTypeComboBox.SelectedItem == null)
        {
            ErrorTextBlock.Text = "Please select a recurring type";
            ErrorTextBlock.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            args.Cancel = true;
            return;
        }

        if (TransactionTypeComboBox.SelectedItem == null)
        {
            ErrorTextBlock.Text = "Please select a transaction type";
            ErrorTextBlock.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            args.Cancel = true;
            return;
        }
    }

    public CreateJobDto GetJobDto()
    {
        return new CreateJobDto
        {
            JobName = JobNameBox.Text,
            Description = DescriptionBox.Text,
            Amount = decimal.Parse(AmountBox.Text),
            ScheduleDate = ScheduleDatePicker.Date.DateTime,
            RecurringType = (RecurringTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty,
            TransactionType = (TransactionTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? string.Empty,
            UserId = App.GetService<IAuthService>().GetUserId() ?? 0 
        };
    }
}
