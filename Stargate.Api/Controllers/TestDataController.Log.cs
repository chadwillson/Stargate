namespace Stargate.Api.Controllers;

public partial class TestDataController
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Creating test data scenario: {Scenario}")]
    partial void LogCreatingScenario(string scenario);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Successfully created scenario: {Scenario} with {PersonCount} person(s)")]
    partial void LogScenarioCreated(string scenario, int personCount);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to create scenario: {Scenario}")]
    partial void LogScenarioCreationFailed(string scenario, Exception ex);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Clearing all test data")]
    partial void LogClearingAllData();

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Successfully cleared all test data")]
    partial void LogDataCleared();

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to clear test data")]
    partial void LogClearDataFailed(Exception ex);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Resetting database to default seed data")]
    partial void LogResettingToDefaultData();

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Successfully reset database to default seed data")]
    partial void LogDataReset();

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to reset database to default seed data")]
    partial void LogResetDataFailed(Exception ex);
}
