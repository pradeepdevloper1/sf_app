//http://www.entityframeworktutorial.net/code-first/foreignkey-dataannotations-attribute-in-code-first.aspx
//https://docs.microsoft.com/en-us/ef/core/get-started/aspnetcore/existing-db
//to update database
//https://stackoverflow.com/questions/50767086/class-library-wth-ef-core-2-1-no-executable-found-matching-command-dotnet-ef
//https://stackoverflow.com/questions/44074992/how-to-update-db-using-code-first-net-core

using Microsoft.EntityFrameworkCore; 
using System.Linq; 

namespace XF.APP.DATA
{
    public class ApplicationContext : DbContext
    {
        public string DbPath { get; set; }

        public ApplicationContext(string databasePath)
        {
            this.DbPath = databasePath;
            Database.EnsureCreated();
        }

        public ApplicationContext(DbContextOptions opts) : base(opts)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(string.Format("Filename={0}", this.DbPath));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            modelBuilder.Seed();
        }

        public DbSet<LookupKey> LookupKey { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }
    }
}
