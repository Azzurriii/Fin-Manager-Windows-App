using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.ViewModels;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.ComponentModel;
using Windows.System;

namespace Fin_Manager_v2.Views;

public sealed partial class FinancialGoalPage : Page
{
    private int _dialogOpenAttempts = 0;
    private readonly SemaphoreSlim _dialogSemaphore = new SemaphoreSlim(1, 1);
    public FinancialGoalViewModel ViewModel
    {
        get;
    }

    public FinancialGoalPage()
    {
        ViewModel = App.GetService<FinancialGoalViewModel>();

        if (ViewModel == null)
        {
            ViewModel = new FinancialGoalViewModel(
                App.GetService<IFinancialGoalService>()
            );
        }

        InitializeComponent();
        DataContext = ViewModel;

        ViewModel.PropertyChanged += ViewModel_PropertyChanged;

        System.Diagnostics.Debug.WriteLine("FinancialGoalPage Constructor Called");
        System.Diagnostics.Debug.WriteLine($"ViewModel is null: {ViewModel == null}");

        AddGoalDialog.XamlRoot = this.XamlRoot;
    }

    //private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    //{
    //    if (e.PropertyName == nameof(ViewModel.IsAddGoalDialogOpen) && ViewModel.IsAddGoalDialogOpen)
    //    {
    //        // Chỉ cho phép một dialog được mở tại một thời điểm
    //        await _dialogSemaphore.WaitAsync();
    //        try
    //        {
    //            System.Diagnostics.Debug.WriteLine("Attempting to show dialog");
    //            await ShowAddGoalDialogAsync();
    //        }
    //        finally
    //        {
    //            _dialogSemaphore.Release();
    //            ViewModel.IsAddGoalDialogOpen = false;
    //        }
    //    }
    //}

    private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.IsAddGoalDialogOpen))
        {
            if (ViewModel.IsAddGoalDialogOpen)
            {
                // Chỉ cho phép một dialog được mở tại một thời điểm
                await _dialogSemaphore.WaitAsync();
                try
                {
                    System.Diagnostics.Debug.WriteLine("Attempting to show dialog");
                    await ShowAddGoalDialogAsync();
                }
                finally
                {
                    _dialogSemaphore.Release();
                }
            }
            else
            {
                // Đóng dialog khi IsAddGoalDialogOpen được đặt thành false
                if (AddGoalDialog != null)
                {
                    AddGoalDialog.Hide();
                }
            }
        }
    }

    private async Task ShowAddGoalDialogAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("ShowAddGoalDialogAsync called");

            //AddGoalDialog.XamlRoot = this.XamlRoot;

            if (AddGoalDialog.XamlRoot == null)
            {
                AddGoalDialog.XamlRoot = this.XamlRoot;
            }

            //var result = await AddGoalDialog.ShowAsync();

            //System.Diagnostics.Debug.WriteLine($"Dialog result: {result}");

            //ViewModel.IsAddGoalDialogOpen = false;
            //_dialogOpenAttempts = 0;

            if (AddGoalDialog != null && AddGoalDialog.XamlRoot != null)
            {
                var result = await AddGoalDialog.ShowAsync();
                ViewModel.IsAddGoalDialogOpen = false;
                _dialogOpenAttempts = 0;
            }
        }
        catch (Exception ex)
        {

        }
    }



    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        //ViewModel.LoadFinancialGoalsCommand.Execute(null);
    }

    private async void AddGoalDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        args.Cancel = true;

        // Validate Goal Name
        //if (string.IsNullOrWhiteSpace(ViewModel.NewGoal.GoalName))
        //{
        //    await ShowErrorDialog("Goal Name is required");
        //    return;
        //}

        //// Validate Target Amount
        //if (ViewModel.NewGoal.TargetAmount <= 0)
        //{
        //    await ShowErrorDialog("Target Amount must be greater than zero");
        //    return;
        //}

        //// Validate Saved Amount
        //if (ViewModel.NewGoal.SavedAmount < 0)
        //{
        //    await ShowErrorDialog("Saved Amount cannot be negative");
        //    return;
        //}

        //// Validate Deadline
        //if (ViewModel.NewGoal.Deadline.HasValue && ViewModel.NewGoal.Deadline.Value < DateTime.Now)
        //{
        //    await ShowErrorDialog("Deadline cannot be in the past");
        //    return;
        //}

        // Save the goal
        await ViewModel.SaveNewGoalCommand.ExecuteAsync(null);
        args.Cancel = false;
    }

    private async void DeleteGoal_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is FinancialGoalModel goal)
        {
            await ViewModel.DeleteGoalCommand.ExecuteAsync(goal);
        }
    }

    private async Task ShowErrorDialog(string message)
    {
        ContentDialog errorDialog = new ContentDialog
        {
            Title = "Error",
            Content = message,
            CloseButtonText = "OK"
        };

        await errorDialog.ShowAsync();
    }

    private void TargetAmountTextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
    {
        args.Cancel = args.NewText.Any(c => !char.IsDigit(c) && c != '.');
    }

    private void SavedAmountTextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
    {
        args.Cancel = args.NewText.Any(c => !char.IsDigit(c) && c != '.');
    }

    private void GoalNameTextBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            // Prevent new line in single-line TextBox
            e.Handled = true;
        }
    }

    private void DeadlineDatePicker_DateChanged(DatePicker sender, DatePickerValueChangedEventArgs args)
    {
        // Ensure deadline is not in the past
        if (sender.Date < DateTimeOffset.Now.Date)
        {
            sender.Date = DateTimeOffset.Now.Date;
        }
    }

    private void CancelAddGoal_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        ViewModel.CancelAddGoalCommand.Execute(null);
    }
}
