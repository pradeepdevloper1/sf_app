
if not exists(Select Top 1 1 from information_schema.tables where Table_Name='tbl_UserModules')
Begin
	
CREATE TABLE [dbo].tbl_UserModules(
	[UserModulesID] [int] identity(1,1),
	[UserID] [int] NOT NULL,
	[FactoryID] [int] NULL,
	[ModuleID] [int]  NULL,
	[createdOn] datetime  NULL

	)	
End

if not exists(select Top 1 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE='PRIMARY KEY' and TABLE_NAME='tbl_UserModules')
Begin
	Alter Table tbl_UserModules add constraint PK_tbl_UserModules_UserModulesID Primary Key (UserModulesID)
End
IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'ModuleName' AND OBJECT_ID = OBJECT_ID(N'tbl_UserModules'))
	ALTER table tbl_UserModules add ModuleName [nvarchar](255)	
GO

