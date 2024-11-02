﻿using System.Net.Http.Json;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services.Interface;

namespace Fin_Manager_v2.Services
{
    public class TagService : ITagService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://localhost:3000";

        public TagService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TagModel>> GetTagsAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<List<TagModel>>($"{_baseUrl}/tags");
            return response ?? new List<TagModel>();
        }

        public async Task<TagModel?> GetTagAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<TagModel>($"{_baseUrl}/tags/{id}");
        }

        public async Task<TagModel> CreateTagAsync(string name)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/tags", new { name });
            return await response.Content.ReadFromJsonAsync<TagModel>()
                   ?? throw new Exception("Failed to create tag");
        }

        public async Task DeleteTagAsync(int id)
        {
            await _httpClient.DeleteAsync($"{_baseUrl}/tags/{id}");
        }
    }
}