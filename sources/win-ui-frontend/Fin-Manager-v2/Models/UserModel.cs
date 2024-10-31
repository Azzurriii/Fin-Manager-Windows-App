using System.Text.Json.Serialization;

namespace Fin_Manager_v2.Models;

public class UserModel
{
    [JsonPropertyName("username")]
    public string Username
    {
        get; set;
    }

    [JsonPropertyName("email")]
    public string Email
    {
        get; set;
    }

    [JsonPropertyName("password")]
    public string Password
    {
        get; set;
    }
}