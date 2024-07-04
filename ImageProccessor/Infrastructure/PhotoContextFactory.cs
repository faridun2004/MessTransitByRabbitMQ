using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace ImageProccessor.Infrastructure
{
    public class PhotoContextFactory : IDesignTimeDbContextFactory<PhotoContext>
    {
        public PhotoContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PhotoContext>();
            var connectionString = configuration.GetConnectionString("ConnectionString");
            optionsBuilder.UseSqlServer(connectionString);

            return new PhotoContext(optionsBuilder.Options);
        }
    }
}
