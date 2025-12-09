using Stargate.Repository.Entities;

namespace Stargate.Repository.Interfaces
{
    public interface IAstronautDetailRepository : IRepository<AstronautDetailEntity>
    {
        Task<AstronautDetailEntity?> GetByPersonIdAsync(int personId, CancellationToken cancellationToken = default);
    }
}
