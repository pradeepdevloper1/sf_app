IF NOT EXISTS(Select top 1 1 from sys.tables where name='tbl_LineTarget')
Begin
	CREATE TABLE [dbo].[tbl_LineTarget](
		[LineTargetID] [bigint] NOT NULL,
		[Module] [nvarchar](255) NOT NULL,
		[Section] [nvarchar](255) NOT NULL,
		[Line] [nvarchar](255) NOT NULL,
		[Style] [nvarchar](255) NOT NULL,
		[SONo] [nvarchar](255) NOT NULL,
		[PONo] [nvarchar](255) NOT NULL,
		[Part] [nvarchar](255) NOT NULL,
		[Color] [nvarchar](255) NOT NULL,
		[SMV] [float] NOT NULL,
		[Operators] [float] NOT NULL,
		[Helpers] [float] NOT NULL,
		[ShiftName] [nvarchar](255) NOT NULL,
		[ShiftHours] [float] NOT NULL,
		[Date] [datetime] NOT NULL,
		[PlannedEffeciency] [float] NOT NULL,
		[PlannedTarget] [int] NOT NULL,
		[UserID] [int] NOT NULL,
		[EntryDate] [datetime] NOT NULL)
		ALTER TABLE tbl_LineTarget ADD CONSTRAINT PK_tbl_LineTarget_LineTargetID PRIMARY KEY (LineTargetID)
END
GO
IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'SizeList' AND OBJECT_ID = OBJECT_ID(N'tbl_LineTarget'))
	ALTER table tbl_LineTarget add SizeList nvarchar(255) not null		
GO
IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'ProcessCode' AND OBJECT_ID = OBJECT_ID(N'tbl_LineTarget'))
	ALTER table tbl_LineTarget add ProcessCode nvarchar(255) null		
GO
IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'ProcessName' AND OBJECT_ID = OBJECT_ID(N'tbl_LineTarget'))
	ALTER table tbl_LineTarget add ProcessName nvarchar(255) null		
GO


