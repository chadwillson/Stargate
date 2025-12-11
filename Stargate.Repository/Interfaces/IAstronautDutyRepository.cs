using Stargate.Repository.Entities;

namespace Stargate.Repository.Interfaces
{
    public interface IAstronautDutyRepository : IRepository<AstronautDutyEntity>
    {
        Task<IEnumerable<AstronautDutyEntity>> GetByPersonIdAsync(int personId, CancellationToken cancellationToken = default);
        Task<AstronautDutyEntity?> GetActiveDutyAsync(int personId, CancellationToken cancellationToken = default);
    }
}
