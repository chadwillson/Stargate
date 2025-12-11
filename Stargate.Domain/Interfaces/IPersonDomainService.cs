using Stargate.Domain.Models;

namespace Stargate.Domain.Interfaces
{
    public interface IPersonDomainService
    {
        Task<DomainValidationResult> ValidatePersonCreationAsync(string name, CancellationToken cancellationToken = default);
        Task<DomainValidationResult> ValidatePersonUpdateAsync(int personId, string currentName, string newName, CancellationToken cancellationToken = default);
    }
}
