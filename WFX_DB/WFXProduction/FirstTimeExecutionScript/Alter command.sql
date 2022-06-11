Alter table tbl_Products add  CreatedDate datetime
Alter table tbl_Products add  UpdatedDate datetime

Alter table tbl_ProductFit add  CreatedDate datetime
Alter table tbl_ProductFit add  UpdatedDate datetime

Alter table tbl_Customer add  CreatedDate datetime
Alter table tbl_Customer add  UpdatedDate datetime

Alter table tbl_Lines add  CreatedDate datetime
Alter table tbl_Lines add  UpdatedDate datetime

Alter table tbl_Shift add  CreatedDate datetime
Alter table tbl_Shift add  UpdatedDate datetime

Alter table tbl_Defects add  CreatedDate datetime
Alter table tbl_Defects add  UpdatedDate datetime

Alter table tbl_Users add  CreatedDate datetime
Alter table tbl_Users add  UpdatedDate datetime

update tbl_Products set CreatedDate=GETDATE(),UpdatedDate=GETDATE()
update tbl_ProductFit set CreatedDate=GETDATE(),UpdatedDate=GETDATE()
update tbl_Customer set CreatedDate=GETDATE(),UpdatedDate=GETDATE()
update tbl_Lines set CreatedDate=GETDATE(),UpdatedDate=GETDATE()
update tbl_Shift set CreatedDate=GETDATE(),UpdatedDate=GETDATE()
update tbl_Defects set CreatedDate=GETDATE(),UpdatedDate=GETDATE()
update tbl_Users set CreatedDate=GETDATE(),UpdatedDate=GETDATE()


alter table  tbl_Factory alter column FactoryCountry int


delete from tbl_FactoryType
delete from tbl_TimeZone
delete from tbl_Country

Alter Table tbl_Lines add  FactoryID int
Alter Table tbl_Shift add  FactoryID int
Alter Table tbl_Defects add  FactoryID int

update tbl_Lines set FactoryID=8
update tbl_Shift set FactoryID=8
update tbl_Defects set FactoryID=8


--alter on 12/July/2021

Alter Table tbl_Factory add NoOfLine int
Alter Table tbl_Factory add SmartLines int

update tbl_Factory set NoOfLine=1,SmartLines=1

ALTER TABLE tbl_Organisations
DROP CONSTRAINT IX_Organisations_OrgnisationName;

ALTER TABLE tbl_Clusters
DROP CONSTRAINT IX_Clusters_ClusterName;

ALTER TABLE tbl_Lines
DROP CONSTRAINT IX_Lines_TabletID,IX_Lines_LineName;

ALTER TABLE tbl_Defects
DROP CONSTRAINT IX_Defects_DefectCode

ALTER TABLE tbl_Users
DROP CONSTRAINT IX_Users_UserName

--alter table  tbl_Lines add ProcessType nvarchar(255)



