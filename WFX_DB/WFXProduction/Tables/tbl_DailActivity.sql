USE [WFX]
GO

/****** Object:  Table [dbo].[tbl_DailActivity]    Script Date: 08-05-2021 15:26:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_DailActivity](
	[DailActivityID] [bigint] NOT NULL,
	[ActivitDate] [datetime] NOT NULL,
	[UserID] [int] NOT NULL,
	[LastUpdatedDateTime] [datetime] NOT NULL,
	[Seconds] [int] NOT NULL,
	[IsActive] [int] NOT NULL,
	[IsPause] [int] NOT NULL,
	[PONo] [nvarchar](255) NOT NULL,
	[LineName] [nvarchar](255) NOT NULL,
	[OverTime] [int] NOT NULL,
	[FirstLogin] [datetime] NOT NULL,
	[LastLogOut] [datetime] NOT NULL,
	[Manhrs] [int] NOT NULL,
 CONSTRAINT [PK_tbl_DailActivity] PRIMARY KEY CLUSTERED 
(
	[DailActivityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

