IF EXISTS (SELECT Top 1 1 FROM tbl_Lines where ProcessType is Null) 
BEGIN
	UPDATE tbl_Lines SET ProcessType = '' where ProcessType is Null
END

IF EXISTS (SELECT Top 1 1 FROM tbl_Lines where ModuleID = 0) 
BEGIN
	UPDATE a SET ModuleID=b.ModuleID
	FROM tbl_Lines a
	JOIN tbl_Modules b 
	on b.FactoryID=a.FactoryID AND  b.ModuleName=a.ModuleName
END
