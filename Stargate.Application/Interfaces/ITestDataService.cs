using Stargate.Domain.Dtos;
using Stargate.Repository.Entities;

namespace Stargate.Application.Interfaces
{
    public interface ITestDataService
    {
        Task<TestDataResponse> CreateBasicScenarioAsync(CancellationToken cancellationToken = default);
        Task<TestDataResponse> CreateComplexScenarioAsync(CancellationToken cancellationToken = default);
        Task<TestDataResponse> CreateEdgeCaseScenarioAsync(CancellationToken cancellationToken = default);
        Task<TestDataResponse> CreateRetiredAstronautScenarioAsync(CancellationToken cancellationToken = default);
        Task<TestDataResponse> CreateMultipleDutiesScenarioAsync(CancellationToken cancellationToken = default);
        Task<BaseResponse> ClearAllTestDataAsync(CancellationToken cancellationToken = default);
        Task<BaseResponse> ResetToDefaultSeedDataAsync(CancellationToken cancellationToken = default);

        Task<PersonAstronautEntity> CreatePersonAsync(string name, CancellationToken cancellationToken = default);
        Task<AstronautDetailEntity> CreateAstronautDetailAsync(int personId, string rank, string dutyTitle, DateTime careerStartDate, DateTime? careerEndDate = null, CancellationToken cancellationToken = default);
        Task<AstronautDutyEntity> CreateAstronautDutyAsync(int personId, string rank, string dutyTitle, DateTime dutyStartDate, DateTime? dutyEndDate = null, CancellationToken cancellationToken = default);
    }
}
