using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.Services.Interface;

public interface IAuthService
{
    Task<bool> LoginAsync(string username, string password);
    Task<HttpResponseMessage> SignUpAsync(UserModel user);
    bool IsAuthenticated
    {
        get;
    }
    void Logout();

    public string GetAccessToken();

    Task FetchUserIdAsync();

    public int? GetUserId();
}