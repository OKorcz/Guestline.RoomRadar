
using System.Text.RegularExpressions;
using Guestline.RoomRadar.Config;
using Guestline.RoomRadar.Models;
using Guestline.RoomRadar.Services;

namespace Guestline.RoomRadar.Commands;

public sealed class AvailableCommand(IFileOpener fileOpener) : ICommand
{

#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
    private readonly Regex commandParser = new(@"Availability\W*\(\W*(?<hid>\S[^,]+),\D*((?<date>\d{8})|(?<daterange>\d{8}-\d{8}))[, ]*\W*(?<roomtype>\w+)\s*\)", RegexOptions.Compiled);

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
        var configuration = ConfigurationProvider.AppConfiguration;

        var match = commandParser.Match(command);

        if (!match.Success ||
            !match.Groups["hid"].Success ||
            !match.Groups["roomtype"].Success ||
            (!match.Groups["date"].Success && !match.Groups["daterange"].Success) ||
            (match.Groups["date"].Success && match.Groups["daterange"].Success))
        {
            return "ERROR, unable to parse command";
        }

        var hotelId = match.Groups["hid"].Value;
        var roomType = match.Groups["roomtype"].Value;

        // Start reading data from json files
        var readHotelsTask = fileOpener.ReadAllFileContentAsJsonObjectAsync<List<Hotel>>(configuration.HotelsDataPath);
        var readBookingsTask = fileOpener.ReadAllFileContentAsJsonObjectAsync<List<Booking>>(configuration.BookingsDataPath);

        // Await for read tasks
        await Task.WhenAll(readHotelsTask, readBookingsTask);

        // Retrieve data from tasks
        var hotels = readHotelsTask.Result;
        var bookings = readBookingsTask.Result;

        // Common checks
        if (hotels == null || hotels.Count == 0)
            return "There is no any hotels in database.";

        var hotel = hotels.SingleOrDefault(h => h.Id == hotelId);

        if (hotel == null)
            return "There is no hotel with id:" + hotelId;

        if (!hotel.RoomTypes.Any(rt => rt.Code == roomType))
            return "This hotel doesn't have room with type of " + roomType;

        // Single date
        var singleDate = match.Groups["date"].Value;

        // Date range
        var daterange = match.Groups["daterange"].Value;


        return await Task.Run(() => "remove error and warning");
    }
}