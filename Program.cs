﻿using System.Text.Json;
using Guestline.RoomRadar.Config;
using Guestline.RoomRadar.Models;
using Guestline.RoomRadar.Services;
using Spectre.Console;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Guestline.RoomRadar.Commands;
using Guestline.RoomRadar.Workers;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{
    services.Configure<HostOptions>(options =>
        {
            options.ServicesStartConcurrently = true;
            options.ServicesStopConcurrently = true;
            options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
        });

    services.AddSingleton<AvailableCommand>();
    services.AddSingleton<IFileOpener, FileOpener>();

    services.AddHostedService<MainWorker>();

    services.AddSingleton<ICommand, AvailableCommand>(sp => sp.GetRequiredService<AvailableCommand>());
});

// Remove default host logger
builder.ConfigureLogging(l =>
{
    l.ClearProviders();
});

builder.Build().Run();