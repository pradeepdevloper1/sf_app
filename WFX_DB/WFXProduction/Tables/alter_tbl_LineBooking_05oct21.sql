alter table tbl_LineBooking add FactoryID int 
update tbl_LineBooking set  FactoryID  = 1 
alter table tbl_LineBooking alter column FactoryID int  not null