﻿using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Fin_Manager_v2.Services;
public class CurrencyService : ICurrencyService
{
    private readonly HttpClient _httpClient;

    public CurrencyService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<CurrencyExchangeModel> ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
    {
        var requestData = new
        {
            amount = amount,
            from = fromCurrency,
            to = toCurrency
        };

        var response = await _httpClient.PostAsJsonAsync("currency/convert", requestData);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CurrencyExchangeModel>();
    }
}