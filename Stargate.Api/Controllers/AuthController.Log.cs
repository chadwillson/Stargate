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

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Forgot password attempt for email: {Email}")]
    partial void LogForgotPasswordAttempt(string email);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Forgot password request for non-existent email: {Email}")]
    partial void LogForgotPasswordUserNotFound(string email);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Password reset email sent to: {Email}")]
    partial void LogForgotPasswordEmailSent(string email);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Forgot password failed for email: {Email}")]
    partial void LogForgotPasswordError(Exception ex, string email);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Reset password attempt with token: {Token}")]
    partial void LogResetPasswordAttempt(string token);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Reset password attempt with invalid token: {Token}")]
    partial void LogResetPasswordInvalidToken(string token);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Reset password attempt with expired token: {Token}")]
    partial void LogResetPasswordExpiredToken(string token);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Password reset successful for user: {Username}")]
    partial void LogResetPasswordSuccess(string username);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Reset password failed for token: {Token}")]
    partial void LogResetPasswordError(Exception ex, string token);
}
