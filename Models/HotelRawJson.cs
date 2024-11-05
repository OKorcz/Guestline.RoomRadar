/// <summary>
/// Sealed class to increase performance of application
/// </summary>
public sealed class HotelRawJson
{
    public string hotelId { get; set; }

    public string arrival { get; set; }

    public string departure { get; set; }

    public string roomType { get; set; }

    public string roomRate { get; set; }
}