using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Fin_Manager_v2.Contracts.Services;
using Fin_Manager_v2.Models;
using Windows.Storage;

namespace Fin_Manager_v2.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private bool _isAuthenticated;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public bool IsAuthenticated => _isAuthenticated;

    /// <summary>Attempts to log in with the provided username and password asynchronously.</summary>
    /// <param name="username">The username for the login attempt.</param>
    /// <param name="password">The password for the login attempt.</param>
    /// <returns>True if the login was successful, false otherwise.</returns>
    public async Task<bool> LoginAsync(string username, string password)
    {
        var loginData = new { username, password };

        // Send login request as JSON and get response
        var response = await _httpClient.PostAsJsonAsync("users/login", loginData);
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

    /// <summary>Signs up a new user asynchronously.</summary>
    /// <param name="user">The user model containing the user information.</param>
    /// <returns>An HTTP response message representing the result of the sign-up operation.</returns>
    public async Task<HttpResponseMessage> SignUpAsync(UserModel user)
    {
        var response = await _httpClient.PostAsJsonAsync("users", user);
        return response;
    }

    /// <summary>
    /// Logs out the current user by setting the authentication status to false and removing the access token from local settings.
    /// </summary>
    public void Logout()
    {
        _isAuthenticated = false;
        // Delete the access token from local storage
        var localSettings = ApplicationData.Current.LocalSettings;
        localSettings.Values.Remove("AccessToken");
    }

    /// <summary>
    /// Asynchronously fetches the user ID from the server and stores it in local settings.
    /// </summary>
    /// <remarks>
    /// This method retrieves the access token from local settings, sends a GET request to the "users/me" endpoint with the token for authentication,
    /// reads the response content as JSON, extracts the user ID, and stores it in local settings.
    /// </remarks>
    /// <exception cref="Exception">Thrown when an error occurs during the process.</exception>
    /// <seealso cref="HttpRequestMessage"/>
    /// <seealso cref="HttpResponseMessage"/>
    /// <seealso cref="JsonElement"/>
    /// <seealso cref="AuthenticationHeaderValue"/>
    /// <seealso cref="ApplicationData.LocalSettings"/>
    /// <seealso cref="HttpClient.SendAsync HttpRequestMessage"/>
    public async Task FetchUserIdAsync()
    {
        try
        {
            // Get the access token from local storage
            var localSettings = ApplicationData.Current.LocalSettings;
            var accessToken = localSettings.Values["AccessToken"] as string;

            // Prepare the request message with authorization header if token is available
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "users/me");

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

                if (userData != null && userData.TryGetValue("email", out var email))
                {
                    // Check if email is of type JsonElement and extract the string value
                    if (email is JsonElement emailElement && emailElement.ValueKind == JsonValueKind.String)
                    {
                        string emailString = emailElement.GetString(); // Get the string value
                        localSettings.Values["Email"] = emailString; // Store the email
                    }
                    else
                    {
                        Console.WriteLine("Email is not in the expected format.");
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

    /// <summary>
    /// Retrieves the access token from the local settings.
    /// </summary>
    /// <returns>The access token if found, otherwise null.</returns>
    public string GetAccessToken()
    {
        var localSettings = ApplicationData.Current.LocalSettings;

        if (localSettings.Values.TryGetValue("AccessToken", out var token))
        {
            return token as string;
        }

        return null;
    }

    /// <summary>Retrieves the user ID stored in local settings.</summary>
    /// <returns>The user ID if found, null otherwise.</returns>
    public int? GetUserId()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        return localSettings.Values["UserId"] as int?;
    }

    public string GetUserEmail()
    {
        var localSettings = ApplicationData.Current.LocalSettings;
        return localSettings.Values["Email"] as string;
    }
}
