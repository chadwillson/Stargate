using Stargate.Domain.Dtos;

namespace Stargate.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> CreateUserAsync(UserRequest request, CancellationToken cancellationToken = default);
        Task<UserResponse> GetUserByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<UserResponse> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<UserListResponse> GetAllUsersAsync(CancellationToken cancellationToken = default);
        Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken = default);
        Task<BaseResponse> DeleteUserAsync(int id, CancellationToken cancellationToken = default);
        Task<BaseResponse> ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
        Task<UserResponse?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);
        Task<RoleListResponse> GetAllRolesAsync(CancellationToken cancellationToken = default);
        Task<UserResponse?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task SetPasswordResetTokenAsync(int userId, string token, DateTime expiry, CancellationToken cancellationToken = default);
        Task<UserResponse?> GetByPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default);
        Task ResetPasswordAsync(int userId, string newPassword, CancellationToken cancellationToken = default);
    }
}
