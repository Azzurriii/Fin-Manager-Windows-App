using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services.Interface;
using System.Net.Http.Json;

namespace Fin_Manager_v2.Services;

public class TransactionService : ITransactionService
{
    private readonly HttpClient _httpClient;
    private readonly ITagService _tagService;
    private const string BaseUrl = "http://localhost:3000";

    public TransactionService(HttpClient httpClient, ITagService tagService)
    {
        _httpClient = httpClient;
        _tagService = tagService;
    }

    public async Task<bool> CreateTransactionAsync(TransactionModel transaction)
    {
        try
        {
            var requestBody = new
            {
                account_id = transaction.AccountId,
                user_id = transaction.UserId,
                transaction_type = transaction.TransactionType,
                amount = transaction.Amount,
                tag_id = transaction.TagId,
                description = transaction.Description,
                transaction_date = transaction.Date.ToString("o")
            };

            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/transactions", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Error creating transaction: {error}");
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception creating transaction: {ex.Message}");
            return false;
        }
    }

    public async Task<decimal> GetTotalAmountAsync(int userId, int? accountId, string transactionType, DateTime startDate, DateTime endDate)
    {
        try
        {
            var requestBody = new
            {
                user_id = userId,
                account_id = accountId,
                transaction_type = transactionType,
                startDate = startDate.ToString("o"),
                endDate = endDate.ToString("o")
            };

            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/transactions/total-amount", requestBody);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<decimal>();
            }
            return 0;
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
            var response =
                await _httpClient.GetFromJsonAsync<List<TransactionModel>>($"{BaseUrl}/transactions/user/{userId}");
            return response ?? new List<TransactionModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting transactions: {ex.Message}");
            return new List<TransactionModel>();
        }
    }
}