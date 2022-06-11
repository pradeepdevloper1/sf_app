alter table tbl_LineTarget add FactoryID int 
update tbl_LineTarget set  FactoryID  = 1 
alter table tbl_LineTarget alter column FactoryID int  not null