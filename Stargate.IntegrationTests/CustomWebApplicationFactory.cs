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
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext configuration
            services.RemoveAll(typeof(DbContextOptions<StargateContext>));

            // Create in-memory SQLite connection that persists for the test lifetime
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // Configure DbContext to use SQLite with the shared connection
            services.AddDbContext<StargateContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            // Ensure database is created
            using var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<StargateContext>();
            db.Database.EnsureCreated();
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
        if (disposing)
        {
            _connection?.Close();
            _connection?.Dispose();
        }
        base.Dispose(disposing);
    }
}
