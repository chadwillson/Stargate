namespace Stargate.Api.Controllers;

public partial class AuthController
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Login attempt for user: {Username}")]
    partial void LogLoginAttempt(string username);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed login attempt for user: {Username}")]
    partial void LogFailedLoginAttempt(string username);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Successful login for user: {Username}")]
    partial void LogSuccessfulLogin(string username);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Login failed for user: {Username}")]
    partial void LogLoginError(Exception ex, string username);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "User logged out, token revoked")]
    partial void LogTokenRevoked();

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Logout failed")]
    partial void LogLogoutError(Exception ex);
}
