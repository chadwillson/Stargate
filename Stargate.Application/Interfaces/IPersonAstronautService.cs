using Stargate.Domain.Dtos;

namespace Stargate.Application.Interfaces
{
    public interface IPersonAstronautService
    {
        Task<PersonAstronautListResponse> GetPeople(string? correlationId, CancellationToken cancellationToken);

        Task<PersonAstronautResponse> GetPersonByName(string name, string? correlationId, CancellationToken cancellationToken);

        Task<PersonAstronautResponse> CreatePerson(PersonRequest request, string? correlationId, CancellationToken cancellationToken);

        Task<PersonAstronautResponse> UpdatePerson(string name, PersonRequest request, string? correlationId, CancellationToken cancellationToken);
    }
}
