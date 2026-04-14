using AstroReader.Domain.Entities;
using AstroReader.Domain.Enums;
using AstroReader.Domain.ValueObjects;

namespace AstroReader.Application.Interpretations.Premium;

public sealed class PremiumInterpretationPreviewUseCase : IPremiumInterpretationPreviewUseCase
{
    private const double DefaultSignDegree = 15d;

    private readonly IInterpretationAnalyzer _interpretationAnalyzer;
    private readonly IInterpretationComposer _interpretationComposer;

    public PremiumInterpretationPreviewUseCase(
        IInterpretationAnalyzer interpretationAnalyzer,
        IInterpretationComposer interpretationComposer)
    {
        _interpretationAnalyzer = interpretationAnalyzer;
        _interpretationComposer = interpretationComposer;
    }

    public PremiumInterpretationPreviewResponse Execute(PremiumInterpretationPreviewRequest request)
    {
        var selection = new ResolvedSignSelection(
            Sun: ParseSign(request.Sun, nameof(request.Sun)),
            Moon: ParseSign(request.Moon, nameof(request.Moon)),
            Ascendant: ParseSign(request.Ascendant, nameof(request.Ascendant)),
            Mercury: ParseSign(request.Mercury, nameof(request.Mercury)),
            Venus: ParseSign(request.Venus, nameof(request.Venus)),
            Mars: ParseSign(request.Mars, nameof(request.Mars)));

        var chart = BuildPreviewChart(selection);
        var analysis = _interpretationAnalyzer.Analyze(chart);
        var composition = _interpretationComposer.Compose(chart, analysis);

        return new PremiumInterpretationPreviewResponse
        {
            Selection = new PremiumInterpretationPreviewSelection
            {
                Sun = selection.Sun.ToString(),
                Moon = selection.Moon.ToString(),
                Ascendant = selection.Ascendant.ToString(),
                Mercury = selection.Mercury.ToString(),
                Venus = selection.Venus.ToString(),
                Mars = selection.Mars.ToString()
            },
            Analysis = analysis,
            Interpretation = PremiumInterpretationResponseMapper.MapComposition(composition)
        };
    }

    private static NatalChart BuildPreviewChart(ResolvedSignSelection selection)
    {
        var planets = new List<PlanetPosition>
        {
            CreatePlanetPosition(Planet.Sun, selection.Sun),
            CreatePlanetPosition(Planet.Moon, selection.Moon),
            CreatePlanetPosition(Planet.Mercury, selection.Mercury),
            CreatePlanetPosition(Planet.Venus, selection.Venus),
            CreatePlanetPosition(Planet.Mars, selection.Mars)
        };

        var houses = Enumerable.Range(1, 12)
            .Select(houseNumber => CreateHousePosition(
                houseNumber,
                NormalizeSign((int)selection.Ascendant + (houseNumber - 1))))
            .ToList();

        return new NatalChart(
            new BirthData(new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc), "UTC"),
            new GeoLocation(0d, 0d),
            planets,
            houses);
    }

    private static PlanetPosition CreatePlanetPosition(Planet planet, ZodiacSign sign)
    {
        return new PlanetPosition
        {
            Planet = planet,
            Sign = sign,
            SignDegree = DefaultSignDegree,
            AbsoluteDegree = GetAbsoluteDegree(sign),
            IsRetrograde = false
        };
    }

    private static HousePosition CreateHousePosition(int houseNumber, ZodiacSign sign)
    {
        return new HousePosition
        {
            HouseNumber = houseNumber,
            Sign = sign,
            AbsoluteDegree = GetAbsoluteDegree(sign)
        };
    }

    private static ZodiacSign ParseSign(string rawValue, string fieldName)
    {
        if (Enum.TryParse<ZodiacSign>(rawValue, ignoreCase: true, out var sign))
        {
            return sign;
        }

        var validValues = string.Join(", ", Enum.GetNames<ZodiacSign>());
        throw new PremiumInterpretationAnalysisException(
            $"El signo '{rawValue}' no es válido para '{fieldName}'. Usa uno de: {validValues}.");
    }

    private static ZodiacSign NormalizeSign(int signIndex)
    {
        var normalizedIndex = ((signIndex % 12) + 12) % 12;
        return (ZodiacSign)normalizedIndex;
    }

    private static double GetAbsoluteDegree(ZodiacSign sign)
    {
        return ((int)sign * 30d) + DefaultSignDegree;
    }

    private sealed record ResolvedSignSelection(
        ZodiacSign Sun,
        ZodiacSign Moon,
        ZodiacSign Ascendant,
        ZodiacSign Mercury,
        ZodiacSign Venus,
        ZodiacSign Mars);
}
