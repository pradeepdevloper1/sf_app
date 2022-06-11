alter table tbl_Modules add FactoryID int 
alter table tbl_Modules add CreatedDate datetime 
alter table tbl_Modules add UpdatedDate datetime 
update tbl_Modules set  FactoryID  = 1 
update tbl_Modules set  CreatedDate  =GETDATE()
update tbl_Modules set  UpdatedDate  = GETDATE()
alter table tbl_Modules alter column FactoryID int  not null