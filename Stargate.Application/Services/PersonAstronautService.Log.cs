using Microsoft.Extensions.Logging;

namespace Stargate.Application.Services;

public partial class PersonAstronautService
{
    // GetPeople
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Retrieving all people")]
    partial void LogRetrievingAllPeople();

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Retrieved {Count} people")]
    partial void LogRetrievedPeople(int count);

    // GetPersonByName
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Retrieving person by name: {Name}")]
    partial void LogRetrievingPersonByName(string name);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Person not found: {Name}")]
    partial void LogPersonNotFound(string name);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Retrieved person: {Name} (ID: {PersonId})")]
    partial void LogRetrievedPerson(string name, int personId);

    // CreatePerson
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Creating new person: {Name}")]
    partial void LogCreatingPerson(string name);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Duplicate person name detected: {Name}")]
    partial void LogDuplicatePersonName(string name);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Created person: {Name} (ID: {PersonId})")]
    partial void LogCreatedPerson(string name, int personId);

    // UpdatePerson
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Updating person: {OldName} to {NewName}")]
    partial void LogUpdatingPerson(string oldName, string newName);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Person not found for update: {Name}")]
    partial void LogPersonNotFoundForUpdate(string name);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Duplicate person name detected during update: {NewName}")]
    partial void LogDuplicatePersonNameDuringUpdate(string newName);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Updated person: {OldName} -> {NewName} (ID: {PersonId})")]
    partial void LogUpdatedPerson(string oldName, string newName, int personId);
}
