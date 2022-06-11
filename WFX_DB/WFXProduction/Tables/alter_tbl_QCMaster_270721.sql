alter table tbl_QCMaster add  SONo nvarchar(255)

update tbl_QCMaster set  SONo = tbl_Orders.SONo
FROM tbl_QCMaster INNER JOIN tbl_Orders ON tbl_Orders.PONo = tbl_QCMaster.PONo

alter table tbl_QCMaster alter column  SONo nvarchar(255) not null