using AstroReader.Application.Charts.Interfaces;
using AstroReader.Application.Charts.UseCases;
using AstroReader.Application.SavedCharts.Interfaces;
using AstroReader.Application.SavedCharts.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace AstroReader.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICalculateNatalChartUseCase, CalculateNatalChartUseCase>();
        services.AddScoped<ISaveChartUseCase, SaveChartUseCase>();
        services.AddScoped<IGetSavedChartByIdUseCase, GetSavedChartByIdUseCase>();
        services.AddScoped<IGetSavedChartsUseCase, GetSavedChartsUseCase>();
        services.AddScoped<AstroReader.Application.Interpretations.IInterpretationEngine, AstroReader.Application.Interpretations.BasicInterpretationEngine>();
        services.AddSingleton<AstroReader.AstroEngine.Contracts.IAstroCalculationEngine, AstroReader.AstroEngine.Implementations.MockAstroCalculationEngine>();
        
        return services;
    }
}
