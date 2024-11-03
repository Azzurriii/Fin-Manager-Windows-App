using System.Text.Json.Serialization;

namespace Fin_Manager_v2.Models;

public class TagModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string TagName { get; set; } = string.Empty;
}