USE [coffeeroomdb]
GO

/****** Object:  Table [jsm33t].[TblPasswordReset]    Script Date: 7/29/2023 2:51:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TblPasswordReset](
	[Id] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[Token] [nvarchar](50) NOT NULL,
	[DateAdded] [datetime] NOT NULL,
	[IsValid] [bit] NULL
) ON [PRIMARY]
GO

