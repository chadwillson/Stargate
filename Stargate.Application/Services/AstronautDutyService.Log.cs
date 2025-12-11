using Microsoft.Extensions.Logging;

namespace Stargate.Application.Services;

public partial class AstronautDutyService
{
    // GetAstronautDutiesByName
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Retrieving astronaut duties for: {Name}")]
    partial void LogRetrievingAstronautDutiesByName(string name);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "No people found matching: {Name}")]
    partial void LogNoPeopleFound(string name);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Retrieved {Count} people with duties for search: {Name}")]
    partial void LogRetrievedPeopleWithDuties(int count, string name);

    // CreateAstronautDuty
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Creating astronaut duty for: {Name}, Title: {DutyTitle}, Rank: {Rank}")]
    partial void LogCreatingAstronautDuty(string name, string dutyTitle, string rank);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Created new person: {Name} (ID: {PersonId})")]
    partial void LogCreatedNewPerson(string name, int personId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Created astronaut duty (ID: {DutyId}) for {Name}: {Rank} - {DutyTitle}")]
    partial void LogCreatedAstronautDuty(int dutyId, string name, string rank, string dutyTitle);
}
