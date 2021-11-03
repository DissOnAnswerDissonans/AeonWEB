CREATE TABLE [dbo].[Players]
(
	[ID] INT NOT NULL PRIMARY KEY Identity, 
    [Nickname] NVARCHAR(50) NOT NULL, 
    [PWHash] INT NULL, 
    [ValueELO] DECIMAL(9, 4) NULL
)
