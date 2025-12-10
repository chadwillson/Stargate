# Quick Start: SQLite Local Development

## Overview

The Stargate project is now configured to use SQLite for local development, making it easy to get started without installing SQL Server.

**Architecture Note**: The SQL Database Project (`Stargate.Database`) is the source of truth for production. SQLite is for local dev convenience only. See `Database-Architecture.md` for details.

## What's Been Configured

✅ **SQLite Database Provider** - Added to `Stargate.Api`
✅ **Automatic Database Creation** - Database created on first run
✅ **Automatic Seeding** - Initial data loaded from `DatabaseSeeder`
✅ **Development Configuration** - `appsettings.Development.json` configured for SQLite
✅ **Gitignore Updated** - Database files excluded from version control

## Running the Application

### 1. Start the Application

Simply run the API and database will be created automatically:

```bash
cd Stargate.Api
dotnet run
```

Or use the launch script:
```bash
E:\Stargate\Stargate\start-stargate.bat
```

### 2. Database Creation

On first run, the system will:
1. Create `stargate.db` in the `Stargate.Api` directory
2. Create all tables (Person, AstronautDetail, AstronautDuty)
3. Seed initial data (John Doe, Jane Doe)

### 3. Access the API

- **API**: http://localhost:5031
- **Swagger UI**: http://localhost:5031/swagger

## Initial Data

The database is seeded with:

**Person:**
- John Doe (ID: 1)
- Jane Doe (ID: 2)

**AstronautDetail:**
- John Doe - Rank: 1LT, Duty: Commander (ID: 1)

**AstronautDuty:**
- John Doe - Rank: 1LT, Duty: Commander, Start: 2024-01-01 (ID: 1)

## Common Tasks

### Reset the Database

To start fresh:

**Windows:**
```bash
cd E:\Stargate\Stargate\Stargate.Api
del stargate.db
dotnet run
```

**PowerShell:**
```powershell
Remove-Item "E:\Stargate\Stargate\Stargate.Api\stargate.db"
dotnet run
```

The database will be recreated and reseeded automatically.

### View Database Contents

**Option 1: DB Browser for SQLite**
1. Download from https://sqlitebrowser.org/
2. Open `E:\Stargate\Stargate\Stargate.Api\stargate.db`

**Option 2: VS Code Extension**
1. Install "SQLite Viewer" extension
2. Right-click `stargate.db` → "Open Database"

**Option 3: Command Line**
```bash
cd E:\Stargate\Stargate\Stargate.Api
sqlite3 stargate.db
.tables
.schema Person
SELECT * FROM Person;
```

### Switch to SQL Server

To use SQL Server instead of SQLite:

1. **Update `appsettings.Development.json`:**
   ```json
   {
     "DatabaseProvider": "SqlServer",
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StargateDB;..."
     }
   }
   ```

2. **Restart the application**

## File Locations

- **Database File**: `E:\Stargate\Stargate\Stargate.Api\stargate.db`
- **Configuration**: `E:\Stargate\Stargate\Stargate.Api\appsettings.Development.json`
- **Seeder (Local)**: `E:\Stargate\Stargate\Stargate.Repository\DatabaseSeeder.cs`
- **Schema (Production)**: `E:\Stargate\Stargate\Stargate.Database\Tables\*.sql`
- **Seed (Production)**: `E:\Stargate\Stargate\Stargate.Database\Post-Deployment\Script.PostDeployment.sql`
- **Full Guide**: `E:\Stargate\Stargate\documents\SQLite-Local-Development-Setup.md`
- **Architecture**: `E:\Stargate\Stargate\documents\Database-Architecture.md`

## Testing CRUD Operations

### Using Swagger UI

1. Navigate to http://localhost:5031/swagger
2. Try these operations:

**GET /api/Person** - List all people
```json
[
  {
    "id": 1,
    "name": "John Doe",
    "currentRank": "1LT",
    "currentDutyTitle": "Commander",
    "careerStartDate": "2024-01-01T00:00:00",
    "careerEndDate": null
  },
  {
    "id": 2,
    "name": "Jane Doe",
    "currentRank": null,
    "currentDutyTitle": null,
    "careerStartDate": null,
    "careerEndDate": null
  }
]
```

**POST /api/Person** - Create a new person
```json
{
  "id": 0,
  "name": "Jack O'Neill"
}
```

**GET /api/AstronautDuty/John Doe** - Get duties by name

**POST /api/AstronautDuty** - Create new duty assignment
```json
{
  "name": "Jack O'Neill",
  "rank": "Colonel",
  "dutyTitle": "SG-1 Commander",
  "dutyStartDate": "2024-12-09T00:00:00"
}
```

## Troubleshooting

### Database Locked Error
- **Cause**: Multiple instances running or file in use
- **Fix**: Stop all API instances, delete `.db-shm` and `.db-wal` files, restart

### Schema Changes Not Applied
- **Cause**: Database schema changed in code but file still exists
- **Fix**: Delete `stargate.db` and restart to recreate with new schema

### Can't Find Database File
- **Location**: Should be in `Stargate.Api` directory
- **Check**: Configuration in `appsettings.Development.json`
- **Connection String**: `"Data Source=stargate.db"` creates file in current directory

## Production vs Development

| Environment | Database | Configuration |
|-------------|----------|--------------|
| **Local Dev** | SQLite | `appsettings.Development.json` |
| **Azure** | SQL Server | `appsettings.json` + environment variables |
| **Integration Tests** | In-Memory | Automatic when running tests |

## Next Steps

1. ✅ Database is ready - no manual setup required
2. 🚀 Start coding - CRUD operations work out of the box
3. 🧪 Run tests - Integration tests use in-memory database
4. 📝 Read full guide - See `SQLite-Local-Development-Setup.md` for details

## Benefits

- **Zero Configuration**: No SQL Server installation needed
- **Fast Reset**: Delete file to start fresh
- **Portable**: Entire database is one file
- **Team Friendly**: Everyone gets same initial state
- **Azure Ready**: Same code works with SQL Server in production
