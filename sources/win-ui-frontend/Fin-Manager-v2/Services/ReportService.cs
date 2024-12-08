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

    public ReportService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };
    }

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

    public async Task<List<CategoryReportModel>> GetCategoryReportAsync(int userId, int? accountId, string type, DateTime startDate, DateTime endDate)
    {
        var query = $"user_id={userId}&account_id={accountId}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
        var response = await _httpClient.GetAsync($"report/category/{type}?{query}");
        Debug.WriteLine($"Category Report Response: {response.StatusCode}");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<CategoryReportModel>>();
        return result ?? new List<CategoryReportModel>();
    }

    public (DateTime StartDate, DateTime EndDate) GetDateRangeFromPeriod(string period, System.DateTimeOffset selectedDate, string selectedMonth, string selectedQuarter, int selectedYear)
    {
        if (period == "Day")
        {
            return (selectedDate.Date, selectedDate.Date);
        }
        else if (period == "Month")
        {
            var month = DateTime.ParseExact(selectedMonth, "MMMM", CultureInfo.InvariantCulture).Month;
            var startDate = new DateTime(selectedYear, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            return (startDate, endDate);
        }
        else if (period == "Quarter")
        {
            var quarter = int.Parse(selectedQuarter[1].ToString());
            var startMonth = (quarter - 1) * 3 + 1;
            var startDate = new DateTime(selectedYear, startMonth, 1);
            var endDate = startDate.AddMonths(3).AddDays(-1);
            return (startDate, endDate);
        }
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