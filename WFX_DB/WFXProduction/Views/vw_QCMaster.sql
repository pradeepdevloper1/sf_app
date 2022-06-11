IF EXISTS (SELECT TOP 1 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'dbo.vw_QCMaster') AND TYPE = N'V')
	Drop  View [dbo].[vw_QCMaster]
Go
CREATE VIEW [dbo].[vw_QCMaster]
AS
SELECT        b.FactoryID, b.Module, b.ProcessName, a.Line, b.SONo, b.PONo, b.Style, b.Fit, b.Product, a.Color, 
                         b.Customer, a.QCDate, a.Qty, a.QCMasterId, a.ShiftName, c.ShiftStartTime, c.ShiftEndTime, 
                         d.UserFirstName
FROM tbl_QCMaster a
JOIN tbl_Orders b on b.FactoryID=a.FactoryID and b.PONo=a.PONo and b.Color=a.Color
JOIN tbl_Shift c on c.FactoryID=a.FactoryID and c.ShiftName=a.ShiftName
JOIN tbl_Users d on d.UserID=a.UserID
GO

