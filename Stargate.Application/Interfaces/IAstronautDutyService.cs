using Stargate.Domain.Dtos;

namespace Stargate.Application.Interfaces
{
    public interface IAstronautDutyService
    {
        Task<AstronautDutiesListResponse> GetAstronautDutiesByName(string name, string? correlationId, CancellationToken cancellationToken);

        Task<CreateAstronautDutyResponse> CreateAstronautDuty(CreateAstronautDutyResponse request, string? correlationId, CancellationToken cancellationToken);
    }
}
