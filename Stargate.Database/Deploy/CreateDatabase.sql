-- Create StargateDB Database if it doesn't exist
USE [master]
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'StargateDB')
BEGIN
    CREATE DATABASE [StargateDB]
END
GO

USE [StargateDB]
GO

-- Create Person table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Person]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Person]
    (
        [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [Name] NVARCHAR(255) NOT NULL
    )
END
GO

-- Create AstronautDetail table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AstronautDetail]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AstronautDetail]
    (
        [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [PersonId] INT NOT NULL,
        [CurrentRank] NVARCHAR(100) NOT NULL,
        [CurrentDutyTitle] NVARCHAR(255) NOT NULL,
        [CareerStartDate] DATETIME2 NOT NULL,
        [CareerEndDate] DATETIME2 NULL,
        CONSTRAINT [FK_AstronautDetail_Person] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person]([Id])
    )
END
GO

-- Create AstronautDuty table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AstronautDuty]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AstronautDuty]
    (
        [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [PersonId] INT NOT NULL,
        [Rank] NVARCHAR(100) NOT NULL,
        [DutyTitle] NVARCHAR(255) NOT NULL,
        [DutyStartDate] DATETIME2 NOT NULL,
        [DutyEndDate] DATETIME2 NULL,
        CONSTRAINT [FK_AstronautDuty_Person] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person]([Id])
    )
END
GO

-- Create LogEntry table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LogEntry]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[LogEntry]
    (
        [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
        [Timestamp] DATETIME2 NOT NULL,
        [Level] NVARCHAR(20) NOT NULL,
        [Category] NVARCHAR(255) NOT NULL,
        [Message] NVARCHAR(MAX) NOT NULL,
        [Exception] NVARCHAR(4000) NULL,
        [StackTrace] NVARCHAR(4000) NULL,
        [Source] NVARCHAR(255) NULL,
        [CorrelationId] NVARCHAR(50) NULL,
        [UserId] NVARCHAR(100) NULL,
        [RequestPath] NVARCHAR(500) NULL,
        [RequestMethod] NVARCHAR(10) NULL,
        [StatusCode] INT NULL,
        [ElapsedMilliseconds] BIGINT NULL,
        [AdditionalData] NVARCHAR(4000) NULL
    )
END
GO

-- Create indexes for LogEntry table
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LogEntry_Timestamp' AND object_id = OBJECT_ID('dbo.LogEntry'))
BEGIN
    CREATE INDEX [IX_LogEntry_Timestamp] ON [dbo].[LogEntry] ([Timestamp])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LogEntry_Level' AND object_id = OBJECT_ID('dbo.LogEntry'))
BEGIN
    CREATE INDEX [IX_LogEntry_Level] ON [dbo].[LogEntry] ([Level])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LogEntry_Category' AND object_id = OBJECT_ID('dbo.LogEntry'))
BEGIN
    CREATE INDEX [IX_LogEntry_Category] ON [dbo].[LogEntry] ([Category])
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_LogEntry_CorrelationId' AND object_id = OBJECT_ID('dbo.LogEntry'))
BEGIN
    CREATE INDEX [IX_LogEntry_CorrelationId] ON [dbo].[LogEntry] ([CorrelationId])
END
GO

PRINT 'Database and tables created successfully!'
GO
