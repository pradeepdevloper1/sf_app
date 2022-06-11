if Exists(Select Top 1 1 From tbl_UserRole Where UserRole='QC')
BEGIN
	declare @ID int
	Update tbl_UserRole set UserRole='Inline QC' Where UserRole='QC'
	Update tbl_UserRole set UserRoleType = UserRoleID

	Insert into tbl_UserRole([UserRoleID], [UserRole], [UserRoleType])
	Select 4, 'Endline QC',3
	UNION
	Select 5, 'Floating QC',3
	UNION
	Select 6, 'Cutting QC',3
	UNION
	Select 7, 'Endline QC',3
	UNION
	Select 8, 'Embroidery QC',3
	UNION
	Select 9, 'Washing QC',3
	UNION
	Select 10, 'Finishing QC',3
	UNION
	Select 11, 'Packing QC',3
	UNION
	Select 12, 'Outsourcing QC',3
END
ELSE
BEGIN
	Insert into tbl_UserRole([UserRoleID], [UserRole], [UserRoleType])
	Select 1, N'Admin', 1
	UNION
	Select 2, N'Planner', 2
	UNION
	Select 3, N'Inline QC', 3
	UNION
	Select 4, N'Endline QC',3
	UNION
	Select 5, N'Floating QC',3
	UNION
	Select 6, N'Cutting QC',3
	UNION
	Select 7, N'Endline QC',3
	UNION
	Select 8, N'Embroidery QC',3
	UNION
	Select 9, N'Washing QC',3
	UNION
	Select 10, N'Finishing QC',3
	UNION
	Select 11, N'Packing QC',3
	UNION
	Select 12, N'Outsourcing QC',3
END