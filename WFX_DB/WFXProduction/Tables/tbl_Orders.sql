IF NOT EXISTS(Select top 1 1 from sys.tables where name='tbl_Orders')
Begin
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
		[S1] [int] NOT NULL,
		[UserID] [int] NOT NULL,
		[EntryDate] [datetime] NOT NULL
	 )
	ALTER TABLE tbl_Orders ADD CONSTRAINT PK_tbl_Orders_OrderID PRIMARY KEY (OrderID)
END
GO
IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'Source' AND OBJECT_ID = OBJECT_ID(N'tbl_Orders'))
	ALTER table tbl_Orders add Source [nvarchar](50)		
GO
IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'LastSyncedAt' AND OBJECT_ID = OBJECT_ID(N'tbl_Orders'))
	ALTER table tbl_Orders add LastSyncedAt [datetime]		
GO
IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'WFXColorCode' AND OBJECT_ID = OBJECT_ID(N'tbl_Orders'))
	ALTER table tbl_Orders add WFXColorCode [nvarchar](255)		
GO
IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'WFXColorName' AND OBJECT_ID = OBJECT_ID(N'tbl_Orders'))
	ALTER table tbl_Orders add WFXColorName [nvarchar](255)		
GO
IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'ProcessCode' AND OBJECT_ID = OBJECT_ID(N'tbl_Orders'))
	ALTER table tbl_Orders add ProcessCode [nvarchar](255)		
GO
IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'ProcessName' AND OBJECT_ID = OBJECT_ID(N'tbl_Orders'))
	ALTER table tbl_Orders add ProcessName [nvarchar](255)		
GO
IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'Fulfillment' AND OBJECT_ID = OBJECT_ID(N'tbl_Orders'))
	ALTER table tbl_Orders add FulfillmentType [nvarchar](50)		
GO
