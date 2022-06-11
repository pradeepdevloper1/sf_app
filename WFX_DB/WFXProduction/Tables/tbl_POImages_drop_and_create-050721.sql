USE [WFX]
GO

/****** Object:  Table [dbo].[tbl_POImages]    Script Date: 05-07-2021 10:07:14 ******/
DROP TABLE [dbo].[tbl_POImages]
GO

/****** Object:  Table [dbo].[tbl_POImages]    Script Date: 05-07-2021 10:07:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_POImages](
	[POImageID] [bigint] NOT NULL,
	[PONo] [nvarchar](255) NOT NULL,
	[ImageName] [nvarchar](255) NOT NULL,
	[ImagePath] [nvarchar](255) NOT NULL,
	[IMGNo] [int] NOT NULL,
	[FactoryID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[EntryDate] [datetime] NOT NULL,
 CONSTRAINT [PK_tbl_POImages] PRIMARY KEY CLUSTERED 
(
	[POImageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


