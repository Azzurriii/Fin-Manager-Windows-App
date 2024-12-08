using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Converters;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fin_Manager_v2.Services;

/// <summary>
/// Service for managing budgets through HTTP requests.
/// </summary>
public class BudgetService : IBudgetService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public BudgetService(HttpClient httpClient, IAuthService authService)
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

    public async Task<BudgetModel?> CreateBudgetAsync(CreateBudgetDto budget)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.PostAsJsonAsync("http://localhost:3000/budget", budget);
        if (response.IsSuccessStatusCode)
        {
            // Deserialize JSON response thành Budget model
            return await response.Content.ReadFromJsonAsync<BudgetModel>();
        }
        return null;
    }

    public async Task<List<BudgetModel>> GetBudgetsAsync()
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync("http://localhost:3000/budget");
            //if (response.IsSuccessStatusCode)
            //{
            //    var json = await response.Content.ReadAsStringAsync();
            //    return JsonSerializer.Deserialize<List<BudgetModel>>(json) ?? new List<BudgetModel>();
            //}

            //return new List<BudgetModel>();
            response.EnsureSuccessStatusCode();

            // Đọc nội dung JSON
            var json = await response.Content.ReadAsStringAsync();

            // Sử dụng phương thức deserialize
            return DeserializeBudgets(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching budgets: {ex.Message}");
            return new List<BudgetModel>();
        }
    }

    public async Task<bool> DeleteBudgetAsync(int budgetId)
    {
        try
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"http://localhost:3000/budget/{budgetId}");

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteBudgetAsync: {ex.Message}");
            return false;
        }
    }

    public List<BudgetModel> DeserializeBudgets(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new DecimalStringConverter() }
        };

        var budgets = JsonSerializer.Deserialize<List<BudgetModel>>(json, options);

        if (budgets == null || budgets.Count == 0)
        {
            Console.WriteLine("Deserialization returned an empty list.");
        }
        else
        {
            foreach (var budget in budgets)
            {
                Console.WriteLine($"Budget ID: {budget.BudgetId}, Budget Amount: {budget.BudgetAmount}");
            }
        }

        return budgets;
    }
}
