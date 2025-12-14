using Microsoft.Extensions.Logging;

namespace Stargate.Application.Services
{
    public partial class TokenService
    {
        [LoggerMessage(
            EventId = 1,
            Level = LogLevel.Information,
            Message = "TokenService.GenerateToken called with username: {Username}, role: {Role}")]
        partial void LogGenerateTokenCalled(string username, string role);

        [LoggerMessage(
            EventId = 2,
            Level = LogLevel.Information,
            Message = "Claims being added to token: {Claims}")]
        partial void LogTokenClaims(string claims);

        [LoggerMessage(
            EventId = 3,
            Level = LogLevel.Information,
            Message = "Token generated successfully for user: {Username}")]
        partial void LogTokenGeneratedSuccessfully(string username);
    }
}
