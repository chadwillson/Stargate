using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Stargate.Repository;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.IntegrationTests.Repositories
{
    [TestClass]
    public class PersonAstronautRepositoryTests
    {
        private CustomWebApplicationFactory _factory = null!;
        private IServiceScope _scope = null!;
        private StargateContext _context = null!;
        private IPersonAstronautRepository _repository = null!;

        [TestInitialize]
        public async Task Setup()
        {
            _factory = new CustomWebApplicationFactory();
            await _factory.ResetDatabaseAsync();
            _scope = _factory.Services.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<StargateContext>();
            _repository = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>().PersonAstronauts;
        }

        [TestCleanup]
        public void Cleanup()
        {
            _scope?.Dispose();
            _factory?.Dispose();
        }

        [TestMethod]
        public async Task AddAsync_ShouldAddPersonToDatabase()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "John Doe" };

            // Act
            await _repository.AddAsync(person);
            await _context.SaveChangesAsync();

            // Assert
            person.Id.Should().BeGreaterThan(0);
            var retrieved = await _repository.GetByIdAsync(person.Id);
            retrieved.Should().NotBeNull();
            retrieved!.Name.Should().Be("John Doe");
        }

        [TestMethod]
        public async Task GetByNameAsync_WhenPersonExists_ShouldReturnPerson()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Jane Smith" };
            await _repository.AddAsync(person);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByNameAsync("Jane Smith");

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Jane Smith");
            result.Id.Should().Be(person.Id);
        }

        [TestMethod]
        public async Task GetByNameAsync_WhenPersonDoesNotExist_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetByNameAsync("Nonexistent Person");

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetByNameWithDetailsAsync_ShouldIncludeAstronautDetail()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Commander Shepard" };
            await _repository.AddAsync(person);
            await _context.SaveChangesAsync();

            var detail = new AstronautDetailEntity
            {
                PersonId = person.Id,
                CurrentRank = "Commander",
                CurrentDutyTitle = "N7 Operative",
                CareerStartDate = DateTime.Now
            };
            _context.AstronautDetails.Add(detail);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByNameWithDetailsAsync("Commander Shepard");

            // Assert
            result.Should().NotBeNull();
            result!.AstronautDetail.Should().NotBeNull();
            result.AstronautDetail!.CurrentRank.Should().Be("Commander");
            result.AstronautDetail.CurrentDutyTitle.Should().Be("N7 Operative");
        }

        [TestMethod]
        public async Task GetByNameWithAllRelationsAsync_ShouldIncludeDetailsAndDuties()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Captain Kirk" };
            await _repository.AddAsync(person);
            await _context.SaveChangesAsync();

            var detail = new AstronautDetailEntity
            {
                PersonId = person.Id,
                CurrentRank = "Captain",
                CurrentDutyTitle = "Starship Commander",
                CareerStartDate = DateTime.Now
            };
            _context.AstronautDetails.Add(detail);

            var duty1 = new AstronautDutyEntity
            {
                PersonId = person.Id,
                Rank = "Lieutenant",
                DutyTitle = "First Officer",
                DutyStartDate = DateTime.Now.AddYears(-5),
                DutyEndDate = DateTime.Now.AddYears(-2)
            };
            var duty2 = new AstronautDutyEntity
            {
                PersonId = person.Id,
                Rank = "Captain",
                DutyTitle = "Starship Commander",
                DutyStartDate = DateTime.Now.AddYears(-2),
                DutyEndDate = null
            };
            _context.AstronautDuties.Add(duty1);
            _context.AstronautDuties.Add(duty2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByNameWithAllRelationsAsync("Captain Kirk");

            // Assert
            result.Should().NotBeNull();
            result!.AstronautDetail.Should().NotBeNull();
            result.AstronautDuties.Should().HaveCount(2);
            result.AstronautDuties.Should().Contain(d => d.Rank == "Lieutenant");
            result.AstronautDuties.Should().Contain(d => d.Rank == "Captain");
        }

        [TestMethod]
        public async Task GetAllWithDetailsAsync_ShouldReturnAllPeopleWithDetails()
        {
            // Arrange
            var person1 = new PersonAstronautEntity { Name = "Person One" };
            var person2 = new PersonAstronautEntity { Name = "Person Two" };
            await _repository.AddAsync(person1);
            await _repository.AddAsync(person2);
            await _context.SaveChangesAsync();

            var detail1 = new AstronautDetailEntity
            {
                PersonId = person1.Id,
                CurrentRank = "Major",
                CurrentDutyTitle = "Pilot",
                CareerStartDate = DateTime.Now
            };
            _context.AstronautDetails.Add(detail1);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllWithDetailsAsync();

            // Assert
            result.Should().HaveCount(2);
            var personOne = result.FirstOrDefault(p => p.Name == "Person One");
            personOne.Should().NotBeNull();
            personOne!.AstronautDetail.Should().NotBeNull();

            var personTwo = result.FirstOrDefault(p => p.Name == "Person Two");
            personTwo.Should().NotBeNull();
            personTwo!.AstronautDetail.Should().BeNull();
        }

        [TestMethod]
        public async Task SearchByNameWithAllRelationsAsync_ShouldReturnMatchingPeople()
        {
            // Arrange
            var person1 = new PersonAstronautEntity { Name = "John Smith" };
            var person2 = new PersonAstronautEntity { Name = "John Doe" };
            var person3 = new PersonAstronautEntity { Name = "Jane Doe" };
            await _repository.AddAsync(person1);
            await _repository.AddAsync(person2);
            await _repository.AddAsync(person3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SearchByNameWithAllRelationsAsync("John");

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(p => p.Name == "John Smith");
            result.Should().Contain(p => p.Name == "John Doe");
            result.Should().NotContain(p => p.Name == "Jane Doe");
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdatePersonInDatabase()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "Old Name" };
            await _repository.AddAsync(person);
            await _context.SaveChangesAsync();

            // Act
            person.Name = "New Name";
            await _repository.UpdateAsync(person);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _repository.GetByIdAsync(person.Id);
            retrieved.Should().NotBeNull();
            retrieved!.Name.Should().Be("New Name");
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldRemovePersonFromDatabase()
        {
            // Arrange
            var person = new PersonAstronautEntity { Name = "To Delete" };
            await _repository.AddAsync(person);
            await _context.SaveChangesAsync();
            var personId = person.Id;

            // Act
            await _repository.DeleteAsync(person);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _repository.GetByIdAsync(personId);
            retrieved.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAllPeople()
        {
            // Arrange
            await _repository.AddAsync(new PersonAstronautEntity { Name = "Person A" });
            await _repository.AddAsync(new PersonAstronautEntity { Name = "Person B" });
            await _repository.AddAsync(new PersonAstronautEntity { Name = "Person C" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
        }
    }
}
