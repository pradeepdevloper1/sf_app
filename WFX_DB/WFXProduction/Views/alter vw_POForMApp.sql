

ALTER VIEW [dbo].[vw_POForMApp]
AS
SELECT        Module, SONo, PONo, Style, Fit, Product, Season, Customer, PlanStDt, ExFactory, PrimaryPart, '-' Hexcode, '-' Fabric, SUM(POQty) AS POQty, OrderStatus, UserID, FactoryID, EntryDate, '-' AS Line
FROM            dbo.tbl_Orders
GROUP BY Module, SONo, PONo, Style, Fit, Product, Season, Customer, PlanStDt, ExFactory, PrimaryPart,   OrderStatus, UserID, FactoryID, EntryDate
HAVING        (PrimaryPart = 1) 
GO


