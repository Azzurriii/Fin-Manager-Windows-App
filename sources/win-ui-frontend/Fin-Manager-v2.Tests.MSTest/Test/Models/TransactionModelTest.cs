using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.Tests.MSTest.Test.Models;
[TestClass]
public class TransactionModelTest
{
    [TestMethod]
    public void FormattedAmount_Income_ShowsPlusSign()
    {
        var transaction = new TransactionModel
        {
            Amount = 1000,
            TransactionType = "INCOME"
        };

        Assert.AreEqual("+1,000 đ", transaction.FormattedAmount);
    }

    [TestMethod]
    public void FormattedAmount_Expense_ShowsMinusSign()
    {
        var transaction = new TransactionModel
        {
            Amount = 1000,
            TransactionType = "EXPENSE"
        };

        Assert.AreEqual("-1,000", transaction.FormattedAmount);
    }

    [TestMethod]
    public void FormattedDate_ReturnsCorrectFormat()
    {
        var date = new DateTime(2024, 3, 15);
        var transaction = new TransactionModel
        {
            Date = date
        };

        Assert.AreEqual("15/03/2024", transaction.FormattedDate);
    }
}
