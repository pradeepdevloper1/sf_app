if not exists(Select Top 1 1 from information_schema.tables where Table_Name='tbl_UserRole')
Begin
	
CREATE TABLE [dbo].[tbl_UserRole](
	[UserRoleID] [int] NOT NULL,
	[UserRole] [int] NOT NULL
	)
	--Alter Table tbl_UserRole add constraint PK_tbl_UserRole_UserRoleID Primary Key (UserRoleID)
End
if not exists(select Top 1 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE='PRIMARY KEY' and TABLE_NAME='tbl_UserRole')
Begin
	Alter Table tbl_UserRole add constraint PK_tbl_UserRole_UserRoleID Primary Key (UserRoleID)
End

IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'UserRoleType' AND OBJECT_ID = OBJECT_ID(N'tbl_UserRole'))
Begin
	ALTER table tbl_UserRole add UserRoleType int NULL	
END
GO


