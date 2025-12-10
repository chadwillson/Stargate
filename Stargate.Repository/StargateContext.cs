using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using System.Text;

using Stargate.Repository.Entities;

using Microsoft.EntityFrameworkCore;

namespace Stargate.Repository
{
    public class StargateContext : DbContext
    {
        public IDbConnection Connection => Database.GetDbConnection();
        public DbSet<PersonAstronautEntity> People { get; set; }
        public DbSet<AstronautDetailEntity> AstronautDetails { get; set; }
        public DbSet<AstronautDutyEntity> AstronautDuties { get; set; }
        public DbSet<LogEntryEntity> LogEntries { get; set; }

        public StargateContext(DbContextOptions<StargateContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StargateContext).Assembly);

            //SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            //add seed data
            modelBuilder.Entity<PersonAstronautEntity>()
                .HasData(
                    new PersonAstronautEntity
                    {
                        Id = 1,
                        Name = "John Doe"
                    },
                    new PersonAstronautEntity
                    {
                        Id = 2,
                        Name = "Jane Doe"
                    }
                );

            modelBuilder.Entity<AstronautDetailEntity>()
                .HasData(
                    new AstronautDetailEntity
                    {
                        Id = 1,
                        PersonId = 1,
                        CurrentRank = "1LT",
                        CurrentDutyTitle = "Commander",
                        CareerStartDate = DateTime.Now
                    }
                );

            modelBuilder.Entity<AstronautDutyEntity>()
                .HasData(
                    new AstronautDutyEntity
                    {
                        Id = 1,
                        PersonId = 1,
                        DutyStartDate = DateTime.Now,
                        DutyTitle = "Commander",
                        Rank = "1LT"
                    }
                );
        }
    }

}
