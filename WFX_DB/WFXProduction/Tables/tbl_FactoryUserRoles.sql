
if not exists(Select Top 1 1 from information_schema.tables where Table_Name='tbl_FactoryUserRoles')
Begin
	
CREATE TABLE [dbo].tbl_FactoryUserRoles(
	[FactoryUserRolesID] [int] identity(1,1),
	[UserID] [int] NOT NULL,	
	[FactoryID] [int] NOT NULL,
	[UserRoleID] [int] NULL,
	[createdOn] [datetime]  NULL
	)	
End

if not exists(select Top 1 1 from INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE='PRIMARY KEY' and TABLE_NAME='tbl_FactoryUserRoles')
Begin
	Alter Table tbl_FactoryUserRoles add constraint PK_tbl_FactoryUserRoles_FactoryUserRolesID Primary Key (FactoryUserRolesID)
End
IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'UserRole' AND OBJECT_ID = OBJECT_ID(N'tbl_FactoryUserRoles'))
	ALTER table tbl_FactoryUserRoles add UserRole [nvarchar](255)	
GO



if  NOT exists(SELECT t1.UserID FROM tbl_FactoryUserRoles t1 LEFT JOIN tbl_Users t2 ON t2.UserID = t1.UserID WHERE t2.UserID IS NULL)
Begin
	insert into tbl_FactoryUserRoles(UserID,FactoryID,UserRoleID,createdOn)
	select userID,factoryID,userRoleID ,GETDATE()  from tbl_Users
End

GO
BEGIN
	WITH CTE([UserID], 
			 [FactoryID], 
		     [UserRoleID], 
    duplicatecount)
    AS (SELECT [UserID], 
           [FactoryID], 
           [UserRoleID], 
           ROW_NUMBER() OVER(PARTITION BY [UserID], 
										  [FactoryID], 
										  [UserRoleID]
           ORDER BY FactoryUserRolesID) AS DuplicateCount
    FROM tbl_FactoryUserRoles)
    DELETE FROM CTE  where duplicatecount>1
END

