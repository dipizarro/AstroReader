using AstroReader.Infrastructure.Persistence;
using AstroReader.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AstroReader.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AstroReaderDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'AstroReaderDb' was not found.");
        }

        services.AddDbContext<AstroReaderDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(AstroReaderDbContext).Assembly.FullName);
            });
        });
        services.AddScoped<AstroReader.Application.SavedCharts.Interfaces.ISavedChartRepository, SavedChartRepository>();

        return services;
    }
}
