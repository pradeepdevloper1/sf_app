using System; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design; 
using System.Reflection;

namespace XF.APP.DATA
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
    {
        public ApplicationContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationContext>();
            builder.UseSqlite("Server=(LocalDB)\\MSSQLLocalDB;Database=DbE2ESiddhaSocietyDev;Trusted_Connection=True;",
                optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(ApplicationContext).GetTypeInfo().Assembly.GetName().Name));

            return new ApplicationContext(builder.Options);
        }
    }
}
