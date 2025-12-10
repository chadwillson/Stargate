using Microsoft.EntityFrameworkCore;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.Repository.Repositories
{
    public class LogRepository : Repository<LogEntryEntity>, ILogRepository
    {
        public LogRepository(StargateContext context) : base(context)
        {
        }

        public async Task<IEnumerable<LogEntryEntity>> GetByLevelAsync(string level, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(l => l.Level == level)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<LogEntryEntity>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(l => l.Category == category)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<LogEntryEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<LogEntryEntity>> GetByCorrelationIdAsync(string correlationId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(l => l.CorrelationId == correlationId)
                .OrderBy(l => l.Timestamp)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<LogEntryEntity>> GetRecentAsync(int count, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .OrderByDescending(l => l.Timestamp)
                .Take(count)
                .ToListAsync(cancellationToken);
        }
    }
}
