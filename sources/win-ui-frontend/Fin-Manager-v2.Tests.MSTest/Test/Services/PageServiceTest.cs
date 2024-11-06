using System;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Services;
using Fin_Manager_v2.ViewModels;
using Fin_Manager_v2.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fin_Manager_v2.Tests.Services
{
    [TestClass]
    public class PageServiceTests
    {
        private IPageService _pageService;

        [TestInitialize]
        public void Initialize()
        {
            _pageService = new PageService();
        }

        [TestMethod]
        public void Configure_AddsPageMappingSuccessfully()
        {
            // Arrange & Act
            (_pageService as PageService)?.Configure<AccountViewModel, AccountPage>();

            // Assert
            var pageType = _pageService.GetPageType(typeof(AccountViewModel).FullName!);
            Assert.AreEqual(typeof(AccountPage), pageType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Configure_DuplicateViewModelKey_ThrowsException()
        {
            // Arrange
            var pageService = _pageService as PageService;
            pageService?.Configure<AccountViewModel, AccountPage>();

            // Act
            pageService?.Configure<AccountViewModel, TransactionPage>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Configure_DuplicatePageType_ThrowsException()
        {
            // Arrange
            var pageService = _pageService as PageService;
            pageService?.Configure<AccountViewModel, AccountPage>();

            // Act
            pageService?.Configure<TransactionViewModel, AccountPage>();
        }

        [TestMethod]
        public void GetPageType_ValidKey_ReturnsCorrectPageType()
        {
            // Arrange
            (_pageService as PageService)?.Configure<TransactionViewModel, TransactionPage>();

            // Act
            var pageType = _pageService.GetPageType(typeof(TransactionViewModel).FullName!);

            // Assert
            Assert.AreEqual(typeof(TransactionPage), pageType);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPageType_InvalidKey_ThrowsException()
        {
            // Act
            _pageService.GetPageType("NonExistentViewModel");
        }
    }
}
