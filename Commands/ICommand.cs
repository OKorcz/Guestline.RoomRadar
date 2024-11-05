namespace Guestline.RoomRadar.Commands;

public interface ICommand
{
    string UsageExample { get; }
    (bool canExecute, string? errorMessage) CanExecute(string command);
    Task<string> ExecuteAsync(string command);
}