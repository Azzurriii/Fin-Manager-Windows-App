using System.Net.Http;
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


    public async Task<bool> SignUpAsync(UserModel user)
    {
        var response = await _httpClient.PostAsJsonAsync("http://localhost:3000/users", user);
        return response.IsSuccessStatusCode;
    }

    public void Logout()
    {
        _isAuthenticated = false;
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
}