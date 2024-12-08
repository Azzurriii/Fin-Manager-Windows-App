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

    /// <summary>Creates a transaction asynchronously.</summary>
    /// <param name="transaction">The transaction model containing transaction details.</param>
    /// <returns>A boolean indicating if the transaction was created successfully.</returns>
    /// <exception cref="Exception">Thrown when an error occurs during the transaction creation process.</exception>
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

    /// <summary>Retrieves the total amount of transactions for a user within a specified date range.</summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="accountId">The ID of the account (nullable).</param>
    /// <param name="transactionType">The type of transaction.</param>
    /// <param name="startDate">The start date of the date range.</param>
    /// <param name="endDate">The end date of the date range.</param>
    /// <returns>The total amount of transactions as a decimal.</returns>
    /// <exception cref="HttpRequestException">Thrown when an error occurs during the HTTP request.</exception>
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

    /// <summary>Retrieves a list of transactions for a specific user asynchronously.</summary>
    /// <param name="userId">The ID of the user whose transactions are to be retrieved.</param>
    /// <returns>A list of TransactionModel objects representing the user's transactions.</returns>
    /// <exception cref="Exception">Thrown when an error occurs during the retrieval process.</exception>
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

    /// <summary>
    /// Updates a transaction asynchronously.
    /// </summary>
    /// <param name="id">The ID of the transaction to update.</param>
    /// <param name="transaction">The transaction model containing updated information.</param>
    /// <returns>A task representing the asynchronous operation. True if the transaction was updated successfully; otherwise, false.</returns>
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

    /// <summary>Deletes a transaction asynchronously.</summary>
    /// <param name="id">The ID of the transaction to delete.</param>
    /// <returns>A boolean indicating whether the transaction was successfully deleted.</returns>
    /// <exception cref="Exception">Thrown when an error occurs during the deletion process.</exception>
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

    /// <summary>Retrieves a list of transactions based on the provided query asynchronously.</summary>
    /// <param name="query">The query parameters to filter transactions.</param>
    /// <returns>A list of TransactionModel objects that match the query.</returns>
    /// <exception cref="Exception">Thrown when an error occurs during the retrieval process.</exception>
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
