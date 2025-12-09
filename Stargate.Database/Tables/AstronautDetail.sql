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
