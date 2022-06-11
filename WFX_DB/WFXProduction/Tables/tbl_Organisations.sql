IF NOT EXISTS(Select top 1 1 from sys.tables where name='tbl_Organisations')
Begin
	CREATE TABLE [dbo].[tbl_Organisations](
		[OrganisationID] [int] NOT NULL,
		[OrganisationName] [nvarchar](255) NOT NULL,
		[OrganisationAddress] [nvarchar](255) NOT NULL,
		[OrganisationLogoPath] [nvarchar](255) NOT NULL
	)
	ALTER TABLE tbl_Organisations ADD CONSTRAINT PK_tbl_Organisations_OrganisationID PRIMARY KEY (OrganisationID)
END
GO
IF NOT EXISTS (SELECT TOP 1 1 FROM SYSCOLUMNS WHERE id = OBJECT_ID('tbl_Organisations') AND name = 'ERPAPIURL')
BEGIN 
	ALTER TABLE tbl_Organisations ADD ERPAPIURL nvarchar(500) NULL	
END

