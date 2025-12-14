using Stargate.Repository.Entities;

namespace Stargate.Repository.Interfaces
{
    public interface IUserRepository : IRepository<UserEntity>
    {
        Task<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<UserEntity?> GetByUsernameWithRoleAsync(string username, CancellationToken cancellationToken = default);
        Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserEntity>> GetAllWithRolesAsync(CancellationToken cancellationToken = default);
        Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
        Task<bool> UsernameExistsAsync(string username, int excludeUserId, CancellationToken cancellationToken = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
        Task<UserEntity?> GetByPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default);
    }
}
