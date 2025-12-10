using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Stargate.Api.Middleware;
using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;

namespace Stargate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ILoggingService _loggingService;
        private readonly ICorrelationIdAccessor _correlationIdAccessor;
        private const string Category = "AuthController";

        // Hardcoded credentials for demo purposes
        private static readonly Dictionary<string, string> _users = new()
        {
            { "admin", "Stargate123!" },
            { "commander", "SGC2024!" },
            { "user", "Password1!" }
        };

        public AuthController(ITokenService tokenService, ILoggingService loggingService, ICorrelationIdAccessor correlationIdAccessor)
        {
            _tokenService = tokenService;
            _loggingService = loggingService;
            _correlationIdAccessor = correlationIdAccessor;
        }

        private string? CorrelationId => _correlationIdAccessor.CorrelationId;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await _loggingService.LogInformationAsync(Category, $"POST /api/auth/login - Login attempt for user: {request.Username}", source: nameof(Login), correlationId: CorrelationId);

                // Validate credentials
                if (!_users.TryGetValue(request.Username, out var expectedPassword) || request.Password != expectedPassword)
                {
                    stopwatch.Stop();
                    await _loggingService.LogWarningAsync(Category, $"Failed login attempt for user: {request.Username}", source: nameof(Login), correlationId: CorrelationId);
                    await _loggingService.LogRequestAsync("/api/auth/login", "POST", 401, stopwatch.ElapsedMilliseconds, correlationId: CorrelationId);

                    return this.GetResponse(new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid username or password",
                        ResponseCode = (int)HttpStatusCode.Unauthorized
                    });
                }

                // Generate token
                var token = _tokenService.GenerateToken(request.Username);

                stopwatch.Stop();
                await _loggingService.LogInformationAsync(Category, $"Successful login for user: {request.Username}", source: nameof(Login), correlationId: CorrelationId);
                await _loggingService.LogRequestAsync("/api/auth/login", "POST", 200, stopwatch.ElapsedMilliseconds, correlationId: CorrelationId);

                return this.GetResponse(new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    ResponseCode = (int)HttpStatusCode.OK,
                    Token = token,
                    Username = request.Username
                });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                await _loggingService.LogErrorAsync(Category, $"POST /api/auth/login failed: {ex.Message}", ex, source: nameof(Login), correlationId: CorrelationId);
                await _loggingService.LogRequestAsync("/api/auth/login", "POST", 500, stopwatch.ElapsedMilliseconds, correlationId: CorrelationId);

                return this.GetResponse(new LoginResponse
                {
                    Message = "An error occurred during login",
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

                if (!string.IsNullOrEmpty(token))
                {
                    _tokenService.RevokeToken(token);
                    await _loggingService.LogInformationAsync(Category, "POST /api/auth/logout - Token revoked", source: nameof(Logout), correlationId: CorrelationId);
                }

                stopwatch.Stop();
                await _loggingService.LogRequestAsync("/api/auth/logout", "POST", 200, stopwatch.ElapsedMilliseconds, correlationId: CorrelationId);

                return this.GetResponse(new BaseResponse
                {
                    Success = true,
                    Message = "Logout successful",
                    ResponseCode = (int)HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                await _loggingService.LogErrorAsync(Category, $"POST /api/auth/logout failed: {ex.Message}", ex, source: nameof(Logout), correlationId: CorrelationId);
                await _loggingService.LogRequestAsync("/api/auth/logout", "POST", 500, stopwatch.ElapsedMilliseconds, correlationId: CorrelationId);

                return this.GetResponse(new BaseResponse
                {
                    Message = ex.Message,
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }
    }
}
