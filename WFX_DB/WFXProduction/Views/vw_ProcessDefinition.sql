IF EXISTS (SELECT TOP 1 1 FROM SYS.OBJECTS WHERE OBJECT_ID = OBJECT_ID(N'dbo.vw_ProcessDefinition') AND TYPE = N'V')
	Drop  View [dbo].vw_ProcessDefinition
Go
CREATE VIEW [dbo].[vw_ProcessDefinition]
AS
SELECT dbo.tbl_ProcessDefinition.ProcessDefinitionID,dbo.tbl_ProcessDefinition.ProcessName,dbo.tbl_ProcessDefinition.ProcessCode,
dbo.tbl_ProcessDefinition.FactoryID, dbo.tbl_Factory.FactoryName
FROM  tbl_ProcessDefinition 
INNER JOIN tbl_Factory On tbl_Factory.FactoryID = tbl_ProcessDefinition.FactoryID 
GO

