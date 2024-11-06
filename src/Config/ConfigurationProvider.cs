using System.Text.Json;
using Guestline.RoomRadar.Services;

namespace Guestline.RoomRadar.Config;

public sealed record AppConfiguration(
    string HotelsDataPath,
    string BookingsDataPath
);

public sealed class ConfigurationProvider : IConfigurationProvider
{
    private const string configPath = "./config.json";
    private readonly Lazy<AppConfiguration> appConfiguration;

    public ConfigurationProvider()
    {
        appConfiguration = new(() => GetAppConfigurationAsync());
    }

    public AppConfiguration AppConfiguration => appConfiguration.Value;

    private AppConfiguration GetAppConfigurationAsync()
    {
        var fileContent = new FileOpener().ReadAllFileContent(configPath);

        return JsonSerializer.Deserialize<AppConfiguration>(fileContent) ?? new AppConfiguration("", "");
    }
}
