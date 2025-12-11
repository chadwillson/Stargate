using Stargate.Repository.Entities;

namespace Stargate.Domain.Interfaces
{
    public interface IAstronautDutyDomainService
    {
        Task<PersonAstronautEntity?> EnsurePersonExistsAsync(string name, CancellationToken cancellationToken = default);
        AstronautDetailEntity PrepareAstronautDetail(AstronautDetailEntity? existing, string rank, string dutyTitle, DateTime startDate, int personId);
        Task<AstronautDutyEntity?> GetAndTerminateActiveDutyAsync(int personId, DateTime newDutyStartDate, CancellationToken cancellationToken = default);
        AstronautDutyEntity CreateNewDuty(int personId, string rank, string dutyTitle, DateTime startDate);
    }
}
