using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fin_Manager_v2.Models;
using System;

namespace Fin_Manager_v2.Tests.MSTest.Test.Models;

[TestClass]
public class AccountModelTest
{
    private AccountModel _account;

    [TestInitialize]
    public void Setup()
    {
        _account = new AccountModel
        {
            AccountId = 1,
            UserId = 1,
            AccountName = "Test Account",
            AccountType = "SAVINGS",
            InitialBalance = 1000000,
            CurrentBalance = 2000000,
            Currency = "VND",
            CreateAt = new DateTime(2024, 3, 15),
            UpdateAt = new DateTime(2024, 3, 15)
        };
    }

    [TestMethod]
    public void AccountProperties_ShouldHaveCorrectValues()
    {
        Assert.AreEqual(1, _account.AccountId);
        Assert.AreEqual(1, _account.UserId);
        Assert.AreEqual("Test Account", _account.AccountName);
        Assert.AreEqual("SAVINGS", _account.AccountType);
        Assert.AreEqual(1000000, _account.InitialBalance);
        Assert.AreEqual(2000000, _account.CurrentBalance);
        Assert.AreEqual("VND", _account.Currency);
    }

    [TestMethod]
    public void AccountDates_ShouldBeSetCorrectly()
    {
        var expectedDate = new DateTime(2024, 3, 15);
        Assert.AreEqual(expectedDate, _account.CreateAt);
        Assert.AreEqual(expectedDate, _account.UpdateAt);
    }
}