using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stargate.Repository;

namespace Stargate.IntegrationTests;

/// <summary>
/// Tests that verify DatabaseSeeder works correctly with SQLite.
/// This ensures the seeding logic matches the SQL Database Project post-deployment script.
/// </summary>
[TestClass]
public sealed class DatabaseSeedingTests
{
    private CustomWebApplicationFactory _factory = null!;

    [TestInitialize]
    public async Task SetUp()
    {
        _factory = new CustomWebApplicationFactory();
        await _factory.ResetDatabaseAsync(); // Start with empty database
    }

    [TestCleanup]
    public void Cleanup()
    {
        _factory?.Dispose();
    }

    [TestMethod]
    public async Task DatabaseSeeder_SeedsPersonData()
    {
        // Act
        _factory.SeedDatabase();

        // Assert
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();

        var people = await db.People.ToListAsync();
        people.Should().HaveCount(2, "seeder should create 2 people");

        var johnDoe = people.FirstOrDefault(p => p.Name == "John Doe");
        johnDoe.Should().NotBeNull("John Doe should be seeded");
        johnDoe!.Id.Should().Be(1, "John Doe should have ID 1");

        var janeDoe = people.FirstOrDefault(p => p.Name == "Jane Doe");
        janeDoe.Should().NotBeNull("Jane Doe should be seeded");
        janeDoe!.Id.Should().Be(2, "Jane Doe should have ID 2");
    }

    [TestMethod]
    public async Task DatabaseSeeder_SeedsAstronautDetailData()
    {
        // Act
        _factory.SeedDatabase();

        // Assert
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();

        var details = await db.AstronautDetails.ToListAsync();
        details.Should().HaveCount(1, "seeder should create 1 astronaut detail");

        var johnDetail = details.First();
        johnDetail.PersonId.Should().Be(1, "detail should be for John Doe");
        johnDetail.CurrentRank.Should().Be("1LT");
        johnDetail.CurrentDutyTitle.Should().Be("Commander");
        johnDetail.CareerStartDate.Should().Be(new DateTime(2024, 1, 1));
        johnDetail.CareerEndDate.Should().BeNull();
    }

    [TestMethod]
    public async Task DatabaseSeeder_SeedsAstronautDutyData()
    {
        // Act
        _factory.SeedDatabase();

        // Assert
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();

        var duties = await db.AstronautDuties.ToListAsync();
        duties.Should().HaveCount(1, "seeder should create 1 astronaut duty");

        var johnDuty = duties.First();
        johnDuty.PersonId.Should().Be(1, "duty should be for John Doe");
        johnDuty.Rank.Should().Be("1LT");
        johnDuty.DutyTitle.Should().Be("Commander");
        johnDuty.DutyStartDate.Should().Be(new DateTime(2024, 1, 1));
        johnDuty.DutyEndDate.Should().BeNull();
    }

    [TestMethod]
    public async Task DatabaseSeeder_OnlyRunsOnce_WhenDataAlreadyExists()
    {
        // Arrange - seed once
        _factory.SeedDatabase();

        // Act - seed again (should be idempotent)
        _factory.SeedDatabase();

        // Assert - should still have same data, not duplicates
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();

        var people = await db.People.ToListAsync();
        people.Should().HaveCount(2, "seeder should not create duplicates");

        var details = await db.AstronautDetails.ToListAsync();
        details.Should().HaveCount(1, "seeder should not create duplicates");

        var duties = await db.AstronautDuties.ToListAsync();
        duties.Should().HaveCount(1, "seeder should not create duplicates");
    }

    [TestMethod]
    public async Task DatabaseSeeder_VerifyForeignKeyRelationships()
    {
        // Act
        _factory.SeedDatabase();

        // Assert
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();

        var johnDoe = await db.People
            .Include(p => p.AstronautDetail)
            .Include(p => p.AstronautDuties)
            .FirstAsync(p => p.Name == "John Doe");

        johnDoe.AstronautDetail.Should().NotBeNull("John should have astronaut detail");
        johnDoe.AstronautDetail!.CurrentRank.Should().Be("1LT");

        johnDoe.AstronautDuties.Should().HaveCount(1, "John should have 1 duty");
        johnDoe.AstronautDuties.First().DutyTitle.Should().Be("Commander");
    }

    [TestMethod]
    public async Task DatabaseSeeder_MatchesSqlPostDeploymentScript_DataValues()
    {
        // This test verifies the C# seeder matches the SQL Database Project
        // Source: Stargate.Database/Post-Deployment/Script.PostDeployment.sql

        // Act
        _factory.SeedDatabase();

        // Assert - Verify exact values from SQL script
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<StargateContext>();

        // Person table verification
        var john = await db.People.FirstAsync(p => p.Id == 1);
        john.Name.Should().Be("John Doe", "must match SQL script");

        var jane = await db.People.FirstAsync(p => p.Id == 2);
        jane.Name.Should().Be("Jane Doe", "must match SQL script");

        // AstronautDetail verification
        var detail = await db.AstronautDetails.FirstAsync(d => d.Id == 1);
        detail.PersonId.Should().Be(1, "must match SQL script");
        detail.CurrentRank.Should().Be("1LT", "must match SQL script");
        detail.CurrentDutyTitle.Should().Be("Commander", "must match SQL script");
        detail.CareerStartDate.Should().Be(new DateTime(2024, 1, 1), "must match SQL script seed date");
        detail.CareerEndDate.Should().BeNull("must match SQL script");

        // AstronautDuty verification
        var duty = await db.AstronautDuties.FirstAsync(d => d.Id == 1);
        duty.PersonId.Should().Be(1, "must match SQL script");
        duty.Rank.Should().Be("1LT", "must match SQL script");
        duty.DutyTitle.Should().Be("Commander", "must match SQL script");
        duty.DutyStartDate.Should().Be(new DateTime(2024, 1, 1), "must match SQL script seed date");
        duty.DutyEndDate.Should().BeNull("must match SQL script");
    }
}
