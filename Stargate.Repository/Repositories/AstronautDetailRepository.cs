using Microsoft.EntityFrameworkCore;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.Repository.Repositories
{
    public class AstronautDetailRepository : Repository<AstronautDetailEntity>, IAstronautDetailRepository
    {
        public AstronautDetailRepository(StargateContext context) : base(context)
        {
        }

        public async Task<AstronautDetailEntity?> GetByPersonIdAsync(int personId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(ad => ad.PersonId == personId, cancellationToken);
        }
    }
}
