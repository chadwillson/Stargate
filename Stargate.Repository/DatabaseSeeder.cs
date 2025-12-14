using Stargate.Repository.Entities;

namespace Stargate.Repository
{
    /// <summary>
    /// Provides database seeding functionality for local SQLite development.
    ///
    /// IMPORTANT: This seeder MUST stay in sync with the SQL Database Project!
    /// Source of Truth: Stargate.Database/Post-Deployment/Script.PostDeployment.sql
    ///
    /// When updating seed data:
    /// 1. Update the SQL post-deployment script FIRST
    /// 2. Then update this C# seeder to match
    ///
    /// This duplication is necessary because:
    /// - Production/Azure uses SQL Server with the SQL Database Project
    /// - Local development uses SQLite with Entity Framework
    /// - SQL Server-specific syntax (IDENTITY_INSERT, etc.) doesn't work in SQLite
    /// </summary>
    public static class DatabaseSeeder
    {
        /// <summary>
        /// Seeds the database with initial data if it's empty.
        /// Must match: Stargate.Database/Post-Deployment/Script.PostDeployment.sql
        /// </summary>
        /// <param name="context">The database context to seed</param>
        public static void Seed(StargateContext context)
        {
            // Seed Person data if not exists
            if (!context.People.Any())
            {
                SeedPeople(context);
            }

            // Seed User/Role data if not exists
            if (!context.Roles.Any())
            {
                SeedUsersAndRoles(context);
            }
        }

        private static void SeedPeople(StargateContext context)
        {

            // Seed date aligned with SQL post-deployment script (2024-01-01)
            var seedDate = new DateTime(2024, 1, 1);

            // Seed Person data
            var people = new[]
            {
                new PersonAstronautEntity { Id = 1, Name = "John Doe" },
                new PersonAstronautEntity { Id = 2, Name = "Jane Doe" }
            };
            context.People.AddRange(people);
            context.SaveChanges();

            // Seed AstronautDetail data
            var details = new[]
            {
                new AstronautDetailEntity
                {
                    Id = 1,
                    PersonId = 1,
                    CurrentRank = "1LT",
                    CurrentDutyTitle = "Commander",
                    CareerStartDate = seedDate,
                    CareerEndDate = null
                }
            };
            context.AstronautDetails.AddRange(details);
            context.SaveChanges();

            // Seed AstronautDuty data
            var duties = new[]
            {
                new AstronautDutyEntity
                {
                    Id = 1,
                    PersonId = 1,
                    Rank = "1LT",
                    DutyTitle = "Commander",
                    DutyStartDate = seedDate,
                    DutyEndDate = null
                }
            };
            context.AstronautDuties.AddRange(duties);
            context.SaveChanges();
        }

        private static void SeedUsersAndRoles(StargateContext context)
        {
            // Seed Role data
            var roles = new[]
            {
                new RoleEntity
                {
                    Id = 1,
                    Name = "Admin",
                    Description = "Administrator with full access",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new RoleEntity
                {
                    Id = 2,
                    Name = "User",
                    Description = "Standard user with limited access",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
            context.Roles.AddRange(roles);
            context.SaveChanges();

            // Seed User data
            // Default passwords (hashed with BCrypt at runtime):
            // admin: Stargate123!
            // user: Password1!
            var users = new[]
            {
                new UserEntity
                {
                    Id = 1,
                    Username = "admin",
                    Email = "admin@stargate.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Stargate123!"),
                    FirstName = "Admin",
                    LastName = "User",
                    RoleId = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new UserEntity
                {
                    Id = 2,
                    Username = "user",
                    Email = "user@stargate.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password1!"),
                    FirstName = "Standard",
                    LastName = "User",
                    RoleId = 2,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
