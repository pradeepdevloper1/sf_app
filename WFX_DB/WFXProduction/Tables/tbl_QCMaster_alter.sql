ALTER TABLE tbl_QCMaster
ADD ShiftName NVARCHAR(255)

UPDATE tbl_QCMaster SET ShiftName = 'S1'