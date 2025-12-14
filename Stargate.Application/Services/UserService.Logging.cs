using Microsoft.Extensions.Logging;

namespace Stargate.Application.Services
{
    public partial class UserService
    {
        [LoggerMessage(
            EventId = 1,
            Level = LogLevel.Warning,
            Message = "Authentication failed for username: {Username} - User not found or inactive")]
        partial void LogAuthenticationFailedUserNotFoundOrInactive(string username);

        [LoggerMessage(
            EventId = 2,
            Level = LogLevel.Information,
            Message = "User retrieved from database - Username: {Username}, RoleId: {RoleId}, Role: {RoleName}")]
        partial void LogUserRetrievedFromDatabase(string username, int roleId, string roleName);

        [LoggerMessage(
            EventId = 3,
            Level = LogLevel.Warning,
            Message = "Authentication failed for username: {Username} - Invalid password")]
        partial void LogAuthenticationFailedInvalidPassword(string username);

        [LoggerMessage(
            EventId = 4,
            Level = LogLevel.Information,
            Message = "UserResponse created - Username: {Username}, RoleId: {RoleId}, RoleName: {RoleName}")]
        partial void LogUserResponseCreated(string username, int roleId, string roleName);
    }
}
