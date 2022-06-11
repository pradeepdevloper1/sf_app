IF EXISTS (SELECT TOP 1 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'dbo.vw_OrderList') AND TYPE = N'V')
	Drop  View [dbo].[vw_OrderList]
Go
Create View [dbo].[vw_OrderList]
AS
SELECT  Module, SONo, PONo, Style, Fit, Product, Season, Customer, ExFactory, OrderRemark, SUM(POQty) AS POQty,
		FactoryID, OrderStatus, EntryDate, 0 AS CompletedQty,0 AS CompletedPer, MAX(OrderID) AS OrderID,
		MAX(LastSyncedAt) LastSyncedAt,ISNULL([Source],'') [Source],
		ISNULL(ProcessCode,'') ProcessCode, ISNULL(ProcessName,'') ProcessName, ISNULL(FulfillmentType,'') FulfillmentType
FROM  dbo.tbl_Orders
GROUP BY Module, SONo, PONo, Style, Fit, Product, Season, Customer, ExFactory, OrderRemark, PrimaryPart, FactoryID,
         OrderStatus, EntryDate,isnull([Source],''),ISNULL(ProcessCode,''), ISNULL(ProcessName,''), ISNULL(FulfillmentType,'')
HAVING  (PrimaryPart = 1)
GO


