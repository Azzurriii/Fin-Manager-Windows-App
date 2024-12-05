using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.Contracts.Services;
public interface ITransactionService
{
    Task<bool> CreateTransactionAsync(TransactionModel transaction);
    Task<List<TransactionModel>> GetUserTransactionsAsync(int userId);
    Task<decimal> GetTotalAmountAsync(int userId, int? accountId, string transactionType, DateTime startDate, DateTime endDate);
    Task<bool> UpdateTransactionAsync(int id, TransactionModel transaction);
    Task<bool> DeleteTransactionAsync(int id);
    Task<List<TransactionModel>> GetTransactionsByQueryAsync(QueryDto query);
}