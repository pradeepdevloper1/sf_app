USE [WFX]
GO

/****** Object:  Table [dbo].[tbl_QCDefectDetails]    Script Date: 08-05-2021 15:32:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbl_QCDefectDetails](
	[QCDefectDetailsId] [bigint] NOT NULL,
	[QCMasterId] [bigint] NOT NULL,
	[DefectType] [nvarchar](255) NOT NULL,
	[DefectName] [nvarchar](255) NOT NULL,
	[OperationName] [nvarchar](255) NOT NULL
) ON [PRIMARY]
GO

