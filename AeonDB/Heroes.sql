CREATE TABLE [dbo].[Heroes]
(
	[ID] SMALLINT NOT NULL PRIMARY KEY Identity, 
    [AsmName] VARCHAR(128) NOT NULL, 
    [Name] VARCHAR(64) NULL Foreign key References Translations([ID]), 
    [Desc] VARCHAR(64) NULL Foreign key References Translations([ID])
)
