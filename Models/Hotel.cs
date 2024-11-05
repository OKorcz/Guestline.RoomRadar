using System.Text.Json.Serialization;

namespace Guestline.RoomRadar.Models;

/// <summary>
/// Sealed class to increase performance of application
/// </summary>
public sealed class Hotel
{

        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("roomTypes")]
        public List<RoomType> RoomTypes { get; set; } = [];

        [JsonPropertyName("rooms")]
        public List<Room> Rooms { get; set; } = [];
}