using Microsoft.Extensions.Logging;
using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;
using Stargate.Domain.Interfaces;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.Application.Services
{
    public partial class PersonAstronautService : IPersonAstronautService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PersonAstronautService> _logger;
        private readonly IPersonDomainService _personDomainService;

        public PersonAstronautService(IUnitOfWork unitOfWork, ILogger<PersonAstronautService> logger, IPersonDomainService personDomainService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _personDomainService = personDomainService;
        }

        public async Task<PersonAstronautListResponse> GetPeople(CancellationToken cancellationToken)
        {
            LogRetrievingAllPeople();

            var result = await _unitOfWork.PersonAstronauts.GetAllWithDetailsAsync(cancellationToken);

            var people = new List<PersonAstronautResponse>();

            foreach (var item in result)
            {
                people.Add(new PersonAstronautResponse
                {
                    CareerEndDate = item.AstronautDetail?.CareerEndDate,
                    CareerStartDate = item.AstronautDetail?.CareerStartDate,
                    CurrentDutyTitle = item.AstronautDetail?.CurrentDutyTitle,
                    CurrentRank = item.AstronautDetail?.CurrentRank,
                    Name = item.Name,
                    PersonId = item.Id
                });
            }

            LogRetrievedPeople(people.Count);

            return new PersonAstronautListResponse
            {
                People = people
            };
        }

        public async Task<PersonAstronautResponse> GetPersonByName(string name, CancellationToken cancellationToken)
        {
            LogRetrievingPersonByName(name);

            var result = await _unitOfWork.PersonAstronauts.GetByNameWithDetailsAsync(name, cancellationToken);

            if (result == null)
            {
                LogPersonNotFound(name);
                return new PersonAstronautResponse
                {
                    Success = false,
                    Message = "Person not found",
                    ResponseCode = 404
                };
            }

            var map = new PersonAstronautResponse
            {
                CareerEndDate = result.AstronautDetail?.CareerEndDate,
                CareerStartDate = result.AstronautDetail?.CareerStartDate,
                CurrentDutyTitle = result.AstronautDetail?.CurrentDutyTitle,
                CurrentRank = result.AstronautDetail?.CurrentRank,
                Name = result.Name,
                PersonId = result.Id
            };

            LogRetrievedPerson(name, result.Id);

            return map;
        }

        public async Task<PersonAstronautResponse> CreatePerson(PersonRequest request, CancellationToken cancellationToken)
        {
            LogCreatingPerson(request.Name);

            // Validate using domain service
            var validationResult = await _personDomainService.ValidatePersonCreationAsync(request.Name, cancellationToken);
            if (!validationResult.IsValid)
            {
                LogDuplicatePersonName(request.Name);
                return new PersonAstronautResponse
                {
                    Success = false,
                    Message = validationResult.ErrorMessage,
                    ResponseCode = 409
                };
            }

            var newPerson = new PersonAstronautEntity()
            {
                Name = request.Name,
            };

            await _unitOfWork.PersonAstronauts.AddAsync(newPerson, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            LogCreatedPerson(newPerson.Name, newPerson.Id);

            return new PersonAstronautResponse()
            {
                PersonId = newPerson.Id,
                Name = newPerson.Name
            };
        }

        public async Task<PersonAstronautResponse> UpdatePerson(string name, PersonRequest request, CancellationToken cancellationToken)
        {
            LogUpdatingPerson(name, request.Name);

            var person = await _unitOfWork.PersonAstronauts.GetByNameAsync(name, cancellationToken);

            if (person == null)
            {
                LogPersonNotFoundForUpdate(name);
                return new PersonAstronautResponse
                {
                    Success = false,
                    Message = "Person not found",
                    ResponseCode = 404
                };
            }

            // Validate using domain service
            var validationResult = await _personDomainService.ValidatePersonUpdateAsync(person.Id, person.Name, request.Name, cancellationToken);
            if (!validationResult.IsValid)
            {
                LogDuplicatePersonNameDuringUpdate(request.Name);
                return new PersonAstronautResponse
                {
                    Success = false,
                    Message = validationResult.ErrorMessage,
                    ResponseCode = 409
                };
            }

            var oldName = person.Name;
            person.Name = request.Name;
            await _unitOfWork.PersonAstronauts.UpdateAsync(person, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            LogUpdatedPerson(oldName, person.Name, person.Id);

            return new PersonAstronautResponse
            {
                PersonId = person.Id,
                Name = person.Name
            };
        }
    }
}
