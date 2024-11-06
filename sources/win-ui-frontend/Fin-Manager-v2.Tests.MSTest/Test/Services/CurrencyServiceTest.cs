using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services;
using RichardSzalay.MockHttp;
using System.Net.Http.Json;
using System.Text.Json;

namespace Fin_Manager_v2.Tests.MSTest.Test.Services;
[TestClass]
public class CurrencyServiceTests
{
    private MockHttpMessageHandler _mockHttp;
    private ICurrencyService _currencyService;
    private const string BaseUrl = "currency/convert";

    [TestInitialize]
    public void Initialize()
    {
        _mockHttp = new MockHttpMessageHandler();
        var client = new HttpClient(_mockHttp) { BaseAddress = new Uri("http://localhost/") };
        _currencyService = new CurrencyService(client);
    }

    [TestMethod]
    public async Task ConvertCurrency_ValidConversion_ShouldSucceed()
    {
        // Arrange
        decimal amount = 5;
        string fromCurrency = "USD";
        string toCurrency = "VND";

        var expectedResponse = new CurrencyExchangeModel
        {
            From = new CurrencyModel { Currency = fromCurrency, Amount = amount },
            To = new CurrencyModel { Currency = toCurrency, Amount = amount * 24000 },
            Rate = 24000,
            Timestamp = DateTime.UtcNow
        };

        _mockHttp.When(HttpMethod.Post, $"*/{BaseUrl}")
                .WithJsonContent(new { amount, from = fromCurrency, to = toCurrency })
                .Respond("application/json", JsonSerializer.Serialize(expectedResponse));

        // Act
        var result = await _currencyService.ConvertCurrencyAsync(amount, fromCurrency, toCurrency);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(fromCurrency, result.From.Currency);
        Assert.AreEqual(amount, result.From.Amount);
        Assert.AreEqual(toCurrency, result.To.Currency);
        Assert.AreEqual(expectedResponse.To.Amount, result.To.Amount);
        Assert.AreEqual(expectedResponse.Rate, result.Rate);
    }

    [TestMethod]
    public async Task ConvertCurrency_InvalidCurrency_ShouldThrowException()
    {
        // Arrange
        decimal amount = 5;
        string fromCurrency = "INVALID";
        string toCurrency = "VND";

        _mockHttp.When(HttpMethod.Post, $"*/{BaseUrl}")
                .Respond(System.Net.HttpStatusCode.BadRequest);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<HttpRequestException>(() =>
            _currencyService.ConvertCurrencyAsync(amount, fromCurrency, toCurrency));
    }

    [TestMethod]
    public async Task ConvertCurrency_ZeroAmount_ShouldSucceed()
    {
        // Arrange
        decimal amount = 0;
        string fromCurrency = "USD";
        string toCurrency = "VND";

        var expectedResponse = new CurrencyExchangeModel
        {
            From = new CurrencyModel { Currency = fromCurrency, Amount = 0 },
            To = new CurrencyModel { Currency = toCurrency, Amount = 0 },
            Rate = 24000,
            Timestamp = DateTime.UtcNow
        };

        _mockHttp.When(HttpMethod.Post, $"*/{BaseUrl}")
                .WithJsonContent(new { amount, from = fromCurrency, to = toCurrency })
                .Respond("application/json", JsonSerializer.Serialize(expectedResponse));

        // Act
        var result = await _currencyService.ConvertCurrencyAsync(amount, fromCurrency, toCurrency);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.From.Amount);
        Assert.AreEqual(0, result.To.Amount);
        Assert.AreEqual(expectedResponse.Rate, result.Rate);
    }

    [TestMethod]
    public async Task ConvertCurrency_ServerError_ShouldThrowException()
    {
        // Arrange
        decimal amount = 100;
        string fromCurrency = "USD";
        string toCurrency = "VND";

        _mockHttp.When(HttpMethod.Post, $"*/{BaseUrl}")
                .Respond(System.Net.HttpStatusCode.InternalServerError);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<HttpRequestException>(() =>
            _currencyService.ConvertCurrencyAsync(amount, fromCurrency, toCurrency));
    }

    [TestMethod]
    public async Task ConvertCurrency_NetworkError_ShouldThrowException()
    {
        // Arrange
        decimal amount = 100;
        string fromCurrency = "USD";
        string toCurrency = "VND";

        _mockHttp.When(HttpMethod.Post, $"*/{BaseUrl}")
                .Throw(new HttpRequestException("Network error"));

        // Act & Assert
        await Assert.ThrowsExceptionAsync<HttpRequestException>(() =>
            _currencyService.ConvertCurrencyAsync(amount, fromCurrency, toCurrency));
    }
}