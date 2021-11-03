CREATE TABLE [dbo].[Stats]
(
	[ID] TINYINT NOT NULL PRIMARY KEY, 
    [AsmName] VARCHAR(128) NOT NULL, 
    [Name] VARCHAR(64) NULL Foreign key References Translations([ID]), 
    [Desc] VARCHAR(64) NULL Foreign key References Translations([ID])
)
