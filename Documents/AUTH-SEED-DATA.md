# Authentication Seed Data

## Default Users

The system is seeded with two default users for testing:

### Admin User
- **Username**: `admin`
- **Password**: `Stargate123!`
- **Email**: `admin@stargate.com`
- **Role**: Admin
- **Permissions**: Full access to all features including user management

### Standard User
- **Username**: `user`
- **Password**: `Password1!`
- **Email**: `user@stargate.com`
- **Role**: User
- **Permissions**: Limited access, cannot access admin features

## BCrypt Password Hashes

For **SQLite Development** (DatabaseSeeder.cs):
- The C# seeder uses BCrypt.Net to hash passwords at runtime
- Placeholder hashes in the code will be replaced when you first run the application

For **SQL Server/Azure** (Post-Deployment script):
- The SQL script currently contains placeholder BCrypt hashes
- **TODO**: Generate real BCrypt hashes and update Script.PostDeployment.sql

To generate proper BCrypt hashes for the SQL script, use an online BCrypt generator or run:
```csharp
using BCrypt.Net;
var adminHash = BCrypt.HashPassword("Stargate123!");
var userHash = BCrypt.HashPassword("Password1!");
```

## Security Notes

**IMPORTANT**: Change these default passwords before deploying to production!

- These credentials are for development and testing only
- In production, force password reset on first login
- Implement proper password policies
- Consider multi-factor authentication
