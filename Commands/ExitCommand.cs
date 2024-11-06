namespace Guestline.RoomRadar.Commands;

public sealed class ExitCommand : ICommand
{
    public string UsageExample => "exit";

    public (bool canExecute, string? errorMessage) CanExecute(string command)
    {
        return (command.Equals("exit", StringComparison.CurrentCultureIgnoreCase), "");
    }

    public Task<string> ExecuteAsync(string command)
    {
        Environment.Exit(1337);

        return Task.FromResult("exit");
    }
}