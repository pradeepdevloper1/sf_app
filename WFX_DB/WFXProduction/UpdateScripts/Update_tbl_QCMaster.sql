IF EXISTS (SELECT Top 1 1  FROM tbl_QCMaster where QCRequestID is Null) 
BEGIN
	Update tbl_QCMaster set QCRequestID = 0  Where QCRequestID  is Null
END

IF EXISTS (SELECT Top 1 1 FROM tbl_QCMaster where TabletID is Null) 
BEGIN
	UPDATE tbl_QCMaster 
	SET tbl_QCMaster.TabletID = tbl_Lines.TabletID
	FROM tbl_QCMaster
	INNER JOIN tbl_Lines
	on tbl_QCMaster.Line = tbl_Lines.LineName
	WHERE tbl_QCMaster.TabletID is Null
END

IF EXISTS (SELECT Top 1 1 FROM tbl_QCMaster where WFXColorCode is Null) 
BEGIN
	UPDATE tbl_QCMaster SET WFXColorCode = '' where WFXColorCode is Null
END

IF EXISTS (SELECT Top 1 1 FROM tbl_QCMaster where WFXColorName is Null) 
BEGIN
	UPDATE tbl_QCMaster SET WFXColorName = '' where WFXColorName is Null
END