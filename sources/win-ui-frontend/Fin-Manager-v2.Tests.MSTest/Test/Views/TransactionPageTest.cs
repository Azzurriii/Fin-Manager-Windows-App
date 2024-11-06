using Fin_Manager_v2.Views;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FinManager.Tests.MSTest;
[TestClass]
public class TransactionPageTest
{
    [UITestMethod]
    public void TransactionPage_InitializesCorrectly()
    {
        // Arrange & Act
        var page = new TransactionPage();

        // Assert
        Assert.IsNotNull(page.ViewModel);
    }

    [UITestMethod]
    public void AddTransactionDialog_OpensAndCloses()
    {
        // Arrange
        var page = new TransactionPage();
        var dialog = page.FindName("AddTransactionDialog") as TeachingTip;

        // Act
        page.ViewModel.IsAddTransactionDialogOpen = true;

        // Assert
        Assert.IsTrue(dialog.IsOpen);

        // Act
        page.ViewModel.IsAddTransactionDialogOpen = false;

        // Assert
        Assert.IsFalse(dialog.IsOpen);
    }
}
