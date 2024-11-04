using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services.Interface;
using Windows.Storage;

namespace Fin_Manager_v2.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private bool _isAuthenticated;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:3000/") };
    }

    public bool IsAuthenticated => _isAuthenticated;

    public async Task<bool> LoginAsync(string username, string password)
    {
        var loginData = new { username, password };

        // Send login request as JSON and get response
        var response = await _httpClient.PostAsJsonAsync("http://localhost:3000/users/login", loginData);
        if (response.IsSuccessStatusCode)
        {
            // Automatically deserialize JSON response to a Dictionary
            var responseData = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            if (responseData != null && responseData.TryGetValue("access_token", out var accessToken))
            {
                // Save the token in local storage
                var localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["AccessToken"] = accessToken;

                _isAuthenticated = true;
                return true;
            }
        }

        _isAuthenticated = false;
        return false;
    }


    public async Task<HttpResponseMessage> SignUpAsync(UserModel user)
    {
        var response = await _httpClient.PostAsJsonAsync("http://localhost:3000/users", user);
        return response;
    }

    public void Logout()
    {
        _isAuthenticated = false;
    }

    public async Task FetchUserIdAsync()
    {
        try
        {
            // Get the access token from local storage
            var localSettings = ApplicationData.Current.LocalSettings;
            var accessToken = localSettings.Values["AccessToken"] as string;

            // Prepare the request message with authorization header if token is available
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/users/me");

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await _httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                // Retrieve user data from the response
                var userData = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

                if (userData != null && userData.TryGetValue("id", out var userId))
                {
                    // Check if userId is of type int and cast it safely
                    if (userId is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Number)
                    {
                        int id = jsonElement.GetInt32(); // Get the integer value
                        localSettings.Values["UserId"] = id; // Store the integer userId
                    }
                    else if (userId is double doubleId) // In case the ID is a double
                    {
                        localSettings.Values["UserId"] = (int)doubleId; // Cast to int and store
                    }
                    else
                    {
                        Console.WriteLine("User ID is not in the expected format.");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching user ID: {ex.Message}");
        }
    }


    public string GetAccessToken()
    {
        var localSettings = ApplicationData.Current.LocalSettings;

        if (localSettings.Values.TryGetValue("AccessToken", out var token))
        {
            return token as string;
        }

        return null;
    }

    public int? GetUserId()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        return localSettings.Values["UserId"] as int?;
    }
}