using System.Net;
using Microsoft.Extensions.Logging;
using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;
using BCrypt.Net;

namespace Stargate.Application.Services
{
    public partial class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;

        public UserService(IUnitOfWork unitOfWork, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<UserResponse> CreateUserAsync(UserRequest request, CancellationToken cancellationToken = default)
        {
            // Check if username already exists
            if (await _unitOfWork.Users.UsernameExistsAsync(request.Username, cancellationToken))
            {
                return new UserResponse
                {
                    Success = false,
                    Message = $"Username '{request.Username}' already exists",
                    ResponseCode = (int)HttpStatusCode.Conflict
                };
            }

            // Check if email already exists
            if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
            {
                return new UserResponse
                {
                    Success = false,
                    Message = $"Email '{request.Email}' already exists",
                    ResponseCode = (int)HttpStatusCode.Conflict
                };
            }

            // Hash the password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new UserEntity
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                FirstName = request.FirstName,
                LastName = request.LastName,
                RoleId = request.RoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with role
            var createdUser = await _unitOfWork.Users.GetByUsernameWithRoleAsync(user.Username, cancellationToken);

            return MapToUserResponse(createdUser!, true, "User created successfully", (int)HttpStatusCode.Created);
        }

        public async Task<UserResponse> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return new UserResponse
                {
                    Success = false,
                    Message = $"User with ID {id} not found",
                    ResponseCode = (int)HttpStatusCode.NotFound
                };
            }

            // Load role
            var userWithRole = await _unitOfWork.Users.GetByUsernameWithRoleAsync(user.Username, cancellationToken);
            return MapToUserResponse(userWithRole!, true, "User retrieved successfully", (int)HttpStatusCode.OK);
        }

        public async Task<UserResponse> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByUsernameWithRoleAsync(username, cancellationToken);
            if (user == null)
            {
                return new UserResponse
                {
                    Success = false,
                    Message = $"User '{username}' not found",
                    ResponseCode = (int)HttpStatusCode.NotFound
                };
            }

            return MapToUserResponse(user, true, "User retrieved successfully", (int)HttpStatusCode.OK);
        }

        public async Task<UserListResponse> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            var users = await _unitOfWork.Users.GetAllWithRolesAsync(cancellationToken);

            return new UserListResponse
            {
                Success = true,
                Message = "Users retrieved successfully",
                ResponseCode = (int)HttpStatusCode.OK,
                Users = users.Select(u => new UserResponse
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    RoleId = u.RoleId,
                    RoleName = u.Role?.Name ?? string.Empty,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt
                })
            };
        }

        public async Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return new UserResponse
                {
                    Success = false,
                    Message = $"User with ID {id} not found",
                    ResponseCode = (int)HttpStatusCode.NotFound
                };
            }

            // Update fields if provided
            if (request.Email != null)
            {
                // Check if new email is already in use by another user
                var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
                if (existingUser != null && existingUser.Id != id)
                {
                    return new UserResponse
                    {
                        Success = false,
                        Message = $"Email '{request.Email}' is already in use",
                        ResponseCode = (int)HttpStatusCode.Conflict
                    };
                }
                user.Email = request.Email;
            }

            if (request.FirstName != null) user.FirstName = request.FirstName;
            if (request.LastName != null) user.LastName = request.LastName;
            if (request.RoleId.HasValue) user.RoleId = request.RoleId.Value;
            if (request.IsActive.HasValue) user.IsActive = request.IsActive.Value;

            if (!string.IsNullOrWhiteSpace(request.NewPassword))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload with role
            var updatedUser = await _unitOfWork.Users.GetByUsernameWithRoleAsync(user.Username, cancellationToken);
            return MapToUserResponse(updatedUser!, true, "User updated successfully", (int)HttpStatusCode.OK);
        }

        public async Task<BaseResponse> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
            if (user == null)
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = $"User with ID {id} not found",
                    ResponseCode = (int)HttpStatusCode.NotFound
                };
            }

            await _unitOfWork.Users.DeleteAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new BaseResponse
            {
                Success = true,
                Message = "User deleted successfully",
                ResponseCode = (int)HttpStatusCode.OK
            };
        }

        public async Task<BaseResponse> ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = $"User with ID {userId} not found",
                    ResponseCode = (int)HttpStatusCode.NotFound
                };
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return new BaseResponse
                {
                    Success = false,
                    Message = "Current password is incorrect",
                    ResponseCode = (int)HttpStatusCode.Unauthorized
                };
            }

            // Hash and save new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new BaseResponse
            {
                Success = true,
                Message = "Password changed successfully",
                ResponseCode = (int)HttpStatusCode.OK
            };
        }

        public async Task<UserResponse?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByUsernameWithRoleAsync(username, cancellationToken);

            if (user == null || !user.IsActive)
            {
                LogAuthenticationFailedUserNotFoundOrInactive(username);
                return null;
            }

            LogUserRetrievedFromDatabase(user.Username, user.RoleId, user.Role?.Name ?? "NULL");

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                LogAuthenticationFailedInvalidPassword(username);
                return null;
            }

            // Update last login time
            user.LastLoginAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var response = MapToUserResponse(user, true, "Authentication successful", (int)HttpStatusCode.OK);
            LogUserResponseCreated(response.Username, response.RoleId, response.RoleName);

            return response;
        }

        public async Task<RoleListResponse> GetAllRolesAsync(CancellationToken cancellationToken = default)
        {
            var roles = await _unitOfWork.Roles.GetAllAsync(cancellationToken);

            return new RoleListResponse
            {
                Success = true,
                Message = "Roles retrieved successfully",
                ResponseCode = (int)HttpStatusCode.OK,
                Roles = roles.Select(r => new RoleResponse
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                })
            };
        }

        public async Task<UserResponse?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
            if (user == null)
            {
                return null;
            }

            // Load role
            var userWithRole = await _unitOfWork.Users.GetByUsernameWithRoleAsync(user.Username, cancellationToken);
            return MapToUserResponse(userWithRole!, true, "User retrieved successfully", (int)HttpStatusCode.OK);
        }

        public async Task SetPasswordResetTokenAsync(int userId, string token, DateTime expiry, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                user.PasswordResetToken = token;
                user.PasswordResetTokenExpiry = expiry;
                user.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<UserResponse?> GetByPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByPasswordResetTokenAsync(token, cancellationToken);
            if (user == null)
            {
                return null;
            }

            // Load role
            var userWithRole = await _unitOfWork.Users.GetByUsernameWithRoleAsync(user.Username, cancellationToken);
            return MapToUserResponse(userWithRole!, true, "User retrieved successfully", (int)HttpStatusCode.OK);
        }

        public async Task ResetPasswordAsync(int userId, string newPassword, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.PasswordResetToken = null;
                user.PasswordResetTokenExpiry = null;
                user.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        private static UserResponse MapToUserResponse(UserEntity user, bool success, string message, int statusCode)
        {
            return new UserResponse
            {
                Success = success,
                Message = message,
                ResponseCode = statusCode,
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? string.Empty,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                PasswordResetTokenExpiry = user.PasswordResetTokenExpiry
            };
        }
    }
}
