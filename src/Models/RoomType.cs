using System.Text.Json.Serialization;

namespace Guestline.RoomRadar.Models;

/// <summary>
/// Sealed class to increase performance of application
/// </summary>
public sealed class RoomType
{
    [JsonPropertyName("code")]
    public string Code { get; set; } = "";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("amenities")]
    public List<string> Amenities { get; set; } = [];

    [JsonPropertyName("features")]
    public List<string> Features { get; set; } = [];
}

