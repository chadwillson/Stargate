using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Stargate.Domain.Dtos;
using Stargate.Repository;

namespace Stargate.IntegrationTests;

[TestClass]
public sealed class TestDataEndpointTests
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

    #region POST /api/testdata/scenarios/* - Create Test Scenarios

    [TestMethod]
    public async Task CreateBasicScenario_ShouldCreateSingleAstronaut()
    {
        var response = await _client.PostAsync("/api/testdata/scenarios/basic", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<TestDataResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Scenario.Should().Be("Basic");
        payload.CreatedPersonIds.Should().HaveCount(1);
        payload.CreatedDetailIds.Should().HaveCount(1);
        payload.CreatedDutyIds.Should().HaveCount(1);

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
        var people = db.People.ToList();
        people.Should().HaveCount(1);
        people.First().Name.Should().Be("Test Basic Astronaut");
    }

    [TestMethod]
    public async Task CreateComplexScenario_ShouldCreateMultipleAstronautsWithHistory()
    {
        var response = await _client.PostAsync("/api/testdata/scenarios/complex", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<TestDataResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Scenario.Should().Be("Complex");
        payload.CreatedPersonIds.Should().HaveCount(3);
        payload.CreatedDetailIds.Should().HaveCount(3);
        payload.CreatedDutyIds.Should().HaveCount(9);

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
        var people = db.People.ToList();
        people.Should().HaveCount(3);
        var duties = db.AstronautDuties.ToList();
        duties.Should().HaveCount(9);
    }

    [TestMethod]
    public async Task CreateEdgeCaseScenario_ShouldCreatePersonsWithAndWithoutDetails()
    {
        var response = await _client.PostAsync("/api/testdata/scenarios/edgecase", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<TestDataResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Scenario.Should().Be("EdgeCase");
        payload.CreatedPersonIds.Should().HaveCount(2);
        payload.CreatedDetailIds.Should().HaveCount(1);
        payload.CreatedDutyIds.Should().BeEmpty();

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
        var people = db.People.ToList();
        people.Should().HaveCount(2);
        var details = db.AstronautDetails.ToList();
        details.Should().HaveCount(1);
    }

    [TestMethod]
    public async Task CreateRetiredAstronautScenario_ShouldCreateRetiredAstronaut()
    {
        var response = await _client.PostAsync("/api/testdata/scenarios/retired", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<TestDataResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Scenario.Should().Be("Retired");
        payload.CreatedPersonIds.Should().HaveCount(1);

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
        var person = db.People.First();
        var detail = db.AstronautDetails.First(d => d.PersonId == person.Id);
        detail.CurrentDutyTitle.Should().Be("RETIRED");
        detail.CareerEndDate.Should().NotBeNull();
    }

    [TestMethod]
    public async Task CreateMultipleDutiesScenario_ShouldCreateAstronautWithCareerProgression()
    {
        var response = await _client.PostAsync("/api/testdata/scenarios/multipleduties", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<TestDataResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();
        payload.Scenario.Should().Be("MultipleDuties");
        payload.CreatedPersonIds.Should().HaveCount(1);
        payload.CreatedDutyIds.Should().HaveCount(4);

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
        var duties = db.AstronautDuties.ToList();
        duties.Should().HaveCount(4);
        var currentDuty = duties.First(d => d.DutyEndDate == null);
        currentDuty.Rank.Should().Be("Major");
    }

    #endregion

    #region DELETE /api/testdata - Clear All Test Data

    [TestMethod]
    public async Task ClearAllTestData_ShouldRemoveAllData()
    {
        await _client.PostAsync("/api/testdata/scenarios/basic", null);

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
            db.People.Should().NotBeEmpty();
        }

        var response = await _client.DeleteAsync("/api/testdata");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<BaseResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
            db.People.Should().BeEmpty();
            db.AstronautDetails.Should().BeEmpty();
            db.AstronautDuties.Should().BeEmpty();
        }
    }

    #endregion

    #region POST /api/testdata/reset - Reset to Default Seed Data

    [TestMethod]
    public async Task ResetToDefaultSeedData_ShouldRestoreDefaultData()
    {
        await _client.PostAsync("/api/testdata/scenarios/complex", null);

        var response = await _client.PostAsync("/api/testdata/reset", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<BaseResponse>();
        payload.Should().NotBeNull();
        payload!.Success.Should().BeTrue();

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
        var people = db.People.ToList();
        people.Should().HaveCount(2);
        people.Should().Contain(p => p.Name == "John Doe");
        people.Should().Contain(p => p.Name == "Jane Doe");
    }

    #endregion

    #region Using Factory Methods

    [TestMethod]
    public async Task FactoryMethod_SeedBasicScenario_ShouldCreateData()
    {
        await _factory.SeedBasicScenarioAsync();

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
        var people = db.People.ToList();
        people.Should().HaveCount(1);
        people.First().Name.Should().Be("Test Basic Astronaut");
    }

    [TestMethod]
    public async Task FactoryMethod_SeedComplexScenario_ShouldCreateMultipleAstronauts()
    {
        await _factory.SeedComplexScenarioAsync();

        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
        var people = db.People.ToList();
        people.Should().HaveCount(3);
    }

    #endregion
}
