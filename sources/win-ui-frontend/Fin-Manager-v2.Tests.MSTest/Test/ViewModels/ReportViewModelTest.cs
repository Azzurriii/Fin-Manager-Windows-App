using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

namespace Fin_Manager_v2.Tests.MSTest.Test.ViewModels
{
    public class ReportViewModelTest
    {
        private readonly Mock<IReportService> _mockReportService;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly ReportViewModel _viewModel;

        public ReportViewModelTest()
        {
            _mockReportService = new Mock<IReportService>();
            _mockAuthService = new Mock<IAuthService>();
            _mockAccountService = new Mock<IAccountService>();

            _mockAuthService.Setup(x => x.GetUserId()).Returns(1);
            _mockAuthService.Setup(x => x.FetchUserIdAsync()).Returns(Task.CompletedTask);

            _mockAccountService.Setup(x => x.GetAccountsAsync()).ReturnsAsync(new List<AccountModel>
        {
            new AccountModel { AccountId = 1, AccountName = "Test Account" }
        });

            SetupDefaultReportServiceMocks();

            _viewModel = new ReportViewModel(
                _mockReportService.Object,
                _mockAuthService.Object,
                _mockAccountService.Object);
        }

        private void SetupDefaultReportServiceMocks()
        {
            _mockReportService.Setup(x => x.GetSummaryAsync(
                It.IsAny<int>(),
                It.IsAny<int?>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
                .ReturnsAsync(new SummaryModel
                {
                    TotalIncome = 1000,
                    TotalExpense = 500,
                    Balance = 500
                });

            _mockReportService.Setup(x => x.GetOverviewAsync(
                It.IsAny<int>(),
                It.IsAny<int?>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
                .ReturnsAsync(new List<OverviewModel>
                {
                new OverviewModel
                {
                    Month = "January",
                    TotalIncome = 500,
                    TotalExpense = 250
                }
                });

            _mockReportService.Setup(x => x.GetCategoryReportAsync(
                It.IsAny<int>(),
                It.IsAny<int?>(),
                It.Is<string>(s => s == "INCOME"),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
                .ReturnsAsync(new List<CategoryReportModel>
                {
                new CategoryReportModel
                {
                    TagName = "Salary",
                    Amount = 1000
                }
                });

            _mockReportService.Setup(x => x.GetCategoryReportAsync(
                It.IsAny<int>(),
                It.IsAny<int?>(),
                It.Is<string>(s => s == "EXPENSE"),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
                .ReturnsAsync(new List<CategoryReportModel>
                {
                new CategoryReportModel
                {
                    TagName = "Groceries",
                    Amount = 500
                }
                });
        }

        [Fact]
        public async Task InitializeUserAndAccountsAsync_ShouldLoadAccountsAndUpdateChartData()
        {
            await Task.Delay(500);

            Assert.NotEmpty(_viewModel.Accounts);
            Assert.Equal(2, _viewModel.Accounts.Count);

            Assert.NotNull(_viewModel.SelectedAccountObj);
        }

        [Fact]
        public void OnSelectedTimePeriodChanged_ShouldUpdatePeriodFlags()
        {
            _viewModel.SelectedTimePeriod = "Month";
            Assert.True(_viewModel.IsMonthPeriod);
            Assert.False(_viewModel.IsQuarterPeriod);
            Assert.False(_viewModel.IsYearPeriod);

            _viewModel.SelectedTimePeriod = "Quarter";
            Assert.False(_viewModel.IsMonthPeriod);
            Assert.True(_viewModel.IsQuarterPeriod);
            Assert.False(_viewModel.IsYearPeriod);

            _viewModel.SelectedTimePeriod = "Year";
            Assert.False(_viewModel.IsMonthPeriod);
            Assert.False(_viewModel.IsQuarterPeriod);
            Assert.True(_viewModel.IsYearPeriod);
        }

        [Fact]
        public void OnSelectedAccountObjChanged_ShouldUpdateAccountId()
        {
            var testAccount = new AccountModel { AccountId = 2, AccountName = "Test Account 2" };
            _viewModel.SelectedAccountObj = testAccount;
            Assert.Equal(2, _viewModel.AccountId);

            var allAccountsOption = new AccountModel { AccountId = 0, AccountName = "All Accounts" };
            _viewModel.SelectedAccountObj = allAccountsOption;
            Assert.Null(_viewModel.AccountId);
        }

        [Fact]
        public async Task UpdateChartData_ShouldPopulateChartProperties()
        {
            await Task.Delay(500);

            Assert.Equal("1,000.00", _viewModel.TotalIncome);
            Assert.Equal("500.00", _viewModel.TotalExpense);
            Assert.Equal("500.00", _viewModel.Balance);

            Assert.NotNull(_viewModel.OverviewSeries);
            Assert.NotEmpty(_viewModel.OverviewSeries);

            Assert.NotNull(_viewModel.IncomeSeries);
            Assert.NotNull(_viewModel.ExpenseSeries);
            Assert.NotEmpty(_viewModel.IncomeSeries);
            Assert.NotEmpty(_viewModel.ExpenseSeries);
        }

        [Fact]
        public void XAxes_ShouldBeConfiguredCorrectly()
        {
            Task.Delay(500).Wait();

            Assert.NotNull(_viewModel.XAxes);
            var xAxis = _viewModel.XAxes.FirstOrDefault();
            Assert.NotNull(xAxis);
            Assert.NotEmpty(xAxis.Labels);
        }
    }
}
