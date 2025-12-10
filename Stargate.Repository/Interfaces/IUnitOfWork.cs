namespace Stargate.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IPersonAstronautRepository PersonAstronauts { get; }
        IAstronautDetailRepository AstronautDetails { get; }
        IAstronautDutyRepository AstronautDuties { get; }
        ILogRepository LogEntries { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
