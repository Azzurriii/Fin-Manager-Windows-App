using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.Kernel.Sketches;
using SkiaSharp;

namespace Fin_Manager_v2.ViewModels;

public partial class ReportViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string _selectedTimePeriod = "Year";

    [ObservableProperty]
    private string _selectedMonth = DateTime.Now.ToString("MMMM");

    [ObservableProperty]
    private string _selectedQuarter = $"Q{(DateTime.Now.Month + 2) / 3}";

    [ObservableProperty]
    private int _selectedYear = DateTime.Now.Year;

    [ObservableProperty]
    private DateTimeOffset _selectedDate = DateTimeOffset.Now;

    // Visibility control properties
    public bool IsDayPeriod => SelectedTimePeriod == "Day";
    public bool IsMonthPeriod => SelectedTimePeriod == "Month";
    public bool IsQuarterPeriod => SelectedTimePeriod == "Quarter";
    public bool IsYearPeriod => SelectedTimePeriod == "Year";

    partial void OnSelectedTimePeriodChanged(string value)
    {
        OnPropertyChanged(nameof(IsDayPeriod));
        OnPropertyChanged(nameof(IsMonthPeriod));
        OnPropertyChanged(nameof(IsQuarterPeriod));
        OnPropertyChanged(nameof(IsYearPeriod));
        UpdateChartData();
    }

    [ObservableProperty]
    private string totalIncome = "$5,000";

    [ObservableProperty]
    private string totalExpense = "$3,000";

    [ObservableProperty]
    private string balance = "$2,000";

    [ObservableProperty]
    private IEnumerable<ISeries> overviewSeries;

    [ObservableProperty]
    private IEnumerable<ISeries> incomeSeries;

    [ObservableProperty]
    private IEnumerable<ISeries> expenseSeries;

    [ObservableProperty]
    private IEnumerable<ICartesianAxis> xAxes;

    public ReportViewModel()
    {
        InitializeCharts();
    }

    private void InitializeCharts()
    {
        // Mock data for Overview chart
        var months = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", 
                            "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        var incomeValues = new double[] { 2, 4, 3, 5, 3, 4, 6, 3, 5, 4, 3, 2 };
        var expenseValues = new double[] { 1, 3, 2, 4, 2, 3, 5, 2, 4, 3, 2, 1 };

        OverviewSeries = new ISeries[]
        {
            new ColumnSeries<double>
            {
                Name = "Income",
                Values = incomeValues,
                Fill = new SolidColorPaint(SKColors.Green),
            },
            new ColumnSeries<double>
            {
                Name = "Expense",
                Values = expenseValues,
                Fill = new SolidColorPaint(SKColors.Red),
            }
        };

        XAxes = new ICartesianAxis[]
        {
            new Axis
            {
                Labels = months,
                LabelsRotation = 0,
                ForceStepToMin = true,
                MinStep = 1,
                TextSize = 12,
                SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 }
            }
        };

        // Mock data for Income pie chart
        IncomeSeries = new[]
        {
            new PieSeries<double> 
            { 
                Values = new[] { 2d }, 
                Name = "Salary",
                Fill = new SolidColorPaint(SKColors.LightGreen)
            },
            new PieSeries<double> 
            { 
                Values = new[] { 1d }, 
                Name = "Investment",
                Fill = new SolidColorPaint(SKColors.MediumSeaGreen)
            },
            new PieSeries<double> 
            { 
                Values = new[] { 1.5 }, 
                Name = "Other",
                Fill = new SolidColorPaint(SKColors.ForestGreen)
            }
        };

        // Mock data for Expense pie chart
        ExpenseSeries = new[]
        {
            new PieSeries<double> 
            { 
                Values = new[] { 1d }, 
                Name = "Food",
                Fill = new SolidColorPaint(SKColors.LightCoral)
            },
            new PieSeries<double> 
            { 
                Values = new[] { 0.5 }, 
                Name = "Transport",
                Fill = new SolidColorPaint(SKColors.IndianRed)
            },
            new PieSeries<double> 
            { 
                Values = new[] { 1.5 }, 
                Name = "Shopping",
                Fill = new SolidColorPaint(SKColors.Crimson)
            }
        };
    }

    private void UpdateChartData()
    {
        // Sẽ implement sau để cập nhật dữ liệu chart dựa trên period được chọn
    }
}