using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.Tests.MSTest.Test.Models;

[TestClass]
public class CurrencyModelTest
{
    private CurrencyModel _currency;

    [TestInitialize]
    public void Setup()
    {
        _currency = new CurrencyModel
        {
            Currency = "VND",
            Amount = 1000000,
            Formatted = "1,000,000 đ"
        };
    }

    [TestMethod]
    public void CurrencyProperties_ShouldHaveCorrectValues()
    {
        Assert.AreEqual("VND", _currency.Currency);
        Assert.AreEqual(1000000, _currency.Amount);
        Assert.AreEqual("1,000,000 đ", _currency.Formatted);
    }

    [TestMethod]
    public void DefaultValues_ShouldBeEmpty()
    {
        var newCurrency = new CurrencyModel();
        Assert.AreEqual(string.Empty, newCurrency.Currency);
        Assert.AreEqual(0, newCurrency.Amount);
        Assert.AreEqual(string.Empty, newCurrency.Formatted);
    }
}
