using System.Text.RegularExpressions;
using Guestline.RoomRadar.Config;
using Guestline.RoomRadar.Helpers;
using Guestline.RoomRadar.Models;
using Guestline.RoomRadar.Services;

namespace Guestline.RoomRadar.Commands;

public sealed class AvailableCommand(IFileOpener fileOpener, IConfigurationProvider configurationProvider) : ICommand
{

#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
    private readonly Regex commandParser = new(@"Availability\W*\(\W*(?<hid>\S[^,]+),\D*((?<date>\d{8})|(?<daterange>\d{8}-\d{8}))\s*,\s*(?<roomtype>\w+)\s*\)", RegexOptions.Compiled);

#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.

    public string UsageExample => "[green]Availability[/]([purple]H1[/], [purple]20240901[/] or [purple]20240901-20240903[/], [purple]SGL[/])";

    public (bool canExecute, string? errorMessage) CanExecute(string command)
    {
        var match = commandParser.Match(command);
        if (match.Success)
        {
            return (true, null!);
        }
        else
        {
            return (false, $"Unable to recognize this command as {nameof(AvailableCommand)}");
        }
    }

    public async Task<string> ExecuteAsync(string command)
    {
        // Retrieve appconfiguration
        var configuration = configurationProvider.AppConfiguration;

        var match = commandParser.Match(command);

        if (!match.Success ||
            !match.Groups["hid"].Success ||
            !match.Groups["roomtype"].Success ||
            (!match.Groups["date"].Success && !match.Groups["daterange"].Success) ||
            (match.Groups["date"].Success && match.Groups["daterange"].Success))
        {
            return "ERROR, unable to execute command due to syntax issue.";
        }

        var hotelId = match.Groups["hid"].Value;
        var roomType = match.Groups["roomtype"].Value;

        // Start reading data from json files
        var readHotelsTask = fileOpener.ReadAllFileContentAsJsonObjectAsync<List<Hotel>>(configuration.HotelsDataPath);
        var readBookingsTask = fileOpener.ReadAllFileContentAsJsonObjectAsync<List<Booking>>(configuration.BookingsDataPath);

        // Common checks
        var hotels = await readHotelsTask;

        if (hotels == null || hotels.Count == 0)
            return "There is no any hotels in database.";

        var hotel = hotels.SingleOrDefault(h => h.Id == hotelId);

        if (hotel == null)
            return "There is no hotel with id:" + hotelId;

        if (!hotel.RoomTypes.Any(rt => rt.Code == roomType))
            return "This hotel doesn't have room with type of " + roomType;

        var bookings = await readBookingsTask;

        if (bookings == null || bookings.Count == 0)
            return "There is no any bookings in database.";

        var bookingsHotelRoomTypeScoped = bookings.Where(b =>
            b.HotelId == hotelId &&
            b.RoomType == roomType).ToList();

        // Single date
        if (match.Groups["date"].Success)
        {
            var date = match.Groups["date"].Value.ConvertToDateOnly();
            var selectedDateTicks = date.ToTicks();

            // Should be 0 if available
            var occupiedBookings = bookingsHotelRoomTypeScoped.Where(b =>
                b.Arrival.ToTicks() <= selectedDateTicks &&
                b.Departure.ToTicks() > selectedDateTicks);

            var availableRoomsCount = hotel.Rooms.Where(r => r.RoomType == roomType).Count() - occupiedBookings.Count();

            if (availableRoomsCount > 0)
            {
                return $"There is/are {availableRoomsCount} available room(s).";
            }

            return "Lack of available rooms.";
        }

        // Date range
        if (match.Groups["daterange"].Success)
        {
            var daterange = match.Groups["daterange"].Value;
            var splittedDateRange = daterange.Split('-');

            var fromDate = splittedDateRange[0].ConvertToDateOnly().ToTicks();
            var toDate = splittedDateRange[1].ConvertToDateOnly().ToTicks();

            // Should be 0 if available
            var occupiedBookings = bookingsHotelRoomTypeScoped.Where(b =>
                (b.Arrival.ToTicks() < fromDate &&
                b.Departure.ToTicks() > fromDate) ||
                (b.Arrival.ToTicks() < toDate &&
                b.Departure.ToTicks() > toDate) ||
                (b.Arrival.ToTicks() > toDate &&
                b.Departure.ToTicks() < fromDate));

            var availableRoomsCount = hotel.Rooms.Where(r => r.RoomType == roomType).Count() - occupiedBookings.Count();

            if (availableRoomsCount > 0)
            {
                return $"There is/are {availableRoomsCount} available room(s).";
            }

            return "Lack of available rooms.";
        }


        return await Task.Run(() => "remove error and warning");
    }
}