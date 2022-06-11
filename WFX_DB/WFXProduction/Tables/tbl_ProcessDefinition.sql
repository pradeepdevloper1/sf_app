IF NOT EXISTS(Select top 1 1 from sys.tables where name='tbl_ProcessDefinition')
Begin
	create TABLE [dbo].[tbl_ProcessDefinition](
		[ProcessDefinitionID] [int] NOT NULL,
		[ProcessCode] [nvarchar](255) NOT NULL,
		[ProcessName] [nvarchar](255) NOT NULL,
		[ProcessType] [nvarchar](255),
		[FactoryID] [int] NOT NULL,
		[CreatedOn] [datetime] NULL,
		[LastChangedOn] [datetime] NULL,)

	ALTER TABLE tbl_ProcessDefinition ADD CONSTRAINT PK_tbl_ProcessDefinition_ProcessDefinitionID PRIMARY KEY (ProcessDefinitionID)
Create Index tbl_ProcessDefinition_ProcessCode on  tbl_ProcessDefinition(FactoryId,ProcessCode)
Create Index tbl_ProcessDefinition_ProcessType on  tbl_ProcessDefinition(FactoryId,ProcessType)

End

