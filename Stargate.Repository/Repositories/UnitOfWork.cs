using Microsoft.EntityFrameworkCore.Storage;
using Stargate.Repository.Interfaces;

namespace Stargate.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StargateContext _context;
        private IDbContextTransaction? _transaction;

        private IPersonAstronautRepository? _personAstronauts;
        private IAstronautDetailRepository? _astronautDetails;
        private IAstronautDutyRepository? _astronautDuties;
        private ILogRepository? _logEntries;
        private IUserRepository? _users;
        private IRoleRepository? _roles;

        public UnitOfWork(StargateContext context)
        {
            _context = context;
        }

        public IPersonAstronautRepository PersonAstronauts
        {
            get { return _personAstronauts ??= new PersonAstronautRepository(_context); }
        }

        public IAstronautDetailRepository AstronautDetails
        {
            get { return _astronautDetails ??= new AstronautDetailRepository(_context); }
        }

        public IAstronautDutyRepository AstronautDuties
        {
            get { return _astronautDuties ??= new AstronautDutyRepository(_context); }
        }

        public ILogRepository LogEntries
        {
            get { return _logEntries ??= new LogRepository(_context); }
        }

        public IUserRepository Users
        {
            get { return _users ??= new UserRepository(_context); }
        }

        public IRoleRepository Roles
        {
            get { return _roles ??= new RoleRepository(_context); }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
