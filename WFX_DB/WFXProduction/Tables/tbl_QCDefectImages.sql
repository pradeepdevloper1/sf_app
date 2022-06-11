IF NOT EXISTS(Select top 1 1 from sys.tables where name='tbl_QCDefectImages')
Begin
	CREATE TABLE [dbo].[tbl_QCDefectImages](
		[QCDefectImagesID] [bigint] identity(1,1),
		[QCMasterId] [bigint] NOT NULL,
		[ImageName] [nvarchar](255) NOT NULL,
		[ImagePath] [nvarchar](255) NOT NULL,
		[FactoryID] int NOT NULL,
		[UserID] [int] NOT NULL,
		[SONo] [nvarchar](255) NOT NULL,
		[PONo] [nvarchar](255) NOT NULL,
		[EntryDate] [datetime] NOT NULL
	 )
	ALTER TABLE tbl_QCDefectImages ADD CONSTRAINT PK_tbl_QCDefectImages_OrderID PRIMARY KEY (QCDefectImagesID)
END
