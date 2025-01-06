using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.ViewModels;
using Microsoft.UI.Xaml;
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
    public class JobViewModelTest
    {
        private readonly Mock<IJobService> _mockJobService;
        private readonly Mock<IDialogService> _mockDialogService;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IMailerService> _mockMailService;


        public JobViewModelTest()
        {
            _mockJobService = new Mock<IJobService>();
            _mockDialogService = new Mock<IDialogService>();
            _mockAuthService = new Mock<IAuthService>();
            _mockMailService = new Mock<IMailerService>();

            // Setup default auth token
            _mockAuthService.Setup(x => x.GetAccessToken()).Returns("test-token");
        }

        [Fact]
        public async Task InitializeAsync_LoadsJobsSuccessfully()
        {
            // Arrange
            var mockJobs = new List<JobModel>
            {
                new JobModel { JobId = 1, JobName = "Job 1", Amount = 1000 },
                new JobModel { JobId = 2, JobName = "Job 2", Amount = 2000 }
            };

            _mockJobService.Setup(x => x.GetJobsAsync())
                .ReturnsAsync(mockJobs);

            var viewModel = new JobViewModel(
                _mockJobService.Object,
                _mockDialogService.Object,
                _mockAuthService.Object,
                _mockMailService.Object
            );

            // Act
            await viewModel.InitializeAsync();

            // Assert
            Assert.True(viewModel.IsInitialized);
            Assert.False(viewModel.HasError);
            Assert.Equal(2, viewModel.Jobs.Count);
            Assert.Equal(mockJobs[0].JobName, viewModel.Jobs[0].JobName);
        }

        [Fact]
        public async Task AddJobAsync_ValidJob_AddsSuccessfully()
        {
            // Arrange
            var jobDto = new CreateJobDto
            {
                JobName = "New Job",
                Amount = 1500,
                RecurringType = "MONTHLY"
            };

            _mockJobService.Setup(x => x.CreateJobAsync(It.IsAny<CreateJobDto>()))
                .ReturnsAsync(true);

            _mockJobService.Setup(x => x.GetJobsAsync())
                .ReturnsAsync(new List<JobModel>
                {
                    new JobModel { JobId = 1, JobName = "New Job", Amount = 1500 }
                });

            var viewModel = new JobViewModel(
                _mockJobService.Object,
                _mockDialogService.Object,
                _mockAuthService.Object,
                _mockMailService.Object
                
            );

            // Act
            await viewModel.AddJobAsync(jobDto);

            // Assert
            Assert.False(viewModel.HasError);
            Assert.Equal(1, viewModel.Jobs.Count);
            Assert.Equal("New Job", viewModel.Jobs[0].JobName);
        }

        [Fact]
        public async Task AddJobAsync_InvalidJob_ShowsError()
        {
            // Arrange
            var invalidJobDto = new CreateJobDto
            {
                JobName = "",
                Amount = 0
            };

            var viewModel = new JobViewModel(
                _mockJobService.Object,
                _mockDialogService.Object,
                _mockAuthService.Object,
                _mockMailService.Object
            );

            // Act
            await viewModel.AddJobAsync(invalidJobDto);

            // Assert
            Assert.True(viewModel.HasError);
            _mockDialogService.Verify(
                x => x.ShowErrorAsync(It.IsAny<string>(), It.IsAny<string>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteJobAsync_RemovesJobSuccessfully()
        {
            // Arrange
            var jobToDelete = new JobModel
            {
                JobId = 1,
                JobName = "Job to Delete"
            };

            _mockJobService.Setup(x => x.DeleteJobAsync(jobToDelete.JobId))
                .ReturnsAsync(true);

            var viewModel = new JobViewModel(
                _mockJobService.Object,
                _mockDialogService.Object,
                _mockAuthService.Object,
                _mockMailService.Object
            );

            // Manually add job to collection
            viewModel.Jobs.Add(jobToDelete);

            // Act
            await viewModel.DeleteJobAsync(jobToDelete);

            // Assert
            Assert.Empty(viewModel.Jobs);
            Assert.False(viewModel.HasError);
        }

        [Fact]
        public async Task UpdateJobAsync_UpdatesJobSuccessfully()
        {
            // Arrange
            var existingJob = new JobModel
            {
                JobId = 1,
                JobName = "Original Job",
                Amount = 1000
            };

            var updateDto = new UpdateJobDto
            {
                JobName = "Updated Job",
                Amount = 2000
            };

            _mockJobService.Setup(x => x.UpdateJobAsync(existingJob.JobId, It.IsAny<UpdateJobDto>()))
                .ReturnsAsync(true);

            _mockJobService.Setup(x => x.GetJobsAsync())
                .ReturnsAsync(new List<JobModel>
                {
                    new JobModel { JobId = 1, JobName = "Updated Job", Amount = 2000 }
                });

            var viewModel = new JobViewModel(
                _mockJobService.Object,
                _mockDialogService.Object,
                _mockAuthService.Object,
                _mockMailService.Object
            );

            // Set selected job
            viewModel.SelectedJob = existingJob;

            // Act
            await viewModel.UpdateJobAsync(updateDto);

            // Assert
            Assert.False(viewModel.HasError);
            _mockDialogService.Verify(
                x => x.ShowSuccessAsync("Success", "Update job successfully"),
                Times.Once
            );
        }

        [Fact]
        public void CollectionVisibility_ReturnsCorrectVisibility()
        {
            // Arrange
            var viewModel = new JobViewModel(
                _mockJobService.Object,
                _mockDialogService.Object,
                _mockAuthService.Object,
                _mockMailService.Object
            );

            // Empty collection
            var emptyCollection = new System.Collections.ObjectModel.ObservableCollection<JobModel>();
            Assert.Equal(Visibility.Visible, viewModel.CollectionVisibility(emptyCollection));

            // Non-empty collection
            emptyCollection.Add(new JobModel());
            Assert.Equal(Visibility.Collapsed, viewModel.CollectionVisibility(emptyCollection));
        }
    }
}
