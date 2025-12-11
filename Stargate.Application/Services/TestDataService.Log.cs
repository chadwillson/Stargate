using Microsoft.Extensions.Logging;

namespace Stargate.Application.Services;

public partial class TestDataService
{
    // CreateBasicScenarioAsync
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Creating basic test scenario")]
    partial void LogCreatingBasicScenario();

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Created basic scenario: Person {PersonId}, Detail {DetailId}, Duty {DutyId}")]
    partial void LogCreatedBasicScenario(int personId, int detailId, int dutyId);

    // CreateComplexScenarioAsync
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Creating complex test scenario")]
    partial void LogCreatingComplexScenario();

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Created complex scenario: {PersonCount} persons with multiple duties")]
    partial void LogCreatedComplexScenario(int personCount);

    // CreateEdgeCaseScenarioAsync
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Creating edge case test scenario")]
    partial void LogCreatingEdgeCaseScenario();

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Created edge case scenario: Person without details and person without duties")]
    partial void LogCreatedEdgeCaseScenario();

    // CreateRetiredAstronautScenarioAsync
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Creating retired astronaut test scenario")]
    partial void LogCreatingRetiredAstronautScenario();

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Created retired astronaut scenario: Person {PersonId} with retirement date {RetirementDate:yyyy-MM-dd}")]
    partial void LogCreatedRetiredAstronautScenario(int personId, DateTime retirementDate);

    // CreateMultipleDutiesScenarioAsync
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Creating multiple duties test scenario")]
    partial void LogCreatingMultipleDutiesScenario();

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Created multiple duties scenario: Person {PersonId} with {DutyCount} duties")]
    partial void LogCreatedMultipleDutiesScenario(int personId, int dutyCount);

    // ClearAllTestDataAsync
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Clearing all test data")]
    partial void LogClearingAllTestData();

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Successfully cleared all test data")]
    partial void LogClearedAllTestData();

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Error clearing test data")]
    partial void LogErrorClearingTestData(Exception ex);

    // ResetToDefaultSeedDataAsync
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Resetting to default seed data")]
    partial void LogResettingToDefaultSeedData();

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Successfully reset to default seed data")]
    partial void LogResetToDefaultSeedData();

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Error resetting to default seed data")]
    partial void LogErrorResettingToDefaultSeedData(Exception ex);
}
