IF NOT EXISTS(Select top 1 1 from sys.tables where name='tbl_translation')
Begin
	create TABLE [dbo].tbl_translation(
		[TranslationID] [int] NOT NULL identity(1,1),
		[ObjectName] [nvarchar](255) NOT NULL,
		[ObjectKey] [nvarchar](255) NOT NULL,
		[TranslatedString] [nvarchar](255) NOT NULL,
		[CreatedOn] [datetime] NULL)

	ALTER TABLE tbl_translation ADD CONSTRAINT PK_tbl_translation_TranslationID PRIMARY KEY (TranslationID)

End