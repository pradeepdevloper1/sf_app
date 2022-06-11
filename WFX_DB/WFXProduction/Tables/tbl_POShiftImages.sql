if not exists(Select Top 1 1 from information_schema.tables where Table_Name='tbl_POShiftImages')
Begin
	CREATE TABLE [dbo].[tbl_POShiftImages](
	[POShiftImageID] [bigint] NOT NULL,
	[PONo] [nvarchar](255) NOT NULL,
	[ImageName] [nvarchar](255) NOT NULL,
	[ImagePath] [nvarchar](255) NOT NULL,
	[ShiftName] [nvarchar](255) NOT NULL,
	[FactoryID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[EntryDate] [datetime] NOT NULL
	)
End

if not exists(select Top 1 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE='PRIMARY KEY' and TABLE_NAME='tbl_POShiftImages')
Begin
	Alter Table tbl_POImagtbl_POShiftImageses add constraint PK_tbl_POShiftImages_POShiftImageID Primary Key (POShiftImageID)
End
IF NOT EXISTS(SELECT TOP 1 1 FROM SYS.COLUMNS WHERE NAME = N'SONo' AND OBJECT_ID = OBJECT_ID(N'tbl_POShiftImages'))
	ALTER TABLE tbl_POShiftImages ADD SONo [nvarchar](255)  NULL	
	
