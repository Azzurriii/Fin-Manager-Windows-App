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

    /// <summary>Initializes a new instance of the AccountService class.</summary>
    /// <param name="httpClient">The HttpClient instance used for HTTP requests.</param>
    /// <param name="authService">The authentication service used for user authentication.</param>
    /// <exception cref="ArgumentNullException">Thrown when httpClient or authService is null.</exception>
    public AccountService(HttpClient httpClient, IAuthService authService)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    /// <summary>Sets the authorization header for HTTP requests using the access token obtained from the authentication service.</summary>
    /// <exception cref="InvalidOperationException">Thrown when no valid authentication token is available.</exception>
    private void SetAuthorizationHeader()
    {
        var token = _authService.GetAccessToken();
        if (string.IsNullOrEmpty(token))
        {
            throw new InvalidOperationException("No valid authentication token. Please log in again.");
        }
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>Retrieves a list of accounts asynchronously.</summary>
    /// <returns>A list of AccountModel objects representing the accounts.</returns>
    public async Task<List<AccountModel>> GetAccountsAsync()
    {
        SetAuthorizationHeader();
        var accounts = await _httpClient.GetFromJsonAsync<List<AccountModel>>("finance-accounts/me");
        return accounts ?? new List<AccountModel>();
    }

    /// <summary>Creates a finance account asynchronously.</summary>
    /// <param name="accountDto">The data transfer object containing account information.</param>
    /// <returns>A task representing the asynchronous operation, returning true if the account creation was successful; otherwise, false.</returns>
    public async Task<bool> CreateAccountAsync(CreateFinanceAccountDto accountDto)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.PostAsJsonAsync("finance-accounts", accountDto);
        return response.IsSuccessStatusCode;
    }

    /// <summary>Updates a finance account asynchronously.</summary>
    /// <param name="account">The finance account data to update.</param>
    /// <returns>A task representing the asynchronous operation. True if the update was successful; otherwise, false.</returns>
    public async Task<bool> UpdateAccountAsync(UpdateFinanceAccountDto account)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.PutAsJsonAsync($"finance-accounts/{account.account_id}", account);
        return response.IsSuccessStatusCode;
    }

    /// <summary>Deletes an account asynchronously.</summary>
    /// <param name="accountId">The ID of the account to delete.</param>
    /// <returns>A task representing the operation, returning true if the deletion was successful, false otherwise.</returns>
    public async Task<bool> DeleteAccountAsync(int accountId)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.DeleteAsync($"finance-accounts/{accountId}");
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Asynchronously retrieves the default account.
    /// </summary>
    /// <returns>The default account model if available; otherwise, null.</returns>
    public async Task<AccountModel?> GetDefaultAccountAsync()
    {
        var accounts = await GetAccountsAsync();
        return accounts.FirstOrDefault();
    }
}