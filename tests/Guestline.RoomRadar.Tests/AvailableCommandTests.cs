using System.Text.Json;
using FluentAssertions;
using Guestline.RoomRadar.Commands;
using Guestline.RoomRadar.Services;

namespace Guestline.RoomRadar.Tests;

public class AvailableCommandTests
{
    private AvailableCommand cut;

    [SetUp]
    public void Setup()
    {
        cut = new AvailableCommand(new DummyFileOpener());
    }

    [TestCase("hotels")]
    [TestCase("bookings")]
    [TestCase("any_other_path")]
    public void ReadFile_DummyFileOpener_NotNullOrEmpty(string path)
    {
        // Arrange
        var dummyFileOpener = new DummyFileOpener();

        // Act
        var result = dummyFileOpener.ReadAllFileContent(path);

        // Assert
        result.Should().NotBeNullOrEmpty();
    }

    [TestCase("Availability(H1, 20240901, SBD)")]
    [TestCase("Availability(H1, 20240901-20240902, SBD)")]
    [TestCase("Availability(H1,20240901,SBD)")]
    [TestCase("Availability(H1,20240901-20240902,SBD)")]
    [TestCase("Availability( H1 , 20240901 , SBD )")]
    [TestCase("Availability( H1 , 20240901-20240902 , SBD )")]
    [TestCase("Availability(    H1    ,     20240901    ,    SBD   )")]
    [TestCase("Availability(    H1    ,     20240901-20240902    ,    SBD   )")]
    public void CanExecute_ProperCommandFormat_ReturnsAsTrue(string command)
    {
        // Act
        var (canExecute, msg) = cut.CanExecute(command);

        // Assert
        canExecute.Should().BeTrue();
        msg.Should().BeNull();
    }

    [TestCase("Availability(H1 20240901, SBD)")]
    [TestCase("Availability(H1, 20240901 SBD)")]
    [TestCase("Availability(H1, 20240901, SBD")]
    [TestCase("Availability(H1, 20240901, SBD")]
    [TestCase("Availability(H1, 2020901, SBD)")]
    [TestCase("Availability(20240901, SBD)")]
    [TestCase("Availability(H1, SBD)")]
    [TestCase("Availability(H1, 20240901)")]
    [TestCase("any_other_command")]
    public void CanExecute_WrongCommandFormat_ReturnsFalse(string command)
    {
        // Act
        var (canExecute, msg) = cut.CanExecute(command);

        // Assert
        canExecute.Should().BeFalse();
        msg.Should().NotBeNull();
    }

    [Test]
    public void UsageExample_NotNull()
    {
        // Assert
        cut.UsageExample.Should().NotBeNullOrEmpty();
    }
}

internal class DummyFileOpener : IFileOpener
{
    public string ReadAllFileContent(string path)
    {
        return File.ReadAllText(path == "hotels" ? "./TestData/hotels.json" : "./TestData/bookings.json");
    }

    public Task<T?> ReadAllFileContentAsJsonObjectAsync<T>(string path)
    {
        return Task.FromResult(JsonSerializer.Deserialize<T>(ReadAllFileContent(path)));
    }

    public Task<string> ReadAllFileContentAsync(string path)
    {
        return Task.FromResult(ReadAllFileContent(path));
    }
}