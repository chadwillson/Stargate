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
GO

CREATE INDEX [IX_LogEntry_Timestamp] ON [dbo].[LogEntry] ([Timestamp])
GO

CREATE INDEX [IX_LogEntry_Level] ON [dbo].[LogEntry] ([Level])
GO

CREATE INDEX [IX_LogEntry_Category] ON [dbo].[LogEntry] ([Category])
GO

CREATE INDEX [IX_LogEntry_CorrelationId] ON [dbo].[LogEntry] ([CorrelationId])
GO
