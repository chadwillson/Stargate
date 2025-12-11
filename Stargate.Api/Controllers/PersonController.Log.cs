namespace Stargate.Api.Controllers;

public partial class PersonController
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Retrieving all people")]
    partial void LogRetrievingAllPeople();

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to retrieve all people")]
    partial void LogFailedToRetrieveAllPeople(Exception ex);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Retrieving person by name: {Name}")]
    partial void LogRetrievingPersonByName(string name);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to retrieve person by name: {Name}")]
    partial void LogFailedToRetrievePersonByName(Exception ex, string name);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Creating person: {Name}")]
    partial void LogCreatingPerson(string name);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to create person: {Name}")]
    partial void LogFailedToCreatePerson(Exception ex, string name);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Updating person: {Name} to {NewName}")]
    partial void LogUpdatingPerson(string name, string newName);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to update person: {Name}")]
    partial void LogFailedToUpdatePerson(Exception ex, string name);
}
