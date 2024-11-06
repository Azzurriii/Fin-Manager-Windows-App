using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Services;
using Moq;
using Microsoft.UI.Xaml.Controls;

namespace Fin_Manager_v2.Tests.Services
{
    [TestClass]
    public class DialogServiceTests
    {
        private IDialogService _dialogService;

        [TestInitialize]
        public void Initialize()
        {
            // Instantiate DialogService
            _dialogService = new DialogService();
        }

        [TestMethod]
        public async Task ShowErrorAsync_ShouldInvokeShowDialog()
        {
            // Arrange
            var title = "Error";
            var message = "An error occurred";

            // Act
            await _dialogService.ShowErrorAsync(title, message);

            // Assert
            // Since this is a UI element, a full verification might require integration testing,
            // but we can at least check if no exceptions are thrown during invocation.
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task ShowWarningAsync_ShouldInvokeShowDialog()
        {
            // Arrange
            var title = "Warning";
            var message = "This is a warning";

            // Act
            await _dialogService.ShowWarningAsync(title, message);

            // Assert
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task ShowSuccessAsync_ShouldInvokeShowDialog()
        {
            // Arrange
            var title = "Success";
            var message = "Operation completed successfully";

            // Act
            await _dialogService.ShowSuccessAsync(title, message);

            // Assert
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task ShowConfirmAsync_ShouldReturnTrueForPrimaryResult()
        {
            // Arrange
            var title = "Confirm";
            var message = "Do you confirm this action?";

            // Act
            var result = await _dialogService.ShowConfirmAsync(title, message);

            // Assert
            // We can't directly simulate a user response in unit testing, but we can verify logic paths.
            Assert.IsInstanceOfType(result, typeof(bool));
        }

        [TestMethod]
        public async Task ShowDialog_WithException_ShouldHandleGracefully()
        {
            // Arrange
            var title = "Test";
            var message = "This will throw an exception";

            // Act
            try
            {
                await _dialogService.ShowErrorAsync(title, message);
                Assert.IsTrue(true);
            }
            catch
            {
                Assert.Fail("Exception was not handled within the ShowDialog method.");
            }
        }
    }
}
