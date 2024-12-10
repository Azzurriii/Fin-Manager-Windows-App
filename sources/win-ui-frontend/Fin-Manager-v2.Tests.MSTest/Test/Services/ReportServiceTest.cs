using Fin_Manager_v2.Models;
using Moq.Protected;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

namespace Fin_Manager_v2.Tests.MSTest.Test.Services
{
    public class ReportServiceTest
    {
        private readonly Mock<HttpClient> _mockHttpClient;
        private readonly ReportService _reportService;

        public ReportServiceTest()
        {
            _mockHttpClient = new Mock<HttpClient>();
            _reportService = new ReportService(_mockHttpClient.Object);
        }

        [Fact]
        public async Task GetSummaryAsync_ValidResponse_ReturnsSummaryModel()
        {
            var expectedSummary = new SummaryModel
            {
                TotalIncome = 1000,
                TotalExpense = 500,
                Balance = 500
            };
            var jsonResponse = JsonSerializer.Serialize(expectedSummary);

            SetupMockHttpClient(HttpStatusCode.OK, jsonResponse);

            var result = await _reportService.GetSummaryAsync(1, null, DateTime.Now, DateTime.Now);

            Assert.NotNull(result);
            Assert.Equal(expectedSummary.TotalIncome, result.TotalIncome);
            Assert.Equal(expectedSummary.TotalExpense, result.TotalExpense);
            Assert.Equal(expectedSummary.Balance, result.Balance);
        }

        [Fact]
        public async Task GetOverviewAsync_ValidResponse_ReturnsOverviewModelList()
        {
            var expectedOverview = new List<OverviewModel>
        {
            new OverviewModel
            {
                Month = "January",
                TotalIncome = 1000,
                TotalExpense = 500
            }
        };
            var jsonResponse = JsonSerializer.Serialize(expectedOverview);

            SetupMockHttpClient(HttpStatusCode.OK, jsonResponse);

            var result = await _reportService.GetOverviewAsync(1, null, DateTime.Now, DateTime.Now);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(expectedOverview[0].Month, result[0].Month);
            Assert.Equal(expectedOverview[0].TotalIncome, result[0].TotalIncome);
        }

        [Fact]
        public async Task GetCategoryReportAsync_ValidResponse_ReturnsCategoryReportModelList()
        {
            var expectedCategories = new List<CategoryReportModel>
            {
                new CategoryReportModel
                {
                    TagName = "Salary",
                    Amount = 1000
                }
            };
            var jsonResponse = JsonSerializer.Serialize(expectedCategories);

            SetupMockHttpClient(HttpStatusCode.OK, jsonResponse);

            var result = await _reportService.GetCategoryReportAsync(1, null, "INCOME", DateTime.Now, DateTime.Now);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(expectedCategories[0].TagName, result[0].TagName);
            Assert.Equal(expectedCategories[0].Amount, result[0].Amount);
        }

        [Theory]
        [InlineData("Day", "2023-05-15", "May", "Q2", 2023, "2023-05-15", "2023-05-15")]
        [InlineData("Month", "2023-05-15", "May", "Q2", 2023, "2023-05-01", "2023-05-31")]
        [InlineData("Quarter", "2023-05-15", "May", "Q2", 2023, "2023-04-01", "2023-06-30")]
        [InlineData("Year", "2023-05-15", "May", "Q2", 2023, "2023-01-01", "2023-12-31")]
        public void GetDateRangeFromPeriod_ReturnsCorrectDateRange(
            string period,
            string selectedDateStr,
            string selectedMonth,
            string selectedQuarter,
            int selectedYear,
            string expectedStartDateStr,
            string expectedEndDateStr)
        {
            var selectedDate = DateTimeOffset.Parse(selectedDateStr);
            var expectedStartDate = DateTime.Parse(expectedStartDateStr);
            var expectedEndDate = DateTime.Parse(expectedEndDateStr);

            var (startDate, endDate) = _reportService.GetDateRangeFromPeriod(
                period,
                selectedDate,
                selectedMonth,
                selectedQuarter,
                selectedYear);

            Assert.Equal(expectedStartDate, startDate);
            Assert.Equal(expectedEndDate, endDate);
        }

        [Fact]
        public void GetDateRangeFromPeriod_InvalidPeriod_ThrowsArgumentException()
        {
            var selectedDate = DateTimeOffset.Now;
            var selectedMonth = "May";
            var selectedQuarter = "Q2";
            var selectedYear = 2023;

            Assert.Throws<ArgumentException>(() =>
                _reportService.GetDateRangeFromPeriod(
                    "InvalidPeriod",
                    selectedDate,
                    selectedMonth,
                    selectedQuarter,
                    selectedYear));
        }

        private void SetupMockHttpClient(HttpStatusCode statusCode, string jsonResponse)
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(jsonResponse)
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClient.SetupGet(x => x).Returns(httpClient);
        }
    }
}
