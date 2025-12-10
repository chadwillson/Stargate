using Stargate.Application.Interfaces;

namespace Stargate.Api.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _environment;
        private static readonly HashSet<string> _publicPaths = new(StringComparer.OrdinalIgnoreCase)
        {
            "/api/auth/login",
            "/swagger",
            "/swagger/index.html"
        };

        public AuthenticationMiddleware(RequestDelegate next, IWebHostEnvironment environment)
        {
            _next = next;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
        {
            // Skip authentication in IntegrationTest environment
            if (_environment.IsEnvironment("IntegrationTest"))
            {
                await _next(context);
                return;
            }

            var path = context.Request.Path.Value ?? string.Empty;

            // Allow public paths and Swagger
            if (_publicPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            // Extract token from Authorization header
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token) || !tokenService.ValidateToken(token))
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"success\":false,\"message\":\"Unauthorized\",\"responseCode\":401}");
                return;
            }

            // Token is valid, continue
            await _next(context);
        }
    }

    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
