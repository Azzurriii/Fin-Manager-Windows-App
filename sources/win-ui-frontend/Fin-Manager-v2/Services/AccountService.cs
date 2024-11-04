﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services.Interface;

namespace Fin_Manager_v2.Services;

public class AccountService : IAccountService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;

    public AccountService(IAuthService authService)
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:3000/") };
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

    public async Task<List<Account>> GetAccountsAsync()
    {
        SetAuthorizationHeader();
        var accounts = await _httpClient.GetFromJsonAsync<List<Account>>("finance-accounts/me");
        return accounts ?? new List<Account>();
    }

    public async Task<bool> CreateAccountAsync(CreateFinanceAccountDto accountDto)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.PostAsJsonAsync("finance-accounts", accountDto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAccountAsync(Account account)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.PutAsJsonAsync($"finance-accounts/{account.AccountId}", account);
        return response.IsSuccessStatusCode;
    }
}