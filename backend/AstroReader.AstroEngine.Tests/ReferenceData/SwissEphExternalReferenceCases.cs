using AstroReader.AstroEngine.Configuration;
using AstroReader.AstroEngine.Contracts;

namespace AstroReader.AstroEngine.Tests.ReferenceData;

public sealed record ExternalReferenceCase
{
    public required string Label { get; init; }
    public required AstroCalculationRequest Request { get; init; }
    public required double ExpectedSunAbsoluteDegree { get; init; }
    public required double ExpectedMoonAbsoluteDegree { get; init; }
    public required double ExpectedAscendantDegree { get; init; }
    public required IReadOnlyDictionary<int, double> ExpectedHouseCusps { get; init; }
    public required string AstroSeekSourceUrl { get; init; }
    public required string AstroThemeSourceUrl { get; init; }
    public required string Notes { get; init; }
}

internal static class SwissEphExternalReferenceCases
{
    public const double PlanetToleranceDegrees = 0.5d;
    public const double AngleToleranceDegrees = 1.0d;
    public const double HouseToleranceDegrees = 1.0d;

    public static IEnumerable<object[]> All()
    {
        yield return new object[]
        {
            new ExternalReferenceCase
            {
                Label = "Linda Cristal - Buenos Aires - 1931-02-24",
                Request = new AstroCalculationRequest(
                    UtcDateTime: new DateTime(1931, 2, 24, 15, 0, 0, DateTimeKind.Utc),
                    Latitude: -34.6,
                    Longitude: -58.38333333333333),
                ExpectedSunAbsoluteDegree = 335.05d,       // Astrotheme: Sun 5°03' Pisces
                ExpectedMoonAbsoluteDegree = 52.533333333d, // Astrotheme: Moon 22°32' Taurus
                ExpectedAscendantDegree = 41.616666667d,    // Astrotheme: AS 11°37' Taurus
                ExpectedHouseCusps = new Dictionary<int, double>
                {
                    [4] = 137.633333333d, // Astrotheme/Google snippet: House IV 17°38' Leo
                    [7] = 221.616666667d  // Astrotheme: House VII 11°37' Scorpio
                },
                AstroSeekSourceUrl = "https://www.astro-seek.com/birth-chart/linda-cristal-horoscope",
                AstroThemeSourceUrl = "https://www.astrotheme.com/astrology/Linda_Cristal",
                Notes = "Inputs: Astro-Seek snippet reported UT 1931-02-24 15:00 and coordinates 34°36'S, 58°23'W. Expected chart values from Astrotheme snippets in Placidus."
            }
        };

        yield return new object[]
        {
            new ExternalReferenceCase
            {
                Label = "Whoopi Goldberg - New York - 1955-11-13",
                Request = new AstroCalculationRequest(
                    UtcDateTime: new DateTime(1955, 11, 13, 17, 48, 0, DateTimeKind.Utc),
                    Latitude: 40.71666666666667,
                    Longitude: -74.0),
                ExpectedSunAbsoluteDegree = 230.566666667d, // Astrotheme: Sun 20°34' Scorpio
                ExpectedMoonAbsoluteDegree = 222.1d,         // Astrotheme: Moon 12°06' Scorpio
                ExpectedAscendantDegree = 319.266666667d,    // Astrotheme: AS 19°16' Aquarius
                ExpectedHouseCusps = new Dictionary<int, double>
                {
                    [4] = 66.9d,   // Astrotheme: House 4 6°54' Gemini
                    [10] = 246.9d  // Astrotheme: House 10 6°54' Sagittarius
                },
                AstroSeekSourceUrl = "https://www.astro-seek.com/birth-chart/whoopi-goldberg-horoscope",
                AstroThemeSourceUrl = "https://www.astrotheme.com/astrology/Whoopi_Goldberg",
                Notes = "Inputs: Astro-Seek snippet reported UT 1955-11-13 17:48 and coordinates 40°43'N, 74°0'W. Expected chart values from Astrotheme snippets in Placidus."
            }
        };

        yield return new object[]
        {
            new ExternalReferenceCase
            {
                Label = "Mark Twain - Florida, Missouri - 1835-11-30",
                Request = new AstroCalculationRequest(
                    UtcDateTime: new DateTime(1835, 11, 30, 10, 52, 0, DateTimeKind.Utc),
                    Latitude: 39.5,
                    Longitude: -91.78333333333333),
                ExpectedSunAbsoluteDegree = 247.566666667d, // Astrotheme: Sun 7°34' Sagittarius
                ExpectedMoonAbsoluteDegree = 15.616666667d, // Astrotheme: Moon 15°37' Aries
                ExpectedAscendantDegree = 219.7d,           // Astrotheme: AS 9°42' Scorpio
                ExpectedHouseCusps = new Dictionary<int, double>
                {
                    [4] = 317.366666667d, // Astrotheme/Google snippet: House IV 17°22' Aquarius
                    [10] = 137.366666667d // Astrotheme: MC / House 10 17°22' Leo
                },
                AstroSeekSourceUrl = "https://www.astro-seek.com/birth-chart/mark-twain-horoscope",
                AstroThemeSourceUrl = "https://www.astrotheme.com/astrology/Mark_Twain",
                Notes = "Inputs: Astro-Seek snippet reported UT 1835-11-30 10:52 (+6:07 LMT) and coordinates 39°30'N, 91°47'W. Expected chart values from Astrotheme snippets in Placidus."
            }
        };
    }

    public static string BuildFailureContext(ExternalReferenceCase referenceCase)
    {
        return $"Reference case: {referenceCase.Label}\n" +
               $"Astro-Seek input source: {referenceCase.AstroSeekSourceUrl}\n" +
               $"Astrotheme expected values source: {referenceCase.AstroThemeSourceUrl}\n" +
               $"Notes: {referenceCase.Notes}";
    }
}
