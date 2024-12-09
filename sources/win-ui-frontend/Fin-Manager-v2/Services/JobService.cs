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

    public JobService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    /// <summary>Sets the authorization header for HTTP requests using the access token obtained from the authentication service.</summary>
    /// <exception cref="InvalidOperationException">Thrown when no valid authentication token is available.</exception>
    private void SetAuthorizationHeader()
    {
        var token = _authService.GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("No valid authentication token. Please log in again.");
        }
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Asynchronously retrieves a list of job models.
    /// </summary>
    /// <returns>A task representing the asynchronous operation that returns a list of JobModel objects.</returns>
    /// <exception cref="Exception">Thrown when an error occurs during the operation.</exception>
    public async Task<List<JobModel>> GetJobsAsync()
    {
        SetAuthorizationHeader();
        try 
        {
            var response = await _httpClient.GetAsync("jobs/me");
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

    /// <summary>Creates a job asynchronously.</summary>
    /// <param name="jobDto">The data transfer object containing job information.</param>
    /// <returns>A task representing the asynchronous operation, returning a boolean indicating if the job creation was successful.</returns>
    public async Task<bool> CreateJobAsync(CreateJobDto jobDto)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.PostAsJsonAsync("jobs", jobDto);
        System.Diagnostics.Debug.WriteLine($"CreateJobAsync: {response.RequestMessage?.RequestUri}");
        return response.IsSuccessStatusCode;
    }

    /// <summary>Updates a job asynchronously.</summary>
    /// <param name="jobId">The ID of the job to update.</param>
    /// <param name="updateJobDto">The data to update the job with.</param>
    /// <returns>A task representing the operation, returning true if the update was successful, false otherwise.</returns>
    public async Task<bool> UpdateJobAsync(int jobId, UpdateJobDto updateJobDto)
    {
        SetAuthorizationHeader();
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"jobs/{jobId}", updateJobDto);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in UpdateJobAsync: {ex.Message}");
            return false;
        }
    }

    /// <summary>Deletes a job asynchronously.</summary>
    /// <param name="jobId">The ID of the job to delete.</param>
    /// <returns>A task representing the asynchronous operation. True if the job was deleted successfully; otherwise, false.</returns>
    public async Task<bool> DeleteJobAsync(int jobId)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.DeleteAsync($"jobs/{jobId}");
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Retrieves a list of jobs associated with a specific user asynchronously.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve jobs for.</param>
    /// <returns>A list of JobModel objects associated with the specified user.</returns>
    public async Task<List<JobModel>> GetJobsByUserIdAsync(int userId)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.GetAsync($"jobs/users/{userId}");
        System.Diagnostics.Debug.WriteLine($"GetJobsByUserIdAsync: {response.RequestMessage?.RequestUri}");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<JobModel>>() ?? new List<JobModel>();
        }
        return new List<JobModel>(); 
    }
}