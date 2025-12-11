using Microsoft.Extensions.Logging;
using Stargate.Application.Interfaces;
using Stargate.Domain.Dtos;
using Stargate.Repository;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.Application.Services
{
    public partial class TestDataService : ITestDataService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TestDataService> _logger;
        private readonly StargateContext _context;

        public TestDataService(
            IUnitOfWork unitOfWork,
            ILogger<TestDataService> logger,
            StargateContext context)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _context = context;
        }

        public async Task<TestDataResponse> CreateBasicScenarioAsync(CancellationToken cancellationToken = default)
        {
            LogCreatingBasicScenario();

            var person = new PersonAstronautEntity
            {
                Name = "Test Basic Astronaut"
            };
            await _unitOfWork.PersonAstronauts.AddAsync(person, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var detail = new AstronautDetailEntity
            {
                PersonId = person.Id,
                CurrentRank = "Captain",
                CurrentDutyTitle = "Pilot",
                CareerStartDate = DateTime.UtcNow.Date,
                CareerEndDate = null
            };
            await _unitOfWork.AstronautDetails.AddAsync(detail, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var duty = new AstronautDutyEntity
            {
                PersonId = person.Id,
                Rank = "Captain",
                DutyTitle = "Pilot",
                DutyStartDate = DateTime.UtcNow.Date,
                DutyEndDate = null
            };
            await _unitOfWork.AstronautDuties.AddAsync(duty, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            LogCreatedBasicScenario(person.Id, detail.Id, duty.Id);

            return new TestDataResponse
            {
                Scenario = "Basic",
                Description = "Single astronaut with one active duty",
                CreatedPersonIds = new List<int> { person.Id },
                CreatedDetailIds = new List<int> { detail.Id },
                CreatedDutyIds = new List<int> { duty.Id }
            };
        }

        public async Task<TestDataResponse> CreateComplexScenarioAsync(CancellationToken cancellationToken = default)
        {
            LogCreatingComplexScenario();

            var personIds = new List<int>();
            var detailIds = new List<int>();
            var dutyIds = new List<int>();

            var people = new[]
            {
                new PersonAstronautEntity { Name = "Test Complex Person 1" },
                new PersonAstronautEntity { Name = "Test Complex Person 2" },
                new PersonAstronautEntity { Name = "Test Complex Person 3" }
            };

            foreach (var person in people)
            {
                await _unitOfWork.PersonAstronauts.AddAsync(person, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                personIds.Add(person.Id);

                var detail = new AstronautDetailEntity
                {
                    PersonId = person.Id,
                    CurrentRank = "Colonel",
                    CurrentDutyTitle = "Commander",
                    CareerStartDate = DateTime.UtcNow.AddYears(-5).Date,
                    CareerEndDate = null
                };
                await _unitOfWork.AstronautDetails.AddAsync(detail, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                detailIds.Add(detail.Id);

                var duties = new[]
                {
                    new AstronautDutyEntity
                    {
                        PersonId = person.Id,
                        Rank = "Lieutenant",
                        DutyTitle = "Junior Officer",
                        DutyStartDate = DateTime.UtcNow.AddYears(-5).Date,
                        DutyEndDate = DateTime.UtcNow.AddYears(-3).Date
                    },
                    new AstronautDutyEntity
                    {
                        PersonId = person.Id,
                        Rank = "Captain",
                        DutyTitle = "Team Leader",
                        DutyStartDate = DateTime.UtcNow.AddYears(-3).Date,
                        DutyEndDate = DateTime.UtcNow.AddYears(-1).Date
                    },
                    new AstronautDutyEntity
                    {
                        PersonId = person.Id,
                        Rank = "Colonel",
                        DutyTitle = "Commander",
                        DutyStartDate = DateTime.UtcNow.AddYears(-1).Date,
                        DutyEndDate = null
                    }
                };

                foreach (var duty in duties)
                {
                    await _unitOfWork.AstronautDuties.AddAsync(duty, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    dutyIds.Add(duty.Id);
                }
            }

            LogCreatedComplexScenario(people.Length);

            return new TestDataResponse
            {
                Scenario = "Complex",
                Description = "Multiple astronauts with multiple duties and career progression",
                CreatedPersonIds = personIds,
                CreatedDetailIds = detailIds,
                CreatedDutyIds = dutyIds
            };
        }

        public async Task<TestDataResponse> CreateEdgeCaseScenarioAsync(CancellationToken cancellationToken = default)
        {
            LogCreatingEdgeCaseScenario();

            var personIds = new List<int>();

            var personWithoutDetails = new PersonAstronautEntity
            {
                Name = "Test Person No Details"
            };
            await _unitOfWork.PersonAstronauts.AddAsync(personWithoutDetails, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            personIds.Add(personWithoutDetails.Id);

            var personWithDetailsNoDuties = new PersonAstronautEntity
            {
                Name = "Test Person No Duties"
            };
            await _unitOfWork.PersonAstronauts.AddAsync(personWithDetailsNoDuties, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            personIds.Add(personWithDetailsNoDuties.Id);

            var detail = new AstronautDetailEntity
            {
                PersonId = personWithDetailsNoDuties.Id,
                CurrentRank = "Cadet",
                CurrentDutyTitle = "In Training",
                CareerStartDate = DateTime.UtcNow.Date,
                CareerEndDate = null
            };
            await _unitOfWork.AstronautDetails.AddAsync(detail, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            LogCreatedEdgeCaseScenario();

            return new TestDataResponse
            {
                Scenario = "EdgeCase",
                Description = "Edge cases: person without details, person without duties",
                CreatedPersonIds = personIds,
                CreatedDetailIds = new List<int> { detail.Id },
                CreatedDutyIds = new List<int>()
            };
        }

        public async Task<TestDataResponse> CreateRetiredAstronautScenarioAsync(CancellationToken cancellationToken = default)
        {
            LogCreatingRetiredAstronautScenario();

            var person = new PersonAstronautEntity
            {
                Name = "Test Retired Astronaut"
            };
            await _unitOfWork.PersonAstronauts.AddAsync(person, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var retirementDate = DateTime.UtcNow.AddMonths(-1).Date;
            var careerEndDate = retirementDate.AddDays(-1);

            var detail = new AstronautDetailEntity
            {
                PersonId = person.Id,
                CurrentRank = "Colonel",
                CurrentDutyTitle = "RETIRED",
                CareerStartDate = DateTime.UtcNow.AddYears(-20).Date,
                CareerEndDate = careerEndDate
            };
            await _unitOfWork.AstronautDetails.AddAsync(detail, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var duties = new[]
            {
                new AstronautDutyEntity
                {
                    PersonId = person.Id,
                    Rank = "Captain",
                    DutyTitle = "Squadron Leader",
                    DutyStartDate = DateTime.UtcNow.AddYears(-20).Date,
                    DutyEndDate = DateTime.UtcNow.AddYears(-10).Date
                },
                new AstronautDutyEntity
                {
                    PersonId = person.Id,
                    Rank = "Colonel",
                    DutyTitle = "Base Commander",
                    DutyStartDate = DateTime.UtcNow.AddYears(-10).Date,
                    DutyEndDate = careerEndDate
                },
                new AstronautDutyEntity
                {
                    PersonId = person.Id,
                    Rank = "Colonel",
                    DutyTitle = "RETIRED",
                    DutyStartDate = retirementDate,
                    DutyEndDate = null
                }
            };

            var dutyIds = new List<int>();
            foreach (var duty in duties)
            {
                await _unitOfWork.AstronautDuties.AddAsync(duty, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                dutyIds.Add(duty.Id);
            }

            LogCreatedRetiredAstronautScenario(person.Id, retirementDate);

            return new TestDataResponse
            {
                Scenario = "Retired",
                Description = "Retired astronaut with complete career history and retirement date",
                CreatedPersonIds = new List<int> { person.Id },
                CreatedDetailIds = new List<int> { detail.Id },
                CreatedDutyIds = dutyIds
            };
        }

        public async Task<TestDataResponse> CreateMultipleDutiesScenarioAsync(CancellationToken cancellationToken = default)
        {
            LogCreatingMultipleDutiesScenario();

            var person = new PersonAstronautEntity
            {
                Name = "Test Multi Duty Astronaut"
            };
            await _unitOfWork.PersonAstronauts.AddAsync(person, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var detail = new AstronautDetailEntity
            {
                PersonId = person.Id,
                CurrentRank = "Major",
                CurrentDutyTitle = "Executive Officer",
                CareerStartDate = DateTime.UtcNow.AddYears(-10).Date,
                CareerEndDate = null
            };
            await _unitOfWork.AstronautDetails.AddAsync(detail, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var duties = new[]
            {
                new AstronautDutyEntity
                {
                    PersonId = person.Id,
                    Rank = "Second Lieutenant",
                    DutyTitle = "Training Officer",
                    DutyStartDate = DateTime.UtcNow.AddYears(-10).Date,
                    DutyEndDate = DateTime.UtcNow.AddYears(-8).Date
                },
                new AstronautDutyEntity
                {
                    PersonId = person.Id,
                    Rank = "First Lieutenant",
                    DutyTitle = "Platoon Leader",
                    DutyStartDate = DateTime.UtcNow.AddYears(-8).Date,
                    DutyEndDate = DateTime.UtcNow.AddYears(-6).Date
                },
                new AstronautDutyEntity
                {
                    PersonId = person.Id,
                    Rank = "Captain",
                    DutyTitle = "Company Commander",
                    DutyStartDate = DateTime.UtcNow.AddYears(-6).Date,
                    DutyEndDate = DateTime.UtcNow.AddYears(-3).Date
                },
                new AstronautDutyEntity
                {
                    PersonId = person.Id,
                    Rank = "Major",
                    DutyTitle = "Executive Officer",
                    DutyStartDate = DateTime.UtcNow.AddYears(-3).Date,
                    DutyEndDate = null
                }
            };

            var dutyIds = new List<int>();
            foreach (var duty in duties)
            {
                await _unitOfWork.AstronautDuties.AddAsync(duty, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                dutyIds.Add(duty.Id);
            }

            LogCreatedMultipleDutiesScenario(person.Id, duties.Length);

            return new TestDataResponse
            {
                Scenario = "MultipleDuties",
                Description = "Single astronaut with complete career progression through multiple duties",
                CreatedPersonIds = new List<int> { person.Id },
                CreatedDetailIds = new List<int> { detail.Id },
                CreatedDutyIds = dutyIds
            };
        }

        public async Task<BaseResponse> ClearAllTestDataAsync(CancellationToken cancellationToken = default)
        {
            LogClearingAllTestData();

            try
            {
                var duties = await _unitOfWork.AstronautDuties.GetAllAsync(cancellationToken);
                foreach (var duty in duties)
                {
                    await _unitOfWork.AstronautDuties.DeleteAsync(duty, cancellationToken);
                }

                var details = await _unitOfWork.AstronautDetails.GetAllAsync(cancellationToken);
                foreach (var detail in details)
                {
                    await _unitOfWork.AstronautDetails.DeleteAsync(detail, cancellationToken);
                }

                var people = await _unitOfWork.PersonAstronauts.GetAllAsync(cancellationToken);
                foreach (var person in people)
                {
                    await _unitOfWork.PersonAstronauts.DeleteAsync(person, cancellationToken);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                LogClearedAllTestData();

                return new BaseResponse
                {
                    Success = true,
                    Message = "All test data cleared successfully"
                };
            }
            catch (Exception ex)
            {
                LogErrorClearingTestData(ex);
                return new BaseResponse
                {
                    Success = false,
                    Message = $"Error clearing test data: {ex.Message}",
                    ResponseCode = 500
                };
            }
        }

        public async Task<BaseResponse> ResetToDefaultSeedDataAsync(CancellationToken cancellationToken = default)
        {
            LogResettingToDefaultSeedData();

            try
            {
                await ClearAllTestDataAsync(cancellationToken);

                DatabaseSeeder.Seed(_context);

                LogResetToDefaultSeedData();

                return new BaseResponse
                {
                    Success = true,
                    Message = "Database reset to default seed data successfully"
                };
            }
            catch (Exception ex)
            {
                LogErrorResettingToDefaultSeedData(ex);
                return new BaseResponse
                {
                    Success = false,
                    Message = $"Error resetting to default seed data: {ex.Message}",
                    ResponseCode = 500
                };
            }
        }

        public async Task<PersonAstronautEntity> CreatePersonAsync(string name, CancellationToken cancellationToken = default)
        {
            var person = new PersonAstronautEntity { Name = name };
            await _unitOfWork.PersonAstronauts.AddAsync(person, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return person;
        }

        public async Task<AstronautDetailEntity> CreateAstronautDetailAsync(
            int personId,
            string rank,
            string dutyTitle,
            DateTime careerStartDate,
            DateTime? careerEndDate = null,
            CancellationToken cancellationToken = default)
        {
            var detail = new AstronautDetailEntity
            {
                PersonId = personId,
                CurrentRank = rank,
                CurrentDutyTitle = dutyTitle,
                CareerStartDate = careerStartDate,
                CareerEndDate = careerEndDate
            };
            await _unitOfWork.AstronautDetails.AddAsync(detail, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return detail;
        }

        public async Task<AstronautDutyEntity> CreateAstronautDutyAsync(
            int personId,
            string rank,
            string dutyTitle,
            DateTime dutyStartDate,
            DateTime? dutyEndDate = null,
            CancellationToken cancellationToken = default)
        {
            var duty = new AstronautDutyEntity
            {
                PersonId = personId,
                Rank = rank,
                DutyTitle = dutyTitle,
                DutyStartDate = dutyStartDate,
                DutyEndDate = dutyEndDate
            };
            await _unitOfWork.AstronautDuties.AddAsync(duty, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return duty;
        }
    }
}
