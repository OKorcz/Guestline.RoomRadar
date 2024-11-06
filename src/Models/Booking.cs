using System.Text.Json.Serialization;
using Guestline.RoomRadar.Helpers;

namespace Guestline.RoomRadar.Models;

/// <summary>
/// Sealed class to increase performance of application
/// </summary>
public sealed class Booking
{
        [JsonPropertyName("hotelId")]
        public string HotelId { get; set; } = "";

        [JsonPropertyName("arrival")]
        public string ArrivalRaw { private get; init; } = "";

        public DateOnly Arrival => ArrivalRaw.ConvertToDateOnly();

        [JsonPropertyName("departure")]
        public string DepartureRaw { private get; init; } = "";

        public DateOnly Departure => DepartureRaw.ConvertToDateOnly();

        [JsonPropertyName("roomType")]
        public string RoomType { get; set; } = "";

        [JsonPropertyName("roomRate")]
        public string RoomRate { get; set; } = "";
}

