using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.Application.Services
{
    public class AstronautDutyService : IAstronautDutyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AstronautDutyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AstronautDutiesListResponse> GetAstronautDutiesByName(string name, CancellationToken cancellationToken)
        {
            var person = await _unitOfWork.PersonAstronauts.GetByNameWithAllRelationsAsync(name, cancellationToken);

            if (person == null)
            {
                return new AstronautDutiesListResponse
                {
                    Success = false,
                    Message = "Person not found",
                    ResponseCode = 404
                };
            }

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

            return new AstronautDutiesListResponse
            {
                Duties = new List<AstronautDutiesByNameResponse> { response }
            };
        }

        public async Task<CreateAstronautDutyResponse> CreateAstronautDuty(CreateAstronautDutyResponse request, CancellationToken cancellationToken)
        {
            // Get the person by name
            var person = await _unitOfWork.PersonAstronauts.GetByNameAsync(request.Name, cancellationToken);

            if (person == null)
            {
                return new CreateAstronautDutyResponse
                {
                    Name = request.Name,
                    Rank = request.Rank,
                    DutyTitle = request.DutyTitle,
                    DutyStartDate = request.DutyStartDate,
                    Success = false,
                    Message = "Person not found",
                    ResponseCode = 404
                };
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
                    astronautDetail.CareerEndDate = request.DutyStartDate.Date;
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
