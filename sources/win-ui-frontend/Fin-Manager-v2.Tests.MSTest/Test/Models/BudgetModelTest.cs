using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fin_Manager_v2.Models;
using System;

namespace Fin_Manager_v2.Tests.MSTest.Test.Models;

[TestClass]
public class BudgetModelTest
{
    private BudgetModel _budget;
    private readonly DateTime _startDate = new DateTime(2024, 3, 1);
    private readonly DateTime _endDate = new DateTime(2024, 3, 31);

    [TestInitialize]
    public void Setup()
    {
        _budget = new BudgetModel
        {
            BudgetId = 1,
            UserId = 1,
            AccountId = 1,
            Category = "Food",
            BudgetAmount = 1000000,
            SpentAmount = 500000,
            StartDate = _startDate,
            EndDate = _endDate
        };
    }

    [TestMethod]
    public void BudgetProperties_ShouldHaveCorrectValues()
    {
        Assert.AreEqual(1, _budget.BudgetId);
        Assert.AreEqual(1, _budget.UserId);
        Assert.AreEqual(1, _budget.AccountId);
        Assert.AreEqual("Food", _budget.Category);
        Assert.AreEqual(1000000, _budget.BudgetAmount);
        Assert.AreEqual(500000, _budget.SpentAmount);
    }

    [TestMethod]
    public void BudgetDates_ShouldBeSetCorrectly()
    {
        Assert.AreEqual(_startDate, _budget.StartDate);
        Assert.AreEqual(_endDate, _budget.EndDate);
    }
}
