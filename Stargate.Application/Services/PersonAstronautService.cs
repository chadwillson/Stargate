using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;
using Stargate.Domain.Interfaces;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.Application.Services
{
    public class PersonAstronautService : IPersonAstronautService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggingService _loggingService;
        private readonly IPersonDomainService _personDomainService;
        private const string Category = "PersonAstronautService";

        public PersonAstronautService(IUnitOfWork unitOfWork, ILoggingService loggingService, IPersonDomainService personDomainService)
        {
            _unitOfWork = unitOfWork;
            _loggingService = loggingService;
            _personDomainService = personDomainService;
        }

        public async Task<PersonAstronautListResponse> GetPeople(string? correlationId, CancellationToken cancellationToken)
        {
            await _loggingService.LogInformationAsync(Category, "Retrieving all people", source: nameof(GetPeople), correlationId: correlationId, cancellationToken: cancellationToken);

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

            await _loggingService.LogInformationAsync(Category, $"Retrieved {people.Count} people", source: nameof(GetPeople), correlationId: correlationId, cancellationToken: cancellationToken);

            return new PersonAstronautListResponse
            {
                People = people
            };
        }

        public async Task<PersonAstronautResponse> GetPersonByName(string name, string? correlationId, CancellationToken cancellationToken)
        {
            await _loggingService.LogInformationAsync(Category, $"Retrieving person by name: {name}", source: nameof(GetPersonByName), correlationId: correlationId, cancellationToken: cancellationToken);

            var result = await _unitOfWork.PersonAstronauts.GetByNameWithDetailsAsync(name, cancellationToken);

            if (result == null)
            {
                await _loggingService.LogWarningAsync(Category, $"Person not found: {name}", source: nameof(GetPersonByName), correlationId: correlationId, cancellationToken: cancellationToken);
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

            await _loggingService.LogInformationAsync(Category, $"Retrieved person: {name} (ID: {result.Id})", source: nameof(GetPersonByName), correlationId: correlationId, cancellationToken: cancellationToken);

            return map;
        }

        public async Task<PersonAstronautResponse> CreatePerson(PersonRequest request, string? correlationId, CancellationToken cancellationToken)
        {
            await _loggingService.LogInformationAsync(Category, $"Creating new person: {request.Name}", source: nameof(CreatePerson), correlationId: correlationId, cancellationToken: cancellationToken);

            // Validate using domain service
            var validationResult = await _personDomainService.ValidatePersonCreationAsync(request.Name, cancellationToken);
            if (!validationResult.IsValid)
            {
                await _loggingService.LogWarningAsync(Category, $"Duplicate person name detected: {request.Name}", source: nameof(CreatePerson), correlationId: correlationId, cancellationToken: cancellationToken);
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

            await _loggingService.LogInformationAsync(Category, $"Created person: {newPerson.Name} (ID: {newPerson.Id})", source: nameof(CreatePerson), correlationId: correlationId, cancellationToken: cancellationToken);

            return new PersonAstronautResponse()
            {
                PersonId = newPerson.Id,
                Name = newPerson.Name
            };
        }

        public async Task<PersonAstronautResponse> UpdatePerson(string name, PersonRequest request, string? correlationId, CancellationToken cancellationToken)
        {
            await _loggingService.LogInformationAsync(Category, $"Updating person: {name} to {request.Name}", source: nameof(UpdatePerson), correlationId: correlationId, cancellationToken: cancellationToken);

            var person = await _unitOfWork.PersonAstronauts.GetByNameAsync(name, cancellationToken);

            if (person == null)
            {
                await _loggingService.LogWarningAsync(Category, $"Person not found for update: {name}", source: nameof(UpdatePerson), correlationId: correlationId, cancellationToken: cancellationToken);
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
                await _loggingService.LogWarningAsync(Category, $"Duplicate person name detected during update: {request.Name}", source: nameof(UpdatePerson), correlationId: correlationId, cancellationToken: cancellationToken);
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

            await _loggingService.LogInformationAsync(Category, $"Updated person: {oldName} -> {person.Name} (ID: {person.Id})", source: nameof(UpdatePerson), correlationId: correlationId, cancellationToken: cancellationToken);

            return new PersonAstronautResponse
            {
                PersonId = person.Id,
                Name = person.Name
            };
        }
    }
}
