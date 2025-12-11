using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Stargate.Application.Interfaces;
using Stargate.Repository;
using Stargate.Repository.Entities;

namespace Stargate.IntegrationTests;

/// <summary>
/// Custom WebApplicationFactory for integration tests.
/// Uses SQLite with in-memory connection to match local development environment.
/// Each test gets a fresh database with optional seeding.
/// </summary>
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"IntegrationTestsDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set environment to IntegrationTest so Program.cs uses in-memory database
        builder.UseEnvironment("IntegrationTest");

        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext configuration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<StargateContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add DbContext with unique in-memory database for this test instance
            services.AddDbContext<StargateContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
        });
    }

    /// <summary>
    /// Resets the database to a clean state (no data).
    /// Call this in TestInitialize to start with an empty database.
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Seeds the database using the same DatabaseSeeder used in local development.
    /// This verifies that the seeding logic works correctly.
    /// </summary>
    public void SeedDatabase()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
        DatabaseSeeder.Seed(db);
    }

    /// <summary>
    /// Gets the TestDataService for creating test scenarios.
    /// </summary>
    public async Task SeedBasicScenarioAsync()
    {
        using var scope = Services.CreateScope();
        var testDataService = scope.ServiceProvider.GetRequiredService<ITestDataService>();
        await testDataService.CreateBasicScenarioAsync();
    }

    /// <summary>
    /// Creates complex test scenario with multiple astronauts and duties.
    /// </summary>
    public async Task SeedComplexScenarioAsync()
    {
        using var scope = Services.CreateScope();
        var testDataService = scope.ServiceProvider.GetRequiredService<ITestDataService>();
        await testDataService.CreateComplexScenarioAsync();
    }

    /// <summary>
    /// Creates edge case scenario for testing.
    /// </summary>
    public async Task SeedEdgeCaseScenarioAsync()
    {
        using var scope = Services.CreateScope();
        var testDataService = scope.ServiceProvider.GetRequiredService<ITestDataService>();
        await testDataService.CreateEdgeCaseScenarioAsync();
    }

    /// <summary>
    /// Creates retired astronaut scenario for testing.
    /// </summary>
    public async Task SeedRetiredAstronautScenarioAsync()
    {
        using var scope = Services.CreateScope();
        var testDataService = scope.ServiceProvider.GetRequiredService<ITestDataService>();
        await testDataService.CreateRetiredAstronautScenarioAsync();
    }

    /// <summary>
    /// Creates scenario with astronaut having multiple duties over time.
    /// </summary>
    public async Task SeedMultipleDutiesScenarioAsync()
    {
        using var scope = Services.CreateScope();
        var testDataService = scope.ServiceProvider.GetRequiredService<ITestDataService>();
        await testDataService.CreateMultipleDutiesScenarioAsync();
    }

    /// <summary>
    /// Creates a person with the specified name.
    /// </summary>
    public async Task<PersonAstronautEntity> CreatePersonAsync(string name)
    {
        using var scope = Services.CreateScope();
        var testDataService = scope.ServiceProvider.GetRequiredService<ITestDataService>();
        return await testDataService.CreatePersonAsync(name);
    }

    /// <summary>
    /// Creates an astronaut detail for a person.
    /// </summary>
    public async Task<AstronautDetailEntity> CreateAstronautDetailAsync(
        int personId,
        string rank,
        string dutyTitle,
        DateTime careerStartDate,
        DateTime? careerEndDate = null)
    {
        using var scope = Services.CreateScope();
        var testDataService = scope.ServiceProvider.GetRequiredService<ITestDataService>();
        return await testDataService.CreateAstronautDetailAsync(personId, rank, dutyTitle, careerStartDate, careerEndDate);
    }

    /// <summary>
    /// Creates an astronaut duty for a person.
    /// </summary>
    public async Task<AstronautDutyEntity> CreateAstronautDutyAsync(
        int personId,
        string rank,
        string dutyTitle,
        DateTime dutyStartDate,
        DateTime? dutyEndDate = null)
    {
        using var scope = Services.CreateScope();
        var testDataService = scope.ServiceProvider.GetRequiredService<ITestDataService>();
        return await testDataService.CreateAstronautDutyAsync(personId, rank, dutyTitle, dutyStartDate, dutyEndDate);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
