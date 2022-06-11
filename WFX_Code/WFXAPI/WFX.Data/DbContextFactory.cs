using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WFX.Data
{
    public class DbContextFactory 
    {
        public DBContext Create()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configBuilder.AddJsonFile("appsettings.json");
            var connectionStringConfig = configBuilder.Build();

            var builder = new DbContextOptionsBuilder<DBContext>();
            builder.UseSqlServer(connectionStringConfig.GetConnectionString("ERPConnection"));
            return new DBContext(builder.Options);
        }

    }
}
