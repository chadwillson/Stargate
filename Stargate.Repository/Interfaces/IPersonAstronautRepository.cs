using Stargate.Repository.Entities;

namespace Stargate.Repository.Interfaces
{
    public interface IPersonAstronautRepository : IRepository<PersonAstronautEntity>
    {
        Task<PersonAstronautEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<PersonAstronautEntity?> GetByNameWithDetailsAsync(string name, CancellationToken cancellationToken = default);
        Task<PersonAstronautEntity?> GetByNameWithAllRelationsAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<PersonAstronautEntity>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<PersonAstronautEntity>> SearchByNameWithAllRelationsAsync(string searchTerm, CancellationToken cancellationToken = default);
    }
}
