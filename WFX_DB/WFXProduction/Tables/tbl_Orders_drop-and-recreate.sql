USE [WFX]
GO

/****** Object:  Table [dbo].[tbl_Orders]    Script Date: 03-05-2021 08:28:31 ******/
DROP TABLE [dbo].[tbl_Orders]
GO

/****** Object:  Table [dbo].[tbl_Orders]    Script Date: 03-05-2021 08:28:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_Orders](
	[OrderID] [bigint] NOT NULL,
	[Module] [nvarchar](255) NOT NULL,
	[SONo] [nvarchar](255) NOT NULL,
	[PONo] [nvarchar](255) NOT NULL,
	[Style] [nvarchar](255) NOT NULL,
	[Fit] [nvarchar](255) NOT NULL,
	[Product] [nvarchar](255) NOT NULL,
	[Season] [nvarchar](255) NOT NULL,
	[Customer] [nvarchar](255) NOT NULL,
	[PlanStDt] [datetime] NOT NULL,
	[ExFactory] [datetime] NOT NULL,
	[PrimaryPart] [int] NOT NULL,
	[Part] [nvarchar](255) NOT NULL,
	[Color] [nvarchar](255) NOT NULL,
	[Hexcode] [nvarchar](255) NOT NULL,
	[Fabric] [nvarchar](255) NOT NULL,
	[OrderRemark] [nvarchar](255) NOT NULL,
	[IsSizeRun] [int] NOT NULL,
	[POQty] [int] NOT NULL,
	[SizeList] [nvarchar](255) NOT NULL,
	[OrderStatus] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[FactoryID] [int] NOT NULL,
	[EntryDate] [datetime] NOT NULL,
 CONSTRAINT [PK_tbl_Orders] PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

