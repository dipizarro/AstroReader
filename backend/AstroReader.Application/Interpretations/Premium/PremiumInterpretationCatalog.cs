using System.Text.Json.Serialization;
using AstroReader.Domain.Enums;

namespace AstroReader.Application.Interpretations.Premium;

public sealed class PremiumInterpretationCatalog
{
    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;

    [JsonPropertyName("planets")]
    public PremiumInterpretationPlanetCatalog Planets { get; init; } = new();

    public InterpretationEntry GetEntry(PremiumInterpretationPosition position, ZodiacSign sign)
    {
        return position switch
        {
            PremiumInterpretationPosition.Sun => Planets.Sun.GetBySign(sign),
            PremiumInterpretationPosition.Moon => Planets.Moon.GetBySign(sign),
            PremiumInterpretationPosition.Ascendant => Planets.Ascendant.GetBySign(sign),
            PremiumInterpretationPosition.Mercury => Planets.Mercury.GetBySign(sign),
            PremiumInterpretationPosition.Venus => Planets.Venus.GetBySign(sign),
            PremiumInterpretationPosition.Mars => Planets.Mars.GetBySign(sign),
            _ => throw new ArgumentOutOfRangeException(nameof(position), position, "Unsupported premium interpretation position.")
        };
    }

    public TEntry GetEntry<TEntry>(PremiumInterpretationPosition position, ZodiacSign sign)
        where TEntry : InterpretationEntry
    {
        var entry = GetEntry(position, sign);

        return entry as TEntry
            ?? throw new InvalidOperationException(
                $"The premium interpretation entry for '{position}' in sign '{sign}' is not of expected type '{typeof(TEntry).Name}'.");
    }
}
