using System.Text.Json;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Net.Http.Json;

namespace Fin_Manager_v2.Services;

public class AnalysisService : IAnalysisService
{
    private readonly HttpClient _httpClient;

    public AnalysisService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AnalysisModel> GetAnalysisAsync(int userId, int? accountId, DateTime startDate, DateTime endDate)
    {
        try
        {
            var url = $"analysis?user_id={userId}&start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}";
            if (accountId.HasValue)
            {
                url += $"&account_id={accountId.Value}";
            }

            Debug.WriteLine($"Requesting analysis data: {url}");
            
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"Analysis Response: {content}");

            var apiResponse = JsonSerializer.Deserialize<JsonElement>(content);
            var data = apiResponse.GetProperty("data");
            
            return JsonSerializer.Deserialize<AnalysisModel>(data.GetRawText(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in GetAnalysisAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<AIAnalysisModel> GetAIAnalysisAsync(AnalysisModel data)
    {
        try{
            var url = $"ai-analysis";

            var requestBody = new
            {
                data = new
                {
                    time_period = data.TimePeriod,
                    spending_summary = data.SpendingSummary,
                    comparison_with_previous_period = data.ComparisonWithPreviousPeriod,
                    top_categories = data.TopCategories,
                    monthly_trend = data.MonthlyTrend,
                    category_details = data.CategoryDetails
                }
            };

            Debug.WriteLine($"Requesting AI analysis: {JsonSerializer.Serialize(requestBody)}");

            var response = await _httpClient.PostAsJsonAsync(url, requestBody);
            var content = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"AI Analysis Response: {content}");
            return JsonSerializer.Deserialize<AIAnalysisModel>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in GetAIAnalysisAsync: {ex.Message}");
            throw;
        }
    }
}