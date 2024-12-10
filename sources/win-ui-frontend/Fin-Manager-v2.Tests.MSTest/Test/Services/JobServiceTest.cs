using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.DTO;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services;
using Moq.Protected;
using Moq;
using System;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http.Json;
using Xunit;
using Assert = Xunit.Assert;

namespace Fin_Manager_v2.Tests.MSTest.Test.Services
{
    public class JobServiceTests
    {
        private readonly Mock<HttpClient> _mockHttpClient;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly JobService _jobService;

        public JobServiceTests()
        {
            // Setup mock HttpClient
            _mockHttpClient = new Mock<HttpClient>();

            // Setup mock AuthService
            _mockAuthService = new Mock<IAuthService>();
            _mockAuthService.Setup(x => x.GetAccessToken()).Returns("test-token");

            // Create JobService with mocked dependencies
            _jobService = new JobService(_mockHttpClient.Object, _mockAuthService.Object);
        }

        [Fact]
        public async Task GetJobsAsync_ReturnsJobsList_WhenSuccessful()
        {
            // Arrange
            var expectedJobs = new List<JobModel>
            {
                new JobModel { JobId = 1, JobName = "Job 1" },
                new JobModel { JobId = 2, JobName = "Job 2" }
            };

            // Setup mock HTTP response
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r =>
                        r.Method == HttpMethod.Get &&
                        r.RequestUri.ToString().Contains("/jobs/me")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(expectedJobs))
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            var authService = _mockAuthService.Object;
            var jobService = new JobService(client, authService);

            // Act
            var result = await jobService.GetJobsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Job 1", result[0].JobName);
        }

        [Fact]
        public async Task CreateJobAsync_ReturnsTrue_WhenSuccessful()
        {
            // Arrange
            var jobDto = new CreateJobDto
            {
                JobName = "New Job",
                Amount = 1000,
                RecurringType = "MONTHLY"
            };

            // Setup mock HTTP response
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r =>
                        r.Method == HttpMethod.Post &&
                        r.RequestUri.ToString().Contains("/jobs")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Created
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            var authService = _mockAuthService.Object;
            var jobService = new JobService(client, authService);

            // Act
            var result = await jobService.CreateJobAsync(jobDto);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateJobAsync_ReturnsTrue_WhenSuccessful()
        {
            // Arrange
            int jobId = 1;
            var updateJobDto = new UpdateJobDto
            {
                JobName = "Updated Job",
                Amount = 1500
            };

            // Setup mock HTTP response
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r =>
                        r.Method == HttpMethod.Put &&
                        r.RequestUri.ToString().Contains($"/jobs/{jobId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            var authService = _mockAuthService.Object;
            var jobService = new JobService(client, authService);

            // Act
            var result = await jobService.UpdateJobAsync(jobId, updateJobDto);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteJobAsync_ReturnsTrue_WhenSuccessful()
        {
            // Arrange
            int jobId = 1;

            // Setup mock HTTP response
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r =>
                        r.Method == HttpMethod.Delete &&
                        r.RequestUri.ToString().Contains($"/jobs/{jobId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            var authService = _mockAuthService.Object;
            var jobService = new JobService(client, authService);

            // Act
            var result = await jobService.DeleteJobAsync(jobId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetJobsByUserIdAsync_ReturnsJobsList_WhenSuccessful()
        {
            // Arrange
            int userId = 1;
            var expectedJobs = new List<JobModel>
            {
                new JobModel { JobId = 1, JobName = "User  Job 1" },
                new JobModel { JobId = 2, JobName = "User  Job 2" }
            };

            // Setup mock HTTP response
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r =>
                        r.Method == HttpMethod.Get &&
                        r.RequestUri.ToString().Contains($"/jobs/users/{userId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(expectedJobs))
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            var authService = _mockAuthService.Object;
            var jobService = new JobService(client, authService);

            // Act
            var result = await jobService.GetJobsByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("User  Job 1", result[0].JobName);
        }

        [Fact]
        public async Task GetJobsAsync_HandlesEmptyResponse()
        {
            // Arrange
            // Setup mock HTTP response with empty list
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r =>
                        r.Method == HttpMethod.Get &&
                        r.RequestUri.ToString().Contains("/jobs/me")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("[]")
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            var authService = _mockAuthService.Object;
            var jobService = new JobService(client, authService);

            // Act
            var result = await jobService.GetJobsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetJobsAsync_HandlesInvalidResponse()
        {
            // Arrange
            // Setup mock HTTP response with invalid JSON
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r =>
                        r.Method == HttpMethod.Get &&
                        r.RequestUri.ToString().Contains("/jobs/me")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("Invalid JSON")
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);
            var authService = _mockAuthService.Object;
            var jobService = new JobService(client, authService);

            // Act and Assert
            await Assert.ThrowsAsync<JsonException>(() => jobService.GetJobsAsync());
        }

        [Fact]
        public async Task GetJobsAsync_HandlesNetworkError()
        {
            // Arrange
            // Setup mock HTTP response with network error
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(r =>
                        r.Method == HttpMethod.Get &&
                        r.RequestUri.ToString().Contains("/jobs/me")),
                    ItExpr.IsAny<CancellationToken>())
                .Throws(new HttpRequestException());

            var client = new HttpClient(mockHttpMessageHandler.Object);
            var authService = _mockAuthService.Object;
            var jobService = new JobService(client, authService);

            // Act and Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => jobService.GetJobsAsync());
        }
    }
}
