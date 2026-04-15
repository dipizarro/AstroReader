using AstroReader.Infrastructure.Persistence;
using AstroReader.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AstroReader.Infrastructure;

public static class DependencyInjection
{
    private const string AstroReaderConnectionStringName = "AstroReaderDb";
    private const string AzureSqlConnectionStringEnvironmentVariable = "SQLCONNSTR_AstroReaderDb";

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString(AstroReaderConnectionStringName) ??
            Environment.GetEnvironmentVariable(AzureSqlConnectionStringEnvironmentVariable);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'AstroReaderDb' was not found.");
        }

        services.AddDbContext<AstroReaderDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(AstroReaderDbContext).Assembly.FullName);
                sql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            });
        });
        services.AddScoped<AstroReader.Application.SavedCharts.Interfaces.ISavedChartRepository, SavedChartRepository>();

        return services;
    }
}
