USE [WFX]
GO

/****** Object:  Table [dbo].[tbl_Operations]    Script Date: 08-05-2021 15:41:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_Operations](
	[OperationID] [int] NOT NULL,
	[OperationName] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_tbl_Operations] PRIMARY KEY CLUSTERED 
(
	[OperationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

