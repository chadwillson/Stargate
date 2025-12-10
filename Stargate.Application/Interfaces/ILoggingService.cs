namespace Stargate.Application.Interfaces
{
    public interface ILoggingService
    {
        Task LogInformationAsync(string category, string message, string? source = null, string? correlationId = null, string? additionalData = null, CancellationToken cancellationToken = default);
        Task LogWarningAsync(string category, string message, string? source = null, string? correlationId = null, string? additionalData = null, CancellationToken cancellationToken = default);
        Task LogErrorAsync(string category, string message, Exception? exception = null, string? source = null, string? correlationId = null, string? additionalData = null, CancellationToken cancellationToken = default);
        Task LogDebugAsync(string category, string message, string? source = null, string? correlationId = null, string? additionalData = null, CancellationToken cancellationToken = default);
        Task LogRequestAsync(string requestPath, string requestMethod, int statusCode, long elapsedMilliseconds, string? correlationId = null, string? userId = null, CancellationToken cancellationToken = default);
    }
}
