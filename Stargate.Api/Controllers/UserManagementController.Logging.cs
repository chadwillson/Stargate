using Microsoft.Extensions.Logging;

namespace Stargate.Api.Controllers
{
    public partial class UserManagementController
    {
        [LoggerMessage(
            EventId = 1,
            Level = LogLevel.Error,
            Message = "Error retrieving users")]
        partial void LogErrorRetrievingUsers(Exception ex);

        [LoggerMessage(
            EventId = 2,
            Level = LogLevel.Error,
            Message = "Error retrieving user {UserId}")]
        partial void LogErrorRetrievingUser(Exception ex, int userId);

        [LoggerMessage(
            EventId = 3,
            Level = LogLevel.Error,
            Message = "Error creating user {Username}")]
        partial void LogErrorCreatingUser(Exception ex, string username);

        [LoggerMessage(
            EventId = 4,
            Level = LogLevel.Error,
            Message = "Error updating user {UserId}")]
        partial void LogErrorUpdatingUser(Exception ex, int userId);

        [LoggerMessage(
            EventId = 5,
            Level = LogLevel.Error,
            Message = "Error deleting user {UserId}")]
        partial void LogErrorDeletingUser(Exception ex, int userId);

        [LoggerMessage(
            EventId = 6,
            Level = LogLevel.Error,
            Message = "Error retrieving roles")]
        partial void LogErrorRetrievingRoles(Exception ex);
    }
}
