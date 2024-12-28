using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.DTO;
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
    public class BudgetViewModelTest
    {
        private readonly Mock<IBudgetService> _budgetServiceMock;
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly Mock<IDialogService> _dialogServiceMock;
        private readonly Mock<ITagService> _tagServiceMock;
        private readonly BudgetViewModel _viewModel;

        public BudgetViewModelTest()
        {
            _budgetServiceMock = new Mock<IBudgetService>();
            _accountServiceMock = new Mock<IAccountService>();
            _dialogServiceMock = new Mock<IDialogService>();
            _tagServiceMock = new Mock<ITagService>();
            _viewModel = new BudgetViewModel(
                _budgetServiceMock.Object,
                _accountServiceMock.Object,
                _dialogServiceMock.Object,
                _tagServiceMock.Object
                );

        }

        [Fact]
        public async Task SaveBudget_ShouldAddBudgetToCollection()
        {
            _viewModel.NewBudget = new CreateBudgetDto
            {
                Category = "Education",
                BudgetAmount = 500
            };
            var createdBudget = new BudgetModel { BudgetId = 3, Category = "Education", BudgetAmount = 500 };
            _budgetServiceMock.Setup(s => s.CreateBudgetAsync(It.IsAny<CreateBudgetDto>()))
                .ReturnsAsync(createdBudget);

            await _viewModel.SaveBudget();

            Assert.Single(_viewModel.Budgets);
            Assert.Equal("Education", _viewModel.Budgets[0].Category);
            _budgetServiceMock.Verify(s => s.CreateBudgetAsync(It.IsAny<CreateBudgetDto>()), Times.Once);
        }

        [Fact]
        public async Task DeleteBudget_ShouldRemoveBudgetFromCollection()
        {
            var budgetToDelete = new BudgetModel { BudgetId = 1, Category = "Groceries" };
            _viewModel.Budgets.Add(budgetToDelete);
            _budgetServiceMock.Setup(s => s.DeleteBudgetAsync(budgetToDelete.BudgetId))
                .ReturnsAsync(true);

            await _viewModel.DeleteBudget(budgetToDelete);

            Assert.Empty(_viewModel.Budgets);
            _budgetServiceMock.Verify(s => s.DeleteBudgetAsync(budgetToDelete.BudgetId), Times.Once);
        }
    }

    public static class TestHelpers
    {
        public static async Task WaitForLoadAsync(this BudgetViewModel viewModel, int expectedCount, int timeoutMilliseconds = 2000)
        {
            var tcs = new TaskCompletionSource<bool>();

            void CollectionChangedHandler(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
            {
                if (viewModel.Budgets.Count == expectedCount)
                {
                    viewModel.Budgets.CollectionChanged -= CollectionChangedHandler;
                    tcs.TrySetResult(true);
                }
            }

            viewModel.Budgets.CollectionChanged += CollectionChangedHandler;

            viewModel.LoadBudgets();

            await Task.WhenAny(tcs.Task, Task.Delay(timeoutMilliseconds));

            viewModel.Budgets.CollectionChanged -= CollectionChangedHandler;
        }
    }
}
