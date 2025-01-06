using Fin_Manager_v2.Contracts.Services;

namespace Fin_Manager_v2.Services;

public class ApiConfiguration : IApiConfiguration
{
    public string BaseUrl => "https://fin-manager-api.up.railway.app/";
}