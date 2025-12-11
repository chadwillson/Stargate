namespace Stargate.Api.Controllers;

public partial class AstronautDutyController
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Retrieving astronaut duties for: {Name}")]
    partial void LogRetrievingAstronautDuties(string name);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to retrieve astronaut duties for: {Name}")]
    partial void LogFailedToRetrieveAstronautDuties(Exception ex, string name);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Creating astronaut duty for: {Name}, Title: {DutyTitle}")]
    partial void LogCreatingAstronautDuty(string name, string dutyTitle);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to create astronaut duty for: {Name}")]
    partial void LogFailedToCreateAstronautDuty(Exception ex, string name);
}
