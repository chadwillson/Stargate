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
