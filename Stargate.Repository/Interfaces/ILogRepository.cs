using Stargate.Repository.Entities;

namespace Stargate.Repository.Interfaces
{
    public interface ILogRepository : IRepository<LogEntryEntity>
    {
        Task<IEnumerable<LogEntryEntity>> GetByLevelAsync(string level, CancellationToken cancellationToken = default);
        Task<IEnumerable<LogEntryEntity>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
        Task<IEnumerable<LogEntryEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<IEnumerable<LogEntryEntity>> GetByCorrelationIdAsync(string correlationId, CancellationToken cancellationToken = default);
        Task<IEnumerable<LogEntryEntity>> GetRecentAsync(int count, CancellationToken cancellationToken = default);
    }
}
