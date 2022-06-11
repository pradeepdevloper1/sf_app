IF EXISTS (SELECT TOP 1 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'dbo.vw_SOView') AND TYPE = N'V')
	Drop  View [dbo].[vw_SOView]
Go
CREATE VIEW [dbo].[vw_SOView]
AS
SELECT        Max(Module) Module, SONo, MAX(Style) Style,MAX(Fit) Fit,MAX(Product) Product,MAX(Season) Season,MAX(Customer) Customer, SUM(POQty) AS SOQty, FactoryID, COUNT(distinct PONo) AS NoOfPO, MIN(OrderStatus) As OrderStatus
FROM            dbo.tbl_Orders
WHERE        (PrimaryPart = 1)
GROUP BY  SONo, FactoryID
GO


