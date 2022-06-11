IF NOT EXISTS(Select top 1 1 from sys.tables where name='tbl_OBFileUploadData')
Begin
	CREATE TABLE [dbo].[tbl_OBFileUploadData](
		[OBFileUploadDataID] [bigint] NOT NULL,
		[OBFileUploadID] [bigint] NOT NULL,
		[OperationCode] [nvarchar](255) NOT NULL,
		[OperationName] [nvarchar](255) NOT NULL,
		[Section] [nvarchar](255) NOT NULL,
		[SMV] [float] NOT NULL,
		[UserID] [int] NOT NULL,
		[CreatedOn] [datetime] NOT NULL,
		[FactoryID] [int] NOT NULL
		)		
	ALTER TABLE tbl_OBFileUploadData ADD CONSTRAINT PK_tbl_OBFileUploadData_OBFileUploadDataID PRIMARY KEY (OBFileUploadDataID)
	CREATE INDEX tbl_OBFileUploadData_OBFileUploadID ON tbl_OBFileUploadData (FactoryID,OBFileUploadID);
	CREATE INDEX tbl_OBFileUploadData_OperationCode ON tbl_OBFileUploadData (FactoryID,OperationCode);
End
