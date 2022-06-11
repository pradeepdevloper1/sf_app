IF EXISTS (SELECT TOP 1 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'dbo.vw_Factory') AND TYPE = N'V')
	Drop  View [dbo].[vw_Factory]
Go
Create View [dbo].[vw_Factory]
AS
SELECT  dbo.tbl_Factory.FactoryID, dbo.tbl_Factory.FactoryName, dbo.tbl_Factory.FactoryAddress, dbo.tbl_Factory.FactoryType, dbo.tbl_Factory.FactoryTimeZone, dbo.tbl_Factory.NoOfShifts, 
		dbo.tbl_Factory.NoOfUsers, dbo.tbl_Organisations.OrganisationName, dbo.tbl_Clusters.ClusterName, '-' AS FactoryStatus, dbo.tbl_Factory.FactoryCountry, dbo.tbl_Factory.ClusterID, 
		dbo.tbl_Organisations.OrganisationID, dbo.tbl_Factory.NoOfLine, dbo.tbl_Factory.SmartLines,dbo.tbl_Factory.LinkedwithERP
FROM    dbo.tbl_Factory 
INNER JOIN   dbo.tbl_Clusters ON dbo.tbl_Factory.ClusterID = dbo.tbl_Clusters.ClusterID 
INNER JOIN   dbo.tbl_Organisations ON dbo.tbl_Clusters.OrganisationID = dbo.tbl_Organisations.OrganisationID
GO


