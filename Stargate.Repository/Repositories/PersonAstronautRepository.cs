using Microsoft.EntityFrameworkCore;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.Repository.Repositories
{
    public class PersonAstronautRepository : Repository<PersonAstronautEntity>, IPersonAstronautRepository
    {
        public PersonAstronautRepository(StargateContext context) : base(context)
        {
        }

        public async Task<PersonAstronautEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
        }

        public async Task<PersonAstronautEntity?> GetByNameWithDetailsAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.AstronautDetail)
                .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
        }

        public async Task<PersonAstronautEntity?> GetByNameWithAllRelationsAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.AstronautDetail)
                .Include(p => p.AstronautDuties)
                .FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
        }

        public async Task<IEnumerable<PersonAstronautEntity>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.AstronautDetail)
                .ToListAsync(cancellationToken);
        }
    }
}
