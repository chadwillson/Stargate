using Stargate.Domain.Dtos;

namespace Stargate.Application.Interfaces
{
    public interface IPersonAstronautService
    {
        Task<PersonAstronautListResponse> GetPeople(CancellationToken cancellationToken);

        Task<PersonAstronautResponse> GetPersonByName(string name, CancellationToken cancellationToken);

        Task<PersonAstronautResponse> CreatePerson(PersonRequest request, CancellationToken cancellationToken);
    }
}
