﻿CREATE TABLE [dbo].[Buys]
(
	[RoundID] INT NOT NULL FOREIGN KEY REFERENCES Rounds(ID), 
    [StatID] TINYINT NOT NULL FOREIGN KEY REFERENCES [Stats](ID),
    [IsOpt] BIT NOT NULL, 
    [Amount] TINYINT NOT NULL,
    [Player] TINYINT NOT NULL 
)
