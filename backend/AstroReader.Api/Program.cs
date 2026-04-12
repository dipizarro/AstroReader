using AstroReader.AstroEngine;
using AstroReader.AstroEngine.Configuration;
using AstroReader.Application;
using AstroReader.Infrastructure;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

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
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
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

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Comentado para desarrollo local si no hay certificados HTTPS configurados

// Enable CORS before Authorization and routing endpoints
app.UseCors("AllowAll");

// Authorization is currently disabled but ready to be added here.
app.UseAuthorization();

app.MapControllers();

app.Run();
