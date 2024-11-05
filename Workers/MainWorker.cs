using Guestline.RoomRadar.Commands;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

namespace Guestline.RoomRadar.Workers;

public sealed class MainWorker(IEnumerable<ICommand> commands) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        AnsiConsole.Write(
            new FigletText("Guestline.RoomRadar")
            .Centered()
            .Color(Color.Purple));

        var command = Console.ReadLine();

        var commandExecuted = false;
        while (command != null && !command.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
        {
            commandExecuted = false;

            foreach (var cmd in commands)
            {
                if (cmd.CanExecute(command).canExecute)
                {
                    var result = await cmd.ExecuteAsync(command);
                    AnsiConsole.MarkupLine($"Result: [yellow]{result}[/]");

                    commandExecuted = true;
                    break;
                }
            }
            if (!commandExecuted)
                switch (command)
                {
                    case "help":
                        AnsiConsole.MarkupLine("Possible commands:");
                        foreach (var cmd in commands)
                        {
                            AnsiConsole.MarkupLine($"[green]{cmd.UsageExample}[/]");
                        }
                        AnsiConsole.MarkupLine("[green]help[/]");
                        AnsiConsole.MarkupLine("[green]exit[/]");
                        break;
                    default:
                        AnsiConsole.MarkupLine($"[red]{command} command not supported! Type 'help' to get all available commands[/]");
                        break;
                }

            command = Console.ReadLine();
        }

        Environment.Exit(1337);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}