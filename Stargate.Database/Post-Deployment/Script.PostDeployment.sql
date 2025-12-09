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

DECLARE @SeedDate DATETIME2 = '2024-01-01T00:00:00';

-- AstronautDetail
SET IDENTITY_INSERT [dbo].[AstronautDetail] ON;
INSERT INTO [dbo].[AstronautDetail] ([Id], [PersonId], [CurrentRank], [CurrentDutyTitle], [CareerStartDate], [CareerEndDate])
SELECT 1, 1, N'1LT', N'Commander', @SeedDate, NULL
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[AstronautDetail] WHERE [Id] = 1);
SET IDENTITY_INSERT [dbo].[AstronautDetail] OFF;
GO

-- AstronautDuty
SET IDENTITY_INSERT [dbo].[AstronautDuty] ON;
INSERT INTO [dbo].[AstronautDuty] ([Id], [PersonId], [Rank], [DutyTitle], [DutyStartDate], [DutyEndDate])
SELECT 1, 1, N'1LT', N'Commander', @SeedDate, NULL
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[AstronautDuty] WHERE [Id] = 1);
SET IDENTITY_INSERT [dbo].[AstronautDuty] OFF;
GO
