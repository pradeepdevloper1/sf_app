
if not exists(Select Top 1 1 from information_schema.tables where Table_Name='tbl_Lines')
Begin
	
CREATE TABLE [dbo].[tbl_Lines](
	[LineID] [int] NOT NULL,
	[ModuleID] [int] NOT NULL,
	[LineName] [nvarchar](255) NOT NULL,
	[InternalLineName] [nvarchar](255) NOT NULL,
	[NoOfMachine] [int] NOT NULL,
	[LineCapacity] [int] NOT NULL,
	[LineloadingPoint] [nvarchar](255) NOT NULL,
	[TabletID] [nvarchar](255) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[FactoryID] [int] NULL,
	[ModuleName] [nvarchar](50) NOT NULL
	)	
End

GO
IF NOT EXISTS(SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_TYPE='PRIMARY KEY' and TABLE_NAME='tbl_Lines')
BEGIN
	ALTER TABLE tbl_Lines ADD CONSTRAINT PK_tbl_Lines_LineID PRIMARY KEY (LineID)
END

GO
IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.COLUMNS WHERE NAME = N'ProcessType' AND OBJECT_ID = OBJECT_ID(N'tbl_Lines'))
	ALTER TABLE tbl_Lines ADD ProcessType [nvarchar](255) NULL	

GO
IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.COLUMNS WHERE Name = N'DeviceSerialNo' AND OBJECT_ID = OBJECT_ID(N'tbl_Lines'))
	ALTER TABLE tbl_Lines ADD DeviceSerialNo [nvarchar](255) NULL	

