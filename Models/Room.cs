using System.Text.Json.Serialization;

namespace Guestline.RoomRadar.Models;

/// <summary>
/// Sealed class to increase performance of application
/// </summary>
public sealed class Room
{
        [JsonPropertyName("roomType")]
        public string RoomType { get; set; } = "";

        [JsonPropertyName("roomId")]
        public string Id { get; set; } = "";
}

