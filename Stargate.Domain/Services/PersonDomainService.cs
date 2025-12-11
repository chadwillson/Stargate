using Stargate.Domain.Interfaces;
using Stargate.Domain.Models;
using Stargate.Repository.Interfaces;

namespace Stargate.Domain.Services
{
    public class PersonDomainService : IPersonDomainService
    {
        private readonly IPersonAstronautRepository _personRepository;

        public PersonDomainService(IPersonAstronautRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<DomainValidationResult> ValidatePersonCreationAsync(string name, CancellationToken cancellationToken = default)
        {
            var exists = await _personRepository.ExistsAsync(name, cancellationToken);

            if (exists)
            {
                return DomainValidationResult.Failure(
                    $"A person with the name '{name}' already exists",
                    "DUPLICATE_NAME");
            }

            return DomainValidationResult.Success();
        }

        public async Task<DomainValidationResult> ValidatePersonUpdateAsync(int personId, string currentName, string newName, CancellationToken cancellationToken = default)
        {
            // If name hasn't changed, no validation needed
            if (currentName == newName)
            {
                return DomainValidationResult.Success();
            }

            // Check if new name conflicts with another person
            var exists = await _personRepository.ExistsAsync(newName, personId, cancellationToken);

            if (exists)
            {
                return DomainValidationResult.Failure(
                    $"A person with the name '{newName}' already exists",
                    "DUPLICATE_NAME");
            }

            return DomainValidationResult.Success();
        }
    }
}
