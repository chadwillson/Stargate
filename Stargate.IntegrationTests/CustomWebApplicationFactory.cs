using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Stargate.Repository;

namespace Stargate.IntegrationTests;

/// <summary>
/// Custom WebApplicationFactory for integration tests.
/// Uses SQLite with in-memory connection to match local development environment.
/// Each test gets a fresh database with optional seeding.
/// </summary>
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set environment to IntegrationTest so Program.cs uses in-memory database
        builder.UseEnvironment("IntegrationTest");

        builder.ConfigureServices(services =>
        {
            // The IntegrationTest environment in Program.cs already configures in-memory database
            // We don't need to do anything else here
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

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
