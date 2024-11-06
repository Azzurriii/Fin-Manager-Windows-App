using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fin_Manager_v2.Tests.MSTest.Test.Services
{
    [TestClass]
    public class TransactionServiceTests
    {
        private MockHttpMessageHandler _mockHttp;
        private ITransactionService _transactionService;

        [TestInitialize]
        public void Initialize()
        {
            _mockHttp = new MockHttpMessageHandler();
            var client = new HttpClient(_mockHttp) { BaseAddress = new Uri("http://localhost/") };
            _transactionService = new TransactionService(client);
        }

        [TestMethod]
        public async Task CreateTransactionAsync_ValidTransaction_ShouldSucceed()
        {
            // Arrange
            var transaction = new TransactionModel
            {
                AccountId = 1,
                UserId = 1,
                TransactionType = "Credit",
                Amount = 1000,
                TagId = 1,
                Description = "Test transaction",
                Date = DateTime.UtcNow
            };

            _mockHttp.When(HttpMethod.Post, "*/transactions")
                     .Respond("application/json", JsonSerializer.Serialize(new { success = true }));

            // Act
            var result = await _transactionService.CreateTransactionAsync(transaction);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task CreateTransactionAsync_FailedTransaction_ShouldReturnFalse()
        {
            // Arrange
            var transaction = new TransactionModel
            {
                AccountId = 1,
                UserId = 1,
                TransactionType = "Credit",
                Amount = 1000,
                TagId = 1,
                Description = "Test transaction",
                Date = DateTime.UtcNow
            };

            _mockHttp.When(HttpMethod.Post, "*/transactions")
                     .Respond(System.Net.HttpStatusCode.BadRequest);

            // Act
            var result = await _transactionService.CreateTransactionAsync(transaction);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task GetTotalAmountAsync_ValidRequest_ShouldReturnAmount()
        {
            // Arrange
            int userId = 1;
            int? accountId = 1;
            string transactionType = "Credit";
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            DateTime endDate = DateTime.UtcNow;
            decimal expectedAmount = 1500;

            _mockHttp.When(HttpMethod.Post, "*/transactions/total-amount")
                     .Respond("application/json", JsonSerializer.Serialize(expectedAmount));

            // Act
            var result = await _transactionService.GetTotalAmountAsync(userId, accountId, transactionType, startDate, endDate);

            // Assert
            Assert.AreEqual(expectedAmount, result);
        }

        [TestMethod]
        public async Task GetTotalAmountAsync_ServerError_ShouldReturnZero()
        {
            // Arrange
            int userId = 1;
            int? accountId = 1;
            string transactionType = "Credit";
            DateTime startDate = DateTime.UtcNow.AddDays(-30);
            DateTime endDate = DateTime.UtcNow;

            _mockHttp.When(HttpMethod.Post, "*/transactions/total-amount")
                     .Respond(System.Net.HttpStatusCode.InternalServerError);

            // Act
            var result = await _transactionService.GetTotalAmountAsync(userId, accountId, transactionType, startDate, endDate);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public async Task GetUserTransactionsAsync_ValidUserId_ShouldReturnTransactions()
        {
            // Arrange
            int userId = 1;
            var expectedTransactions = new List<TransactionModel>
            {
                new TransactionModel { AccountId = 1, UserId = userId, TransactionType = "Credit", Amount = 1000, TagId = 1, Description = "Test transaction", Date = DateTime.UtcNow }
            };

            _mockHttp.When(HttpMethod.Get, $"*/transactions/user/{userId}")
                     .Respond("application/json", JsonSerializer.Serialize(expectedTransactions));

            // Act
            var result = await _transactionService.GetUserTransactionsAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expectedTransactions[0].Amount, result[0].Amount);
        }

        [TestMethod]
        public async Task GetUserTransactionsAsync_InvalidUserId_ShouldReturnEmptyList()
        {
            // Arrange
            int userId = 999;

            _mockHttp.When(HttpMethod.Get, $"*/transactions/user/{userId}")
                     .Respond("application/json", "[]");

            // Act
            var result = await _transactionService.GetUserTransactionsAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetUserTransactionsAsync_ServerError_ShouldReturnEmptyList()
        {
            // Arrange
            int userId = 1;

            _mockHttp.When(HttpMethod.Get, $"*/transactions/user/{userId}")
                     .Respond(System.Net.HttpStatusCode.InternalServerError);

            // Act
            var result = await _transactionService.GetUserTransactionsAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}
