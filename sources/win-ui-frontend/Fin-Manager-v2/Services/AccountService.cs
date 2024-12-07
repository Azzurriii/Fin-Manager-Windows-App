using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.Services;

public class AccountService : IAccountService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public AccountService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
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

    public async Task<List<AccountModel>> GetAccountsAsync()
    {
        SetAuthorizationHeader();
        var accounts = await _httpClient.GetFromJsonAsync<List<AccountModel>>("finance-accounts/me");
        return accounts ?? new List<AccountModel>();
    }

    public async Task<bool> CreateAccountAsync(CreateFinanceAccountDto accountDto)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.PostAsJsonAsync("finance-accounts", accountDto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAccountAsync(UpdateFinanceAccountDto account)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.PutAsJsonAsync($"finance-accounts/{account.account_id}", account);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAccountAsync(int accountId)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.DeleteAsync($"finance-accounts/{accountId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<AccountModel?> GetDefaultAccountAsync()
    {
        var accounts = await GetAccountsAsync();
        return accounts.FirstOrDefault();
    }
}