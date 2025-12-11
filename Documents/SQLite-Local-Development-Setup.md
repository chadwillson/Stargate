# SQLite Local Development Setup Guide

## Overview

This guide explains how to set up SQLite for local development in the Stargate project. SQLite provides a lightweight, file-based database that mirrors the production Azure SQL Database schema while being easy to set up and version control.

**IMPORTANT**: The **SQL Database Project** (`Stargate.Database`) is the **source of truth** for schema and seed data. This SQLite setup is for local development convenience only. See [Database-Architecture.md](Database-Architecture.md) for the complete architectural decision.

## Why SQLite for Local Development?

- **Zero Configuration**: No SQL Server installation required
- **Portable**: Database is a single file that can be committed to version control
- **Fast Development**: Instant database reset and seeding
- **Azure Parity**: EF Core migrations work identically for both SQLite and SQL Server
- **Team Consistency**: Every developer gets the same database state

## Database Schema

The Stargate database consists of three tables:

### Person
```sql
Id (INT, Primary Key, Auto-increment)
Name (NVARCHAR(255), NOT NULL)
```

### AstronautDetail
```sql
Id (INT, Primary Key, Auto-increment)
PersonId (INT, Foreign Key -> Person.Id)
CurrentRank (NVARCHAR(100), NOT NULL)
CurrentDutyTitle (NVARCHAR(255), NOT NULL)
CareerStartDate (DATETIME2, NOT NULL)
CareerEndDate (DATETIME2, NULL)
```

### AstronautDuty
```sql
Id (INT, Primary Key, Auto-increment)
PersonId (INT, Foreign Key -> Person.Id)
Rank (NVARCHAR(100), NOT NULL)
DutyTitle (NVARCHAR(255), NOT NULL)
DutyStartDate (DATETIME2, NOT NULL)
DutyEndDate (DATETIME2, NULL)
```

## Implementation Steps

### 1. Install SQLite NuGet Package

Add the SQLite provider to the Stargate.Api project:

```bash
cd Stargate.Api
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

### 2. Update Connection String

**appsettings.Development.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=stargate.db",
    "SqlServerConnection": "Server=(localdb)\\mssqllocaldb;Database=StargateDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "DatabaseProvider": "Sqlite"
}
```

**appsettings.json (Production/Azure):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=StargateDB;..."
  },
  "DatabaseProvider": "SqlServer"
}
```

### 3. Update Program.cs

Modify the database configuration to support both SQLite and SQL Server:

```csharp
// Configure Database
var useInMemory = builder.Environment.IsEnvironment("IntegrationTest");
var databaseProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";

builder.Services.AddDbContext<StargateContext>(options =>
{
    if (useInMemory)
    {
        options.UseInMemoryDatabase("IntegrationTestsDb");
    }
    else if (databaseProvider == "Sqlite")
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
    else
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

// Initialize database for development
if (builder.Environment.IsDevelopment() && databaseProvider == "Sqlite")
{
    var serviceProvider = builder.Services.BuildServiceProvider();
    using var scope = serviceProvider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<StargateContext>();

    // Ensure database is created and seeded
    context.Database.EnsureCreated();
    DatabaseSeeder.Seed(context);
}
```

### 4. Create Database Seeder

Create a new class to handle seeding:

**Stargate.Repository/DatabaseSeeder.cs:**
```csharp
public static class DatabaseSeeder
{
    public static void Seed(StargateContext context)
    {
        // Check if already seeded
        if (context.People.Any())
        {
            return; // Database already seeded
        }

        // Seed Person data
        var people = new[]
        {
            new PersonAstronautEntity { Id = 1, Name = "John Doe" },
            new PersonAstronautEntity { Id = 2, Name = "Jane Doe" }
        };
        context.People.AddRange(people);
        context.SaveChanges();

        // Seed AstronautDetail data
        var seedDate = new DateTime(2024, 1, 1);
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
```

## Usage

### Starting Fresh

To reset your local database:

1. **Delete the database file:**
   ```bash
   rm E:\Stargate\Stargate.Api\stargate.db
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```
   The database will be automatically recreated and seeded.

### Switching Between SQLite and SQL Server

**For SQLite (Local Development):**
```json
"DatabaseProvider": "Sqlite"
```

**For SQL Server (Azure/Production):**
```json
"DatabaseProvider": "SqlServer"
```

### Database File Location

The SQLite database file (`stargate.db`) will be created in:
```
E:\Stargate\Stargate.Api\stargate.db
```

### .gitignore

Add to `.gitignore` to avoid committing the database file:
```
*.db
*.db-shm
*.db-wal
```

## Deployment to Azure SQL Database

When deploying to Azure, **use the SQL Database Project** (not EF migrations):

1. **Update appsettings.json** with Azure SQL connection string
2. **Set DatabaseProvider** to "SqlServer"
3. **Deploy Stargate.Database** SQL project to Azure SQL:
   - Right-click `Stargate.Database` in Visual Studio
   - Select "Publish"
   - Target your Azure SQL Database
   - Schema and seed data will be deployed from `.sql` files

**Note**: We do NOT use EF migrations. The SQL Database Project is the source of truth for production schema.

## Keeping Schema in Sync

Since the schema is defined in two places:
1. **SQL Database Project** (production) → `Stargate.Database/Tables/*.sql`
2. **EF Core Entities** (local dev) → `Stargate.Repository/Entities/*.cs`

**When making schema changes:**
1. Update SQL Database Project FIRST (source of truth)
2. Update EF Core entity classes to match
3. Update both seed data locations if needed
4. Delete local `stargate.db` to force recreation

See [Database-Architecture.md](Database-Architecture.md) for detailed synchronization process.

## Troubleshooting

### Database Locked Error
If you get a "database is locked" error:
- Stop all running instances of the API
- Delete the `.db-shm` and `.db-wal` files
- Restart the application

### Schema Mismatch
If you change the entity models:
1. Delete the SQLite database file
2. Restart the application to recreate with new schema

### Can't See Data in Database
Use a SQLite viewer:
- **DB Browser for SQLite**: https://sqlitebrowser.org/
- **VS Code Extension**: SQLite Viewer
- **Command Line**: `sqlite3 stargate.db`

## Benefits Summary

✅ **No SQL Server required** for local development
✅ **Instant database reset** - just delete the file
✅ **Version control friendly** - can commit seed database
✅ **Same codebase** works for SQLite and SQL Server
✅ **Fast startup** - no database server connection
✅ **Easy testing** - each test can have its own database file

## Production Deployment

In production (Azure), the system will:
1. Use the Azure SQL Database connection string
2. Apply migrations from the Stargate.Database SQL project
3. Use the post-deployment seed script for initial data
4. Benefit from Azure SQL's scalability and managed features

The local SQLite setup is purely for development convenience and mirrors the production schema exactly.
