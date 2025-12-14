using Microsoft.Extensions.Logging;

namespace Stargate.Api.Controllers
{
    public partial class UILoggingController
    {
        [LoggerMessage(
            EventId = 1,
            Level = LogLevel.Error,
            Message = "{Message}\nStackTrace: {StackTrace}")]
        partial void LogUIErrorWithStackTrace(string message, string stackTrace);

        [LoggerMessage(
            EventId = 2,
            Level = LogLevel.Error,
            Message = "{Message}")]
        partial void LogUIError(string message);

        [LoggerMessage(
            EventId = 3,
            Level = LogLevel.Warning,
            Message = "{Message}")]
        partial void LogUIWarning(string message);

        [LoggerMessage(
            EventId = 4,
            Level = LogLevel.Information,
            Message = "{Message}")]
        partial void LogUIInfo(string message);

        [LoggerMessage(
            EventId = 5,
            Level = LogLevel.Error,
            Message = "Error processing UI log")]
        partial void LogErrorProcessingUILog(Exception ex);
    }
}
