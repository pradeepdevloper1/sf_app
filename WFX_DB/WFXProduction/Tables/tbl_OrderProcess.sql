IF NOT EXISTS(Select top 1 1 from sys.tables where name='tbl_OrderProcess')
Begin
CREATE TABLE [dbo].[tbl_OrderProcess](
	[OrderProcessID] [bigint] NOT NULL,
	[ProcessCode] [nvarchar](255) NOT NULL,
	[ProcessName] [nvarchar](255) NOT NULL,
	[AfterProcessCode] [nvarchar](255),
	[AfterProcessName] [nvarchar](255),
	[SONo] [nvarchar](255) NOT NULL,
	[PONo] [nvarchar](255) NOT NULL,
	[FactoryID] int NOT NULL,
	[UserID] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL
)
ALTER TABLE tbl_OrderProcess ADD CONSTRAINT PK_tbl_OrderProcess_OrderProcessID PRIMARY KEY (OrderProcessID)
CREATE INDEX tbl_OrderProcess_ProcessCode ON tbl_OrderProcess (FactoryID,ProcessCode);
CREATE INDEX tbl_OrderProcess_AfterProcessCode ON tbl_OrderProcess (FactoryID,AfterProcessCode);
CREATE INDEX tbl_OrderProcess_SONo_PONo ON tbl_OrderProcess (FactoryID,SONo,PONo);
END

IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'ProcessEnabled' AND OBJECT_ID = OBJECT_ID(N'tbl_OrderProcess'))
	ALTER table tbl_OrderProcess add ProcessEnabled bit NULL	


