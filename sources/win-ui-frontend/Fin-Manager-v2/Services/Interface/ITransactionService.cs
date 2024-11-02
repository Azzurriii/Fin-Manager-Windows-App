using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services.Interface;

public interface ITransactionService
{
    Task<bool> CreateTransactionAsync(TransactionModel transaction);
    Task<List<TransactionModel>> GetUserTransactionsAsync(int userId);
    Task<decimal> GetTotalAmountAsync(int userId, int? accountId, string transactionType, DateTime startDate, DateTime endDate);
}