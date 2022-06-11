IF EXISTS (SELECT TOP 1 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'dbo.vw_POList') AND TYPE = N'V')
	Drop  View [dbo].[vw_POList]
Go
CREATE VIEW [dbo].[vw_POList]
AS
SELECT OrderID, Module, SONo, PONo, Style, Fit, Product, Season, Customer, PlanStDt, ExFactory, PrimaryPart, Part, Color, Hexcode, Fabric, OrderRemark, IsSizeRun, POQty, SizeList, OrderStatus, UserID, FactoryID, EntryDate, 
        '-' AS Line,ISNULL(Source,'') Source,ISNULL(WFXColorCode,'') WFXColorCode,ISNULL(WFXColorName,'') WFXColorName,ISNULL(ProcessCode,'') ProcessCode, ISNULL(ProcessName,'') ProcessName,
        ISNULL(FulfillmentType,'') FulfillmentType
FROM  dbo.tbl_Orders
GO
