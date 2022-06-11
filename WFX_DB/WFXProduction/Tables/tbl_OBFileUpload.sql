IF NOT EXISTS(Select top 1 1 from sys.tables where name='tbl_OBFileUpload')
Begin
	CREATE TABLE [dbo].[tbl_OBFileUpload](
		[OBFileUploadID] [bigint] NOT NULL,
		[ProductID] [int] NOT NULL,
		[ProcessCode] [nvarchar](255) NOT NULL,
		[FileName] [nvarchar](255) NOT NULL,
		[FilePath] [nvarchar](255) NOT NULL,	
		[FactoryID] int NOT NULL,
		[UserID] [int] NOT NULL,
		[CreatedOn] [datetime] NOT NULL
	)
	ALTER TABLE tbl_OBFileUpload ADD CONSTRAINT PK_tbl_OBFileUpload_OBFileUploadID PRIMARY KEY (OBFileUploadID)
	CREATE INDEX tbl_OBFileUpload_ProductID ON tbl_OBFileUpload (FactoryID,ProductID);
	CREATE INDEX tbl_OBFileUpload_ProcessCode ON tbl_OBFileUpload (FactoryID,ProcessCode);
END



