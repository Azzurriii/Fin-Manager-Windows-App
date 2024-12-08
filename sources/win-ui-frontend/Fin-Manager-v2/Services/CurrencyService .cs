using Fin_Manager_v2.Contracts.Services;
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

    /// <summary>
    /// Converts an amount from one currency to another asynchronously.
    /// </summary>
    /// <param name="amount">The amount to convert.</param>
    /// <param name="fromCurrency">The currency to convert from.</param>
    /// <param name="toCurrency">The currency to convert to.</param>
    /// <returns>A task representing the asynchronous operation. The result is a CurrencyExchangeModel object.</returns>
    /// <exception cref="HttpRequestException">Thrown when an HTTP request error occurs.</exception>
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