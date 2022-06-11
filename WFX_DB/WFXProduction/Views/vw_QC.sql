IF EXISTS (SELECT TOP 1 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'dbo.vw_QC') AND TYPE = N'V')
	Drop  View [dbo].vw_QC
Go
CREATE VIEW [dbo].[vw_QC]
AS
SELECT  a.QCMasterId, a.QCDate, a.TypeOfWork,a.PONo, a.Color, a.Part, a.Size, a.Qty, a.QCStatus, a.UserID, a.FactoryID, a.ShiftName, ISNULL(b.QCDefectDetailsId, 0) AS QCDefectDetailsId, 
        ISNULL(b.DefectType, N'-') AS DefectType, ISNULL(b.DefectName, N'-') AS DefectName, ISNULL(b.OperationName, N'-') AS OperationName, a.Module, a.Line, 
		ISNULL(a.ProcessCode,'') ProcessCode,a.SONO,ISNULL(a.ProcessName,'') ProcessName
FROM            tbl_QCMaster a LEFT OUTER JOIN
                tbl_QCDefectDetails b ON b.QCMasterId = a.QCMasterId
GO


