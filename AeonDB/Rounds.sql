﻿CREATE TABLE [dbo].[Rounds]
(
	[ID] INT NOT NULL PRIMARY KEY, 
    [GameID] INT NOT NULL FOREIGN KEY REFERENCES Games(ID), 
    [Number] SMALLINT NOT NULL
)
