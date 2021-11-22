﻿CREATE TABLE [dbo].[Product]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
	[Code] VARCHAR(50) NOT NULL UNIQUE,
	[Stoc] INT NOT NULL DEFAULT 0,
	[UnitPrice] FLOAT NOT NULL,
)
