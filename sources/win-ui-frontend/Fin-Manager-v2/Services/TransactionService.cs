using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services.Interface;
using System.Net.Http.Json;

namespace Fin_Manager_v2.Services;

public class TransactionService : ITransactionService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://localhost:3000";

    public TransactionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> CreateTransactionAsync(TransactionModel transaction)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/transactions", transaction);
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<decimal> GetTotalAmountByTagAsync(int tagId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<decimal>($"{BaseUrl}/transactions/tag/{tagId}/total");
            return response;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public async Task<decimal> GetTotalAmountByTypeAsync(string transactionType)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<decimal>($"{BaseUrl}/transactions/type/{transactionType}/total");
            return response;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public async Task<List<TransactionModel>> GetUserTransactionsAsync(int userId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<TransactionModel>>($"{BaseUrl}/transactions/user/{userId}");
            return response ?? new List<TransactionModel>();
        }
        catch (Exception)
        {
            return new List<TransactionModel>();
        }
    }
}