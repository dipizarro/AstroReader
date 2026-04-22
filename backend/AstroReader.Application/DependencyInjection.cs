using AstroReader.Application.Charts.Interfaces;
using AstroReader.Application.Charts.UseCases;
using AstroReader.Application.Interpretations.Premium;
using AstroReader.Application.PersonalProfiles.Interfaces;
using AstroReader.Application.PersonalProfiles.UseCases;
using AstroReader.Application.SavedCharts.Interfaces;
using AstroReader.Application.SavedCharts.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace AstroReader.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IPremiumInterpretationCatalogProvider, JsonPremiumInterpretationCatalogProvider>();
        services.AddScoped<IPremiumInterpretationContextResolver, PremiumInterpretationContextResolver>();
        services.AddScoped<IInterpretationAnalyzer, PremiumInterpretationAnalyzer>();
        services.AddScoped<IInterpretationComposer, PremiumInterpretationComposer>();
        services.AddScoped<IPremiumInterpretationPreviewUseCase, PremiumInterpretationPreviewUseCase>();
        services.AddScoped<ICalculateNatalChartUseCase, CalculateNatalChartUseCase>();
        services.AddScoped<ICreatePersonalProfileUseCase, CreatePersonalProfileUseCase>();
        services.AddScoped<IGetPersonalProfileByIdUseCase, GetPersonalProfileByIdUseCase>();
        services.AddScoped<IGetPersonalProfileBySavedChartIdUseCase, GetPersonalProfileBySavedChartIdUseCase>();
        services.AddScoped<ISaveChartUseCase, SaveChartUseCase>();
        services.AddScoped<IGetSavedChartByIdUseCase, GetSavedChartByIdUseCase>();
        services.AddScoped<IGetSavedChartsUseCase, GetSavedChartsUseCase>();
        services.AddScoped<AstroReader.Application.Interpretations.IInterpretationEngine, AstroReader.Application.Interpretations.BasicInterpretationEngine>();
        
        return services;
    }
}
