IF NOT EXISTS(Select top 1 1 from sys.tables where name='tbl_OrderIssue')
Begin
	CREATE TABLE [dbo].[tbl_OrderIssue](
		[OrderIssueId] [bigint] NOT NULL,
		[IssueDate] [datetime] NOT NULL,
		[SONo] [nvarchar](255) NOT NULL,
		[PONo] [nvarchar](255) NOT NULL,
		[Color] [nvarchar](255) NOT NULL,
		[Part] [nvarchar](255) NOT NULL,
		[Size] [nvarchar](255) NOT NULL,
		[Qty] [int] NOT NULL,
		[UserID] [int] NOT NULL,
		[FactoryID] [int] NOT NULL,
		[BatchNumber] varchar(255) NOT NULL,
		[QCRequestID] bigint NOT NULL,
		[TabletID] nvarchar(500) NOT NULL,
		[WFXColorCode] nvarchar(100) NOT NULL,
		[WFXColorName] nvarchar(100) NOT NULL,
	)
	ALTER TABLE tbl_OrderIssue ADD CONSTRAINT PK_tbl_OrderIssue_OrderIssueId PRIMARY KEY (OrderIssueId)
END
GO
IF  EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'WFXStockGRNID' AND OBJECT_ID = OBJECT_ID(N'tbl_OrderIssue'))
	ALTER TABLE tbl_OrderIssue ALTER COLUMN WFXStockGRNID bigint;	
