IF NOT EXISTS(Select top 1 1 from sys.tables where name='tbl_QCMaster')
Begin
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
	 CONSTRAINT [PK_tbl_QCMaster] PRIMARY KEY CLUSTERED 
	(
		[QCMasterId] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
IF NOT EXISTS(SELECT Top 1 1 FROM sys.columns WHERE Name = N'ShiftName' AND OBJECT_ID = OBJECT_ID(N'tbl_QCMaster')) 
BEGIN
    ALTER TABLE tbl_QCMaster ADD  ShiftName nvarchar(510) NULL
END
IF NOT EXISTS(SELECT Top 1 1 FROM sys.columns WHERE Name = N'Module' AND OBJECT_ID = OBJECT_ID(N'tbl_QCMaster')) 
BEGIN
    ALTER TABLE tbl_QCMaster ADD  Module nvarchar(510) NULL
END
IF NOT EXISTS(SELECT Top 1 1 FROM sys.columns WHERE Name = N'Line' AND OBJECT_ID = OBJECT_ID(N'tbl_QCMaster')) 
BEGIN
    ALTER TABLE tbl_QCMaster ADD  Line nvarchar(510) NULL
END
IF NOT EXISTS(SELECT Top 1 1 FROM sys.columns WHERE Name = N'SONo' AND OBJECT_ID = OBJECT_ID(N'tbl_QCMaster')) 
BEGIN
    ALTER TABLE tbl_QCMaster ADD  SONo nvarchar(510) NULL
END
IF NOT EXISTS(SELECT Top 1 1 FROM sys.columns WHERE Name = N'WFXStockGRNID' AND OBJECT_ID = OBJECT_ID(N'tbl_QCMaster')) 
BEGIN
    ALTER TABLE tbl_QCMaster ADD  WFXStockGRNID decimal(28,0) NULL
END

IF NOT EXISTS(SELECT Top 1 1 FROM sys.columns WHERE Name = N'BatchNumber' AND OBJECT_ID = OBJECT_ID(N'tbl_QCMaster')) 
BEGIN
    ALTER TABLE tbl_QCMaster ADD  BatchNumber int NULL
END
IF EXISTS(SELECT Top 1 1 FROM sys.columns WHERE Name = N'BatchNumber' AND OBJECT_ID = OBJECT_ID(N'tbl_QCMaster')) 
BEGIN
    ALTER TABLE tbl_QCMaster Alter column  BatchNumber varchar(255) NULL
END

IF NOT EXISTS(SELECT Top 1 1 FROM sys.columns WHERE Name = N'QCRequestID' AND OBJECT_ID = OBJECT_ID(N'tbl_QCMaster')) 
BEGIN
    ALTER TABLE tbl_QCMaster ADD  QCRequestID bigint NULL
END

IF NOT EXISTS(SELECT Top 1 1 FROM sys.columns WHERE Name = N'TabletID' AND OBJECT_ID = OBJECT_ID(N'tbl_QCMaster')) 
BEGIN
    ALTER TABLE tbl_QCMaster ADD  TabletID nvarchar(500) NULL
END

IF NOT EXISTS(SELECT Top 1 1 FROM sys.columns WHERE Name = N'WFXColorCode' AND OBJECT_ID = OBJECT_ID(N'tbl_QCMaster')) 
BEGIN
    ALTER TABLE tbl_QCMaster ADD  WFXColorCode nvarchar(100) NULL
END

IF NOT EXISTS(SELECT Top 1 1 FROM sys.columns WHERE Name = N'WFXColorName' AND OBJECT_ID = OBJECT_ID(N'tbl_QCMaster')) 
BEGIN
    ALTER TABLE tbl_QCMaster ADD  WFXColorName nvarchar(100) NULL
END
IF NOT EXISTS(SELECT Top 1 1 FROM sys.columns WHERE Name = N'ProcessCode' AND OBJECT_ID = OBJECT_ID(N'tbl_QCMaster')) 
BEGIN
    ALTER TABLE tbl_QCMaster ADD  ProcessCode nvarchar(100) NULL
END
IF NOT EXISTS(SELECT Top 1 1 FROM sys.columns WHERE Name = N'ProcessName' AND OBJECT_ID = OBJECT_ID(N'tbl_QCMaster')) 
BEGIN
    ALTER TABLE tbl_QCMaster ADD  ProcessName nvarchar(100) NULL
END
