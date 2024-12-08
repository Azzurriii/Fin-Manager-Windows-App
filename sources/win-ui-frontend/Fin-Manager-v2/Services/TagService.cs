using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.Services;
public class TagService : ITagService
{
    private readonly HttpClient _httpClient;

    public TagService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>Retrieves a list of tags asynchronously from the API.</summary>
    /// <returns>A list of TagModel objects representing the tags fetched from the API.</returns>
    /// <exception cref="HttpRequestException">Thrown when a network error occurs while getting tags.</exception>
    /// <exception cref="Exception">Thrown when an error occurs during the retrieval process.</exception>
    public async Task<List<TagModel>> GetTagsAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Fetching tags from API...");
            var response = await _httpClient.GetAsync("tags");

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

    /// <summary>
    /// Asynchronously retrieves a TagModel object based on the provided ID.
    /// </summary>
    /// <param name="id">The ID of the tag to retrieve.</param>
    /// <returns>
    /// A Task representing the asynchronous operation. The task result contains the TagModel object
    /// corresponding to the provided ID, or null if no such tag is found.
    /// </returns>
    public async Task<TagModel?> GetTagAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<TagModel>($"tags/{id}");
    }

    /// <summary>Creates a new tag asynchronously.</summary>
    /// <param name="name">The name of the tag to create.</param>
    /// <returns>A TagModel representing the newly created tag.</returns>
    /// <exception cref="Exception">Thrown when failed to create the tag.</exception>
    public async Task<TagModel> CreateTagAsync(string name)
    {
        var response = await _httpClient.PostAsJsonAsync("tags", new { name });
        return await response.Content.ReadFromJsonAsync<TagModel>()
               ?? throw new Exception("Failed to create tag");
    }

    /// <summary>Retrieves a list of tags based on the specified type asynchronously.</summary>
    /// <param name="type">The type of tags to retrieve.</param>
    /// <returns>A list of TagModel objects corresponding to the specified type, or an empty list if an error occurs.</returns>
    /// <exception cref="Exception">Thrown when an error occurs during the retrieval process.</exception>
    public async Task<List<TagModel>> GetTagsByTypeAsync(string type)
    {
        try 
        {
            return await _httpClient.GetFromJsonAsync<List<TagModel>>($"tags/type/{type}")
                   ?? new List<TagModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting tags by type: {ex.Message}");
            return new List<TagModel>();
        }
    }

    /// <summary>Deletes a tag asynchronously.</summary>
    /// <param name="id">The ID of the tag to be deleted.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteTagAsync(int id)
    {
        await _httpClient.DeleteAsync($"tags/{id}");
    }
}
