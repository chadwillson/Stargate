using Microsoft.EntityFrameworkCore;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.Repository.Repositories
{
    public class RoleRepository : Repository<RoleEntity>, IRoleRepository
    {
        public RoleRepository(StargateContext context) : base(context)
        {
        }

        public async Task<RoleEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
        }

        public async Task<IEnumerable<RoleEntity>> GetAllWithUsersAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(r => r.Users)
                .ToListAsync(cancellationToken);
        }
    }
}
