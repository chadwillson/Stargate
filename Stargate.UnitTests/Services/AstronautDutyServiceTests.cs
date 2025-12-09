using FluentAssertions;

using Moq;

using Stargate.Application.Services;
using Stargate.Domain.Dtos;
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
        private AstronautDutyService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPersonRepo = new Mock<IPersonAstronautRepository>();
            _mockDetailRepo = new Mock<IAstronautDetailRepository>();
            _mockDutyRepo = new Mock<IAstronautDutyRepository>();

            _mockUnitOfWork.Setup(x => x.PersonAstronauts).Returns(_mockPersonRepo.Object);
            _mockUnitOfWork.Setup(x => x.AstronautDetails).Returns(_mockDetailRepo.Object);
            _mockUnitOfWork.Setup(x => x.AstronautDuties).Returns(_mockDutyRepo.Object);

            _service = new AstronautDutyService(_mockUnitOfWork.Object);
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
            _mockPersonRepo.Setup(x => x.GetByNameWithAllRelationsAsync("John Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(person);

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
            _mockPersonRepo.Setup(x => x.GetByNameWithAllRelationsAsync("Unknown", It.IsAny<CancellationToken>()))
                .ReturnsAsync((PersonAstronautEntity?)null);

            // Act
            var result = await _service.GetAstronautDutiesByName("Unknown", CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ResponseCode.Should().Be(404);
            result.Message.Should().Be("Person not found");
        }

        [TestMethod]
        public async Task CreateAstronautDuty_WhenPersonDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var request = new CreateAstronautDutyResponse
            {
                Name = "Unknown",
                Rank = "Captain",
                DutyTitle = "Commander",
                DutyStartDate = DateTime.Now
            };
            _mockPersonRepo.Setup(x => x.GetByNameAsync("Unknown", It.IsAny<CancellationToken>()))
                .ReturnsAsync((PersonAstronautEntity?)null);

            // Act
            var result = await _service.CreateAstronautDuty(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ResponseCode.Should().Be(404);
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

            _mockPersonRepo.Setup(x => x.GetByNameAsync("John Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(person);
            _mockDetailRepo.Setup(x => x.GetByPersonIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AstronautDetailEntity?)null);
            _mockDutyRepo.Setup(x => x.GetByPersonIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AstronautDutyEntity>());

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

            _mockPersonRepo.Setup(x => x.GetByNameAsync("John Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(person);
            _mockDetailRepo.Setup(x => x.GetByPersonIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((AstronautDetailEntity?)null);
            _mockDutyRepo.Setup(x => x.GetByPersonIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AstronautDutyEntity>());

            AstronautDetailEntity? capturedDetail = null;
            _mockDetailRepo.Setup(x => x.AddAsync(It.IsAny<AstronautDetailEntity>(), It.IsAny<CancellationToken>()))
                .Callback<AstronautDetailEntity, CancellationToken>((d, ct) => capturedDetail = d);

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

            _mockPersonRepo.Setup(x => x.GetByNameAsync("John Doe", It.IsAny<CancellationToken>()))
                .ReturnsAsync(person);
            _mockDetailRepo.Setup(x => x.GetByPersonIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingDetail);
            _mockDutyRepo.Setup(x => x.GetByPersonIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<AstronautDutyEntity> { activeDuty });

            // Act
            await _service.CreateAstronautDuty(request, CancellationToken.None);

            // Assert
            activeDuty.DutyEndDate.Should().Be(new DateTime(2024, 1, 14));  // One day before new duty
            _mockDutyRepo.Verify(x => x.UpdateAsync(activeDuty, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
