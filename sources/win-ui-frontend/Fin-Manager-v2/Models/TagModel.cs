using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace Fin_Manager_v2.Models;

public partial class TagModel : ObservableObject
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string TagName { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [ObservableProperty]
    private bool _isSelected;
}