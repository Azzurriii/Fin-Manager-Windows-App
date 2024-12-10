using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Assert = Xunit.Assert;
using RichardSzalay.MockHttp;

namespace Fin_Manager_v2.Tests.MSTest.Test.Services
{
    public class BudgetServiceTest
    {
        private readonly MockHttpMessageHandler _mockHttp;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly BudgetService _budgetService;

        public BudgetServiceTest()
        {
            _mockHttp = new MockHttpMessageHandler();
            _authServiceMock = new Mock<IAuthService>();
            _authServiceMock.Setup(auth => auth.GetAccessToken()).Returns("mock-access-token");
            var httpClient = _mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:3000/");
            _budgetService = new BudgetService(httpClient, _authServiceMock.Object);
        }

        [Fact]
        public async Task CreateBudgetAsync_ShouldReturnCreatedBudget()
        {
            var newBudget = new CreateBudgetDto { Category = "Health", BudgetAmount = 300 };
            var mockBudget = new BudgetModel { BudgetId = 1, Category = "Health", BudgetAmount = 300 };

            _mockHttp.When(HttpMethod.Post, "/budget")
                .Respond(HttpStatusCode.OK, JsonContent.Create(mockBudget));

            var result = await _budgetService.CreateBudgetAsync(newBudget);

            Assert.NotNull(result);
            Assert.Equal("Health", result.Category);
            Assert.Equal(300, result.BudgetAmount);
        }

        [Fact]
        public async Task GetBudgetsAsync_ShouldReturnListOfBudgets()
        {
            var mockBudgets = new List<BudgetModel>
            {
                new BudgetModel { BudgetId = 1, Category = "Groceries", BudgetAmount = 200 },
                new BudgetModel { BudgetId = 2, Category = "Transport", BudgetAmount = 100 }
            };

            _mockHttp.When(HttpMethod.Get, "/budget")
                .Respond(HttpStatusCode.OK, JsonContent.Create(mockBudgets));

            var budgets = await _budgetService.GetBudgetsAsync();

            Assert.NotNull(budgets);
            Assert.Equal(2, budgets.Count);
            Assert.Equal("Groceries", budgets[0].Category);
            Assert.Equal(200, budgets[0].BudgetAmount);
            Assert.Equal("Transport", budgets[1].Category);
            Assert.Equal(100, budgets[1].BudgetAmount);
        }
    }
}
