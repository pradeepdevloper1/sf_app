IF Not EXISTS (SELECT Top 1 1 FROM tbl_OrderProcess where ProcessEnabled is Not Null) 
BEGIN
	UPDATE tbl_OrderProcess SET ProcessEnabled = 1 where ProcessEnabled is Null
END

