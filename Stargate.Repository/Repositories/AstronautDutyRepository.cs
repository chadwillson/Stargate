using Microsoft.EntityFrameworkCore;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.Repository.Repositories
{
    public class AstronautDutyRepository : Repository<AstronautDutyEntity>, IAstronautDutyRepository
    {
        public AstronautDutyRepository(StargateContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AstronautDutyEntity>> GetByPersonIdAsync(int personId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(ad => ad.PersonId == personId)
                .OrderByDescending(ad => ad.DutyStartDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<AstronautDutyEntity?> GetActiveDutyAsync(int personId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(ad => ad.PersonId == personId && ad.DutyEndDate == null, cancellationToken);
        }
    }
}
