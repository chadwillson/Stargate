# Database Architecture

## Overview

The Stargate project uses **different database strategies for different environments** to optimize for both production performance and development convenience.

## Architecture Decision

### SQL Database Project as Source of Truth

The **Stargate.Database** SQL Database Project is the **single source of truth** for:
- Database schema (tables, constraints, indexes)
- Post-deployment seed data
- Azure/Production deployments

### No EF Migrations Needed

**We do NOT use EF Core Migrations** because:
1. The SQL Database Project already defines our schema
2. Migrations would duplicate what the SQL project does
3. We don't need version-controlled schema changes - the SQL project handles that

## Environment-Specific Implementations

### Production / Azure

```
SQL Database Project (Stargate.Database)
    ↓
Azure SQL Database
```

- **Schema Source**: `Stargate.Database/Tables/*.sql`
- **Seed Data**: `Stargate.Database/Post-Deployment/Script.PostDeployment.sql`
- **Deployment**: Visual Studio Database Project publish
- **Features**: Full SQL Server features, IDENTITY columns, advanced indexes

### Local Development

```
EF Core Entity Classes (Stargate.Repository)
    ↓
SQLite Database (stargate.db)
```

- **Schema Source**: EF Core creates from `PersonAstronautEntity`, etc.
- **Seed Data**: `DatabaseSeeder.cs` C# code
- **Deployment**: Automatic on app startup (`EnsureCreated()`)
- **Features**: Simplified SQLite features, portable file-based database

### Integration Tests

```
EF Core Entity Classes
    ↓
In-Memory Database
```

- **Schema Source**: EF Core in-memory provider
- **Seed Data**: Test-specific data
- **Deployment**: Created per test
- **Features**: Ultra-fast, no persistence

## Keeping in Sync

### The Challenge

Since we don't use migrations, we maintain schema in TWO places:
1. **SQL Database Project** (`.sql` files) → Production
2. **EF Core Entities** (`.cs` files) → Local dev

These MUST stay synchronized manually.

### Synchronization Process

When making schema changes:

#### 1. Update SQL Database Project FIRST (Source of Truth)

```sql
-- Stargate.Database/Tables/Person.sql
ALTER TABLE [dbo].[Person]
ADD [Email] NVARCHAR(255) NULL
```

#### 2. Update EF Core Entity to Match

```csharp
// Stargate.Repository/Entities/PersonAstronautEntity.cs
public class PersonAstronautEntity
{
    // ... existing properties
    public string? Email { get; set; }  // ADD THIS
}
```

#### 3. Update Seed Data (if needed)

**SQL Script:**
```sql
-- Stargate.Database/Post-Deployment/Script.PostDeployment.sql
INSERT INTO [dbo].[Person] ([Id], [Name], [Email])
SELECT 1, N'John Doe', N'john@example.com'
```

**C# Seeder:**
```csharp
// Stargate.Repository/DatabaseSeeder.cs
new PersonAstronautEntity
{
    Id = 1,
    Name = "John Doe",
    Email = "john@example.com"  // ADD THIS
}
```

### Verification Checklist

After any schema change:

- [ ] SQL table definitions updated
- [ ] EF Core entity classes match
- [ ] SQL post-deployment script updated (if seed data changed)
- [ ] DatabaseSeeder.cs matches seed data
- [ ] Integration tests still pass
- [ ] Local SQLite database deleted and recreated successfully

## Why This Approach?

### Benefits

✅ **Zero Local Setup** - No SQL Server required for development
✅ **Fast Development** - SQLite is instant, no server overhead
✅ **Production-Ready** - SQL Project ensures proper Azure deployment
✅ **Clear Separation** - Each environment optimized for its use case
✅ **No Migration Conflicts** - SQL Project is always authoritative

### Trade-offs

⚠️ **Manual Sync Required** - Entity classes must match SQL definitions
⚠️ **Two Seed Locations** - SQL script + C# seeder (documented in code)
⚠️ **SQLite Limitations** - Some SQL Server features not available locally

## Alternative: SQL Server Everywhere

If you prefer NOT to use SQLite:

### Use SQL Server LocalDB for Development

**Update `appsettings.Development.json`:**
```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=StargateDB;..."
  }
}
```

**Deploy SQL Database Project:**
1. Right-click Stargate.Database in Visual Studio
2. Select "Publish"
3. Target: `(localdb)\mssqllocaldb`
4. Database: `StargateDB`

**Pros:**
- Same database everywhere
- SQL Database Project is sole source
- No synchronization needed

**Cons:**
- Requires SQL Server installation
- Slower than SQLite
- Database state persists (can't just delete file)

## Comparison Matrix

| Aspect | SQL Project (Production) | EF + SQLite (Local Dev) | EF + SQL Server LocalDB |
|--------|--------------------------|------------------------|------------------------|
| **Schema Source** | .sql files | Entity classes | .sql files |
| **Seed Data** | SQL script | C# seeder | SQL script |
| **Setup** | Azure deployment | Zero-config | SQL Server install |
| **Speed** | Cloud latency | Instant | Local server |
| **Features** | Full SQL Server | SQLite subset | Full SQL Server |
| **Sync Required** | N/A | Manual | No |

## Best Practices

### For Schema Changes

1. **Always update SQL Database Project first** - it's the source of truth
2. **Document breaking changes** in commit messages
3. **Test both environments** after schema changes
4. **Delete local SQLite** after entity changes to force recreation

### For Seed Data Changes

1. **Update SQL post-deployment script first**
2. **Copy same data to DatabaseSeeder.cs**
3. **Use same dates/values** for consistency
4. **Add comments** referencing the SQL script

### For Team Development

1. **Communicate schema changes** to team
2. **Document in PR description** what changed
3. **Include verification steps** in PR checklist
4. **Delete `*.db` files** before pulling schema changes

## Conclusion

This architecture provides:
- **Optimal production deployment** via SQL Database Project
- **Optimal development experience** via SQLite
- **Clear ownership** - SQL Project is authoritative
- **No migration confusion** - one source of schema truth

The manual synchronization trade-off is acceptable because:
- Schema changes are infrequent
- Entities already mirror tables (standard practice)
- Clear documentation makes sync straightforward
- Benefits (zero-setup local dev) outweigh costs
