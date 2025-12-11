using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using Stargate.Application.Interfaces;
using Stargate.Application.Services;
using Stargate.Domain.Dtos;
using Stargate.Domain.Interfaces;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.UnitTests.Services
{
    [TestClass]
    public class AstronautDutyServiceTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IPersonAstronautRepository> _mockPersonRepo;
        private Mock<IAstronautDetailRepository> _mockDetailRepo;
        private Mock<IAstronautDutyRepository> _mockDutyRepo;
        private Mock<ILogger<AstronautDutyService>> _mockLogger;
        private Mock<IAstronautDutyDomainService> _mockDutyDomainService;
        private AstronautDutyService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPersonRepo = new Mock<IPersonAstronautRepository>();
            _mockDetailRepo = new Mock<IAstronautDetailRepository>();
            _mockDutyRepo = new Mock<IAstronautDutyRepository>();
            _mockLogger = new Mock<ILogger<AstronautDutyService>>();
            _mockDutyDomainService = new Mock<IAstronautDutyDomainService>();

            _mockUnitOfWork.Setup(x => x.PersonAstronauts).Returns(_mockPersonRepo.Object);
            _mockUnitOfWork.Setup(x => x.AstronautDetails).Returns(_mockDetailRepo.Object);
            _mockUnitOfWork.Setup(x => x.AstronautDuties).Returns(_mockDutyRepo.Object);

            _service = new AstronautDutyService(_mockUnitOfWork.Object, _mockLogger.Object, _mockDutyDomainService.Object);
        }

        [TestMethod]
        public async Task GetAstronautDutiesByName_WhenPersonExists_ShouldReturnDuties()
        {
            // Arrange
            var person = new PersonAstronautEntity
            {
                Id = 1,
                Name = "John Doe",
                AstronautDetail = new AstronautDetailEntity { CurrentRank = "Captain", CurrentDutyTitle = "Commander" },
                AstronautDuties = new List<AstronautDutyEntity>
                {
                    new AstronautDutyEntity { Id = 1, Rank = "Captain", DutyTitle = "Commander", DutyStartDate = DateTime.Now }
                }
            };
            _mockPersonRepo.Setup(x => x.SearchByNameWithAllRelationsAsync("John Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PersonAstronautEntity> { person });

            // Act
            var result = await _service.GetAstronautDutiesByName("John Doe", CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Duties.Should().HaveCount(1);
            result.Duties[0].Person.Should().NotBeNull();
            result.Duties[0].AstronautDuties.Should().HaveCount(1);
        }

        [TestMethod]
        public async Task GetAstronautDutiesByName_WhenPersonDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            _mockPersonRepo.Setup(x => x.SearchByNameWithAllRelationsAsync("Unknown", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PersonAstronautEntity>());

            // Act
            var result = await _service.GetAstronautDutiesByName("Unknown", CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ResponseCode.Should().Be(404);
            result.Message.Should().Be("No people found matching the search term");
        }

        [TestMethod]
        public async Task CreateAstronautDuty_WhenPersonDoesNotExist_ShouldCreatePerson()
        {
            // Arrange
            var request = new CreateAstronautDutyResponse
            {
                Name = "New Person",
                Rank = "Captain",
                DutyTitle = "Commander",
                DutyStartDate = DateTime.Now
            };

            PersonAstronautEntity? capturedPerson = null;
            _mockPersonRepo.Setup(x => x.AddAsync(It.IsAny<PersonAstronautEntity>(), It.IsAny<CancellationToken>()))
                .Callback<PersonAstronautEntity, CancellationToken>((p, ct) =>
                {
                    capturedPerson = p;
                    p.Id = 99; // Simulate database assigning ID
                });

            _mockDetailRepo.Setup(x => x.GetByPersonIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AstronautDetailEntity?)null);
            _mockDutyRepo.Setup(x => x.GetByPersonIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AstronautDutyEntity>());

            var preparedDetail = new AstronautDetailEntity
            {
                PersonId = 99,
                CurrentRank = "Captain",
                CurrentDutyTitle = "Commander",
                CareerStartDate = DateTime.Now
            };

            var newDuty = new AstronautDutyEntity
            {
                PersonId = 99,
                Rank = "Captain",
                DutyTitle = "Commander",
                DutyStartDate = DateTime.Now
            };

            // Mock domain service methods
            _mockDutyDomainService.Setup(x => x.EnsurePersonExistsAsync("New Person", It.IsAny<CancellationToken>()))
                .ReturnsAsync((PersonAstronautEntity?)null); // Returns null to indicate person needs to be created
            _mockDutyDomainService.Setup(x => x.PrepareAstronautDetail(null, "Captain", "Commander", It.IsAny<DateTime>(), 99))
                .Returns(preparedDetail);
            _mockDutyDomainService.Setup(x => x.GetAndTerminateActiveDutyAsync(99, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AstronautDutyEntity?)null);
            _mockDutyDomainService.Setup(x => x.CreateNewDuty(99, "Captain", "Commander", It.IsAny<DateTime>()))
                .Returns(newDuty);

            // Act
            var result = await _service.CreateAstronautDuty(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            capturedPerson.Should().NotBeNull();
            capturedPerson!.Name.Should().Be("New Person");
            _mockPersonRepo.Verify(x => x.AddAsync(It.IsAny<PersonAstronautEntity>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [TestMethod]
        public async Task CreateAstronautDuty_ForNewAstronaut_ShouldCreateDetailAndDuty()
        {
            // Arrange
            var person = new PersonAstronautEntity { Id = 1, Name = "John Doe" };
            var request = new CreateAstronautDutyResponse
            {
                Name = "John Doe",
                Rank = "Captain",
                DutyTitle = "Commander",
                DutyStartDate = DateTime.Now
            };

            var preparedDetail = new AstronautDetailEntity
            {
                PersonId = 1,
                CurrentRank = "Captain",
                CurrentDutyTitle = "Commander",
                CareerStartDate = DateTime.Now
            };

            var newDuty = new AstronautDutyEntity
            {
                PersonId = 1,
                Rank = "Captain",
                DutyTitle = "Commander",
                DutyStartDate = DateTime.Now
            };

            _mockPersonRepo.Setup(x => x.GetByNameAsync("John Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(person);
            _mockDetailRepo.Setup(x => x.GetByPersonIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AstronautDetailEntity?)null);
            _mockDutyRepo.Setup(x => x.GetByPersonIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AstronautDutyEntity>());

            // Mock domain service methods
            _mockDutyDomainService.Setup(x => x.EnsurePersonExistsAsync("John Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(person);
            _mockDutyDomainService.Setup(x => x.PrepareAstronautDetail(null, "Captain", "Commander", It.IsAny<DateTime>(), 1))
                .Returns(preparedDetail);
            _mockDutyDomainService.Setup(x => x.GetAndTerminateActiveDutyAsync(1, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AstronautDutyEntity?)null);
            _mockDutyDomainService.Setup(x => x.CreateNewDuty(1, "Captain", "Commander", It.IsAny<DateTime>()))
                .Returns(newDuty);

            // Act
            var result = await _service.CreateAstronautDuty(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            _mockDetailRepo.Verify(x => x.AddAsync(It.IsAny<AstronautDetailEntity>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockDutyRepo.Verify(x => x.AddAsync(It.IsAny<AstronautDutyEntity>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task CreateAstronautDuty_WithRetiredTitle_ShouldSetCareerEndDate()
        {
            // Arrange
            var person = new PersonAstronautEntity { Id = 1, Name = "John Doe" };
            var request = new CreateAstronautDutyResponse
            {
                Name = "John Doe",
                Rank = "Captain",
                DutyTitle = "RETIRED",
                DutyStartDate = new DateTime(2024, 1, 15)
            };

            var preparedDetail = new AstronautDetailEntity
            {
                PersonId = 1,
                CurrentRank = "Captain",
                CurrentDutyTitle = "RETIRED",
                CareerStartDate = new DateTime(2024, 1, 15),
                CareerEndDate = new DateTime(2024, 1, 14) // One day before
            };

            var newDuty = new AstronautDutyEntity
            {
                PersonId = 1,
                Rank = "Captain",
                DutyTitle = "RETIRED",
                DutyStartDate = new DateTime(2024, 1, 15)
            };

            _mockPersonRepo.Setup(x => x.GetByNameAsync("John Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(person);
            _mockDetailRepo.Setup(x => x.GetByPersonIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AstronautDetailEntity?)null);
            _mockDutyRepo.Setup(x => x.GetByPersonIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AstronautDutyEntity>());

            AstronautDetailEntity? capturedDetail = null;
            _mockDetailRepo.Setup(x => x.AddAsync(It.IsAny<AstronautDetailEntity>(), It.IsAny<CancellationToken>()))
                .Callback<AstronautDetailEntity, CancellationToken>((d, ct) => capturedDetail = d);

            // Mock domain service methods
            _mockDutyDomainService.Setup(x => x.EnsurePersonExistsAsync("John Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(person);
            _mockDutyDomainService.Setup(x => x.PrepareAstronautDetail(null, "Captain", "RETIRED", new DateTime(2024, 1, 15), 1))
                .Returns(preparedDetail);
            _mockDutyDomainService.Setup(x => x.GetAndTerminateActiveDutyAsync(1, new DateTime(2024, 1, 15), It.IsAny<CancellationToken>()))
                .ReturnsAsync((AstronautDutyEntity?)null);
            _mockDutyDomainService.Setup(x => x.CreateNewDuty(1, "Captain", "RETIRED", new DateTime(2024, 1, 15)))
                .Returns(newDuty);

            // Act
            await _service.CreateAstronautDuty(request, CancellationToken.None);

            // Assert
            capturedDetail.Should().NotBeNull();
            capturedDetail!.CareerEndDate.Should().Be(new DateTime(2024, 1, 14));  // One day before
        }

        [TestMethod]
        public async Task CreateAstronautDuty_WhenPersonHasActiveDuty_ShouldEndPreviousDuty()
        {
            // Arrange
            var person = new PersonAstronautEntity { Id = 1, Name = "John Doe" };
            var existingDetail = new AstronautDetailEntity { Id = 1, PersonId = 1, CurrentRank = "Lieutenant" };
            var activeDuty = new AstronautDutyEntity { Id = 1, PersonId = 1, DutyEndDate = null };
            var request = new CreateAstronautDutyResponse
            {
                Name = "John Doe",
                Rank = "Captain",
                DutyTitle = "Commander",
                DutyStartDate = new DateTime(2024, 1, 15)
            };

            var updatedDetail = new AstronautDetailEntity
            {
                Id = 1,
                PersonId = 1,
                CurrentRank = "Captain",
                CurrentDutyTitle = "Commander"
            };

            var terminatedDuty = new AstronautDutyEntity
            {
                Id = 1,
                PersonId = 1,
                DutyEndDate = new DateTime(2024, 1, 14) // One day before new duty
            };

            var newDuty = new AstronautDutyEntity
            {
                PersonId = 1,
                Rank = "Captain",
                DutyTitle = "Commander",
                DutyStartDate = new DateTime(2024, 1, 15)
            };

            _mockPersonRepo.Setup(x => x.GetByNameAsync("John Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(person);
            _mockDetailRepo.Setup(x => x.GetByPersonIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingDetail);
            _mockDutyRepo.Setup(x => x.GetByPersonIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AstronautDutyEntity> { activeDuty });

            // Mock domain service methods
            _mockDutyDomainService.Setup(x => x.EnsurePersonExistsAsync("John Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(person);
            _mockDutyDomainService.Setup(x => x.PrepareAstronautDetail(existingDetail, "Captain", "Commander", new DateTime(2024, 1, 15), 1))
                .Returns(updatedDetail);
            _mockDutyDomainService.Setup(x => x.GetAndTerminateActiveDutyAsync(1, new DateTime(2024, 1, 15), It.IsAny<CancellationToken>()))
                .ReturnsAsync(terminatedDuty);
            _mockDutyDomainService.Setup(x => x.CreateNewDuty(1, "Captain", "Commander", new DateTime(2024, 1, 15)))
                .Returns(newDuty);

            // Act
            await _service.CreateAstronautDuty(request, CancellationToken.None);

            // Assert
            terminatedDuty.DutyEndDate.Should().Be(new DateTime(2024, 1, 14));  // One day before new duty
            _mockDutyRepo.Verify(x => x.UpdateAsync(It.IsAny<AstronautDutyEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
