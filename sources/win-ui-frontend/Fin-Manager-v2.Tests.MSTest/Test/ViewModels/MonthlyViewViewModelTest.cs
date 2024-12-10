using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.ViewModels;
using Microsoft.UI.Xaml.Controls;
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
    public class MonthlyViewViewModelTest
    {
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly Mock<ITagService> _mockTagService;
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IDialogService> _mockDialogService;

        public MonthlyViewViewModelTest()
        {
            _mockTransactionService = new Mock<ITransactionService>();
            _mockTagService = new Mock<ITagService>();
            _mockAccountService = new Mock<IAccountService>();
            _mockAuthService = new Mock<IAuthService>();
            _mockDialogService = new Mock<IDialogService>();

            _mockAuthService.Setup(x => x.GetUserId()).Returns(1);

            _mockAccountService.Setup(x => x.GetAccountsAsync())
                .ReturnsAsync(new List<AccountModel>
                {
                new AccountModel
                {
                    AccountId = 1,
                    AccountName = "Test Account",
                    InitialBalance = 1000,
                    CurrentBalance = 1500
                }
                });

            _mockTagService.Setup(x => x.GetTagsAsync())
                .ReturnsAsync(new List<TagModel>
                {
                new TagModel { Id = 1, TagName = "Salary" },
                new TagModel { Id = 2, TagName = "Expense" }
                });

            _mockTagService.Setup(x => x.GetTagAsync(1))
                .ReturnsAsync(new TagModel { Id = 1, TagName = "Salary" });
            _mockTagService.Setup(x => x.GetTagAsync(2))
                .ReturnsAsync(new TagModel { Id = 2, TagName = "Expense" });

            _mockTransactionService.Setup(x => x.GetTransactionsByQueryAsync(It.IsAny<QueryDto>()))
                .ReturnsAsync(new List<TransactionModel>
                {
                new TransactionModel
                {
                    TransactionId = 1,
                    Amount = 1000,
                    TransactionType = "INCOME",
                    TagId = 1
                },
                new TransactionModel
                {
                    TransactionId = 2,
                    Amount = 500,
                    TransactionType = "EXPENSE",
                    TagId = 2
                }
                });
        }

        [Fact]
        public async Task InitializeAsync_LoadsAccountsTagsAndTransactions()
        {
            var viewModel = new MonthlyViewViewModel(
                _mockTransactionService.Object,
                _mockTagService.Object,
                _mockAccountService.Object,
                _mockAuthService.Object
            );

            await Task.Delay(500);

            Assert.NotEmpty(viewModel.Accounts);
            Assert.NotEmpty(viewModel.AvailableTags);
            Assert.NotEmpty(viewModel.Transactions);
            Assert.NotNull(viewModel.SelectedAccount);
        }

        [Fact]
        public async Task LoadTransactionsAsync_CalculatesCorrectedTotals()
        {
            var viewModel = new MonthlyViewViewModel(
                _mockTransactionService.Object,
                _mockTagService.Object,
                _mockAccountService.Object,
                _mockAuthService.Object
            );

            await Task.Delay(500);

            Assert.Equal(1000m, viewModel.TotalIncome);
            Assert.Equal(500m, viewModel.TotalExpense);
            Assert.Equal(1500m, viewModel.Balance);
        }

        [Fact]
        public async Task OnSelectedAccountChanged_ReloadsTransactions()
        {
            var viewModel = new MonthlyViewViewModel(
                _mockTransactionService.Object,
                _mockTagService.Object,
                _mockAccountService.Object,
                _mockAuthService.Object
            );

            await Task.Delay(500);

            var newAccount = new AccountModel
            {
                AccountId = 2,
                AccountName = "New Account",
                InitialBalance = 2000
            };
            viewModel.SelectedAccount = newAccount;

            await Task.Delay(500);

            _mockTransactionService.Verify(
                x => x.GetTransactionsByQueryAsync(It.Is<QueryDto>(
                    q => q.AccountId == newAccount.AccountId)),
                Times.Once);
        }

        [Fact]
        public async Task OnTagSelectionChanged_ReloadsTransactions()
        {
            var viewModel = new MonthlyViewViewModel(
                _mockTransactionService.Object,
                _mockTagService.Object,
                _mockAccountService.Object,
                _mockAuthService.Object
            );

            await Task.Delay(500);

            var selectedTag = viewModel.AvailableTags.First();
            var mockListBox = new Mock<ListBox>();
            mockListBox.Setup(x => x.SelectedItems)
                .Returns(new List<object> { selectedTag });

            viewModel.OnTagSelectionChanged(mockListBox.Object, null);

            await Task.Delay(500);

            _mockTransactionService.Verify(
                x => x.GetTransactionsByQueryAsync(It.Is<QueryDto>(
                    q => q.TagIds.Contains(selectedTag.Id))),
                Times.Once);
        }

        [Fact]
        public async Task DateRangeChanged_ReloadsTransactions()
        {
            var viewModel = new MonthlyViewViewModel(
                _mockTransactionService.Object,
                _mockTagService.Object,
                _mockAccountService.Object,
                _mockAuthService.Object
            );

            await Task.Delay(500);

            var newStartDate = DateTimeOffset.Now.AddMonths(-2);
            var newEndDate = DateTimeOffset.Now.AddMonths(1);
            viewModel.StartDate = newStartDate;
            viewModel.EndDate = newEndDate;

            await Task.Delay(500);

            _mockTransactionService.Verify(
                x => x.GetTransactionsByQueryAsync(It.Is<QueryDto>(
                    q => q.StartDate == newStartDate.DateTime &&
                         q.EndDate == newEndDate.DateTime)),
                Times.Exactly(2) // Initial load + date change
            );
        }
    }
}
