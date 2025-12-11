using Stargate.Domain.Interfaces;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.Domain.Services
{
    public class AstronautDutyDomainService : IAstronautDutyDomainService
    {
        private readonly IPersonAstronautRepository _personRepository;
        private readonly IAstronautDutyRepository _dutyRepository;

        public AstronautDutyDomainService(
            IPersonAstronautRepository personRepository,
            IAstronautDutyRepository dutyRepository)
        {
            _personRepository = personRepository;
            _dutyRepository = dutyRepository;
        }

        public async Task<PersonAstronautEntity?> EnsurePersonExistsAsync(string name, CancellationToken cancellationToken = default)
        {
            var person = await _personRepository.GetByNameAsync(name, cancellationToken);

            if (person == null)
            {
                // Return null to indicate person needs to be created
                // Application layer will handle the creation and persistence
                return null;
            }

            return person;
        }

        public AstronautDetailEntity PrepareAstronautDetail(AstronautDetailEntity? existing, string rank, string dutyTitle, DateTime startDate, int personId)
        {
            if (existing == null)
            {
                // Create new astronaut detail
                var newDetail = new AstronautDetailEntity
                {
                    PersonId = personId,
                    CurrentDutyTitle = dutyTitle,
                    CurrentRank = rank,
                    CareerStartDate = startDate.Date
                };

                // Apply RETIRED business rule
                if (dutyTitle == "RETIRED")
                {
                    newDetail.CareerEndDate = startDate.AddDays(-1).Date;
                }

                return newDetail;
            }
            else
            {
                // Update existing astronaut detail
                existing.CurrentDutyTitle = dutyTitle;
                existing.CurrentRank = rank;

                // Apply RETIRED business rule
                if (dutyTitle == "RETIRED")
                {
                    existing.CareerEndDate = startDate.AddDays(-1).Date;
                }
                else
                {
                    // Clear career end date if returning from retirement
                    existing.CareerEndDate = null;
                }

                return existing;
            }
        }

        public async Task<AstronautDutyEntity?> GetAndTerminateActiveDutyAsync(int personId, DateTime newDutyStartDate, CancellationToken cancellationToken = default)
        {
            var activeDuty = await _dutyRepository.GetActiveDutyAsync(personId, cancellationToken);

            if (activeDuty != null)
            {
                // Business rule: Previous duty ends one day before new duty starts
                activeDuty.DutyEndDate = newDutyStartDate.AddDays(-1).Date;
            }

            return activeDuty;
        }

        public AstronautDutyEntity CreateNewDuty(int personId, string rank, string dutyTitle, DateTime startDate)
        {
            return new AstronautDutyEntity
            {
                PersonId = personId,
                Rank = rank,
                DutyTitle = dutyTitle,
                DutyStartDate = startDate.Date,
                DutyEndDate = null
            };
        }
    }
}
