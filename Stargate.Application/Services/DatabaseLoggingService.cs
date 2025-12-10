using Stargate.Application.Interfaces;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.Application.Services
{
    public class DatabaseLoggingService : ILoggingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DatabaseLoggingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task LogInformationAsync(string category, string message, string? source = null, string? correlationId = null, string? additionalData = null, CancellationToken cancellationToken = default)
        {
            await LogAsync("Information", category, message, null, source, correlationId, additionalData, cancellationToken);
        }

        public async Task LogWarningAsync(string category, string message, string? source = null, string? correlationId = null, string? additionalData = null, CancellationToken cancellationToken = default)
        {
            await LogAsync("Warning", category, message, null, source, correlationId, additionalData, cancellationToken);
        }

        public async Task LogErrorAsync(string category, string message, Exception? exception = null, string? source = null, string? correlationId = null, string? additionalData = null, CancellationToken cancellationToken = default)
        {
            await LogAsync("Error", category, message, exception, source, correlationId, additionalData, cancellationToken);
        }

        public async Task LogDebugAsync(string category, string message, string? source = null, string? correlationId = null, string? additionalData = null, CancellationToken cancellationToken = default)
        {
            await LogAsync("Debug", category, message, null, source, correlationId, additionalData, cancellationToken);
        }

        public async Task LogRequestAsync(string requestPath, string requestMethod, int statusCode, long elapsedMilliseconds, string? correlationId = null, string? userId = null, CancellationToken cancellationToken = default)
        {
            var logEntry = new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = statusCode >= 400 ? "Error" : "Information",
                Category = "HttpRequest",
                Message = $"{requestMethod} {requestPath} responded {statusCode} in {elapsedMilliseconds}ms",
                RequestPath = requestPath,
                RequestMethod = requestMethod,
                StatusCode = statusCode,
                ElapsedMilliseconds = elapsedMilliseconds,
                CorrelationId = correlationId,
                UserId = userId
            };

            await _unitOfWork.LogEntries.AddAsync(logEntry, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task LogAsync(string level, string category, string message, Exception? exception, string? source, string? correlationId, string? additionalData, CancellationToken cancellationToken)
        {
            var logEntry = new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Category = category,
                Message = message,
                Exception = exception?.Message,
                StackTrace = exception?.StackTrace,
                Source = source,
                CorrelationId = correlationId,
                AdditionalData = additionalData
            };

            await _unitOfWork.LogEntries.AddAsync(logEntry, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
