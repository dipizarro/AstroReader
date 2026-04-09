using AstroReader.AstroEngine.Configuration;
using AstroReader.AstroEngine.Contracts;
using AstroReader.AstroEngine.Implementations;
using AstroReader.AstroEngine.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AstroReader.AstroEngine;

public static class DependencyInjection
{
    public static IServiceCollection AddAstroEngineServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<SwissEphOptions>()
            .Bind(configuration.GetSection(SwissEphOptions.SectionName));

        services.AddSingleton<ISwissEphClientFactory, SwissEphClientFactory>();
        services.AddSingleton<IAstroLongitudeProbe, AstroLongitudeProbe>();
        services.AddSingleton<MockAstroCalculationEngine>();
        services.AddSingleton<SwissEphAstroCalculationEngine>();

        services.AddSingleton<IAstroCalculationEngine>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<SwissEphOptions>>().Value;

            return options.EnableSwissEphForNatalCharts
                ? serviceProvider.GetRequiredService<SwissEphAstroCalculationEngine>()
                : serviceProvider.GetRequiredService<MockAstroCalculationEngine>();
        });

        return services;
    }
}
