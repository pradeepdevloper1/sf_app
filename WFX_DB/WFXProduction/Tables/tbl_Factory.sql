IF NOT EXISTS(Select top 1 1 from sys.tables where name='tbl_Factory')
Begin
	CREATE TABLE [dbo].[tbl_Factory](
		[FactoryID] [int] NOT NULL,
		[ClusterID] [int] NOT NULL,
		[FactoryName] [nvarchar](255) NOT NULL,
		[FactoryAddress] [nvarchar](255) NOT NULL,
		[FactoryType] [nvarchar](255) NOT NULL,
		[FactoryHead] [nvarchar](255) NOT NULL,
		[FactoryEmail] [nvarchar](255) NOT NULL,
		[FactoryContactNumber] [int] NOT NULL,
		[FactoryCountry] [int] NULL,
		[FactoryTimeZone] [nvarchar](255) NOT NULL,
		[NoOfShifts] [int] NOT NULL,
		[DecimalValue] [int] NOT NULL,
		[PTMPrice] [float] NOT NULL,
		[NoOfUsers] [int] NOT NULL,
		[FactoryOffOn] [nvarchar](255) NOT NULL,
		[MeasuringUnit] [nvarchar](255) NOT NULL,
		[DataScale] [int] NOT NULL,
		[NoOfLine] [int] NULL,
		[SmartLines] [int] NULL)

	ALTER TABLE tbl_Factory ADD CONSTRAINT PK_tbl_Factory_FactoryID PRIMARY KEY (FactoryID)
End
GO
IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'LinkedwithERP' AND OBJECT_ID = OBJECT_ID(N'tbl_Factory'))
	ALTER table tbl_Factory add LinkedwithERP [nvarchar](50)		
GO
