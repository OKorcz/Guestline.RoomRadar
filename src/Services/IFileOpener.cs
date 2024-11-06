namespace Guestline.RoomRadar.Services;

public interface IFileOpener
{
    Task<string> ReadAllFileContentAsync(string path);
    string ReadAllFileContent(string path);

    Task<T?> ReadAllFileContentAsJsonObjectAsync<T>(string path);
}