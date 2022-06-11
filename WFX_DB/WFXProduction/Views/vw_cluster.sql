USE [WFX]
GO

/****** Object:  View [dbo].[vw_Culster]    Script Date: 6/21/2021 11:31:59 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



Create VIEW [dbo].[vw_Culster]
AS
SELECT        dbo.tbl_Clusters.ClusterID, dbo.tbl_Clusters.OrganisationID, dbo.tbl_Clusters.ClusterName, dbo.tbl_Organisations.OrganisationName, dbo.tbl_Clusters.ClusterHead, dbo.tbl_Clusters.ClusterEmail, 
                         dbo.tbl_Clusters.ClusterRegion,dbo.fn_factorycount(dbo.tbl_Clusters.ClusterID) as FactoryCount
FROM            dbo.tbl_Organisations INNER JOIN
                         dbo.tbl_Clusters ON dbo.tbl_Organisations.OrganisationID = dbo.tbl_Clusters.OrganisationID
GO


