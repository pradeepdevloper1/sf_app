	
if not exists(Select Top 1 1 from information_schema.tables where Table_Name='tbl_Users')
Begin
CREATE TABLE [dbo].tbl_Users(
	[UserID] [int] NOT NULL,
	[FactoryID] [int] NOT NULL,
	[UserFirstName] [nvarchar](255) NOT NULL,
	[UserLastName] [nvarchar](255) NOT NULL,
	[UserName] [nvarchar](255) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[UserRoleID] [int] NOT NULL,
	[Members] [int] NOT NULL,
	[UserType] [nvarchar](255) NOT NULL,
	[UserEmail] [nvarchar](255) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	)	
End

if not exists(select Top 1 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE='PRIMARY KEY' and TABLE_NAME='tbl_Users')
Begin
	Alter Table tbl_Users add constraint PK_tbl_Users_UserID Primary Key (UserID)
End

if  exists(select Top 1 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE='FOREIGN KEY' and TABLE_NAME='tbl_Users' and CONSTRAINT_NAME='FK_Users_UserRoleID')
Begin
	ALTER TABLE tbl_Users DROP CONSTRAINT FK_Users_UserRoleID
End




