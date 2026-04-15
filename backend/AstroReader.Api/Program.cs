using AstroReader.AstroEngine;
using AstroReader.AstroEngine.Configuration;
using AstroReader.AstroEngine.Contracts;
using AstroReader.Application;
using AstroReader.Infrastructure;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
const string CorsPolicyName = "AstroReaderFrontend";

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddAstroEngineServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

// Configure Exception Handler
builder.Services.AddExceptionHandler<AstroReader.Api.Infrastructure.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>()?
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(origin => origin.Trim().TrimEnd('/'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray() ?? [];

        if (allowedOrigins.Length == 0)
        {
            throw new InvalidOperationException(
                "Cors:AllowedOrigins must contain at least one frontend origin. Configure it with environment variables in Azure App Service.");
        }

        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AstroReader API",
        Version = "v1",
        Description = "Backend API for AstroReader"
    });
});

var app = builder.Build();

var startupLogger = app.Services
    .GetRequiredService<ILoggerFactory>()
    .CreateLogger("AstroReader.Startup");
var astroEngineOptions = app.Services.GetRequiredService<IOptions<SwissEphOptions>>().Value;

startupLogger.LogInformation(
    "Astro engine configured. ActiveEngine={ActiveEngine}, CalculationEngineSetting={CalculationEngineSetting}, HouseSystem={HouseSystem}, EphemerisPath={EphemerisPath}, SwissEnabled={SwissEnabled}",
    astroEngineOptions.GetConfiguredEngineName(),
    astroEngineOptions.CalculationEngine,
    astroEngineOptions.HouseSystem,
    astroEngineOptions.GetEphemerisPathForLogs(),
    astroEngineOptions.ShouldUseSwissEph());

var astroEngineSmokeCheck = app.Services.GetRequiredService<IAstroEngineSmokeCheck>().Run();

if (astroEngineSmokeCheck.IsHealthy)
{
    startupLogger.LogInformation(
        "Astro engine smoke check status: healthy. ActiveEngine={ActiveEngine}, HouseSystem={HouseSystem}, EphemerisPath={EphemerisPath}, Skipped={Skipped}, Message={Message}",
        astroEngineSmokeCheck.ActiveEngine,
        astroEngineSmokeCheck.HouseSystem,
        astroEngineSmokeCheck.EphemerisPath,
        astroEngineSmokeCheck.Skipped,
        astroEngineSmokeCheck.Message);
}
else
{
    startupLogger.LogError(
        "Astro engine smoke check status: degraded. ActiveEngine={ActiveEngine}, HouseSystem={HouseSystem}, EphemerisPath={EphemerisPath}, Code={Code}, Message={Message}",
        astroEngineSmokeCheck.ActiveEngine,
        astroEngineSmokeCheck.HouseSystem,
        astroEngineSmokeCheck.EphemerisPath,
        astroEngineSmokeCheck.ErrorCode,
        astroEngineSmokeCheck.Message);
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS is enforced at Azure App Service level. Keep this disabled to avoid proxy redirect issues on Linux App Service.

// Enable CORS before Authorization and routing endpoints
app.UseCors(CorsPolicyName);

// Authorization is currently disabled but ready to be added here.
app.UseAuthorization();

app.MapControllers();

app.Run();
