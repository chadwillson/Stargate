using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Stargate.Application.Services;
using Stargate.Domain.Dtos;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.UnitTests.Services
{
    [TestClass]
    public class PersonAstronautServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IPersonAstronautRepository> _mockPersonRepo;
        private PersonAstronautService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPersonRepo = new Mock<IPersonAstronautRepository>();

            _mockUnitOfWork.Setup(x => x.PersonAstronauts).Returns(_mockPersonRepo.Object);

            _service = new PersonAstronautService(_mockUnitOfWork.Object);
        }

        [TestMethod]
        public async Task GetPeople_ShouldReturnAllPeople()
        {
            // Arrange
            var people = new List<PersonAstronautEntity>
            {
                new PersonAstronautEntity { Id = 1, Name = "John Doe", AstronautDetail = new AstronautDetailEntity { CurrentRank = "Captain" } },
                new PersonAstronautEntity { Id = 2, Name = "Jane Doe", AstronautDetail = null }
            };
            _mockPersonRepo.Setup(x => x.GetAllWithDetailsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(people);

            // Act
            var result = await _service.GetPeople(CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.People.Should().HaveCount(2);
            result.Success.Should().BeTrue();
        }

        [TestMethod]
        public async Task GetPersonByName_WhenPersonExists_ShouldReturnPerson()
        {
            // Arrange
            var person = new PersonAstronautEntity
            {
                Id = 1,
                Name = "John Doe",
                AstronautDetail = new AstronautDetailEntity { CurrentRank = "Captain", CurrentDutyTitle = "Commander" }
            };
            _mockPersonRepo.Setup(x => x.GetByNameWithDetailsAsync("John Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(person);

            // Act
            var result = await _service.GetPersonByName("John Doe", CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("John Doe");
            result.PersonId.Should().Be(1);
            result.CurrentRank.Should().Be("Captain");
        }

        [TestMethod]
        public async Task GetPersonByName_WhenPersonDoesNotExist_ShouldReturnEmptyResponse()
        {
            // Arrange
            _mockPersonRepo.Setup(x => x.GetByNameWithDetailsAsync("Unknown", It.IsAny<CancellationToken>()))
                .ReturnsAsync((PersonAstronautEntity?)null);

            // Act
            var result = await _service.GetPersonByName("Unknown", CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().BeEmpty();
        }

        [TestMethod]
        public async Task CreatePerson_ShouldCreateAndReturnPerson()
        {
            // Arrange
            var request = new PersonRequest { Name = "New Person" };
            var newPerson = new PersonAstronautEntity { Id = 1, Name = "New Person" };

            _mockPersonRepo.Setup(x => x.AddAsync(It.IsAny<PersonAstronautEntity>(), It.IsAny<CancellationToken>()))
                .Callback<PersonAstronautEntity, CancellationToken>((p, ct) => p.Id = 1)
                .ReturnsAsync(newPerson);

            // Act
            var result = await _service.CreatePerson(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("New Person");
            result.PersonId.Should().Be(1);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdatePerson_WhenPersonExists_ShouldUpdatePerson()
        {
            // Arrange
            var existingPerson = new PersonAstronautEntity { Id = 1, Name = "Old Name" };
            var request = new PersonRequest { Name = "New Name" };

            _mockPersonRepo.Setup(x => x.GetByNameAsync("Old Name", It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingPerson);

            // Act
            var result = await _service.UpdatePerson("Old Name", request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("New Name");
            result.Success.Should().BeTrue();
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdatePerson_WhenPersonDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var request = new PersonRequest { Name = "New Name" };
            _mockPersonRepo.Setup(x => x.GetByNameAsync("Unknown", It.IsAny<CancellationToken>()))
                .ReturnsAsync((PersonAstronautEntity?)null);

            // Act
            var result = await _service.UpdatePerson("Unknown", request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ResponseCode.Should().Be(404);
            result.Message.Should().Be("Person not found");
        }
    }
}
