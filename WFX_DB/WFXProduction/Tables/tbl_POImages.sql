if not exists(Select Top 1 1 from information_schema.tables where Table_Name='tbl_POImages')
Begin
CREATE TABLE [dbo].tbl_POImages(
	[POImageID] [bigint] NOT NULL,
	[PONo] [nvarchar](255) NOT NULL,
	[ImageName] [nvarchar](255) NOT NULL,
	[ImagePath] [nvarchar](255) NOT NULL,
	[IMGNo] [int] NOT NULL,
	[FactoryID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[EntryDate] [datetime] NOT NULL
	)
	End

if not exists(select Top 1 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE='PRIMARY KEY' and TABLE_NAME='tbl_POImages')
Begin
	Alter Table tbl_POImages add constraint PK_tbl_POImages_POImageID Primary Key (POImageID)
End
IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.COLUMNS WHERE NAME = N'SONo' AND OBJECT_ID = OBJECT_ID(N'tbl_POImages'))
	ALTER TABLE tbl_POImages ADD SONo [nvarchar](255)  NULL	

IF EXISTS(SELECT TOP 1 1 FROM SYS.COLUMNS WHERE NAME = N'PONo' AND OBJECT_ID = OBJECT_ID(N'tbl_POImages'))
	ALTER TABLE tbl_POImages ALTER COLUMN PONo [nvarchar](255) NULL	

