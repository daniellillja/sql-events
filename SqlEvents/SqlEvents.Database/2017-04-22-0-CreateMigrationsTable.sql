﻿CREATE TABLE [dbo].[Migrations]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [DateApplied] DATETIME NOT NULL, 
    [Hash] NCHAR(10) NOT NULL
)
