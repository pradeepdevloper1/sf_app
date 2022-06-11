USE [WFX]
GO

/****** Object:  Table [dbo].[tbl_FactoryType]    Script Date: 6/1/2021 4:54:27 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_FactoryType](
	[FactorytypeID] [int] NOT NULL,
	[FactoryType] [nvarchar](150) NULL,
 CONSTRAINT [PK_tbl_FactoryType] PRIMARY KEY CLUSTERED 
(
	[FactorytypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


