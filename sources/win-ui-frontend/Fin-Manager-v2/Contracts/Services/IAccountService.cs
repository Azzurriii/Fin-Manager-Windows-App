using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.Contracts.Services;

public interface IAccountService
{
    Task<List<AccountModel>> GetAccountsAsync();
    Task<bool> CreateAccountAsync(CreateFinanceAccountDto accountDto);
    Task<bool> UpdateAccountAsync(UpdateFinanceAccountDto account);
    Task<bool> DeleteAccountAsync(int accountId);
    Task<AccountModel?> GetDefaultAccountAsync();
}