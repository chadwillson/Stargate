-- Seed data aligned with legacy API defaults
SET XACT_ABORT ON;
GO

-- Persons
SET IDENTITY_INSERT [dbo].[Person] ON;
INSERT INTO [dbo].[Person] ([Id], [Name])
SELECT 1, N'John Doe'
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Person] WHERE [Id] = 1);

INSERT INTO [dbo].[Person] ([Id], [Name])
SELECT 2, N'Jane Doe'
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Person] WHERE [Id] = 2);
SET IDENTITY_INSERT [dbo].[Person] OFF;
GO

-- AstronautDetail
-- Seed date aligned with DatabaseSeeder.cs (2024-01-01)
SET IDENTITY_INSERT [dbo].[AstronautDetail] ON;
INSERT INTO [dbo].[AstronautDetail] ([Id], [PersonId], [CurrentRank], [CurrentDutyTitle], [CareerStartDate], [CareerEndDate])
SELECT 1, 1, N'1LT', N'Commander', '2024-01-01T00:00:00', NULL
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[AstronautDetail] WHERE [Id] = 1);
SET IDENTITY_INSERT [dbo].[AstronautDetail] OFF;
GO

-- AstronautDuty
SET IDENTITY_INSERT [dbo].[AstronautDuty] ON;
INSERT INTO [dbo].[AstronautDuty] ([Id], [PersonId], [Rank], [DutyTitle], [DutyStartDate], [DutyEndDate])
SELECT 1, 1, N'1LT', N'Commander', '2024-01-01T00:00:00', NULL
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[AstronautDuty] WHERE [Id] = 1);
SET IDENTITY_INSERT [dbo].[AstronautDuty] OFF;
GO

-- Roles
SET IDENTITY_INSERT [dbo].[Role] ON;
INSERT INTO [dbo].[Role] ([Id], [Name], [Description], [CreatedAt], [UpdatedAt])
SELECT 1, N'Admin', N'Administrator with full access', GETUTCDATE(), GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Role] WHERE [Id] = 1);

INSERT INTO [dbo].[Role] ([Id], [Name], [Description], [CreatedAt], [UpdatedAt])
SELECT 2, N'User', N'Standard user with limited access', GETUTCDATE(), GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Role] WHERE [Id] = 2);
SET IDENTITY_INSERT [dbo].[Role] OFF;
GO

-- Users
-- Default passwords (all hashed with BCrypt):
-- admin: Stargate123! -> $2a$11$8YQVz7X8YQVz7X8YQVz7.O8YQVz7X8YQVz7X8YQVz7X8YQVz7X8YQ (placeholder)
-- user: Password1! -> $2a$11$1YQVz7X8YQVz7X8YQVz7.O8YQVz7X8YQVz7X8YQVz7X8YQVz7X8YQ (placeholder)
SET IDENTITY_INSERT [dbo].[User] ON;
INSERT INTO [dbo].[User] ([Id], [Username], [Email], [PasswordHash], [FirstName], [LastName], [RoleId], [IsActive], [CreatedAt], [UpdatedAt])
SELECT 1, N'admin', N'admin@stargate.com', N'$2a$11$8YQVz7X8YQVz7X8YQVz7.O8YQVz7X8YQVz7X8YQVz7X8YQVz7X8YQ', N'Admin', N'User', 1, 1, GETUTCDATE(), GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[User] WHERE [Id] = 1);

INSERT INTO [dbo].[User] ([Id], [Username], [Email], [PasswordHash], [FirstName], [LastName], [RoleId], [IsActive], [CreatedAt], [UpdatedAt])
SELECT 2, N'user', N'user@stargate.com', N'$2a$11$1YQVz7X8YQVz7X8YQVz7.O8YQVz7X8YQVz7X8YQVz7X8YQVz7X8YQ', N'Standard', N'User', 2, 1, GETUTCDATE(), GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[User] WHERE [Id] = 2);
SET IDENTITY_INSERT [dbo].[User] OFF;
GO
