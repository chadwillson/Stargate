using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;

namespace Stargate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        // Hardcoded credentials for demo purposes
        private static readonly Dictionary<string, string> _users = new()
        {
            { "admin", "Stargate123!" },
            { "commander", "SGC2024!" },
            { "user", "Password1!" }
        };

        public AuthController(ITokenService tokenService, ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                LogLoginAttempt(request.Username);

                // Validate credentials
                if (!_users.TryGetValue(request.Username, out var expectedPassword) || request.Password != expectedPassword)
                {
                    LogFailedLoginAttempt(request.Username);

                    return this.GetResponse(new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid username or password",
                        ResponseCode = (int)HttpStatusCode.Unauthorized
                    });
                }

                // Generate token
                var token = _tokenService.GenerateToken(request.Username);

                LogSuccessfulLogin(request.Username);

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
                LogLoginError(ex, request.Username);

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
            try
            {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

                if (!string.IsNullOrEmpty(token))
                {
                    _tokenService.RevokeToken(token);
                    LogTokenRevoked();
                }

                return this.GetResponse(new BaseResponse
                {
                    Success = true,
                    Message = "Logout successful",
                    ResponseCode = (int)HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                LogLogoutError(ex);

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
