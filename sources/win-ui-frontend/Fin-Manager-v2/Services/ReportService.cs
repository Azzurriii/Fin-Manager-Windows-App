using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using HarfBuzzSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

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
        response.EnsureSuccessStatusCode(); // Ensure the request was successful
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
        response.EnsureSuccessStatusCode(); // Ensure the request was successful
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
        response.EnsureSuccessStatusCode(); // Ensure the request was successful
        var result = await response.Content.ReadFromJsonAsync<List<ReportModel.TagAmountModel>>();
        return result;
    }

    public (DateTime StartDate, DateTime EndDate) GetDateRangeFromPeriod(string period, DateTimeOffset selectedDate, string selectedMonth, string selectedQuarter, int selectedYear)
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