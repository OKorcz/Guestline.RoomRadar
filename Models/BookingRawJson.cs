/// <summary>
/// Sealed class to increase performance of application
/// </summary>
public sealed class RoomRawJson
{
    public string roomType { get; set; }

    public string roomId { get; set; }
}

/// <summary>
/// Sealed class to increase performance of application
/// </summary>
public sealed class RoomTypeRawJson
{
    public string code { get; set; }

    public string description { get; set; }

    public List<string> amenities { get; set; }

    public List<string> features { get; set; }
}

/// <summary>
/// Sealed class to increase performance of application
/// </summary>
public sealed class BookingRawJson
{
    public string id { get; set; }

    public string name { get; set; }

    public List<RoomTypeRawJson> roomTypes { get; set; }

    public List<RoomRawJson> rooms { get; set; }
}

