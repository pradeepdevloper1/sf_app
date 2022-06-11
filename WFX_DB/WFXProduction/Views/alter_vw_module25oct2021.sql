USE [WFX]
GO

/****** Object:  View [dbo].[vw_Module]    Script Date: 25-10-2021 09:17:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[vw_Module]
AS
SELECT        dbo.tbl_Modules.ModuleID, dbo.tbl_Modules.ModuleName, dbo.tbl_Factory.FactoryID, dbo.tbl_Factory.FactoryName
FROM            dbo.tbl_Modules INNER JOIN
                         dbo.tbl_Factory ON dbo.tbl_Modules.FactoryID = dbo.tbl_Factory.FactoryID
GO


