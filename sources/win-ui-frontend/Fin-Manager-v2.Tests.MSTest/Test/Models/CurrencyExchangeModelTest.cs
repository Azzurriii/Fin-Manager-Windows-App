using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fin_Manager_v2.Models;
using System;

namespace Fin_Manager_v2.Tests.MSTest.Test.Models;

[TestClass]
public class CurrencyExchangeModelTest
{
    private CurrencyExchangeModel _exchange;
    private readonly DateTime _timestamp = new DateTime(2024, 3, 15);

    [TestInitialize]
    public void Setup()
    {
        _exchange = new CurrencyExchangeModel
        {
            From = new CurrencyModel { Currency = "USD", Amount = 1, Formatted = "$1" },
            To = new CurrencyModel { Currency = "VND", Amount = 24525, Formatted = "24,525 đ" },
            Rate = 24525,
            Timestamp = _timestamp
        };
    }

    [TestMethod]
    public void ExchangeProperties_ShouldHaveCorrectValues()
    {
        Assert.AreEqual("USD", _exchange.From.Currency);
        Assert.AreEqual("VND", _exchange.To.Currency);
        Assert.AreEqual(24525, _exchange.Rate);
        Assert.AreEqual(_timestamp, _exchange.Timestamp);
    }
} 