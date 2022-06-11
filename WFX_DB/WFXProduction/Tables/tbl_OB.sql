IF NOT EXISTS(Select top 1 1 from sys.tables where name='tbl_OB')
Begin
	CREATE TABLE [dbo].[tbl_OB](
		[OBID] [bigint] NOT NULL,
		[PONo] [nvarchar](255) NOT NULL,
		[SNo] [int] NOT NULL,
		[OperationCode] [nvarchar](255) NOT NULL,
		[OperationName] [nvarchar](255) NOT NULL,
		[Section] [nvarchar](255) NOT NULL,
		[SMV] [float] NOT NULL,
		[UserID] [int] NOT NULL,
		[EntryDate] [datetime] NOT NULL)
		
	ALTER TABLE tbl_OB ADD CONSTRAINT PK_tbl_OB_OBID PRIMARY KEY (OBID)

End
IF NOT EXISTS (SELECT TOP 1 1 FROM syscolumns WHERE id = OBJECT_ID('tbl_OB') AND name = 'FactoryID')
    Alter table tbl_OB ADD FactoryID int NULL

IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.COLUMNS WHERE NAME = N'SONo' AND OBJECT_ID = OBJECT_ID(N'tbl_OB'))
	ALTER TABLE tbl_OB ADD SONo [nvarchar](255)  NULL	

IF EXISTS(SELECT TOP 1 1 FROM SYS.COLUMNS WHERE NAME = N'PONo' AND OBJECT_ID = OBJECT_ID(N'tbl_OB'))
	ALTER TABLE tbl_OB ALTER COLUMN PONo [nvarchar](255) NULL	