using System.Text.Json;
using FluentAssertions;
using Guestline.RoomRadar.Commands;
using Guestline.RoomRadar.Config;
using Guestline.RoomRadar.Services;

namespace Guestline.RoomRadar.Tests;

/// <summary>
/// Tests related to provided data from Guestline
/// </summary>
public sealed class AvailableCommandTests
{
    private const string executionErrorMessage = "ERROR, unable to execute command due to syntax issue.";
    private AvailableCommand cut;

    [OneTimeSetUp]
    public void Setup()
    {
        cut = new AvailableCommand(
            new DummyFileOpener(),
            new DummyConfigurationProvider());
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

    [TestCase("Availability(H1, 20240901, SGL)")]
    [TestCase("Availability(H1, 20240901-20240902, SGL)")]
    [TestCase("Availability(H1,20240901,SGL)")]
    [TestCase("Availability(H1,20240901-20240902,SGL)")]
    [TestCase("Availability( H1 , 20240901 , SGL )")]
    [TestCase("Availability( H1 , 20240901-20240902 , SGL )")]
    [TestCase("Availability(    H1    ,     20240901    ,    SGL   )")]
    [TestCase("Availability(    H1    ,     20240901-20240902    ,    SGL   )")]
    public void CanExecute_ProperCommandFormat_ReturnsAsTrue(string command)
    {
        // Act
        var (canExecute, msg) = cut.CanExecute(command);

        // Assert
        canExecute.Should().BeTrue();
        msg.Should().BeNull();
    }

    [TestCase("Availability(H1 20240901, SGL)")]
    [TestCase("Availability(H1, 20240901 SGL)")]
    [TestCase("Availability(H1, 20240901, SGL")]
    [TestCase("Availability(H1, 20240901, SGL")]
    [TestCase("Availability(H1, 2020901, SGL)")]
    [TestCase("Availability(20240901, SGL)")]
    [TestCase("Availability(H1, SGL)")]
    [TestCase("Availability(H1, 20240901)")]
    [TestCase("Availability(H1 20240901-20240902, SGL)")]
    [TestCase("Availability(H1, 20240901-20240902 SGL)")]
    [TestCase("Availability(H1, 20240901-20240902, SGL")]
    [TestCase("Availability(H1, 20240901-20240902, SGL")]
    [TestCase("Availability(H1, 2020901-20240902, SGL)")]
    [TestCase("Availability(H1, 20240901-2020902, SGL)")]
    [TestCase("Availability(20240901-20240902, SGL)")]
    [TestCase("Availability(H1, 20240901-20240902)")]
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

    [TestCase("Availability(, 20240901, SGL)")]
    [TestCase("Availability(20240901, SGL)")]
    [TestCase("Availability(H1, 20240901, )")]
    [TestCase("Availability(H1, 20240901)")]
    [TestCase("Availability(H1, , SGL)")]
    [TestCase("Availability(H1, SGL)")]
    [TestCase("Availability(H1)")]
    [TestCase("Availability()")]
    public async Task Execute_CommandWithoutParameters_ReturnsErrorMessage(string command)
    {
        // Act
        var result = await cut.ExecuteAsync(command);

        // Arrange
        result.Should().Be(executionErrorMessage);
    }

    [TestCase("Availability(H1, 20240901, SGL)", 2)]
    [TestCase("Availability(H1, 20240902, SGL)", 1)]
    [TestCase("Availability(H1, 20240903, SGL)", 1)]
    [TestCase("Availability(H1, 20240904, SGL)", 1)]
    [TestCase("Availability(H1, 20240905, SGL)", 2)]
    public async Task Execute_CommandWithAllParametersOk_ReturnsProperResponse(string command, int availableRoomsCount)
    {
        // Act
        var result = await cut.ExecuteAsync(command);

        // Arrange
        result.Should().NotBeNullOrWhiteSpace();
        result.Should().Be(OkResponse(availableRoomsCount));
    }

    [TestCase("Availability(H1, 20240901-20240901, SGL)", 2)]
    [TestCase("Availability(H1, 20240903-20240903, SGL)", 1)]
    [TestCase("Availability(H1, 20240901-20240902, SGL)", 2)]
    [TestCase("Availability(H1, 20240901-20240903, SGL)", 1)]
    [TestCase("Availability(H1, 20240903-20240904, SGL)", 1)]
    [TestCase("Availability(H1, 20240905-20240907, SGL)", 2)]
    public async Task Execute_CommandWithAllParametersOk_DateRange_ReturnsProperResponse(string command, int availableRoomsCount)
    {
        // Act
        var result = await cut.ExecuteAsync(command);

        // Arrange
        result.Should().NotBeNullOrWhiteSpace();
        result.Should().Be(OkResponse(availableRoomsCount));
    }

    private string OkResponse(int availableRoomsCount) => $"There is/are {availableRoomsCount} available room(s).";
}

internal sealed class DummyFileOpener : IFileOpener
{
    public string ReadAllFileContent(string path)
    {
        return File.ReadAllText(path.Contains("hotels") ? "./TestData/hotels.json" : "./TestData/bookings.json");
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

internal sealed class DummyConfigurationProvider : IConfigurationProvider
{
    public AppConfiguration AppConfiguration => new("hotels", "bookings");
}
