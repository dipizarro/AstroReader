using AstroReader.Domain.Entities;
using AstroReader.Domain.Enums;

namespace AstroReader.Application.Interpretations.Premium;

public sealed record PremiumInterpretationContext
{
    public required NatalChart Chart { get; init; }

    public required PremiumInterpretationCoverageAssessment Coverage { get; init; }

    public required ZodiacSign SunSign { get; init; }

    public required ZodiacSign MoonSign { get; init; }

    public required ZodiacSign AscendantSign { get; init; }

    public required ZodiacSign MercurySign { get; init; }

    public required ZodiacSign VenusSign { get; init; }

    public required ZodiacSign MarsSign { get; init; }

    public SunInterpretationEntry? Sun { get; init; }

    public MoonInterpretationEntry? Moon { get; init; }

    public AscendantInterpretationEntry? Ascendant { get; init; }

    public MercuryInterpretationEntry? Mercury { get; init; }

    public VenusInterpretationEntry? Venus { get; init; }

    public MarsInterpretationEntry? Mars { get; init; }

    public bool HasCompleteCoverage => Coverage.IsComplete;

    public SunInterpretationEntry RequireSun() =>
        Sun ?? throw CreateMissingEntryException(PremiumInterpretationPosition.Sun, SunSign);

    public MoonInterpretationEntry RequireMoon() =>
        Moon ?? throw CreateMissingEntryException(PremiumInterpretationPosition.Moon, MoonSign);

    public AscendantInterpretationEntry RequireAscendant() =>
        Ascendant ?? throw CreateMissingEntryException(PremiumInterpretationPosition.Ascendant, AscendantSign);

    public MercuryInterpretationEntry RequireMercury() =>
        Mercury ?? throw CreateMissingEntryException(PremiumInterpretationPosition.Mercury, MercurySign);

    public VenusInterpretationEntry RequireVenus() =>
        Venus ?? throw CreateMissingEntryException(PremiumInterpretationPosition.Venus, VenusSign);

    public MarsInterpretationEntry RequireMars() =>
        Mars ?? throw CreateMissingEntryException(PremiumInterpretationPosition.Mars, MarsSign);

    private static PremiumInterpretationAnalysisException CreateMissingEntryException(
        PremiumInterpretationPosition position,
        ZodiacSign sign)
    {
        return new PremiumInterpretationAnalysisException(
            $"Falta la entrada premium '{position.ToString().ToLowerInvariant()}.{sign.ToString().ToLowerInvariant()}' para continuar el análisis.");
    }
}
