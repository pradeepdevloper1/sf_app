alter table tbl_Shift 
alter column ShiftStartTime nvarchar(255)

alter table tbl_Shift 
alter column ShiftEndTime nvarchar(255)

alter table tbl_Shift 
drop constraint IX_Shift_ShiftName

alter table  tbl_Shift 
add constraint IX_Shift_FactoryID_ShiftName unique(FactoryID,ShiftName)