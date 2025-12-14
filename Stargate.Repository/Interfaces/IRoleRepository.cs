using Stargate.Repository.Entities;

namespace Stargate.Repository.Interfaces
{
    public interface IRoleRepository : IRepository<RoleEntity>
    {
        Task<RoleEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<RoleEntity>> GetAllWithUsersAsync(CancellationToken cancellationToken = default);
    }
}
