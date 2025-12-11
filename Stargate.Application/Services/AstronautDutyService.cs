using Microsoft.Extensions.Logging;
using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;
using Stargate.Domain.Interfaces;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.Application.Services
{
    public partial class AstronautDutyService : IAstronautDutyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AstronautDutyService> _logger;
        private readonly IAstronautDutyDomainService _dutyDomainService;

        public AstronautDutyService(IUnitOfWork unitOfWork, ILogger<AstronautDutyService> logger, IAstronautDutyDomainService dutyDomainService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _dutyDomainService = dutyDomainService;
        }

        public async Task<AstronautDutiesListResponse> GetAstronautDutiesByName(string name, CancellationToken cancellationToken)
        {
            LogRetrievingAstronautDutiesByName(name);

            var people = await _unitOfWork.PersonAstronauts.SearchByNameWithAllRelationsAsync(name, cancellationToken);

            if (!people.Any())
            {
                LogNoPeopleFound(name);
                return new AstronautDutiesListResponse
                {
                    Success = false,
                    Message = "No people found matching the search term",
                    ResponseCode = 404
                };
            }

            var duties = new List<AstronautDutiesByNameResponse>();

            foreach (var person in people)
            {
                var response = new AstronautDutiesByNameResponse
                {
                    Person = new PersonAstronautResponse
                    {
                        PersonId = person.Id,
                        Name = person.Name,
                        CurrentRank = person.AstronautDetail?.CurrentRank ?? string.Empty,
                        CurrentDutyTitle = person.AstronautDetail?.CurrentDutyTitle ?? string.Empty,
                        CareerStartDate = person.AstronautDetail?.CareerStartDate,
                        CareerEndDate = person.AstronautDetail?.CareerEndDate
                    },
                    AstronautDuties = person.AstronautDuties.Select(duty => new AstronautDutyResponse
                    {
                        Id = duty.Id,
                        PersonId = duty.PersonId,
                        Rank = duty.Rank,
                        DutyTitle = duty.DutyTitle,
                        DutyStartDate = duty.DutyStartDate,
                        DutyEndDate = duty.DutyEndDate
                    }).ToList()
                };

                duties.Add(response);
            }

            LogRetrievedPeopleWithDuties(duties.Count, name);

            return new AstronautDutiesListResponse
            {
                Duties = duties
            };
        }

        public async Task<CreateAstronautDutyResponse> CreateAstronautDuty(CreateAstronautDutyResponse request, CancellationToken cancellationToken)
        {
            LogCreatingAstronautDuty(request.Name, request.DutyTitle, request.Rank);

            // Use domain service to check if person exists
            var person = await _dutyDomainService.EnsurePersonExistsAsync(request.Name, cancellationToken);

            if (person == null)
            {
                // Domain service indicated person needs to be created
                person = new PersonAstronautEntity
                {
                    Name = request.Name
                };

                await _unitOfWork.PersonAstronauts.AddAsync(person, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                LogCreatedNewPerson(request.Name, person.Id);
            }

            // Get existing astronaut detail
            var astronautDetail = await _unitOfWork.AstronautDetails.GetByPersonIdAsync(person.Id, cancellationToken);

            // Use domain service to prepare astronaut detail (handles RETIRED logic)
            var preparedDetail = _dutyDomainService.PrepareAstronautDetail(astronautDetail, request.Rank, request.DutyTitle, request.DutyStartDate, person.Id);

            if (astronautDetail == null)
            {
                await _unitOfWork.AstronautDetails.AddAsync(preparedDetail, cancellationToken);
            }
            else
            {
                await _unitOfWork.AstronautDetails.UpdateAsync(preparedDetail, cancellationToken);
            }

            // Use domain service to terminate active duty
            var terminatedDuty = await _dutyDomainService.GetAndTerminateActiveDutyAsync(person.Id, request.DutyStartDate, cancellationToken);

            if (terminatedDuty != null)
            {
                await _unitOfWork.AstronautDuties.UpdateAsync(terminatedDuty, cancellationToken);
            }

            // Use domain service to create new duty
            var newAstronautDuty = _dutyDomainService.CreateNewDuty(person.Id, request.Rank, request.DutyTitle, request.DutyStartDate);

            await _unitOfWork.AstronautDuties.AddAsync(newAstronautDuty, cancellationToken);

            // Save all changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            LogCreatedAstronautDuty(newAstronautDuty.Id, request.Name, request.Rank, request.DutyTitle);

            return new CreateAstronautDutyResponse
            {
                Id = newAstronautDuty.Id,
                Name = request.Name,
                Rank = request.Rank,
                DutyTitle = request.DutyTitle,
                DutyStartDate = request.DutyStartDate
            };
        }

    }
}
