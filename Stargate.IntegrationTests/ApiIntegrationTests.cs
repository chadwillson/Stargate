using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Stargate.Domain.Dtos;
using Stargate.Repository;
using Stargate.Repository.Entities;

namespace Stargate.IntegrationTests;

[TestClass]
public sealed class ApiIntegrationTests
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

    [TestMethod]
    public async Task GetPeople_ReturnsSeededPerson()
    {
        var seeded = await SeedPersonAsync(new PersonAstronautEntity { Name = "John Doe" });

        var response = await _client.GetAsync("/api/person");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<PersonAstronautListResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.People.Should().ContainSingle(p => p.Name == "John Doe" && p.PersonId == seeded.Id);
    }

    [TestMethod]
    public async Task CreatePerson_PersistsAndReturnsPerson()
    {
        var request = new PersonRequest { Name = "New Person" };

        var response = await _client.PostAsJsonAsync("/api/person", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<PersonAstronautResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Name.Should().Be("New Person");
        payload.PersonId.Should().BeGreaterThan(0);

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
        (await db.People.AnyAsync(p => p.Id == payload.PersonId && p.Name == "New Person")).Should().BeTrue();
    }

    [TestMethod]
    public async Task GetAstronautDutiesByName_WhenPersonMissing_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/astronautduty/Unknown");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var payload = await response.Content.ReadFromJsonAsync<AstronautDutiesListResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeFalse();
        payload.ResponseCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [TestMethod]
    public async Task CreateAstronautDuty_ForExistingPerson_CreatesDutyAndUpdatesDetail()
    {
        var person = await SeedPersonAsync(new PersonAstronautEntity { Name = "Jane Doe" });

        var request = new CreateAstronautDutyResponse
        {
            Name = "Jane Doe",
            Rank = "Captain",
            DutyTitle = "Commander",
            DutyStartDate = new DateTime(2024, 1, 15)
        };

        var response = await _client.PostAsJsonAsync("/api/astronautduty", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<CreateAstronautDutyResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Id.Should().BeGreaterThan(0);
        payload.Name.Should().Be("Jane Doe");

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();

        var duties = await db.AstronautDuties.Where(d => d.PersonId == person.Id).ToListAsync();
        duties.Should().ContainSingle();
        duties[0].DutyTitle.Should().Be("Commander");
        duties[0].Rank.Should().Be("Captain");

        var detail = await db.AstronautDetails.SingleAsync(d => d.PersonId == person.Id);
        detail.CurrentDutyTitle.Should().Be("Commander");
        detail.CurrentRank.Should().Be("Captain");
    }

    private async Task<PersonAstronautEntity> SeedPersonAsync(PersonAstronautEntity person)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();

        db.People.Add(person);
        await db.SaveChangesAsync();

        return person;
    }
}
