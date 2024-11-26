using System.Globalization;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Fin_Manager_v2.Services;

public class ReportService : IReportService
{
    private readonly HttpClient _httpClient;

    public ReportService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ReportModel.SummaryModel> GetSummaryAsync(int userId, int accountId, DateTime startDate, DateTime endDate)
    {
        var request = new
        {
            user_id = userId,
            account_id = accountId,
            startDate = startDate.ToString("yyyy-MM-dd"),
            endDate = endDate.ToString("yyyy-MM-dd")
        };

        var response = await _httpClient.PostAsJsonAsync("report/summary", request);
        var result = await response.Content.ReadFromJsonAsync<ReportModel.SummaryModel>();
        return result;
    }

    public async Task<List<ReportModel.OverviewModel>> GetOverviewAsync(int userId, int accountId, DateTime startDate, DateTime endDate)
    {
        var request = new
        {
            user_id = userId,
            account_id = accountId,
            startDate = startDate.ToString("yyyy-MM-dd"),
            endDate = endDate.ToString("yyyy-MM-dd")
        };

        var response = await _httpClient.PostAsJsonAsync("report/overview", request);
        var result = await response.Content.ReadFromJsonAsync<List<ReportModel.OverviewModel>>();
        return result;
    }

    public async Task<List<ReportModel.TagAmountModel>> GetCategoryReportAsync(int userId, int accountId, string type, DateTime startDate, DateTime endDate)
    {
        var request = new
        {
            user_id = userId,
            account_id = accountId,
            timePeriod = type,
            startDate = startDate.ToString("yyyy-MM-dd"),
            endDate = endDate.ToString("yyyy-MM-dd")
        };

        var response = await _httpClient.PostAsJsonAsync($"report/category/{type}", request);
        var result = await response.Content.ReadFromJsonAsync<List<ReportModel.TagAmountModel>>();
        return result;
    }

    public (DateTime StartDate, DateTime EndDate) GetDateRangeFromPeriod(
    string period,
    DateTimeOffset? selectedDate = null,
    string selectedMonth = null,
    string selectedQuarter = null,
    int? selectedYear = null)
    {
        // Validate input parameters based on period
        ValidateInputParameters(period, selectedDate, selectedMonth, selectedQuarter, selectedYear);

        return period?.ToLower() switch
        {
        "day" => (selectedDate!.Value.Date, selectedDate.Value.Date),

        "month" => {
        var month = DateTime.ParseExact(selectedMonth!, "MMMM", CultureInfo.InvariantCulture).Month;
            var startDate = new DateTime(selectedYear!.Value, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return (startDate, endDate);
    },
        
        "quarter" => {
            var quarter = int.Parse(selectedQuarter![1].ToString());
    var startMonth = (quarter - 1) * 3 + 1;
    var startDate = new DateTime(selectedYear!.Value, startMonth, 1);
    var endDate = startDate.AddMonths(3).AddDays(-1);
            return (startDate, endDate);
        },
        
        "year" => {
            var startDate = new DateTime(selectedYear!.Value, 1, 1);
var endDate = new DateTime(selectedYear.Value, 12, 31);
            return (startDate, endDate);
        },
        
        _ => throw new ArgumentException($"Invalid period: {period}")
    };
}

private void ValidateInputParameters(
    string period,
    DateTimeOffset? selectedDate,
    string selectedMonth,
    string selectedQuarter,
    int? selectedYear)
{
    if (string.IsNullOrWhiteSpace(period))
        throw new ArgumentNullException(nameof(period), "Period cannot be null or empty.");

    switch (period?.ToLower())
    {
        case "day":
            if (!selectedDate.HasValue)
                throw new ArgumentNullException(nameof(selectedDate), "Selected date is required for daily period.");
            break;

        case "month":
            if (string.IsNullOrWhiteSpace(selectedMonth))
                throw new ArgumentNullException(nameof(selectedMonth), "Selected month is required for monthly period.");
            if (!selectedYear.HasValue)
                throw new ArgumentNullException(nameof(selectedYear), "Selected year is required for monthly period.");

            // Validate month name
            try
            {
                DateTime.ParseExact(selectedMonth, "MMMM", CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new ArgumentException("Invalid month name. Use full month name (e.g., 'January').");
            }
            break;

        case "quarter":
            if (string.IsNullOrWhiteSpace(selectedQuarter))
                throw new ArgumentNullException(nameof(selectedQuarter), "Selected quarter is required for quarterly period.");
            if (!selectedYear.HasValue)
                throw new ArgumentNullException(nameof(selectedYear), "Selected year is required for quarterly period.");

            // Validate quarter format
            if (!Regex.IsMatch(selectedQuarter, @"^Q[1-4]$"))
                throw new ArgumentException("Quarter must be in format 'Q1', 'Q2', 'Q3', or 'Q4'.");
            break;

        case "year":
            if (!selectedYear.HasValue)
                throw new ArgumentNullException(nameof(selectedYear), "Selected year is required for yearly period.");
            break;

        default:
            throw new ArgumentException($"Invalid period: {period}. Supported periods are: Day, Month, Quarter, Year.");
    }
}
}