using AstroReader.AstroEngine.Configuration;
using AstroReader.AstroEngine.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AstroReader.AstroEngine.Tests;

public class SwissEphSpikeValidationTests
{
    [Fact]
    public void SameLocationDifferentDates_ShouldChangeSignsDegreesAscendantAndHouses()
    {
        using var serviceProvider = BuildSwissEngineServiceProvider();
        var engine = serviceProvider.GetRequiredService<IAstroCalculationEngine>();

        var first = engine.Calculate(new AstroCalculationRequest(
            UtcDateTime: new DateTime(1990, 1, 15, 12, 0, 0, DateTimeKind.Utc),
            Latitude: -34.6037,
            Longitude: -58.3816));

        var second = engine.Calculate(new AstroCalculationRequest(
            UtcDateTime: new DateTime(1990, 8, 15, 12, 0, 0, DateTimeKind.Utc),
            Latitude: -34.6037,
            Longitude: -58.3816));

        AssertPlanetSetLooksReal(first);
        AssertPlanetSetLooksReal(second);

        var sunFirst = first.PlanetaryPositions[0];
        var sunSecond = second.PlanetaryPositions[0];
        var moonFirst = first.PlanetaryPositions[1];
        var moonSecond = second.PlanetaryPositions[1];

        Assert.NotEqual(sunFirst.ZodiacSignIndex, sunSecond.ZodiacSignIndex);
        Assert.NotEqual(moonFirst.ZodiacSignIndex, moonSecond.ZodiacSignIndex);
        Assert.NotEqual(Math.Round(sunFirst.AbsoluteDegree, 6), Math.Round(sunSecond.AbsoluteDegree, 6));
        Assert.NotEqual(Math.Round(moonFirst.AbsoluteDegree, 6), Math.Round(moonSecond.AbsoluteDegree, 6));
        Assert.NotEqual(Math.Round(first.AscendantDegree, 6), Math.Round(second.AscendantDegree, 6));
        Assert.NotEqual(Math.Round(first.Houses[1], 6), Math.Round(second.Houses[1], 6));
        Assert.NotEqual(Math.Round(first.Houses[10], 6), Math.Round(second.Houses[10], 6));
    }

    [Fact]
    public void SameInstantDifferentLocations_ShouldChangeAscendantAndHouseCusps()
    {
        using var serviceProvider = BuildSwissEngineServiceProvider();
        var engine = serviceProvider.GetRequiredService<IAstroCalculationEngine>();

        var buenosAires = engine.Calculate(new AstroCalculationRequest(
            UtcDateTime: new DateTime(1990, 1, 15, 12, 0, 0, DateTimeKind.Utc),
            Latitude: -34.6037,
            Longitude: -58.3816));

        var london = engine.Calculate(new AstroCalculationRequest(
            UtcDateTime: new DateTime(1990, 1, 15, 12, 0, 0, DateTimeKind.Utc),
            Latitude: 51.5074,
            Longitude: -0.1278));

        AssertPlanetSetLooksReal(buenosAires);
        AssertPlanetSetLooksReal(london);

        // Los planetas geocéntricos no deberían cambiar por el lugar si el instante UTC es el mismo.
        Assert.Equal(Math.Round(buenosAires.PlanetaryPositions[0].AbsoluteDegree, 6), Math.Round(london.PlanetaryPositions[0].AbsoluteDegree, 6));
        Assert.Equal(Math.Round(buenosAires.PlanetaryPositions[1].AbsoluteDegree, 6), Math.Round(london.PlanetaryPositions[1].AbsoluteDegree, 6));

        // El ascendente y las casas sí deben variar con la ubicación.
        Assert.NotEqual(Math.Round(buenosAires.AscendantDegree, 6), Math.Round(london.AscendantDegree, 6));
        Assert.NotEqual(Math.Round(buenosAires.Houses[1], 6), Math.Round(london.Houses[1], 6));
        Assert.NotEqual(Math.Round(buenosAires.Houses[7], 6), Math.Round(london.Houses[7], 6));
        Assert.NotEqual(Math.Round(buenosAires.Houses[10], 6), Math.Round(london.Houses[10], 6));
    }

    [Fact]
    public void SwissEngine_ShouldClearlyDifferFromLegacyMockBehavior()
    {
        using var serviceProvider = BuildSwissEngineServiceProvider();
        var engine = serviceProvider.GetRequiredService<IAstroCalculationEngine>();

        var result = engine.Calculate(new AstroCalculationRequest(
            UtcDateTime: new DateTime(1990, 1, 15, 12, 0, 0, DateTimeKind.Utc),
            Latitude: -34.6037,
            Longitude: -58.3816));

        AssertPlanetSetLooksReal(result);

        Assert.NotEqual(49.4, Math.Round(result.PlanetaryPositions[0].AbsoluteDegree, 1));
        Assert.NotEqual(127.8, Math.Round(result.PlanetaryPositions[1].AbsoluteDegree, 1));
        Assert.NotEqual(185.0, Math.Round(result.AscendantDegree, 1));
        Assert.Equal(12, result.Houses.Count);
    }

    private static ServiceProvider BuildSwissEngineServiceProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{SwissEphOptions.SectionName}:CalculationEngine"] = SwissEphOptions.SwissEphEngineMode,
                [$"{SwissEphOptions.SectionName}:HouseSystem"] = SwissEphOptions.DefaultHouseSystem,
                [$"{SwissEphOptions.SectionName}:EphemerisPath"] = string.Empty
            })
            .Build();

        var services = new ServiceCollection();
        services.AddAstroEngineServices(configuration);

        return services.BuildServiceProvider();
    }

    private static void AssertPlanetSetLooksReal(AstroCalculationResult result)
    {
        var expectedPlanetIds = new[] { 0, 1, 2, 3, 4, 5, 6 };

        Assert.Equal(12, result.Houses.Count);

        foreach (var house in result.Houses)
        {
            Assert.InRange(house.Key, 1, 12);
            Assert.InRange(house.Value, 0, 360);
        }

        Assert.InRange(result.AscendantDegree, 0, 360);

        foreach (var planetId in expectedPlanetIds)
        {
            Assert.True(result.PlanetaryPositions.ContainsKey(planetId), $"No se encontró el planeta esperado con id {planetId}.");

            var planet = result.PlanetaryPositions[planetId];
            Assert.InRange(planet.AbsoluteDegree, 0, 360);
            Assert.InRange(planet.ZodiacSignIndex, 0, 11);
            Assert.InRange(planet.SignDegree, 0, 30);
        }
    }
}
