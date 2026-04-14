using AstroReader.Domain.Entities;
using AstroReader.Domain.Enums;
using AstroReader.Domain.ValueObjects;

namespace AstroReader.Application.Interpretations.Premium;

public sealed class PremiumInterpretationPreviewUseCase : IPremiumInterpretationPreviewUseCase
{
    private const double DefaultSignDegree = 15d;

    private readonly IPremiumInterpretationContextResolver _premiumInterpretationContextResolver;
    private readonly IInterpretationAnalyzer _interpretationAnalyzer;
    private readonly IInterpretationComposer _interpretationComposer;

    public PremiumInterpretationPreviewUseCase(
        IPremiumInterpretationContextResolver premiumInterpretationContextResolver,
        IInterpretationAnalyzer interpretationAnalyzer,
        IInterpretationComposer interpretationComposer)
    {
        _premiumInterpretationContextResolver = premiumInterpretationContextResolver;
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
        var context = _premiumInterpretationContextResolver.Resolve(chart);
        var coverageAssessment = context.Coverage;

        if (!coverageAssessment.IsComplete)
        {
            var status = coverageAssessment.HasAnyCoverage
                ? InterpretationCoverageStatus.Partial
                : InterpretationCoverageStatus.Fallback;

            return new PremiumInterpretationPreviewResponse
            {
                Selection = ToSelectionDto(selection),
                Analysis = new InterpretationAnalysisResult(),
                Interpretation = PremiumInterpretationFallbackFactory.Create(
                    selection.Sun,
                    selection.Moon,
                    selection.Ascendant,
                    coverageAssessment.ToDto(status))
            };
        }

        try
        {
            var analysis = _interpretationAnalyzer.Analyze(context);
            var composition = _interpretationComposer.Compose(context, analysis);

            return new PremiumInterpretationPreviewResponse
            {
                Selection = ToSelectionDto(selection),
                Analysis = analysis,
                Interpretation = PremiumInterpretationResponseMapper.MapComposition(
                    composition,
                    coverageAssessment.ToDto(
                        InterpretationCoverageStatus.Complete,
                        PremiumInterpretationResponseMapper.PrimaryComposedBlocks))
            };
        }
        catch (PremiumInterpretationCatalogException)
        {
            return new PremiumInterpretationPreviewResponse
            {
                Selection = ToSelectionDto(selection),
                Analysis = new InterpretationAnalysisResult(),
                Interpretation = PremiumInterpretationFallbackFactory.Create(
                    selection.Sun,
                    selection.Moon,
                    selection.Ascendant,
                    coverageAssessment.ToDto(InterpretationCoverageStatus.Fallback))
            };
        }
        catch (PremiumInterpretationAnalysisException)
        {
            return new PremiumInterpretationPreviewResponse
            {
                Selection = ToSelectionDto(selection),
                Analysis = new InterpretationAnalysisResult(),
                Interpretation = PremiumInterpretationFallbackFactory.Create(
                    selection.Sun,
                    selection.Moon,
                    selection.Ascendant,
                    coverageAssessment.ToDto(InterpretationCoverageStatus.Fallback))
            };
        }
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

    private static PremiumInterpretationPreviewSelection ToSelectionDto(ResolvedSignSelection selection)
    {
        return new PremiumInterpretationPreviewSelection
        {
            Sun = selection.Sun.ToString(),
            Moon = selection.Moon.ToString(),
            Ascendant = selection.Ascendant.ToString(),
            Mercury = selection.Mercury.ToString(),
            Venus = selection.Venus.ToString(),
            Mars = selection.Mars.ToString()
        };
    }

    private sealed record ResolvedSignSelection(
        ZodiacSign Sun,
        ZodiacSign Moon,
        ZodiacSign Ascendant,
        ZodiacSign Mercury,
        ZodiacSign Venus,
        ZodiacSign Mars);
}
