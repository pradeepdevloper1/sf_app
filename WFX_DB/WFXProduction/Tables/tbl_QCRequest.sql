IF NOT EXISTS(Select top 1 1 from sys.tables where name='tbl_QCRequest')
Begin
	CREATE TABLE [dbo].[tbl_QCRequest](
		[QCRequestID] [bigint] NOT NULL,
		[TranNum] [nvarchar](255) NOT NULL,
		[PONo] [nvarchar](255) NOT NULL,
		[Quantity] int NOT NULL,
		[SyncedAt] [datetime]  NULL,
		[GRNstatus] [nvarchar](255) NOT NULL,
		[StockGRNID] bigint  Default 0 NOT NULL,
		[ErrorMessage] [nvarchar](2000)  NULL,
		[FactoryID] int NOT NULL,
		[SONo] [nvarchar](255) NOT NULL
	 )
	ALTER TABLE tbl_QCRequest ADD CONSTRAINT PK_tbl_QCRequest_QCRequestID PRIMARY KEY (QCRequestID)

END
GO
IF NOT EXISTS(SELECT Top 1 1 FROM sys.columns WHERE Name = N'RequestType' AND OBJECT_ID = OBJECT_ID(N'tbl_QCRequest')) 
BEGIN
    ALTER TABLE tbl_QCRequest ADD  RequestType nvarchar(50) NULL
END
IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'LineNumber' AND OBJECT_ID = OBJECT_ID(N'tbl_QCRequest'))
	ALTER table tbl_QCRequest add LineNumber [nvarchar](255)		
GO
IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'StyleRef' AND OBJECT_ID = OBJECT_ID(N'tbl_QCRequest'))
	ALTER table tbl_QCRequest add StyleRef [nvarchar](255)		
GO