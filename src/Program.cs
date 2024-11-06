using Guestline.RoomRadar.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Guestline.RoomRadar.Commands;
using Guestline.RoomRadar.Workers;
using Microsoft.Extensions.Logging;
using Guestline.RoomRadar.Config;
using Spectre.Console;

AnsiConsole.Clear();

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{
    services.Configure<HostOptions>(options =>
        {
            options.ServicesStartConcurrently = true;
            options.ServicesStopConcurrently = true;
            options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
        });

    services.AddSingleton<IConfigurationProvider, ConfigurationProvider>();

    services.AddSingleton<AvailableCommand>();
    services.AddSingleton<HelpCommand>();
    services.AddSingleton<ExitCommand>();

    services.AddSingleton<IFileOpener, FileOpener>();

    services.AddHostedService<MainWorker>();

    services.AddSingleton<ICommand, AvailableCommand>(sp => sp.GetRequiredService<AvailableCommand>());
    services.AddSingleton<ICommand, ExitCommand>(sp => sp.GetRequiredService<ExitCommand>());
});

// Remove default host logger
builder.ConfigureLogging(l =>
{
    l.ClearProviders();
});

builder.Build().Run();