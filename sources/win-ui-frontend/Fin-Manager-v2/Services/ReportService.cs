using ABI.System;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using HarfBuzzSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Exception = System.Exception;

public class ReportService : IReportService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>Initializes a new instance of the ReportService class.</summary>
    /// <param name="httpClient">The HttpClient used to make HTTP requests.</param>
    public ReportService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
    }

    /// <summary>
    /// Asynchronously retrieves a summary for a user within a specified date range.
    /// </summary>
    /// <param name="userId">The ID of the user for whom the summary is requested.</param>
    /// <param name="accountId">The optional account ID for filtering.</param>
    /// <param name="startDate">The start date of the summary period.</param>
    /// <param name="endDate">The end date of the summary period.</param>
    /// <returns>A Task representing the asynchronous operation that returns a SummaryModel object.</returns>
    /// <exception cref="HttpRequestException">Thrown when an HTTP request error occurs.</exception>
    /// <exception cref="JsonException">Thrown when an error occurs during JSON deserialization.</exception>
    public async Task<SummaryModel> GetSummaryAsync(int userId, int? accountId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var query = $"user_id={userId}&account_id={accountId}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
            using var response = await _httpClient.GetAsync($"report/summary?{query}");
            
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"Summary Response: {jsonString}");
            
            var result = JsonSerializer.Deserialize<SummaryModel>(jsonString, _jsonOptions);
            return result ?? new SummaryModel();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in GetSummaryAsync: {ex.Message}");
            return new SummaryModel();
        }
    }

    /// <summary>
    /// Retrieves an overview asynchronously based on the provided parameters.
    /// </summary>
    /// <param name="userId">The ID of the user for whom the overview is requested.</param>
    /// <param name="accountId">The ID of the account for which the overview is requested (optional).</param>
    /// <param name="startDate">The start date of the overview period.</param>
    /// <param name="endDate">The end date of the overview period.</param>
    /// <returns>A list of OverviewModel objects representing the overview data.</returns>
    /// <exception cref="HttpRequestException">Thrown when an HTTP request error occurs.</exception>
    /// <exception cref="JsonException">Thrown when an error occurs during JSON deserialization.</exception
    public async Task<List<OverviewModel>> GetOverviewAsync(int userId, int? accountId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var query = $"user_id={userId}&account_id={accountId}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
            using var response = await _httpClient.GetAsync($"report/overview?{query}");
            
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"Overview Response: {jsonString}");
            
            var result = JsonSerializer.Deserialize<List<OverviewModel>>(jsonString, _jsonOptions);
            return result ?? new List<OverviewModel>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in GetOverviewAsync: {ex.Message}");
            return new List<OverviewModel>();
        }
    }

    /// <summary>
    /// Retrieves a category report asynchronously based on the specified parameters.
    /// </summary>
    /// <param name="userId">The ID of the user for whom the report is generated.</param>
    /// <param name="accountId">The ID of the account for which the report is generated (optional).</param>
    /// <param name="type">The type of the category report.</param>
    /// <param name="startDate">The start date of the report period.</param>
    /// <param name="endDate">The end date of the report period.</param>
    /// <returns>A list of CategoryReportModel objects representing the category report.</returns>
    public async Task<List<CategoryReportModel>> GetCategoryReportAsync(int userId, int? accountId, string type, DateTime startDate, DateTime endDate)
    {
        var query = $"user_id={userId}&account_id={accountId}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
        var response = await _httpClient.GetAsync($"report/category/{type}?{query}");
        Debug.WriteLine($"Category Report Response: {response.StatusCode}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<CategoryReportModel>>();
        return result ?? new List<CategoryReportModel>();
    }

    /// <summary>
    /// Get the date range based on the selected period.
    /// </summary>
    /// <param name="period">The selected period (Day, Month, Quarter, Year).</param>
    /// <param name="selectedDate">The selected date.</param>
    /// <param name="selectedMonth">The selected month.</param>
    /// <param name="selectedQuarter">The selected quarter.</param>
    /// <param name="selectedYear">The selected year.</param>
    /// <returns>A tuple containing the start date and end date of the date range.</returns>
    /// <exception cref="ArgumentException">Thrown when the period is invalid.</exception>
    public (DateTime StartDate, DateTime EndDate) GetDateRangeFromPeriod(string period, System.DateTimeOffset selectedDate, string selectedMonth, string selectedQuarter, int selectedYear)
    {
        // Get the date range based on the selected period
        if (period == "Day")
        {
            return (selectedDate.Date, selectedDate.Date);
        }
        // Get the start and end date of the selected month
        else if (period == "Month")
        {
            var month = DateTime.ParseExact(selectedMonth, "MMMM", CultureInfo.InvariantCulture).Month;
            var startDate = new DateTime(selectedYear, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return (startDate, endDate);
        }
        // Get the start date and end date of the selected quarter
        else if (period == "Quarter")
        {
            var quarter = int.Parse(selectedQuarter[1].ToString());
            var startMonth = (quarter - 1) * 3 + 1;
            var startDate = new DateTime(selectedYear, startMonth, 1);
            var endDate = startDate.AddMonths(3).AddDays(-1);
            return (startDate, endDate);
        }
        // Start and end date of selected year
        else if (period == "Year")
        {
            var startDate = new DateTime(selectedYear, 1, 1);
            var endDate = new DateTime(selectedYear, 12, 31);
            return (startDate, endDate);
        }
        else
        {
            throw new ArgumentException("Invalid period");
        }
    }
}