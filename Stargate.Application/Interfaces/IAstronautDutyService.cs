using Stargate.Domain.Dtos;

namespace Stargate.Application.Interfaces
{
    public interface IAstronautDutyService
    {
        Task<AstronautDutiesListResponse> GetAstronautDutiesByName(string name, CancellationToken cancellationToken);

        Task<CreateAstronautDutyResponse> CreateAstronautDuty(CreateAstronautDutyResponse request, CancellationToken cancellationToken);
    }
}
