IF EXISTS (SELECT Top 1 1 FROM tbl_OB where SONo IS NULL) 
BEGIN
	UPDATE a SET PONo=b.PONo
	FROM tbl_OB a
	JOIN tbl_orders b 
	on b.FactoryID=a.FactoryID AND  b.SONo=a.SONo
END