using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stargate.Domain.Dtos;
using Stargate.Repository;
using Stargate.Repository.Entities;

namespace Stargate.IntegrationTests;

/// <summary>
/// Integration tests for AstronautDuty API endpoints using SQLite.
/// Tests all operations to ensure they work correctly with the SQLite database.
/// </summary>
[TestClass]
public sealed class AstronautDutyEndpointTests
{
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [TestInitialize]
    public async Task SetUp()
    {
        _factory = new CustomWebApplicationFactory();
        await _factory.ResetDatabaseAsync();
        _client = _factory.CreateClient();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }

    #region GET /api/astronautduty/{name} - Get Duties By Name

    [TestMethod]
    public async Task GetAstronautDutiesByName_WhenPersonExists_ReturnsDuties()
    {
        // Arrange
        _factory.SeedDatabase();

        // Act
        var response = await _client.GetAsync("/api/astronautduty/John Doe");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<AstronautDutiesListResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Duties.Should().HaveCount(1);

        var result = payload.Duties.First();
        result.Person.Should().NotBeNull();
        result.Person!.Name.Should().Be("John Doe");
        result.Person.CurrentRank.Should().Be("1LT");
        result.Person.CurrentDutyTitle.Should().Be("Commander");
        result.AstronautDuties.Should().HaveCount(1);
        result.AstronautDuties.First().DutyStartDate.Should().Be(new DateTime(2024, 1, 1));
    }

    [TestMethod]
    public async Task GetAstronautDutiesByName_WhenPersonNotExists_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/astronautduty/Unknown Person");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var payload = await response.Content.ReadFromJsonAsync<AstronautDutiesListResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeFalse();
        payload.ResponseCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task GetAstronautDutiesByName_WhenPersonExistsButNoDuties_ReturnsEmptyList()
    {
        // Arrange
        await SeedPersonAsync(new PersonAstronautEntity { Name = "No Duties Person" });

        // Act
        var response = await _client.GetAsync("/api/astronautduty/No Duties Person");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<AstronautDutiesListResponse>();
        payload!.Success.Should().BeTrue();
        payload.Duties.Should().HaveCount(1);
        payload.Duties.First().Person.Should().NotBeNull();
        payload.Duties.First().AstronautDuties.Should().BeEmpty();
    }

    [TestMethod]
    public async Task GetAstronautDutiesByName_WithMultipleDuties_ReturnsAllDuties()
    {
        // Arrange
        var person = await SeedPersonAsync(new PersonAstronautEntity { Name = "Multi Duty Person" });

        await SeedAstronautDutyAsync(new AstronautDutyEntity
        {
            PersonId = person.Id,
            Rank = "Lieutenant",
            DutyTitle = "First Assignment",
            DutyStartDate = new DateTime(2020, 1, 1),
            DutyEndDate = new DateTime(2022, 12, 31)
        });

        await SeedAstronautDutyAsync(new AstronautDutyEntity
        {
            PersonId = person.Id,
            Rank = "Captain",
            DutyTitle = "Second Assignment",
            DutyStartDate = new DateTime(2023, 1, 1)
        });

        // Act
        var response = await _client.GetAsync("/api/astronautduty/Multi Duty Person");

        // Assert
        var payload = await response.Content.ReadFromJsonAsync<AstronautDutiesListResponse>();
        payload!.Duties.Should().HaveCount(1);
        var result = payload.Duties.First();
        result.AstronautDuties.Should().HaveCount(2);
        result.AstronautDuties.Should().Contain(d => d.DutyTitle == "First Assignment");
        result.AstronautDuties.Should().Contain(d => d.DutyTitle == "Second Assignment");
    }

    #endregion

    #region POST /api/astronautduty - Create Astronaut Duty

    [TestMethod]
    public async Task CreateAstronautDuty_ForNewPerson_CreatesPersonDutyAndDetail()
    {
        // Arrange - Create person first
        await SeedPersonAsync(new PersonAstronautEntity { Name = "Teal'c" });

        var request = new CreateAstronautDutyResponse
        {
            Name = "Teal'c",
            Rank = "Master",
            DutyTitle = "Jaffa Warrior",
            DutyStartDate = new DateTime(2024, 6, 1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/astronautduty", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<CreateAstronautDutyResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Name.Should().Be("Teal'c");
        payload.Rank.Should().Be("Master");
        payload.DutyTitle.Should().Be("Jaffa Warrior");

        // Verify in database
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();

        // Should create person
        var person = await db.People.FirstOrDefaultAsync(p => p.Name == "Teal'c");
        person.Should().NotBeNull();

        // Should create duty
        var duty = await db.AstronautDuties.FirstOrDefaultAsync(d => d.PersonId == person!.Id);
        duty.Should().NotBeNull();
        duty!.Rank.Should().Be("Master");

        // Should create detail
        var detail = await db.AstronautDetails.FirstOrDefaultAsync(d => d.PersonId == person!.Id);
        detail.Should().NotBeNull();
        detail!.CurrentRank.Should().Be("Master");
        detail.CurrentDutyTitle.Should().Be("Jaffa Warrior");
        detail.CareerStartDate.Should().Be(new DateTime(2024, 6, 1));
    }

    [TestMethod]
    public async Task CreateAstronautDuty_ForExistingPerson_CreatesNewDutyAndUpdatesDetail()
    {
        // Arrange
        var person = await SeedPersonAsync(new PersonAstronautEntity { Name = "Jack O'Neill" });
        await SeedAstronautDetailAsync(new AstronautDetailEntity
        {
            PersonId = person.Id,
            CurrentRank = "Colonel",
            CurrentDutyTitle = "SG-1 Commander",
            CareerStartDate = new DateTime(2020, 1, 1)
        });

        var request = new CreateAstronautDutyResponse
        {
            Name = "Jack O'Neill",
            Rank = "Brigadier General",
            DutyTitle = "Base Commander",
            DutyStartDate = new DateTime(2024, 1, 1)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/astronautduty", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<CreateAstronautDutyResponse>();
        payload!.Success.Should().BeTrue();

        // Verify detail was updated to new duty
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();

        var detail = await db.AstronautDetails.FirstAsync(d => d.PersonId == person.Id);
        detail.CurrentRank.Should().Be("Brigadier General", "detail should be updated to new rank");
        detail.CurrentDutyTitle.Should().Be("Base Commander", "detail should be updated to new duty");
        detail.CareerStartDate.Should().Be(new DateTime(2020, 1, 1), "career start should not change");

        // Verify new duty was created
        var duties = await db.AstronautDuties.Where(d => d.PersonId == person.Id).ToListAsync();
        duties.Should().HaveCount(1);
        duties.First().DutyTitle.Should().Be("Base Commander");
    }

    [TestMethod]
    public async Task CreateAstronautDuty_WithEmptyName_ReturnsValidationError()
    {
        // Arrange
        var request = new CreateAstronautDutyResponse
        {
            Name = "",
            Rank = "Captain",
            DutyTitle = "Test",
            DutyStartDate = DateTime.Now
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/astronautduty", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task CreateAstronautDuty_WithEmptyRank_ReturnsValidationError()
    {
        // Arrange
        var request = new CreateAstronautDutyResponse
        {
            Name = "Test Person",
            Rank = "",
            DutyTitle = "Test Duty",
            DutyStartDate = DateTime.Now
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/astronautduty", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task CreateAstronautDuty_WithEmptyDutyTitle_ReturnsValidationError()
    {
        // Arrange
        var request = new CreateAstronautDutyResponse
        {
            Name = "Test Person",
            Rank = "Captain",
            DutyTitle = "",
            DutyStartDate = DateTime.Now
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/astronautduty", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task CreateAstronautDuty_WithFutureDate_CreatesSuccessfully()
    {
        // Arrange - Create person first
        await SeedPersonAsync(new PersonAstronautEntity { Name = "Future Person" });

        var futureDate = DateTime.Now.AddYears(1);
        var request = new CreateAstronautDutyResponse
        {
            Name = "Future Person",
            Rank = "Captain",
            DutyTitle = "Future Mission",
            DutyStartDate = futureDate
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/astronautduty", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<CreateAstronautDutyResponse>();
        payload!.DutyStartDate.Should().BeCloseTo(futureDate, TimeSpan.FromSeconds(1));
    }

    #endregion

    #region End-to-End Scenarios

    [TestMethod]
    public async Task EndToEnd_CreatePersonThenAssignMultipleDuties()
    {
        // Scenario: Create a person, then assign multiple duties

        // Step 1: Create person first
        await SeedPersonAsync(new PersonAstronautEntity { Name = "Cameron Mitchell" });

        // Step 2: Create first duty
        var firstDuty = new CreateAstronautDutyResponse
        {
            Name = "Cameron Mitchell",
            Rank = "Lieutenant Colonel",
            DutyTitle = "SG-1 Commander",
            DutyStartDate = new DateTime(2023, 1, 1)
        };
        var response1 = await _client.PostAsJsonAsync("/api/astronautduty", firstDuty);
        response1.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 3: Add second duty
        var secondDuty = new CreateAstronautDutyResponse
        {
            Name = "Cameron Mitchell",
            Rank = "Colonel",
            DutyTitle = "Base Commander",
            DutyStartDate = new DateTime(2024, 1, 1)
        };
        var response2 = await _client.PostAsJsonAsync("/api/astronautduty", secondDuty);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 4: Verify all duties are returned
        var dutiesResponse = await _client.GetAsync("/api/astronautduty/Cameron Mitchell");
        var dutiesPayload = await dutiesResponse.Content.ReadFromJsonAsync<AstronautDutiesListResponse>();
        dutiesPayload!.Duties.Should().HaveCount(1);
        dutiesPayload.Duties.First().AstronautDuties.Should().HaveCount(2);

        // Step 5: Verify current detail matches latest duty
        var personResponse = await _client.GetAsync("/api/person/Cameron Mitchell");
        var personPayload = await personResponse.Content.ReadFromJsonAsync<PersonAstronautResponse>();
        personPayload!.CurrentRank.Should().Be("Colonel");
        personPayload.CurrentDutyTitle.Should().Be("Base Commander");
    }

    #endregion

    #region Helper Methods

    private async Task<PersonAstronautEntity> SeedPersonAsync(PersonAstronautEntity person)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
        db.People.Add(person);
        await db.SaveChangesAsync();
        return person;
    }

    private async Task<AstronautDetailEntity> SeedAstronautDetailAsync(AstronautDetailEntity detail)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
        db.AstronautDetails.Add(detail);
        await db.SaveChangesAsync();
        return detail;
    }

    private async Task<AstronautDutyEntity> SeedAstronautDutyAsync(AstronautDutyEntity duty)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
        db.AstronautDuties.Add(duty);
        await db.SaveChangesAsync();
        return duty;
    }

    #endregion
}
