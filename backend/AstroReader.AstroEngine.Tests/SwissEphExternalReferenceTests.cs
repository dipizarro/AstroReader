using AstroReader.AstroEngine;
using AstroReader.AstroEngine.Configuration;
using AstroReader.AstroEngine.Contracts;
using AstroReader.AstroEngine.Tests.ReferenceData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AstroReader.AstroEngine.Tests;

public class SwissEphExternalReferenceTests
{
    [Theory]
    [MemberData(nameof(ReferenceCases))]
    public void SwissEphEngine_ShouldMatchExternalReferenceForPlanetsAndAngles(ExternalReferenceCase referenceCase)
    {
        using var serviceProvider = BuildSwissEngineServiceProvider();
        var engine = serviceProvider.GetRequiredService<IAstroCalculationEngine>();

        var result = engine.Calculate(referenceCase.Request);
        var context = SwissEphExternalReferenceCases.BuildFailureContext(referenceCase);

        AssertWithinTolerance(
            referenceCase.ExpectedSunAbsoluteDegree,
            result.PlanetaryPositions[0].AbsoluteDegree,
            SwissEphExternalReferenceCases.PlanetToleranceDegrees,
            $"Sun mismatch.\n{context}");

        AssertWithinTolerance(
            referenceCase.ExpectedMoonAbsoluteDegree,
            result.PlanetaryPositions[1].AbsoluteDegree,
            SwissEphExternalReferenceCases.PlanetToleranceDegrees,
            $"Moon mismatch.\n{context}");

        AssertWithinTolerance(
            referenceCase.ExpectedAscendantDegree,
            result.AscendantDegree,
            SwissEphExternalReferenceCases.AngleToleranceDegrees,
            $"Ascendant mismatch.\n{context}");

        foreach (var expectedHouse in referenceCase.ExpectedHouseCusps)
        {
            Assert.True(result.Houses.ContainsKey(expectedHouse.Key), $"Missing house {expectedHouse.Key}.\n{context}");

            AssertWithinTolerance(
                expectedHouse.Value,
                result.Houses[expectedHouse.Key],
                SwissEphExternalReferenceCases.HouseToleranceDegrees,
                $"House {expectedHouse.Key} mismatch.\n{context}");
        }
    }

    public static IEnumerable<object[]> ReferenceCases()
    {
        return SwissEphExternalReferenceCases.All();
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

    private static void AssertWithinTolerance(double expected, double actual, double tolerance, string message)
    {
        var difference = AngularDistance(expected, actual);
        Assert.True(
            difference <= tolerance,
            $"{message}\nExpected: {expected:F6}° | Actual: {actual:F6}° | Difference: {difference:F6}° | Tolerance: {tolerance:F6}°");
    }

    private static double AngularDistance(double expected, double actual)
    {
        var normalizedExpected = NormalizeDegrees(expected);
        var normalizedActual = NormalizeDegrees(actual);
        var difference = Math.Abs(normalizedExpected - normalizedActual);

        return difference > 180d ? 360d - difference : difference;
    }

    private static double NormalizeDegrees(double degrees)
    {
        return (degrees % 360d + 360d) % 360d;
    }
}
