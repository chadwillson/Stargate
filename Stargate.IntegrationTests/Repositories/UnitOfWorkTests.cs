using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Stargate.Repository;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.IntegrationTests.Repositories
{
    [TestClass]
    public class UnitOfWorkTests
    {
        private CustomWebApplicationFactory _factory = null!;
        private IServiceScope _scope = null!;
        private StargateContext _context = null!;
        private IUnitOfWork _unitOfWork = null!;

        [TestInitialize]
        public async Task Setup()
        {
            _factory = new CustomWebApplicationFactory();
            await _factory.ResetDatabaseAsync();
            _scope = _factory.Services.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<StargateContext>();
            _unitOfWork = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _scope?.Dispose();
            _factory?.Dispose();
        }

        [TestMethod]
        public void UnitOfWork_ShouldProvideAccessToAllRepositories()
        {
            // Assert
            _unitOfWork.PersonAstronauts.Should().NotBeNull();
            _unitOfWork.AstronautDetails.Should().NotBeNull();
            _unitOfWork.AstronautDuties.Should().NotBeNull();
            _unitOfWork.LogEntries.Should().NotBeNull();
        }

        [TestMethod]
        public async Task SaveChangesAsync_ShouldPersistAllChanges()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Test Person" };
            await _unitOfWork.PersonAstronauts.AddAsync(person);

            // Act
            var result = await _unitOfWork.SaveChangesAsync();

            // Assert
            result.Should().BeGreaterThan(0);
            person.Id.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task SaveChangesAsync_WithMultipleOperations_ShouldPersistAllInTransaction()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Transactional Person" };
            await _unitOfWork.PersonAstronauts.AddAsync(person);
            await _unitOfWork.SaveChangesAsync();

            var detail = new AstronautDetailEntity
            {
                PersonId = person.Id,
                CurrentRank = "Captain",
                CurrentDutyTitle = "Commander",
                CareerStartDate = DateTime.Now
            };
            await _unitOfWork.AstronautDetails.AddAsync(detail);

            var duty = new AstronautDutyEntity
            {
                PersonId = person.Id,
                Rank = "Captain",
                DutyTitle = "Commander",
                DutyStartDate = DateTime.Now
            };
            await _unitOfWork.AstronautDuties.AddAsync(duty);

            // Act
            var result = await _unitOfWork.SaveChangesAsync();

            // Assert
            result.Should().Be(2); // Two entities saved
            detail.Id.Should().BeGreaterThan(0);
            duty.Id.Should().BeGreaterThan(0);

            // Verify data integrity
            var retrievedPerson = await _unitOfWork.PersonAstronauts.GetByNameWithAllRelationsAsync("Transactional Person");
            retrievedPerson.Should().NotBeNull();
            retrievedPerson!.AstronautDetail.Should().NotBeNull();
            retrievedPerson.AstronautDuties.Should().HaveCount(1);
        }

        [TestMethod]
        public async Task SaveChangesAsync_WithNoChanges_ShouldReturnZero()
        {
            // Act
            var result = await _unitOfWork.SaveChangesAsync();

            // Assert
            result.Should().Be(0);
        }

        [TestMethod]
        public async Task UnitOfWork_MultipleRepositories_ShouldShareSameContext()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Shared Context Person" };
            await _unitOfWork.PersonAstronauts.AddAsync(person);
            await _unitOfWork.SaveChangesAsync();

            // Act - Add detail through different repository
            var detail = new AstronautDetailEntity
            {
                PersonId = person.Id,
                CurrentRank = "Major",
                CurrentDutyTitle = "Pilot",
                CareerStartDate = DateTime.Now
            };
            await _unitOfWork.AstronautDetails.AddAsync(detail);
            await _unitOfWork.SaveChangesAsync();

            // Assert - Should be able to retrieve through relationship
            var personWithDetail = await _unitOfWork.PersonAstronauts.GetByNameWithDetailsAsync("Shared Context Person");
            personWithDetail.Should().NotBeNull();
            personWithDetail!.AstronautDetail.Should().NotBeNull();
            personWithDetail.AstronautDetail!.PersonId.Should().Be(person.Id);
        }

        [TestMethod]
        public async Task SaveChangesAsync_WithUpdate_ShouldPersistChanges()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Original Name" };
            await _unitOfWork.PersonAstronauts.AddAsync(person);
            await _unitOfWork.SaveChangesAsync();

            // Act
            person.Name = "Updated Name";
            await _unitOfWork.PersonAstronauts.UpdateAsync(person);
            var result = await _unitOfWork.SaveChangesAsync();

            // Assert
            result.Should().BeGreaterThan(0);
            var retrieved = await _unitOfWork.PersonAstronauts.GetByIdAsync(person.Id);
            retrieved.Should().NotBeNull();
            retrieved!.Name.Should().Be("Updated Name");
        }

        [TestMethod]
        public async Task SaveChangesAsync_WithDelete_ShouldRemoveEntity()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "To Delete" };
            await _unitOfWork.PersonAstronauts.AddAsync(person);
            await _unitOfWork.SaveChangesAsync();
            var personId = person.Id;

            // Act
            await _unitOfWork.PersonAstronauts.DeleteAsync(person);
            var result = await _unitOfWork.SaveChangesAsync();

            // Assert
            result.Should().BeGreaterThan(0);
            var retrieved = await _unitOfWork.PersonAstronauts.GetByIdAsync(personId);
            retrieved.Should().BeNull();
        }

        [TestMethod]
        public async Task SaveChangesAsync_WithLogging_ShouldPersistLogEntries()
        {
            // Arrange
            var logEntry = new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Information",
                Category = "UnitOfWorkTest",
                Message = "Test log message"
            };
            await _unitOfWork.LogEntries.AddAsync(logEntry);

            // Act
            var result = await _unitOfWork.SaveChangesAsync();

            // Assert
            result.Should().BeGreaterThan(0);
            logEntry.Id.Should().BeGreaterThan(0);
            var retrieved = await _unitOfWork.LogEntries.GetByIdAsync(logEntry.Id);
            retrieved.Should().NotBeNull();
        }

        [TestMethod]
        public async Task SaveChangesAsync_MultipleCallsInSequence_ShouldWorkCorrectly()
        {
            // Arrange & Act
            var person1 = new PersonAstronautEntity { Name = "Person 1" };
            await _unitOfWork.PersonAstronauts.AddAsync(person1);
            var result1 = await _unitOfWork.SaveChangesAsync();

            var person2 = new PersonAstronautEntity { Name = "Person 2" };
            await _unitOfWork.PersonAstronauts.AddAsync(person2);
            var result2 = await _unitOfWork.SaveChangesAsync();

            var person3 = new PersonAstronautEntity { Name = "Person 3" };
            await _unitOfWork.PersonAstronauts.AddAsync(person3);
            var result3 = await _unitOfWork.SaveChangesAsync();

            // Assert
            result1.Should().Be(1);
            result2.Should().Be(1);
            result3.Should().Be(1);

            var allPeople = await _unitOfWork.PersonAstronauts.GetAllAsync();
            allPeople.Should().HaveCount(3);
        }
    }
}
