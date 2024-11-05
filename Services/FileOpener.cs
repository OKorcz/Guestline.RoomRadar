
using System.Text.Json;

namespace Guestline.RoomRadar.Services;

public sealed class FileOpener : IFileOpener
{
    public string ReadAllFileContent(string path) =>
        File.ReadAllText(path);

    public async Task<T?> ReadAllFileContentAsJsonObjectAsync<T>(string path)
    {
        var content = await ReadAllFileContentAsync(path);

        return JsonSerializer.Deserialize<T>(content);
    }

    public Task<string> ReadAllFileContentAsync(string path) =>
        File.ReadAllTextAsync(path);
}