USE [WFX]
GO

/****** Object:  View [dbo].[vw_Organisation]    Script Date: 6/21/2021 11:33:09 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER VIEW [dbo].[vw_Organisation]
AS
SELECT        OrganisationID, OrganisationName, OrganisationAddress, OrganisationLogoPath, dbo.fn_clustercount(OrganisationID) AS ClusterCount,1 as FactoryCount
FROM            dbo.tbl_Organisations
GO


