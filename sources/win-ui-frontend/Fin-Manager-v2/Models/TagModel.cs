using System.Text.Json.Serialization;

namespace Fin_Manager_v2.Models;

public class TagModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}