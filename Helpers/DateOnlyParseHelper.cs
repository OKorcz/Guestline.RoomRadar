namespace Guestline.RoomRadar.Helpers;

public static class DateOnlyParseHelper
{
    public static DateOnly ConvertToDateOnly(this string dateString)
    {
        try
        {
            return DateOnly.ParseExact(dateString, "yyyyMMdd");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
            return DateOnly.MinValue;
        }
    }
}