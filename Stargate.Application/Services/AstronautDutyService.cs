using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.Application.Services
{
    public class AstronautDutyService : IAstronautDutyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggingService _loggingService;
        private const string Category = "AstronautDutyService";

        public AstronautDutyService(IUnitOfWork unitOfWork, ILoggingService loggingService)
        {
            _unitOfWork = unitOfWork;
            _loggingService = loggingService;
        }

        public async Task<AstronautDutiesListResponse> GetAstronautDutiesByName(string name, string? correlationId, CancellationToken cancellationToken)
        {
            await _loggingService.LogInformationAsync(Category, $"Retrieving astronaut duties for: {name}", source: nameof(GetAstronautDutiesByName), correlationId: correlationId, cancellationToken: cancellationToken);

            var people = await _unitOfWork.PersonAstronauts.SearchByNameWithAllRelationsAsync(name, cancellationToken);

            if (!people.Any())
            {
                await _loggingService.LogWarningAsync(Category, $"No people found matching: {name}", source: nameof(GetAstronautDutiesByName), correlationId: correlationId, cancellationToken: cancellationToken);
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

            await _loggingService.LogInformationAsync(Category, $"Retrieved {duties.Count} people with duties for search: {name}", source: nameof(GetAstronautDutiesByName), correlationId: correlationId, cancellationToken: cancellationToken);

            return new AstronautDutiesListResponse
            {
                Duties = duties
            };
        }

        public async Task<CreateAstronautDutyResponse> CreateAstronautDuty(CreateAstronautDutyResponse request, string? correlationId, CancellationToken cancellationToken)
        {
            await _loggingService.LogInformationAsync(Category, $"Creating astronaut duty for: {request.Name}, Title: {request.DutyTitle}, Rank: {request.Rank}", source: nameof(CreateAstronautDuty), correlationId: correlationId, cancellationToken: cancellationToken);

            // Get or create the person by name
            var person = await _unitOfWork.PersonAstronauts.GetByNameAsync(request.Name, cancellationToken);

            if (person == null)
            {
                // Create new person
                person = new PersonAstronautEntity
                {
                    Name = request.Name
                };

                await _unitOfWork.PersonAstronauts.AddAsync(person, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _loggingService.LogInformationAsync(Category, $"Created new person: {request.Name} (ID: {person.Id})", source: nameof(CreateAstronautDuty), correlationId: correlationId, cancellationToken: cancellationToken);
            }

            // Get or create astronaut detail
            var astronautDetail = await _unitOfWork.AstronautDetails.GetByPersonIdAsync(person.Id, cancellationToken);

            if (astronautDetail == null)
            {
                astronautDetail = new AstronautDetailEntity
                {
                    PersonId = person.Id,
                    CurrentDutyTitle = request.DutyTitle,
                    CurrentRank = request.Rank,
                    CareerStartDate = request.DutyStartDate.Date
                };

                if (request.DutyTitle == "RETIRED")
                {
                    astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                }

                await _unitOfWork.AstronautDetails.AddAsync(astronautDetail, cancellationToken);
            }
            else
            {
                astronautDetail.CurrentDutyTitle = request.DutyTitle;
                astronautDetail.CurrentRank = request.Rank;

                if (request.DutyTitle == "RETIRED")
                {
                    astronautDetail.CareerEndDate = request.DutyStartDate.AddDays(-1).Date;
                }

                await _unitOfWork.AstronautDetails.UpdateAsync(astronautDetail, cancellationToken);
            }

            // Get active duties and end them
            var activeDuties = await _unitOfWork.AstronautDuties.GetByPersonIdAsync(person.Id, cancellationToken);
            var currentActiveDuty = activeDuties.FirstOrDefault(d => d.DutyEndDate == null);

            if (currentActiveDuty != null)
            {
                currentActiveDuty.DutyEndDate = request.DutyStartDate.AddDays(-1).Date;
                await _unitOfWork.AstronautDuties.UpdateAsync(currentActiveDuty, cancellationToken);
            }

            // Create new duty
            var newAstronautDuty = new AstronautDutyEntity
            {
                PersonId = person.Id,
                Rank = request.Rank,
                DutyTitle = request.DutyTitle,
                DutyStartDate = request.DutyStartDate.Date,
                DutyEndDate = null
            };

            await _unitOfWork.AstronautDuties.AddAsync(newAstronautDuty, cancellationToken);

            // Save all changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _loggingService.LogInformationAsync(Category, $"Created astronaut duty (ID: {newAstronautDuty.Id}) for {request.Name}: {request.Rank} - {request.DutyTitle}", source: nameof(CreateAstronautDuty), correlationId: correlationId, cancellationToken: cancellationToken);

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
