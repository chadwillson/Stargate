using Serilog.Context;

namespace Stargate.Api.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        public const string CorrelationIdHeader = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICorrelationIdAccessor correlationIdAccessor)
        {
            var correlationId = GetOrCreateCorrelationId(context);
            correlationIdAccessor.CorrelationId = correlationId;

            context.Response.Headers[CorrelationIdHeader] = correlationId;

            // Store in HttpContext.Items for request logging access
            context.Items["CorrelationId"] = correlationId;

            // Push correlation ID to Serilog LogContext for automatic enrichment
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }

        private static string GetOrCreateCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId)
                && !string.IsNullOrWhiteSpace(correlationId))
            {
                return correlationId.ToString();
            }

            return Guid.NewGuid().ToString("N");
        }
    }

    public static class CorrelationIdMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorrelationIdMiddleware>();
        }
    }
}
