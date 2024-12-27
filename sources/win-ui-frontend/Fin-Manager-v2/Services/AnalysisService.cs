using System.Text.Json;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using System.Diagnostics;
using System.Text.Json.Serialization;

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
}