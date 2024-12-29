﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services;
using System.Collections.ObjectModel;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.DTO;
using LiveChartsCore.SkiaSharpView.Extensions;
using LiveChartsCore;

namespace Fin_Manager_v2.ViewModels;

public partial class FinancialGoalViewModel : ObservableObject
{
    private readonly IFinancialGoalService _financialGoalService;

    [ObservableProperty]
    private ObservableCollection<FinancialGoalModel> _financialGoals;

    [ObservableProperty]
    private bool _isAddGoalDialogOpen;

    //[ObservableProperty]
    //private FinancialGoalModel _newGoal = new();

    [ObservableProperty]
    private CreateFinancialGoalDto _newGoal = new();

    //public IEnumerable<ISeries> Series { get; set; } =
    //    GaugeGenerator.BuildSolidGauge(
    //        new GaugeItem(
    //            30,          // the gauge value
    //            series =>    // the series style
    //            {
    //                series.MaxRadialColumnWidth = 50;
    //                series.DataLabelsSize = 50;
    //            }));

    //private void UpdateGoalProgressChart()
    //{
    //    var firstGoal = FinancialGoals.FirstOrDefault();
    //    if (firstGoal != null)
    //    {
    //        Series = GaugeGenerator.BuildSolidGauge(
    //            new GaugeItem(
    //                firstGoal.CompletionPercentage,
    //                series =>
    //                {
    //                    series.MaxRadialColumnWidth = 50;
    //                    series.DataLabelsSize = 50;
    //                }));
    //    }
    //}

    public FinancialGoalViewModel(IFinancialGoalService financialGoalService)
    {
        _financialGoalService = financialGoalService;
        FinancialGoals = new ObservableCollection<FinancialGoalModel>();
        //NewGoal = new FinancialGoalModel();
        NewGoal = new CreateFinancialGoalDto();
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await LoadFinancialGoalsAsync();
        //UpdateGoalProgressChart();
    }

    [RelayCommand]
    private async Task LoadFinancialGoalsAsync()
    {
        var goals = await _financialGoalService.GetAllFinancialGoalsAsync();
        FinancialGoals.Clear();
        foreach (var goal in goals)
        {
            FinancialGoals.Add(goal);
        }
    }

    [RelayCommand]
    public void OpenAddGoalDialog()
    {
        System.Diagnostics.Debug.WriteLine("OpenAddGoalDialog called");

        //NewGoal = new FinancialGoalModel();
        NewGoal = new CreateFinancialGoalDto();
        IsAddGoalDialogOpen = true;

        System.Diagnostics.Debug.WriteLine($"IsAddGoalDialogOpen set to: {IsAddGoalDialogOpen}");
    }

    [RelayCommand]
    public async Task SaveNewGoal()
    {
        if (string.IsNullOrWhiteSpace(NewGoal.GoalName))
        {
            // Hiển thị thông báo lỗi
            return;
        }

        var createdGoal = await _financialGoalService.CreateFinancialGoalAsync(NewGoal);
        if (createdGoal != null)
        {
            FinancialGoals.Add(createdGoal);
            IsAddGoalDialogOpen = false;
        }
    }

    [RelayCommand]
    public async Task DeleteGoal(FinancialGoalModel goal)
    {
        bool result = await _financialGoalService.DeleteFinancialGoalAsync(goal.GoalId);
        if (result)
        {
            FinancialGoals.Remove(goal);
        }
    }

    [RelayCommand]
    public void CancelAddGoal()
    {
        IsAddGoalDialogOpen = false;
    }
}
