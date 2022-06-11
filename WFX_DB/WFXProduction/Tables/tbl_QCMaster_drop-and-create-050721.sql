USE [WFX]
GO

/****** Object:  Table [dbo].[tbl_QCMaster]    Script Date: 05-07-2021 10:06:13 ******/
DROP TABLE [dbo].[tbl_QCMaster]
GO

/****** Object:  Table [dbo].[tbl_QCMaster]    Script Date: 05-07-2021 10:06:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_QCMaster](
	[QCMasterId] [bigint] NOT NULL,
	[QCDate] [datetime] NOT NULL,
	[TypeOfWork] [int] NOT NULL,
	[PONo] [nvarchar](255) NOT NULL,
	[Color] [nvarchar](255) NOT NULL,
	[Part] [nvarchar](255) NOT NULL,
	[Size] [nvarchar](255) NOT NULL,
	[Qty] [int] NOT NULL,
	[QCStatus] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[FactoryID] [int] NOT NULL,
	[ShiftName] [nvarchar](255) NULL,
	[Module] [nvarchar](255) NOT NULL,
	[Line] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tbl_QCMaster] PRIMARY KEY CLUSTERED 
(
	[QCMasterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


