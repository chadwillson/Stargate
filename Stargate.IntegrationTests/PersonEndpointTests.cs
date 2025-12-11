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
/// Integration tests for Person API endpoints using SQLite.
/// Tests all CRUD operations to ensure they work correctly with the SQ database.
/// </summary>
[TestClass]
public sealed class PersonEndpointTests
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

    #region GET /api/person - Get All People

    [TestMethod]
    public async Task GetPeople_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/person");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<PersonAstronautListResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.People.Should().BeEmpty();
    }

    [TestMethod]
    public async Task GetPeople_WithSeededData_ReturnsAllPeople()
    {
        // Arrange
        _factory.SeedDatabase();

        // Act
        var response = await _client.GetAsync("/api/person");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<PersonAstronautListResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.People.Should().HaveCount(2);
        payload.People.Should().Contain(p => p.Name == "John Doe");
        payload.People.Should().Contain(p => p.Name == "Jane Doe");
    }

    [TestMethod]
    public async Task GetPeople_WithAstronautDetails_ReturnsPersonWithDetails()
    {
        // Arrange
        _factory.SeedDatabase();

        // Act
        var response = await _client.GetAsync("/api/person");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<PersonAstronautListResponse>();

        var john = payload!.People.First(p => p.Name == "John Doe");
        john.CurrentRank.Should().Be("1LT");
        john.CurrentDutyTitle.Should().Be("Commander");
        john.CareerStartDate.Should().NotBeNull();
    }

    #endregion

    #region GET /api/person/{name} - Get Person By Name

    [TestMethod]
    public async Task GetPersonByName_WhenExists_ReturnsPerson()
    {
        // Arrange
        _factory.SeedDatabase();

        // Act
        var response = await _client.GetAsync("/api/person/John Doe");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<PersonAstronautResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Name.Should().Be("John Doe");
        payload.CurrentRank.Should().Be("1LT");
        payload.CurrentDutyTitle.Should().Be("Commander");
    }

    [TestMethod]
    public async Task GetPersonByName_WhenNotExists_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/person/NonExistent Person");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var payload = await response.Content.ReadFromJsonAsync<PersonAstronautResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeFalse();
        payload.ResponseCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task GetPersonByName_WithSpecialCharacters_HandlesUrlEncoding()
    {
        // Arrange
        await _factory.CreatePersonAsync("Jack O'Neill");

        // Act - Name with apostrophe should be URL encoded
        var response = await _client.GetAsync("/api/person/Jack O'Neill");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<PersonAstronautResponse>();
        payload!.Name.Should().Be("Jack O'Neill");
    }

    #endregion

    #region POST /api/person - Create Person

    [TestMethod]
    public async Task CreatePerson_WithValidData_CreatesAndReturnsPerson()
    {
        // Arrange
        var request = new PersonRequest { Id = 0, Name = "Daniel Jackson" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/person", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<PersonAstronautResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Name.Should().Be("Daniel Jackson");
        payload.PersonId.Should().BeGreaterThan(0);

        // Verify in database
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
        var person = await db.People.FirstOrDefaultAsync(p => p.Name == "Daniel Jackson");
        person.Should().NotBeNull();
    }

    [TestMethod]
    public async Task CreatePerson_WithEmptyName_ReturnsValidationError()
    {
        // Arrange
        var request = new PersonRequest { Id = 0, Name = "" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/person", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task CreatePerson_WithDuplicateName_ReturnsConflict()
    {
        // Arrange - create first person
        await _client.PostAsJsonAsync("/api/person", new PersonRequest { Id = 0, Name = "Test Person" });

        // Act - attempt to create second person with same name
        var response = await _client.PostAsJsonAsync("/api/person", new PersonRequest { Id = 0, Name = "Test Person" });

        // Assert - Should reject duplicate names (business requirement: Person is uniquely identified by Name)
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var payload = await response.Content.ReadFromJsonAsync<PersonAstronautResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeFalse();
        payload.ResponseCode.Should().Be(409);
        payload.Message.Should().Contain("already exists");

        // Verify only one person exists
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
        var people = await db.People.Where(p => p.Name == "Test Person").ToListAsync();
        people.Should().HaveCount(1, "duplicate names should not be allowed");
    }

    #endregion

    #region PUT /api/person/{name} - Update Person

    [TestMethod]
    public async Task UpdatePerson_WhenExists_UpdatesName()
    {
        // Arrange
        var person = await _factory.CreatePersonAsync("Old Name");

        // Act
        var request = new PersonRequest { Id = person.Id, Name = "New Name" };
        var response = await _client.PutAsJsonAsync($"/api/person/Old Name", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<PersonAstronautResponse>();
        payload!.Success.Should().BeTrue();
        payload.Name.Should().Be("New Name");

        // Verify in database
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
        var updated = await db.People.FindAsync(person.Id);
        updated!.Name.Should().Be("New Name");
    }

    [TestMethod]
    public async Task UpdatePerson_WhenNotExists_ReturnsNotFound()
    {
        // Arrange
        var request = new PersonRequest { Id = 999, Name = "New Name" };

        // Act
        var response = await _client.PutAsJsonAsync("/api/person/NonExistent", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task UpdatePerson_WithEmptyName_ReturnsValidationError()
    {
        // Arrange
        await _factory.CreatePersonAsync("Test Person");

        // Act
        var request = new PersonRequest { Id = 1, Name = "" };
        var response = await _client.PutAsJsonAsync("/api/person/Test Person", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [TestMethod]
    public async Task UpdatePerson_WithDuplicateName_ReturnsConflict()
    {
        // Arrange
        await _factory.CreatePersonAsync("Person One");
        await _factory.CreatePersonAsync("Person Two");

        // Act - attempt to rename Person Two to Person One (duplicate)
        var request = new PersonRequest { Id = 2, Name = "Person One" };
        var response = await _client.PutAsJsonAsync("/api/person/Person Two", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var payload = await response.Content.ReadFromJsonAsync<PersonAstronautResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeFalse();
        payload.ResponseCode.Should().Be(409);
        payload.Message.Should().Contain("already exists");
    }

    #endregion

    #region Integration with AstronautDetail

    [TestMethod]
    public async Task GetPerson_WithAstronautDetail_ReturnsCompleteInfo()
    {
        // Arrange
        var person = await _factory.CreatePersonAsync("Samantha Carter");
        await _factory.CreateAstronautDetailAsync(
            person.Id,
            "Colonel",
            "Chief of Research",
            new DateTime(2020, 1, 1));

        // Act
        var response = await _client.GetAsync("/api/person/Samantha Carter");

        // Assert
        var payload = await response.Content.ReadFromJsonAsync<PersonAstronautResponse>();
        payload!.Name.Should().Be("Samantha Carter");
        payload.CurrentRank.Should().Be("Colonel");
        payload.CurrentDutyTitle.Should().Be("Chief of Research");
        payload.CareerStartDate.Should().Be(new DateTime(2020, 1, 1));
    }

    #endregion

}
