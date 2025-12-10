using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Stargate.Repository;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.IntegrationTests.Repositories
{
    [TestClass]
    public class AstronautDetailRepositoryTests
    {
        private CustomWebApplicationFactory _factory = null!;
        private IServiceScope _scope = null!;
        private StargateContext _context = null!;
        private IAstronautDetailRepository _repository = null!;
        private IPersonAstronautRepository _personRepository = null!;

        [TestInitialize]
        public async Task Setup()
        {
            _factory = new CustomWebApplicationFactory();
            await _factory.ResetDatabaseAsync();
            _scope = _factory.Services.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<StargateContext>();
            var unitOfWork = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            _repository = unitOfWork.AstronautDetails;
            _personRepository = unitOfWork.PersonAstronauts;
        }

        [TestCleanup]
        public void Cleanup()
        {
            _scope?.Dispose();
            _factory?.Dispose();
        }

        [TestMethod]
        public async Task AddAsync_ShouldAddDetailToDatabase()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Test Person" };
            await _personRepository.AddAsync(person);
            await _context.SaveChangesAsync();

            var detail = new AstronautDetailEntity
            {
                PersonId = person.Id,
                CurrentRank = "Captain",
                CurrentDutyTitle = "Commander",
                CareerStartDate = DateTime.Now
            };

            // Act
            await _repository.AddAsync(detail);
            await _context.SaveChangesAsync();

            // Assert
            detail.Id.Should().BeGreaterThan(0);
            var retrieved = await _repository.GetByIdAsync(detail.Id);
            retrieved.Should().NotBeNull();
            retrieved!.CurrentRank.Should().Be("Captain");
        }

        [TestMethod]
        public async Task GetByPersonIdAsync_WhenDetailExists_ShouldReturnDetail()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Astronaut One" };
            await _personRepository.AddAsync(person);
            await _context.SaveChangesAsync();

            var detail = new AstronautDetailEntity
            {
                PersonId = person.Id,
                CurrentRank = "Major",
                CurrentDutyTitle = "Pilot",
                CareerStartDate = DateTime.Now
            };
            await _repository.AddAsync(detail);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByPersonIdAsync(person.Id);

            // Assert
            result.Should().NotBeNull();
            result!.CurrentRank.Should().Be("Major");
            result.CurrentDutyTitle.Should().Be("Pilot");
            result.PersonId.Should().Be(person.Id);
        }

        [TestMethod]
        public async Task GetByPersonIdAsync_WhenDetailDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetByPersonIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdateDetailInDatabase()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Promoted Astronaut" };
            await _personRepository.AddAsync(person);
            await _context.SaveChangesAsync();

            var detail = new AstronautDetailEntity
            {
                PersonId = person.Id,
                CurrentRank = "Lieutenant",
                CurrentDutyTitle = "Co-Pilot",
                CareerStartDate = DateTime.Now
            };
            await _repository.AddAsync(detail);
            await _context.SaveChangesAsync();

            // Act
            detail.CurrentRank = "Captain";
            detail.CurrentDutyTitle = "Commander";
            await _repository.UpdateAsync(detail);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _repository.GetByPersonIdAsync(person.Id);
            retrieved.Should().NotBeNull();
            retrieved!.CurrentRank.Should().Be("Captain");
            retrieved.CurrentDutyTitle.Should().Be("Commander");
        }

        [TestMethod]
        public async Task UpdateAsync_WithCareerEndDate_ShouldUpdateCorrectly()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Retiring Astronaut" };
            await _personRepository.AddAsync(person);
            await _context.SaveChangesAsync();

            var detail = new AstronautDetailEntity
            {
                PersonId = person.Id,
                CurrentRank = "Colonel",
                CurrentDutyTitle = "Flight Director",
                CareerStartDate = DateTime.Now.AddYears(-10),
                CareerEndDate = null
            };
            await _repository.AddAsync(detail);
            await _context.SaveChangesAsync();

            // Act
            var endDate = DateTime.Now.AddDays(-1);
            detail.CareerEndDate = endDate;
            detail.CurrentDutyTitle = "RETIRED";
            await _repository.UpdateAsync(detail);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _repository.GetByPersonIdAsync(person.Id);
            retrieved.Should().NotBeNull();
            retrieved!.CareerEndDate.Should().NotBeNull();
            retrieved.CareerEndDate!.Value.Date.Should().Be(endDate.Date);
            retrieved.CurrentDutyTitle.Should().Be("RETIRED");
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldRemoveDetailFromDatabase()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Delete Test" };
            await _personRepository.AddAsync(person);
            await _context.SaveChangesAsync();

            var detail = new AstronautDetailEntity
            {
                PersonId = person.Id,
                CurrentRank = "Ensign",
                CurrentDutyTitle = "Cadet",
                CareerStartDate = DateTime.Now
            };
            await _repository.AddAsync(detail);
            await _context.SaveChangesAsync();
            var detailId = detail.Id;

            // Act
            await _repository.DeleteAsync(detail);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _repository.GetByIdAsync(detailId);
            retrieved.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAllDetails()
        {
            // Arrange
            var person1 = new PersonAstronautEntity { Name = "Person 1" };
            var person2 = new PersonAstronautEntity { Name = "Person 2" };
            await _personRepository.AddAsync(person1);
            await _personRepository.AddAsync(person2);
            await _context.SaveChangesAsync();

            await _repository.AddAsync(new AstronautDetailEntity
            {
                PersonId = person1.Id,
                CurrentRank = "Captain",
                CurrentDutyTitle = "Commander",
                CareerStartDate = DateTime.Now
            });
            await _repository.AddAsync(new AstronautDetailEntity
            {
                PersonId = person2.Id,
                CurrentRank = "Major",
                CurrentDutyTitle = "Pilot",
                CareerStartDate = DateTime.Now
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
        }
    }
}
