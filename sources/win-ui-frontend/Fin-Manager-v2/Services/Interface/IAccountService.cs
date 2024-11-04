using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.Services.Interface;

public interface IAccountService
{
    Task<List<AccountModel>> GetAccountsAsync();
    Task<bool> CreateAccountAsync(CreateFinanceAccountDto accountDto);
    Task<bool> UpdateAccountAsync(AccountModel account);
}