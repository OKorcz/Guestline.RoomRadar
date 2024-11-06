using System.Text.Json;
using Guestline.RoomRadar.Services;

namespace Guestline.RoomRadar.Config;

public sealed record AppConfiguration(
    string HotelsDataPath,
    string BookingsDataPath
);

public static class ConfigurationProvider
{
    private const string configPath = "./config.json";
    private static readonly Lazy<AppConfiguration> appConfiguration = new(() => GetAppConfigurationAsync());

    public static AppConfiguration AppConfiguration => appConfiguration.Value;

    private static AppConfiguration GetAppConfigurationAsync()
    {
        var fileContent = new FileOpener().ReadAllFileContent(configPath);

        return JsonSerializer.Deserialize<AppConfiguration>(fileContent) ?? new AppConfiguration("", "");
    }
}