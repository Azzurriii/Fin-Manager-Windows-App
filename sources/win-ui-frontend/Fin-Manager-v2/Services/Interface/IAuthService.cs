using Fin_Manager_v2.Models;

namespace Fin_Manager_v2.Services.Interface;

public interface IAuthService
{
    Task<bool> LoginAsync(string username, string password);
    Task<bool> SignUpAsync(UserModel user);
    bool IsAuthenticated
    {
        get;
    }
    void Logout();
}