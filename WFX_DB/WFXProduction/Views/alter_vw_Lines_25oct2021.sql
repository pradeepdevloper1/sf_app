USE [WFX]
GO

/****** Object:  View [dbo].[vw_Lines]    Script Date: 25-10-2021 09:05:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[vw_Lines]
AS
SELECT        dbo.tbl_Lines.LineID, dbo.tbl_Lines.ModuleID, dbo.tbl_Lines.LineName, dbo.tbl_Lines.InternalLineName, dbo.tbl_Lines.NoOfMachine, dbo.tbl_Lines.LineCapacity, dbo.tbl_Lines.LineloadingPoint, dbo.tbl_Lines.TabletID, 
                         dbo.tbl_Factory.FactoryID, dbo.tbl_Factory.FactoryName, dbo.tbl_Lines.ModuleName
FROM            dbo.tbl_Factory INNER JOIN
                         dbo.tbl_Lines ON dbo.tbl_Factory.FactoryID = dbo.tbl_Lines.FactoryID
GO


