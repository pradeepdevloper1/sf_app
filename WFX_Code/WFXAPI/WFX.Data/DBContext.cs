using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WFX.Entities;
using WFX.Entities.Table;

namespace WFX.Data
{
    public partial class DBContext : DbContext
    {
        public DBContext()
        {

        }

        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {

        }

        #region TableDbSet
        public virtual DbSet<tbl_Organisations> tbl_Organisations { get; set; }
        public virtual DbSet<tbl_Clusters> tbl_Clusters { get; set; }
        public virtual DbSet<tbl_Factory> tbl_Factory { get; set; }
        public virtual DbSet<tbl_Customer> tbl_Customer { get; set; }
        public virtual DbSet<tbl_Users> tbl_Users { get; set; }
        public virtual DbSet<tbl_Modules> tbl_Modules { get; set; }
        public virtual DbSet<tbl_Lines> tbl_Lines { get; set; }
        public virtual DbSet<tbl_Orders> tbl_Orders { get; set; }
        public virtual DbSet<tbl_LineTarget> tbl_LineTarget { get; set; }
        public virtual DbSet<tbl_LineBooking> tbl_LineBooking { get; set; }
        public virtual DbSet<tbl_QCMaster> tbl_QCMaster { get; set; }
        public virtual DbSet<tbl_QCDefectDetails> tbl_QCDefectDetails { get; set; }
        public virtual DbSet<tbl_OrderStatus> tbl_OrderStatus { get; set; }
        public virtual DbSet<tbl_DailActivity> tbl_DailActivity { get; set; }
        public virtual DbSet<tbl_Operations> tbl_Operations { get; set; }
        public virtual DbSet<tbl_Defects> tbl_Defects { get; set; }
        public virtual DbSet<tbl_OB> tbl_OB { get; set; }
        public virtual DbSet<tbl_POImages> tbl_POImages { get; set; }
        public virtual DbSet<tbl_Shift> tbl_Shift { get; set; }

        public virtual DbSet<tbl_Products> tbl_Products { get; set; }
        public virtual DbSet<tbl_ProductFit> tbl_ProductFit { get; set; }
        public virtual DbSet<tbl_FactoryType> tbl_FactoryType { get; set; }
        public virtual DbSet<tbl_Country> tbl_Country { get; set; }
        public virtual DbSet<tbl_TimeZone> tbl_TimeZone { get; set; }
        public virtual DbSet<tbl_POShiftImages> tbl_POShiftImages { get; set; }
        public virtual DbSet<tbl_QCRequest> tbl_QCRequest { get; set; }
        public virtual DbSet<tbl_QCDefectImages> tbl_QCDefectImages { get; set; }
        public virtual DbSet<tbl_FactoryUserRoles> tbl_FactoryUserRoles { get; set; }
        public virtual DbSet<tbl_UserModules> tbl_UserModules { get; set; }
        public virtual DbSet<tbl_ProcessDefinition> tbl_ProcessDefinition { get; set; }
        public virtual DbSet<tbl_OrderProcess> tbl_OrderProcess { get; set; }
        public virtual DbSet<tbl_UserRole> tbl_UserRole{ get; set; }
        public virtual DbSet<tbl_OBFileUpload> tbl_OBFileUpload { get; set; }
        public virtual DbSet<tbl_OBFileUploadData> tbl_OBFileUploadData { get; set; }
        public virtual DbSet<tbl_Translation> tbl_Translation { get; set; }
        public virtual DbSet<tbl_OrderIssue> tbl_OrderIssue { get; set; }

        #endregion

        #region ViewDbSet

        #region Transaction Views
        public virtual DbSet<vw_SOView> vw_SOView { get; set; }
        public virtual DbSet<vw_OrderList> vw_OrderList { get; set; }
        public virtual DbSet<vw_QC> vw_QC { get; set; }
        public virtual DbSet<vw_POList> vw_POList { get; set; }
        public virtual DbSet<vw_POForMApp> vw_POForMApp { get; set; }
        #endregion

        // Admin
        public virtual DbSet<vw_Module> vw_Module { get; set; }
        public virtual DbSet<vw_Lines> vw_Lines { get; set; }
        public virtual DbSet<vw_Culster> vw_Culster { get; set; }
        public virtual DbSet<vw_Factory> vw_Factory { get; set; }
        public virtual DbSet<vw_Organisation> vw_Organisation { get; set; }
        public virtual DbSet<vw_ProcessDefinition>vw_ProcessDefinition { get; set; }

        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Tables
            modelBuilder.Entity<tbl_Organisations>(entity =>
            {
                entity.HasKey(e => e.OrganisationID);
            });

            modelBuilder.Entity<tbl_Clusters>(entity =>
            {
                entity.HasKey(e => e.ClusterID);

            });

            modelBuilder.Entity<tbl_Factory>(entity =>
            {
                entity.HasKey(e => e.FactoryID);
            });

            modelBuilder.Entity<tbl_Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerID);
            });

            modelBuilder.Entity<tbl_Users>(entity =>
            {
                entity.HasKey(e => e.UserID);
            });

            modelBuilder.Entity<tbl_Modules>(entity =>
            {
                entity.HasKey(e => e.ModuleID);
            });

            modelBuilder.Entity<tbl_Lines>(entity =>
            {
                entity.HasKey(e => e.LineID);
            });

            modelBuilder.Entity<tbl_Orders>(entity =>
            {
                entity.HasKey(e => e.OrderID);
            });

            modelBuilder.Entity<tbl_LineTarget>(entity =>
            {
                entity.HasKey(e => e.LineTargetID);
            });

            modelBuilder.Entity<tbl_LineBooking>(entity =>
            {
                entity.HasKey(e => e.LineBookingID);
            });

            modelBuilder.Entity<tbl_QCMaster>(entity =>
            {
                entity.HasKey(e => e.QCMasterId);
            });

            modelBuilder.Entity<tbl_QCDefectDetails>(entity =>
            {
                entity.HasKey(e => e.QCDefectDetailsId);

                entity.HasOne(e => e.tbl_QCMaster)
                    .WithMany(e => e.tbl_QCDefectDetails)
                    .HasForeignKey(e => e.QCMasterId);
            });

            modelBuilder.Entity<tbl_OrderStatus>(entity =>
            {
                entity.HasKey(e => e.OrdrStatusID);
            });

            modelBuilder.Entity<tbl_DailActivity>(entity =>
            {
                entity.HasKey(e => e.DailActivityID);
            });
            modelBuilder.Entity<tbl_Products>(entity =>
            {
                entity.HasKey(e => e.ProductID);
            });

            modelBuilder.Entity<tbl_ProductFit>(entity =>
            {
                entity.HasKey(e => e.ProductFitID);
            });

            modelBuilder.Entity<tbl_Operations>(entity =>
            {
                entity.HasKey(e => e.OperationID);
            });

            modelBuilder.Entity<tbl_Defects>(entity =>
            {
                entity.HasKey(e => e.DefectID);
            });

            modelBuilder.Entity<tbl_OB>(entity =>
            {
                entity.HasKey(e => e.OBID);
            });

            modelBuilder.Entity<tbl_POImages>(entity =>
            {
                entity.HasKey(e => e.POImageID);
            });

            modelBuilder.Entity<tbl_Shift>(entity =>
            {
                entity.HasKey(e => e.ShiftID);
            });

            modelBuilder.Entity<tbl_Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerID);
            });

            modelBuilder.Entity<tbl_FactoryType>(entity =>
            {
                entity.HasKey(e => e.FactorytypeID);
            });

            modelBuilder.Entity<tbl_Country>(entity =>
            {
                entity.HasKey(e => e.CountryID);
            });
            modelBuilder.Entity<tbl_TimeZone>(entity =>
            {
                entity.HasKey(e => e.TimeZoneID);
            });

            modelBuilder.Entity<tbl_POShiftImages>(entity =>
            {
                entity.HasKey(e => e.POShiftImageID);
            });
            modelBuilder.Entity<tbl_FactoryUserRoles>(entity =>
            {
                entity.HasKey(e => e.FactoryUserRolesID);

                entity.HasOne(e => e.tbl_Users)
                    .WithMany(e => e.tbl_FactoryUserRoles)
                    .HasForeignKey(e => e.UserID);
            });
            modelBuilder.Entity<tbl_UserModules>(entity =>
            {
                entity.HasKey(e => e.UserModulesID);

                entity.HasOne(e => e.tbl_Users)
                    .WithMany(e => e.tbl_UserModules)
                    .HasForeignKey(e => e.UserID);
            });
            modelBuilder.Entity<tbl_ProcessDefinition>(entity =>
            {
                entity.HasKey(e => e.ProcessDefinitionID);
            });
            modelBuilder.Entity<tbl_UserRole>(entity =>
            {
                entity.HasKey(e => e.UserRoleID);
            });
            modelBuilder.Entity<tbl_OBFileUpload>(entity =>
            {
                entity.HasKey(e => e.OBFileUploadID);
            });
            modelBuilder.Entity<tbl_OBFileUploadData>(entity =>
            {
                entity.HasKey(e => e.OBFileUploadDataID);
            });
            modelBuilder.Entity<tbl_Translation>(entity =>
            {
                entity.HasKey(e => e.TranslationID);
            });


            #endregion

            #region Views

            #region Transaction
            modelBuilder.Entity<vw_SOView>(entity =>
            {
                entity.HasKey(e => e.SONo);
            });

            modelBuilder.Entity<vw_OrderList>(entity =>
            {
                entity.HasKey(e => e.PONo);
            });

            modelBuilder.Entity<vw_QC>(entity =>
            {
                entity.HasKey(e => e.QCDefectDetailsId);
            });

            modelBuilder.Entity<vw_POList>(entity =>
            {
                entity.HasKey(e => e.OrderID);
            });

            modelBuilder.Entity<vw_POForMApp>(entity =>
            {
                entity.HasKey(e => e.PONo);
            });
            #endregion

            // Admin
            modelBuilder.Entity<vw_Lines>(entity =>
            {
                entity.HasKey(e => e.LineID);
            });

            modelBuilder.Entity<vw_Module>(entity =>
            {
                entity.HasKey(e => e.ModuleID);
            });
            modelBuilder.Entity<vw_ProcessDefinition>(entity =>
            {
                entity.HasKey(e => e.ProcessDefinitionID);
            });

            modelBuilder.Entity<vw_Culster>(entity =>
            {
                entity.HasKey(e => e.ClusterID);
            });

            modelBuilder.Entity<vw_Factory>(entity =>
            {
                entity.HasKey(e => e.FactoryID);
            });
            modelBuilder.Entity<vw_Organisation>(entity =>
            {
                entity.HasKey(e => e.OrganisationID);
            });
            modelBuilder.Entity<vw_QCMaster>(entity =>
            {
                entity.HasKey(e => e.QCMasterId);
            });
            modelBuilder.Entity<tbl_QCRequest>(entity =>
            {
                entity.HasKey(e => e.QCRequestID);
            });
            modelBuilder.Entity<tbl_QCDefectImages>(entity =>
            {
                entity.HasKey(e => e.QCDefectImagesID);
            });
            modelBuilder.Entity<tbl_OrderProcess>(entity =>
            {
                entity.HasKey(e => e.OrderProcessID);
            });
            modelBuilder.Entity<tbl_OrderIssue>(entity =>
            {
                entity.HasKey(e => e.OrderIssueId);
            });
            #endregion



            OnModelCreatingPartial(modelBuilder);

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public virtual DbSet<vw_QCMaster> vw_QCMaster { get; set; }
    }
}
