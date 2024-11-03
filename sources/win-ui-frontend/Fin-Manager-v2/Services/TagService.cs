using System.Net.Http.Json;
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
            try 
            {
                System.Diagnostics.Debug.WriteLine("Fetching tags from API...");
                var response = await _httpClient.GetAsync($"{_baseUrl}/tags");
                
                if (!response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"API returned status code: {response.StatusCode}");
                    return new List<TagModel>();
                }

                var tags = await response.Content.ReadFromJsonAsync<List<TagModel>>();
                System.Diagnostics.Debug.WriteLine($"Successfully fetched {tags?.Count ?? 0} tags");
                return tags ?? new List<TagModel>();
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Network error getting tags: {ex.Message}");
                return new List<TagModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting tags: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return new List<TagModel>();
            }
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
