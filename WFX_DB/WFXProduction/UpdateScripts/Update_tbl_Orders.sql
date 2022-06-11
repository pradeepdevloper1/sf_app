Update tbl_Orders set LastSyncedAt = ''  where LastSyncedAt IS NULL

IF EXISTS (SELECT Top 1 1 FROM tbl_Orders where WFXColorCode is Null) 
BEGIN
	UPDATE tbl_Orders SET WFXColorCode = '' where WFXColorCode is Null
END

IF EXISTS (SELECT Top 1 1 FROM tbl_Orders where WFXColorName is Null) 
BEGIN
	UPDATE tbl_Orders SET WFXColorName = '' where WFXColorName is Null
END