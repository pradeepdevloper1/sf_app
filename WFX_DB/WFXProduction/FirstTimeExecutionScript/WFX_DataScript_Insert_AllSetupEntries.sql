USE [WFX]
GO
INSERT [dbo].[tbl_Organisations] ([OrganisationID], [OrganisationName], [OrganisationAddress], [OrganisationLogoPath]) VALUES (1, N'O1', N'-', N'-')
GO
INSERT [dbo].[tbl_Organisations] ([OrganisationID], [OrganisationName], [OrganisationAddress], [OrganisationLogoPath]) VALUES (2, N'O2', N'-', N'-')
GO
INSERT [dbo].[tbl_Organisations] ([OrganisationID], [OrganisationName], [OrganisationAddress], [OrganisationLogoPath]) VALUES (3, N'O3', N'-', N'-')
GO
INSERT [dbo].[tbl_Clusters] ([ClusterID], [OrganisationID], [ClusterName], [ClusterHead], [ClusterEmail], [ClusterRegion]) VALUES (1, 1, N'C1', N'-', N'-', N'-')
GO
INSERT [dbo].[tbl_Clusters] ([ClusterID], [OrganisationID], [ClusterName], [ClusterHead], [ClusterEmail], [ClusterRegion]) VALUES (2, 1, N'C2', N'-', N'-', N'-')
GO
INSERT [dbo].[tbl_Clusters] ([ClusterID], [OrganisationID], [ClusterName], [ClusterHead], [ClusterEmail], [ClusterRegion]) VALUES (3, 1, N'C3', N'-', N'-', N'-')
GO
INSERT [dbo].[tbl_Clusters] ([ClusterID], [OrganisationID], [ClusterName], [ClusterHead], [ClusterEmail], [ClusterRegion]) VALUES (4, 2, N'C4', N'-', N'-', N'-')
GO
INSERT [dbo].[tbl_Clusters] ([ClusterID], [OrganisationID], [ClusterName], [ClusterHead], [ClusterEmail], [ClusterRegion]) VALUES (5, 2, N'C5', N'-', N'-', N'-')
GO
INSERT [dbo].[tbl_Clusters] ([ClusterID], [OrganisationID], [ClusterName], [ClusterHead], [ClusterEmail], [ClusterRegion]) VALUES (6, 3, N'C6', N'-', N'-', N'-')
GO
INSERT [dbo].[tbl_Factory] ([FactoryID], [ClusterID], [FactoryName], [FactoryAddress], [FactoryType], [FactoryHead], [FactoryEmail], [FactoryContactNumber], [FactoryCountry], [FactoryTimeZone], [NoOfShifts], [DecimalValue], [PTMPrice], [NoOfUsers], [FactoryOffOn], [MeasuringUnit], [DataScale]) VALUES (1, 1, N'F1', N'-', N'-', N'-', N'-', 0, N'-', N'-', 3, 2, 2, 10, N'-', N'-', 1)
GO
INSERT [dbo].[tbl_Factory] ([FactoryID], [ClusterID], [FactoryName], [FactoryAddress], [FactoryType], [FactoryHead], [FactoryEmail], [FactoryContactNumber], [FactoryCountry], [FactoryTimeZone], [NoOfShifts], [DecimalValue], [PTMPrice], [NoOfUsers], [FactoryOffOn], [MeasuringUnit], [DataScale]) VALUES (2, 2, N'F2', N'-', N'-', N'-', N'-', 0, N'-', N'-', 3, 2, 2, 10, N'-', N'-', 1)
GO
INSERT [dbo].[tbl_Factory] ([FactoryID], [ClusterID], [FactoryName], [FactoryAddress], [FactoryType], [FactoryHead], [FactoryEmail], [FactoryContactNumber], [FactoryCountry], [FactoryTimeZone], [NoOfShifts], [DecimalValue], [PTMPrice], [NoOfUsers], [FactoryOffOn], [MeasuringUnit], [DataScale]) VALUES (3, 3, N'F3', N'-', N'-', N'-', N'-', 0, N'-', N'-', 3, 2, 2, 10, N'-', N'-', 1)
GO
INSERT [dbo].[tbl_Factory] ([FactoryID], [ClusterID], [FactoryName], [FactoryAddress], [FactoryType], [FactoryHead], [FactoryEmail], [FactoryContactNumber], [FactoryCountry], [FactoryTimeZone], [NoOfShifts], [DecimalValue], [PTMPrice], [NoOfUsers], [FactoryOffOn], [MeasuringUnit], [DataScale]) VALUES (4, 4, N'F4', N'-', N'-', N'-', N'-', 0, N'-', N'-', 3, 2, 2, 10, N'-', N'-', 1)
GO
INSERT [dbo].[tbl_Factory] ([FactoryID], [ClusterID], [FactoryName], [FactoryAddress], [FactoryType], [FactoryHead], [FactoryEmail], [FactoryContactNumber], [FactoryCountry], [FactoryTimeZone], [NoOfShifts], [DecimalValue], [PTMPrice], [NoOfUsers], [FactoryOffOn], [MeasuringUnit], [DataScale]) VALUES (5, 5, N'F5', N'-', N'-', N'-', N'-', 0, N'-', N'-', 3, 2, 2, 10, N'-', N'-', 1)
GO
INSERT [dbo].[tbl_Factory] ([FactoryID], [ClusterID], [FactoryName], [FactoryAddress], [FactoryType], [FactoryHead], [FactoryEmail], [FactoryContactNumber], [FactoryCountry], [FactoryTimeZone], [NoOfShifts], [DecimalValue], [PTMPrice], [NoOfUsers], [FactoryOffOn], [MeasuringUnit], [DataScale]) VALUES (6, 6, N'F6', N'-', N'-', N'-', N'-', 0, N'-', N'-', 3, 2, 2, 10, N'-', N'-', 1)
GO
INSERT [dbo].[tbl_Factory] ([FactoryID], [ClusterID], [FactoryName], [FactoryAddress], [FactoryType], [FactoryHead], [FactoryEmail], [FactoryContactNumber], [FactoryCountry], [FactoryTimeZone], [NoOfShifts], [DecimalValue], [PTMPrice], [NoOfUsers], [FactoryOffOn], [MeasuringUnit], [DataScale]) VALUES (7, 6, N'F7', N'-', N'-', N'-', N'-', 0, N'-', N'-', 3, 2, 2, 10, N'-', N'-', 1)
GO
INSERT [dbo].[tbl_Departmnents] ([DepartmentID], [FactoryID], [DepartmentName], [DepartmentHead], [DepartmentEmail]) VALUES (1, 1, N'Stitching', N'-', N'-')
GO
INSERT [dbo].[tbl_Products] ([ProductID], [FactoryID], [ProductName]) VALUES (1, 1, N't-shirt')
GO
INSERT [dbo].[tbl_Products] ([ProductID], [FactoryID], [ProductName]) VALUES (2, 1, N'trouser')
GO
INSERT [dbo].[tbl_Products] ([ProductID], [FactoryID], [ProductName]) VALUES (3, 1, N'jacket ')
GO
INSERT [dbo].[tbl_Products] ([ProductID], [FactoryID], [ProductName]) VALUES (4, 2, N'shirt')
GO
INSERT [dbo].[tbl_Products] ([ProductID], [FactoryID], [ProductName]) VALUES (5, 2, N'vest')
GO
INSERT [dbo].[tbl_Products] ([ProductID], [FactoryID], [ProductName]) VALUES (6, 2, N'cap')
GO
INSERT [dbo].[tbl_ProductFit] ([ProductFitID], [FactoryID], [FitType]) VALUES (1, 1, N'slim')
GO
INSERT [dbo].[tbl_ProductFit] ([ProductFitID], [FactoryID], [FitType]) VALUES (2, 1, N'regular')
GO
INSERT [dbo].[tbl_ProductFit] ([ProductFitID], [FactoryID], [FitType]) VALUES (3, 1, N'plus size ')
GO
INSERT [dbo].[tbl_ProductFit] ([ProductFitID], [FactoryID], [FitType]) VALUES (4, 1, N'petite')
GO
INSERT [dbo].[tbl_ProductFit] ([ProductFitID], [FactoryID], [FitType]) VALUES (5, 1, N'tapered')
GO
INSERT [dbo].[tbl_ProductFit] ([ProductFitID], [FactoryID], [FitType]) VALUES (6, 2, N'slim')
GO
INSERT [dbo].[tbl_ProductFit] ([ProductFitID], [FactoryID], [FitType]) VALUES (7, 2, N'regular')
GO
INSERT [dbo].[tbl_ProductFit] ([ProductFitID], [FactoryID], [FitType]) VALUES (8, 2, N'plus size ')
GO
INSERT [dbo].[tbl_ProductFit] ([ProductFitID], [FactoryID], [FitType]) VALUES (9, 2, N'petite')
GO
INSERT [dbo].[tbl_ProductFit] ([ProductFitID], [FactoryID], [FitType]) VALUES (10, 2, N'tapered')
GO
INSERT [dbo].[tbl_Customer] ([CustomerID], [FactoryID], [CustomerName]) VALUES (1, 1, N'Nike')
GO
INSERT [dbo].[tbl_Customer] ([CustomerID], [FactoryID], [CustomerName]) VALUES (2, 1, N'puma')
GO
INSERT [dbo].[tbl_Customer] ([CustomerID], [FactoryID], [CustomerName]) VALUES (3, 1, N'lacoste')
GO
INSERT [dbo].[tbl_Customer] ([CustomerID], [FactoryID], [CustomerName]) VALUES (4, 2, N'adidas')
GO
INSERT [dbo].[tbl_Customer] ([CustomerID], [FactoryID], [CustomerName]) VALUES (5, 2, N'zara')
GO
INSERT [dbo].[tbl_Users] ([UserID], [FactoryID], [UserFirstName], [UserLastName], [UserName], [Password], [UserRoleID], [Members], [UserType], [UserEmail]) VALUES (1, 1, N'Ratnesh', N'Rai', N'user1', N'user1', 1, 1, N'Production', N'-')
GO
INSERT [dbo].[tbl_Users] ([UserID], [FactoryID], [UserFirstName], [UserLastName], [UserName], [Password], [UserRoleID], [Members], [UserType], [UserEmail]) VALUES (2, 1, N'Sunil', N'Gupta', N'user2', N'user2', 2, 1, N'Production', N'-')
GO
INSERT [dbo].[tbl_Users] ([UserID], [FactoryID], [UserFirstName], [UserLastName], [UserName], [Password], [UserRoleID], [Members], [UserType], [UserEmail]) VALUES (3, 1, N'Rishi ', N'Pandey', N'user3', N'user3', 3, 1, N'Production', N'-')
GO
INSERT [dbo].[tbl_Users] ([UserID], [FactoryID], [UserFirstName], [UserLastName], [UserName], [Password], [UserRoleID], [Members], [UserType], [UserEmail]) VALUES (4, 2, N'user4', N'-', N'user4', N'user4', 1, 1, N'Production', N'-')
GO
INSERT [dbo].[tbl_Users] ([UserID], [FactoryID], [UserFirstName], [UserLastName], [UserName], [Password], [UserRoleID], [Members], [UserType], [UserEmail]) VALUES (5, 2, N'user5', N'-', N'user5', N'user5', 2, 1, N'Production', N'-')
GO
INSERT [dbo].[tbl_Users] ([UserID], [FactoryID], [UserFirstName], [UserLastName], [UserName], [Password], [UserRoleID], [Members], [UserType], [UserEmail]) VALUES (6, 2, N'user6', N'-', N'user6', N'user6', 3, 1, N'Production', N'-')
GO
INSERT [dbo].[tbl_Modules] ([ModuleID], [DepartmentID], [ModuleName]) VALUES (1, 1, N'FL1')
GO
INSERT [dbo].[tbl_Modules] ([ModuleID], [DepartmentID], [ModuleName]) VALUES (2, 1, N'FL2')
GO
INSERT [dbo].[tbl_Modules] ([ModuleID], [DepartmentID], [ModuleName]) VALUES (3, 1, N'FL3')
GO
INSERT [dbo].[tbl_Modules] ([ModuleID], [DepartmentID], [ModuleName]) VALUES (4, 1, N'FL4')
GO
INSERT [dbo].[tbl_Defects] ([DefectID], [DepartmentID], [DefectCode], [DefectName], [DefectType], [DefectLevel]) VALUES (1, 1, N'DQ01', N'wrong thread', N'Trim', N'High')
GO
INSERT [dbo].[tbl_Defects] ([DefectID], [DepartmentID], [DefectCode], [DefectName], [DefectType], [DefectLevel]) VALUES (2, 1, N'DQ02', N'skip stitch', N'Construction', N'Medium ')
GO
INSERT [dbo].[tbl_Defects] ([DefectID], [DepartmentID], [DefectCode], [DefectName], [DefectType], [DefectLevel]) VALUES (3, 1, N'DQ03', N'hole', N'Fabric', N'High')
GO
INSERT [dbo].[tbl_Defects] ([DefectID], [DepartmentID], [DefectCode], [DefectName], [DefectType], [DefectLevel]) VALUES (4, 1, N'DQ04', N'multple thread', N'Construction', N'Medium ')
GO
INSERT [dbo].[tbl_Lines] ([LineID], [ModuleID], [LineName], [InternalLineName], [NoOfMachine], [LineCapacity], [LineloadingPoint], [TabletID]) VALUES (1, 1, N'L1', N'tango', 20, 10, N'P1', N'TB1')
GO
INSERT [dbo].[tbl_Lines] ([LineID], [ModuleID], [LineName], [InternalLineName], [NoOfMachine], [LineCapacity], [LineloadingPoint], [TabletID]) VALUES (2, 1, N'L2', N'charli', 20, 10, N'P2', N'TB2')
GO
INSERT [dbo].[tbl_Lines] ([LineID], [ModuleID], [LineName], [InternalLineName], [NoOfMachine], [LineCapacity], [LineloadingPoint], [TabletID]) VALUES (3, 1, N'L3', N'alpha', 20, 10, N'P3', N'TB3')
GO
INSERT [dbo].[tbl_Lines] ([LineID], [ModuleID], [LineName], [InternalLineName], [NoOfMachine], [LineCapacity], [LineloadingPoint], [TabletID]) VALUES (4, 2, N'L4', N'bravo', 20, 10, N'P4', N'TB4')
GO
INSERT [dbo].[tbl_Lines] ([LineID], [ModuleID], [LineName], [InternalLineName], [NoOfMachine], [LineCapacity], [LineloadingPoint], [TabletID]) VALUES (5, 2, N'L5', N'krox', 20, 10, N'P5', N'TB5')
GO
INSERT [dbo].[tbl_Shift] ([ShiftID], [ModuleID], [ShiftName], [ShiftStartTime], [ShiftEndTime]) VALUES (1, 1, N'S1', CAST(N'06:00:00' AS Time), CAST(N'14:00:00' AS Time))
GO
INSERT [dbo].[tbl_Shift] ([ShiftID], [ModuleID], [ShiftName], [ShiftStartTime], [ShiftEndTime]) VALUES (2, 1, N'S2', CAST(N'14:00:00' AS Time), CAST(N'22:00:00' AS Time))
GO
INSERT [dbo].[tbl_Shift] ([ShiftID], [ModuleID], [ShiftName], [ShiftStartTime], [ShiftEndTime]) VALUES (3, 1, N'S3', CAST(N'22:00:00' AS Time), CAST(N'06:00:00' AS Time))
GO
INSERT [dbo].[tbl_Calendar] ([CalendarID], [ModuleID], [Calendar]) VALUES (1, 1, N'C1')
GO
INSERT [dbo].[tbl_Calendar] ([CalendarID], [ModuleID], [Calendar]) VALUES (2, 2, N'C2')
GO
INSERT [dbo].[tbl_Calendar] ([CalendarID], [ModuleID], [Calendar]) VALUES (3, 3, N'C3')
GO
