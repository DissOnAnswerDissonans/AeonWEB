CREATE TABLE [dbo].[Attacks]
(
	[RoundID] INT NOT NULL FOREIGN KEY REFERENCES Rounds(ID), 
    [Number] SMALLINT NOT NULL, 
    [Result] VARCHAR(32) NULL, 
    [Crit1] BIT NOT NULL, 
    [Crit2] BIT NOT NULL 
)
