namespace Guestline.RoomRadar.Helpers;

public static class DateOnlyTickHelper
{
    public static long ToTicks(this DateOnly date)
    {
        return date.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(0))).Ticks;
    }
}