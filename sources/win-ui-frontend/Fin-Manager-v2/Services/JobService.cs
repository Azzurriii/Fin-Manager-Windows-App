using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.Services;

public class JobService : IJobService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private readonly string _baseUrl = "http://localhost:3000";

    public JobService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    private void SetAuthorizationHeader()
    {
        var token = _authService.GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("No valid authentication token. Please log in again.");
        }
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<JobModel>> GetJobsAsync()
    {
        SetAuthorizationHeader();
        try 
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/jobs/me");
            response.EnsureSuccessStatusCode();
            var jobs = await response.Content.ReadFromJsonAsync<List<JobModel>>();
            return jobs ?? new List<JobModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in GetJobsAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> CreateJobAsync(CreateJobDto jobDto)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/jobs", jobDto);
        System.Diagnostics.Debug.WriteLine($"CreateJobAsync: {response.RequestMessage?.RequestUri}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateJobAsync(int jobId, UpdateJobDto updateJobDto)
    {
        SetAuthorizationHeader();
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/jobs/{jobId}", updateJobDto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in UpdateJobAsync: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteJobAsync(int jobId)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.DeleteAsync($"{_baseUrl}/jobs/{jobId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<JobModel>> GetJobsByUserIdAsync(int userId)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.GetAsync($"{_baseUrl}/jobs/users/{userId}");
        System.Diagnostics.Debug.WriteLine($"GetJobsByUserIdAsync: {response.RequestMessage?.RequestUri}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<JobModel>>() ?? new List<JobModel>();
        }
        return new List<JobModel>(); 
    }
}