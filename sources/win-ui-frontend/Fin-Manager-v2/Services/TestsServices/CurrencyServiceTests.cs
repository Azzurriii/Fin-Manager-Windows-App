using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services;
using RichardSzalay.MockHttp;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

public class CurrencyServiceTests
{
    [Fact]
    public async Task ConvertCurrencyAsync_ShouldReturnExchangeModel_WhenResponseIsSuccessful()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();

        // Expected response
        var expectedModel = new CurrencyExchangeModel
        {
            From = new CurrencyModel
            {
                Currency = "USD",
                Amount = 5,
                Formatted = "$5.00"
            },
            To = new CurrencyModel
            {
                Currency = "VND",
                Amount = 126556.37M,
                Formatted = "₫126,556"
            },
            Rate = 25311.273999999998M,
            Timestamp = DateTime.Parse("2024-11-05T15:39:17.771Z")
        };

        // Mock the HTTP response
        mockHttp.When("http://localhost/currency/convert")
                .Respond("application/json", @"
                {
                    ""from"": {
                        ""currency"": ""USD"",
                        ""amount"": 5,
                        ""formatted"": ""$5.00""
                    },
                    ""to"": {
                        ""currency"": ""VND"",
                        ""amount"": 126556.37,
                        ""formatted"": ""₫126,556""
                    },
                    ""rate"": 25311.273999999998,
                    ""timestamp"": ""2024-11-05T15:39:17.771Z""
                }");

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        var service = new CurrencyService(httpClient);

        // Act
        var result = await service.ConvertCurrencyAsync(5, "USD", "VND");

        // Assert
        Assert.Equal(expectedModel.From.Currency, result.From.Currency);
        Assert.Equal(expectedModel.From.Amount, result.From.Amount);
        Assert.Equal(expectedModel.From.Formatted, result.From.Formatted);

        Assert.Equal(expectedModel.To.Currency, result.To.Currency);
        Assert.Equal(expectedModel.To.Amount, result.To.Amount);
        Assert.Equal(expectedModel.To.Formatted, result.To.Formatted);

        Assert.Equal(expectedModel.Rate, result.Rate);
        Assert.Equal(expectedModel.Timestamp, result.Timestamp);
    }

    [Fact]
    public async Task ConvertCurrencyAsync_ShouldThrowException_WhenResponseIsUnsuccessful()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();

        // Mock the unsuccessful response (status code 500)
        mockHttp.When("http://localhost/currency/convert")
                .Respond(HttpStatusCode.InternalServerError);

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost/");

        var service = new CurrencyService(httpClient);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => service.ConvertCurrencyAsync(5, "USD", "VND"));
    }
}
