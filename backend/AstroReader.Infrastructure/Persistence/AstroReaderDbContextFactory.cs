using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AstroReader.Infrastructure.Persistence;

public class AstroReaderDbContextFactory : IDesignTimeDbContextFactory<AstroReaderDbContext>
{
    private const string DefaultConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=AstroReaderDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    public AstroReaderDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AstroReaderDbContext>();

        optionsBuilder.UseSqlServer(DefaultConnectionString, sql =>
        {
            sql.MigrationsAssembly(typeof(AstroReaderDbContext).Assembly.FullName);
        });

        return new AstroReaderDbContext(optionsBuilder.Options);
    }
}
