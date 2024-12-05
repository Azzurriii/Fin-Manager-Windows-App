using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using System.Net.Http.Json;

namespace Fin_Manager_v2.Services;

public class TransactionService : ITransactionService
{
    private readonly HttpClient _httpClient;

    public TransactionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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

            var response = await _httpClient.PostAsJsonAsync("transactions", requestBody);

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

            var response = await _httpClient.PostAsJsonAsync("transactions/total-amount", requestBody);
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
            var response = await _httpClient.GetFromJsonAsync<List<TransactionModel>>($"transactions/user/{userId}");
            return response ?? new List<TransactionModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting transactions: {ex.Message}");
            return new List<TransactionModel>();
        }
    }

    public async Task<bool> UpdateTransactionAsync(int id, TransactionModel transaction)
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

            var response = await _httpClient.PutAsJsonAsync($"transactions/{id}", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Error updating transaction: {error}");
            }

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating transaction: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteTransactionAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"transactions/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error deleting transaction: {ex.Message}");
            return false;
        }
    }

    public async Task<List<TransactionModel>> GetTransactionsByQueryAsync(QueryDto query)
{
    try
    {
        var response = await _httpClient.PostAsJsonAsync("transactions/query", query);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<TransactionModel>>() ?? new List<TransactionModel>();
        }
        return new List<TransactionModel>();
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Error getting transactions by query: {ex.Message}");
        return new List<TransactionModel>();
    }
}

    
}
