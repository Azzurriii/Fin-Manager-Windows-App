using Microsoft.UI.Xaml.Controls;
using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.Controls;

public sealed partial class JobEditControl : UserControl
{
    public JobModel Job { get; }

    public JobEditControl(JobModel job)
    {
        Job = job;
        this.InitializeComponent();
    }

    private void OnTransactionTypeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (TransactionTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            Job.TransactionType = selectedItem.Content.ToString();
        }
    }

    private void OnRecurringTypeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RecurringTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            Job.RecurringType = selectedItem.Content.ToString();
        }
    }

    public bool ValidateInput()
    {
        bool isValid = true;
        ErrorTextBlock.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;

        if (string.IsNullOrWhiteSpace(Job.JobName))
        {
            isValid = false;
            ErrorTextBlock.Text = "Vui lòng nh�p tên công việc";
        }
        else if (Job.Amount <= 0)
        {
            isValid = false;
            ErrorTextBlock.Text = "Số tiền phải lớn hơn 0";
        }
        else if (string.IsNullOrWhiteSpace(Job.TransactionType))
        {
            isValid = false;
            ErrorTextBlock.Text = "Vui lòng chọn loại giao dịch";
        }
        else if (string.IsNullOrWhiteSpace(Job.RecurringType))
        {
            isValid = false;
            ErrorTextBlock.Text = "Vui lòng chọn loại lặp lại";
        }

        if (!isValid)
        {
            ErrorTextBlock.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        }

        return isValid;
    }
}