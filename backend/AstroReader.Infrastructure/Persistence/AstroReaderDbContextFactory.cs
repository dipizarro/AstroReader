using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AstroReader.Infrastructure.Persistence;

public class AstroReaderDbContextFactory : IDesignTimeDbContextFactory<AstroReaderDbContext>
{
    private const string ConnectionStringEnvironmentVariable = "ConnectionStrings__AstroReaderDb";
    private const string DefaultConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=AstroReaderDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    public AstroReaderDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AstroReaderDbContext>();
        var connectionString = Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariable);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = DefaultConnectionString;
        }

        optionsBuilder.UseSqlServer(connectionString, sql =>
        {
            sql.MigrationsAssembly(typeof(AstroReaderDbContext).Assembly.FullName);
            sql.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null);
        });

        return new AstroReaderDbContext(optionsBuilder.Options);
    }
}
