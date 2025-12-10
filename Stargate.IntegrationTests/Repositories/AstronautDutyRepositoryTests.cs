using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Stargate.Repository;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.IntegrationTests.Repositories
{
    [TestClass]
    public class AstronautDutyRepositoryTests
    {
        private CustomWebApplicationFactory _factory = null!;
        private IServiceScope _scope = null!;
        private StargateContext _context = null!;
        private IAstronautDutyRepository _repository = null!;
        private IPersonAstronautRepository _personRepository = null!;

        [TestInitialize]
        public async Task Setup()
        {
            _factory = new CustomWebApplicationFactory();
            await _factory.ResetDatabaseAsync();
            _scope = _factory.Services.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<StargateContext>();
            var unitOfWork = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            _repository = unitOfWork.AstronautDuties;
            _personRepository = unitOfWork.PersonAstronauts;
        }

        [TestCleanup]
        public void Cleanup()
        {
            _scope?.Dispose();
            _factory?.Dispose();
        }

        [TestMethod]
        public async Task AddAsync_ShouldAddDutyToDatabase()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Test Astronaut" };
            await _personRepository.AddAsync(person);
            await _context.SaveChangesAsync();

            var duty = new AstronautDutyEntity
            {
                PersonId = person.Id,
                Rank = "Lieutenant",
                DutyTitle = "Navigator",
                DutyStartDate = DateTime.Now,
                DutyEndDate = null
            };

            // Act
            await _repository.AddAsync(duty);
            await _context.SaveChangesAsync();

            // Assert
            duty.Id.Should().BeGreaterThan(0);
            var retrieved = await _repository.GetByIdAsync(duty.Id);
            retrieved.Should().NotBeNull();
            retrieved!.Rank.Should().Be("Lieutenant");
            retrieved.DutyTitle.Should().Be("Navigator");
        }

        [TestMethod]
        public async Task GetByPersonIdAsync_ShouldReturnAllDutiesForPerson()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Career Astronaut" };
            await _personRepository.AddAsync(person);
            await _context.SaveChangesAsync();

            var duty1 = new AstronautDutyEntity
            {
                PersonId = person.Id,
                Rank = "Ensign",
                DutyTitle = "Flight Engineer",
                DutyStartDate = DateTime.Now.AddYears(-5),
                DutyEndDate = DateTime.Now.AddYears(-3)
            };
            var duty2 = new AstronautDutyEntity
            {
                PersonId = person.Id,
                Rank = "Lieutenant",
                DutyTitle = "Mission Specialist",
                DutyStartDate = DateTime.Now.AddYears(-3),
                DutyEndDate = DateTime.Now.AddYears(-1)
            };
            var duty3 = new AstronautDutyEntity
            {
                PersonId = person.Id,
                Rank = "Captain",
                DutyTitle = "Commander",
                DutyStartDate = DateTime.Now.AddYears(-1),
                DutyEndDate = null
            };

            await _repository.AddAsync(duty1);
            await _repository.AddAsync(duty2);
            await _repository.AddAsync(duty3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByPersonIdAsync(person.Id);

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(d => d.Rank == "Ensign");
            result.Should().Contain(d => d.Rank == "Lieutenant");
            result.Should().Contain(d => d.Rank == "Captain");
        }

        [TestMethod]
        public async Task GetByPersonIdAsync_WhenNoDuties_ShouldReturnEmptyList()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Non-Astronaut" };
            await _personRepository.AddAsync(person);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByPersonIdAsync(person.Id);

            // Assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public async Task GetByPersonIdAsync_WithMultiplePeople_ShouldOnlyReturnDutiesForSpecificPerson()
        {
            // Arrange
            var person1 = new PersonAstronautEntity { Name = "Astronaut One" };
            var person2 = new PersonAstronautEntity { Name = "Astronaut Two" };
            await _personRepository.AddAsync(person1);
            await _personRepository.AddAsync(person2);
            await _context.SaveChangesAsync();

            await _repository.AddAsync(new AstronautDutyEntity
            {
                PersonId = person1.Id,
                Rank = "Captain",
                DutyTitle = "Commander",
                DutyStartDate = DateTime.Now
            });
            await _repository.AddAsync(new AstronautDutyEntity
            {
                PersonId = person2.Id,
                Rank = "Major",
                DutyTitle = "Pilot",
                DutyStartDate = DateTime.Now
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByPersonIdAsync(person1.Id);

            // Assert
            result.Should().HaveCount(1);
            result.First().PersonId.Should().Be(person1.Id);
            result.First().Rank.Should().Be("Captain");
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdateDutyInDatabase()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Active Astronaut" };
            await _personRepository.AddAsync(person);
            await _context.SaveChangesAsync();

            var duty = new AstronautDutyEntity
            {
                PersonId = person.Id,
                Rank = "Lieutenant",
                DutyTitle = "Co-Pilot",
                DutyStartDate = DateTime.Now.AddYears(-1),
                DutyEndDate = null
            };
            await _repository.AddAsync(duty);
            await _context.SaveChangesAsync();

            // Act
            var endDate = DateTime.Now.AddDays(-1);
            duty.DutyEndDate = endDate;
            await _repository.UpdateAsync(duty);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _repository.GetByIdAsync(duty.Id);
            retrieved.Should().NotBeNull();
            retrieved!.DutyEndDate.Should().NotBeNull();
            retrieved.DutyEndDate!.Value.Date.Should().Be(endDate.Date);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldRemoveDutyFromDatabase()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Temp Astronaut" };
            await _personRepository.AddAsync(person);
            await _context.SaveChangesAsync();

            var duty = new AstronautDutyEntity
            {
                PersonId = person.Id,
                Rank = "Ensign",
                DutyTitle = "Trainee",
                DutyStartDate = DateTime.Now
            };
            await _repository.AddAsync(duty);
            await _context.SaveChangesAsync();
            var dutyId = duty.Id;

            // Act
            await _repository.DeleteAsync(duty);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _repository.GetByIdAsync(dutyId);
            retrieved.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAllDuties()
        {
            // Arrange
            var person1 = new PersonAstronautEntity { Name = "Person 1" };
            var person2 = new PersonAstronautEntity { Name = "Person 2" };
            await _personRepository.AddAsync(person1);
            await _personRepository.AddAsync(person2);
            await _context.SaveChangesAsync();

            await _repository.AddAsync(new AstronautDutyEntity
            {
                PersonId = person1.Id,
                Rank = "Captain",
                DutyTitle = "Commander",
                DutyStartDate = DateTime.Now
            });
            await _repository.AddAsync(new AstronautDutyEntity
            {
                PersonId = person1.Id,
                Rank = "Lieutenant",
                DutyTitle = "Co-Pilot",
                DutyStartDate = DateTime.Now.AddYears(-2),
                DutyEndDate = DateTime.Now.AddDays(-1)
            });
            await _repository.AddAsync(new AstronautDutyEntity
            {
                PersonId = person2.Id,
                Rank = "Major",
                DutyTitle = "Navigator",
                DutyStartDate = DateTime.Now
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
        }

        [TestMethod]
        public async Task AddAsync_WithNullDutyEndDate_ShouldCreateActiveDuty()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "New Commander" };
            await _personRepository.AddAsync(person);
            await _context.SaveChangesAsync();

            var duty = new AstronautDutyEntity
            {
                PersonId = person.Id,
                Rank = "Commander",
                DutyTitle = "Station Chief",
                DutyStartDate = DateTime.Now,
                DutyEndDate = null
            };

            // Act
            await _repository.AddAsync(duty);
            await _context.SaveChangesAsync();

            // Assert
            var duties = await _repository.GetByPersonIdAsync(person.Id);
            var activeDuty = duties.FirstOrDefault(d => d.DutyEndDate == null);
            activeDuty.Should().NotBeNull();
            activeDuty!.Rank.Should().Be("Commander");
        }
    }
}
