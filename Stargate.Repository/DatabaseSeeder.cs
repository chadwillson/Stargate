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
            // Check if already seeded
            if (context.People.Any())
            {
                return; // Database already contains data
            }

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
    }
}
