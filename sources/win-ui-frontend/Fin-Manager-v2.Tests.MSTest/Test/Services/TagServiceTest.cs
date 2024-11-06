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
    public class TagServiceTests
    {
        private MockHttpMessageHandler _mockHttp;
        private ITagService _tagService;

        [TestInitialize]
        public void Initialize()
        {
            _mockHttp = new MockHttpMessageHandler();
            var client = new HttpClient(_mockHttp) { BaseAddress = new Uri("http://localhost/") };
            _tagService = new TagService(client);
        }

        [TestMethod]
        public async Task GetTagsAsync_ValidRequest_ShouldReturnTags()
        {
            // Arrange
            var expectedTags = new List<TagModel>
            {
                new TagModel { Id = 1, TagName = "Finance" },
                new TagModel { Id = 2, TagName = "Groceries" }
            };

            _mockHttp.When(HttpMethod.Get, "*/tags")
                     .Respond("application/json", JsonSerializer.Serialize(expectedTags));

            // Act
            var result = await _tagService.GetTagsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(expectedTags[0].TagName, result[0].TagName);
        }

        [TestMethod]
        public async Task GetTagsAsync_ApiError_ShouldReturnEmptyList()
        {
            // Arrange
            _mockHttp.When(HttpMethod.Get, "*/tags")
                     .Respond(System.Net.HttpStatusCode.InternalServerError);

            // Act
            var result = await _tagService.GetTagsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task GetTagAsync_ValidId_ShouldReturnTag()
        {
            // Arrange
            int tagId = 1;
            var expectedTag = new TagModel { Id = tagId, TagName = "Finance" };

            _mockHttp.When(HttpMethod.Get, $"*/tags/{tagId}")
                     .Respond("application/json", JsonSerializer.Serialize(expectedTag));

            // Act
            var result = await _tagService.GetTagAsync(tagId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedTag.TagName, result.TagName);
        }

        [TestMethod]
        public async Task GetTagAsync_InvalidId_ShouldReturnNull()
        {
            // Arrange
            int tagId = 999;

            _mockHttp.When(HttpMethod.Get, $"*/tags/{tagId}")
                     .Respond(System.Net.HttpStatusCode.NotFound);

            // Act
            var result = await _tagService.GetTagAsync(tagId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task CreateTagAsync_ValidRequest_ShouldReturnCreatedTag()
        {
            // Arrange
            string tagName = "Health";
            var expectedTag = new TagModel { Id = 3, TagName = tagName };

            _mockHttp.When(HttpMethod.Post, "*/tags")
                     .WithJsonContent(new { name = tagName })
                     .Respond("application/json", JsonSerializer.Serialize(expectedTag));

            // Act
            var result = await _tagService.CreateTagAsync(tagName);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedTag.TagName, result.TagName);
        }

        [TestMethod]
        public async Task CreateTagAsync_ApiError_ShouldThrowException()
        {
            // Arrange
            string tagName = "Health";

            _mockHttp.When(HttpMethod.Post, "*/tags")
                     .Respond(System.Net.HttpStatusCode.InternalServerError);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _tagService.CreateTagAsync(tagName));
        }

        [TestMethod]
        public async Task DeleteTagAsync_ValidId_ShouldSucceed()
        {
            // Arrange
            int tagId = 1;

            _mockHttp.When(HttpMethod.Delete, $"*/tags/{tagId}")
                     .Respond(System.Net.HttpStatusCode.NoContent);

            // Act
            await _tagService.DeleteTagAsync(tagId);

            // Assert
            // No exception should be thrown
        }

        [TestMethod]
        public async Task DeleteTagAsync_ApiError_ShouldThrowException()
        {
            // Arrange
            int tagId = 999;

            _mockHttp.When(HttpMethod.Delete, $"*/tags/{tagId}")
                     .Respond(System.Net.HttpStatusCode.InternalServerError);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(() => _tagService.DeleteTagAsync(tagId));
        }
    }
}
