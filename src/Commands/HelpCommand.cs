using Spectre.Console;

namespace Guestline.RoomRadar.Commands;

public sealed class HelpCommand(IEnumerable<ICommand> commands) : ICommand
{
    public string UsageExample => "help";

    public (bool canExecute, string? errorMessage) CanExecute(string command)
    {
        return (command.Equals("help", StringComparison.CurrentCultureIgnoreCase), "");
    }

    public Task<string> ExecuteAsync(string command)
    {

        return Task.FromResult(string.Join(Environment.NewLine, commands.Select(c => c.UsageExample).Append(UsageExample)));
    }
}