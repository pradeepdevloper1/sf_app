IF NOT EXISTS(SELECT TOP 1 1 FROM sys.columns WHERE Name = N'UserRoleType' AND OBJECT_ID = OBJECT_ID(N'tbl_UserRole'))
	ALTER table tbl_UserRole add UserRoleType int  Null		
GO

