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
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ITokenService tokenService, IUserService userService, IEmailService emailService, ILogger<AuthController> logger)
        {
            _tokenService = tokenService;
            _userService = userService;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                LogLoginAttempt(request.Username);

                // Authenticate user using database
                var user = await _userService.AuthenticateAsync(request.Username, request.Password);

                if (user == null)
                {
                    LogFailedLoginAttempt(request.Username);

                    return this.GetResponse(new LoginResponse
                    {
                        Success = false,
                        Message = "Invalid username or password",
                        ResponseCode = (int)HttpStatusCode.Unauthorized
                    });
                }

                // Log the user details for debugging
                _logger.LogInformation("User authenticated - Username: {Username}, RoleId: {RoleId}, RoleName: {RoleName}",
                    user.Username, user.RoleId, user.RoleName);

                // Generate JWT token with role
                var token = _tokenService.GenerateToken(user.Username, user.RoleName);

                _logger.LogInformation("JWT token generated for user: {Username} with role: {RoleName}",
                    user.Username, user.RoleName);

                LogSuccessfulLogin(request.Username);

                return this.GetResponse(new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    ResponseCode = (int)HttpStatusCode.OK,
                    Token = token,
                    Username = user.Username
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                LogForgotPasswordAttempt(request.Email);

                var user = await _userService.GetByEmailAsync(request.Email);

                if (user == null)
                {
                    // For security, always return success even if user doesn't exist
                    LogForgotPasswordUserNotFound(request.Email);
                    return this.GetResponse(new BaseResponse
                    {
                        Success = true,
                        Message = "If your email is registered, you will receive a password reset link.",
                        ResponseCode = (int)HttpStatusCode.OK
                    });
                }

                // Generate reset token
                var resetToken = Guid.NewGuid().ToString();
                var expiry = DateTime.UtcNow.AddHours(1);

                await _userService.SetPasswordResetTokenAsync(user.Id, resetToken, expiry);

                // Send email
                await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken, user.Username);

                LogForgotPasswordEmailSent(request.Email);

                return this.GetResponse(new BaseResponse
                {
                    Success = true,
                    Message = "If your email is registered, you will receive a password reset link.",
                    ResponseCode = (int)HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                LogForgotPasswordError(ex, request.Email);

                return this.GetResponse(new BaseResponse
                {
                    Message = "An error occurred while processing your request",
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                LogResetPasswordAttempt(request.Token);

                var user = await _userService.GetByPasswordResetTokenAsync(request.Token);

                if (user == null)
                {
                    LogResetPasswordInvalidToken(request.Token);
                    return this.GetResponse(new BaseResponse
                    {
                        Success = false,
                        Message = "Invalid or expired reset token",
                        ResponseCode = (int)HttpStatusCode.BadRequest
                    });
                }

                // Check if token is expired
                if (user.PasswordResetTokenExpiry == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
                {
                    LogResetPasswordExpiredToken(request.Token);
                    return this.GetResponse(new BaseResponse
                    {
                        Success = false,
                        Message = "Invalid or expired reset token",
                        ResponseCode = (int)HttpStatusCode.BadRequest
                    });
                }

                // Reset password
                await _userService.ResetPasswordAsync(user.Id, request.NewPassword);

                LogResetPasswordSuccess(user.Username);

                return this.GetResponse(new BaseResponse
                {
                    Success = true,
                    Message = "Password reset successful. You can now login with your new password.",
                    ResponseCode = (int)HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                LogResetPasswordError(ex, request.Token);

                return this.GetResponse(new BaseResponse
                {
                    Message = "An error occurred while resetting your password",
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRequest request)
        {
            try
            {
                _logger.LogInformation("Registration attempt for username: {Username}, email: {Email}", request.Username, request.Email);

                // Create user with default "User" role (roleId: 2)
                var registerRequest = new UserRequest
                {
                    Username = request.Username,
                    Email = request.Email,
                    Password = request.Password,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    RoleId = 2 // Default "User" role
                };

                var response = await _userService.CreateUserAsync(registerRequest);

                if (response.Success)
                {
                    _logger.LogInformation("User registered successfully: {Username}", request.Username);
                }
                else
                {
                    _logger.LogWarning("User registration failed for {Username}: {Message}", request.Username, response.Message);
                }

                return this.GetResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for username: {Username}", request.Username);

                return this.GetResponse(new UserResponse
                {
                    Message = "An error occurred during registration. Please try again.",
                    Success = false,
                    ResponseCode = (int)HttpStatusCode.InternalServerError
                });
            }
        }
    }
}
