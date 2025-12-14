using Microsoft.EntityFrameworkCore;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.Repository.Repositories
{
    public class UserRepository : Repository<UserEntity>, IUserRepository
    {
        public UserRepository(StargateContext context) : base(context)
        {
        }

        public async Task<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<UserEntity?> GetByUsernameWithRoleAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<IEnumerable<UserEntity>> GetAllWithRolesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.Role)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AnyAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<bool> UsernameExistsAsync(string username, int excludeUserId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AnyAsync(u => u.Username == username && u.Id != excludeUserId, cancellationToken);
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AnyAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<UserEntity?> GetByPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.PasswordResetToken == token, cancellationToken);
        }
    }
}
