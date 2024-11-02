using System.Text.Json.Serialization;

namespace Fin_Manager_v2.Models;

public class TagModel
{
    [JsonPropertyName("tag_id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("is_selected")]
    public bool IsSelected { get; set; }
}