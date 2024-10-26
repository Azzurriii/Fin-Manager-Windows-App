using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Fin_Manager_v2.Models;
using Fin_Manager_v2.Services.Interface;

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
        var jsonContent = JsonSerializer.Serialize(loginData);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("http://localhost:3000/users/login", httpContent);
        if (response.IsSuccessStatusCode)
        {
            _isAuthenticated = true;
            return true;
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
}