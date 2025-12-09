using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.Application.Services
{
    public class PersonAstronautService : IPersonAstronautService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PersonAstronautService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PersonAstronautListResponse> GetPeople(CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.PersonAstronauts.GetAllWithDetailsAsync(cancellationToken);

            var people = new List<PersonAstronautResponse>();

            foreach (var item in result)
            {
                people.Add(new PersonAstronautResponse
                {
                    CareerEndDate = item.AstronautDetail?.CareerEndDate,
                    CareerStartDate = item.AstronautDetail?.CareerStartDate ?? DateTime.MinValue,
                    CurrentDutyTitle = item.AstronautDetail?.CurrentDutyTitle ?? string.Empty,
                    CurrentRank = item.AstronautDetail?.CurrentRank ?? string.Empty,
                    Name = item.Name,
                    PersonId = item.Id
                });
            }

            return new PersonAstronautListResponse
            {
                People = people
            };
        }

        public async Task<PersonAstronautResponse> GetPersonByName(string name, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.PersonAstronauts.GetByNameWithDetailsAsync(name, cancellationToken);

            if (result == null)
            {
                return new PersonAstronautResponse();
            }

            var map = new PersonAstronautResponse
            {
                CareerEndDate = result.AstronautDetail?.CareerEndDate,
                CareerStartDate = result.AstronautDetail?.CareerStartDate ?? DateTime.MinValue,
                CurrentDutyTitle = result.AstronautDetail?.CurrentDutyTitle ?? string.Empty,
                CurrentRank = result.AstronautDetail?.CurrentRank ?? string.Empty,
                Name = result.Name,
                PersonId = result.Id
            };

            return map;
        }

        public async Task<PersonAstronautResponse> CreatePerson(PersonRequest request, CancellationToken cancellationToken)
        {
            var newPerson = new PersonAstronautEntity()
            {
                Name = request.Name,
            };

            await _unitOfWork.PersonAstronauts.AddAsync(newPerson, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PersonAstronautResponse()
            {
                PersonId = newPerson.Id,
                Name = newPerson.Name
            };
        }

        public async Task<PersonAstronautResponse> UpdatePerson(string name, PersonRequest request, CancellationToken cancellationToken)
        {
            var person = await _unitOfWork.PersonAstronauts.GetByNameAsync(name, cancellationToken);

            if (person == null)
            {
                return new PersonAstronautResponse
                {
                    Success = false,
                    Message = "Person not found",
                    ResponseCode = 404
                };
            }

            person.Name = request.Name;
            await _unitOfWork.PersonAstronauts.UpdateAsync(person, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PersonAstronautResponse
            {
                PersonId = person.Id,
                Name = person.Name
            };
        }
    }
}
