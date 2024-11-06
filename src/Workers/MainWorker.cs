using Guestline.RoomRadar.Commands;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

namespace Guestline.RoomRadar.Workers;

public sealed class MainWorker(IEnumerable<ICommand> commands, HelpCommand helpCommand) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        commands = commands.Append(helpCommand);

        AnsiConsole.Write(
            new FigletText("Guestline.RoomRadar")
            .Centered()
            .Color(Color.Purple));

        var command = AnsiConsole.Prompt(new TextPrompt<string>(">"));


        _ = Task.Run(async () =>
        {
            while (true)
            {
                if (!commands.Any(c => c.CanExecute(command).canExecute))
                {
                    AnsiConsole.MarkupLine($"Result: [yellow]There is no command '{command}'[/]");
                }
                else
                {
                    foreach (var cmd in commands)
                    {
                        if (cmd.CanExecute(command).canExecute)
                        {
                            var result = await cmd.ExecuteAsync(command);
                            AnsiConsole.MarkupLine($"[yellow]{result}[/]");

                            break;
                        }

                    }
                }

                command = AnsiConsole.Prompt(new TextPrompt<string>(">"));
            }
        }, cancellationToken);

        // To remove warning CS1998
        await Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}