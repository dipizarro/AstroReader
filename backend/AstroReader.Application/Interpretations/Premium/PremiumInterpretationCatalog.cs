using System.Text.Json.Serialization;
using AstroReader.Domain.Enums;

namespace AstroReader.Application.Interpretations.Premium;

public sealed class PremiumInterpretationCatalog
{
    [JsonPropertyName("sun")]
    public ZodiacInterpretationSet<SunInterpretationEntry> Sun { get; init; } = new();

    [JsonPropertyName("moon")]
    public ZodiacInterpretationSet<MoonInterpretationEntry> Moon { get; init; } = new();

    [JsonPropertyName("ascendant")]
    public ZodiacInterpretationSet<AscendantInterpretationEntry> Ascendant { get; init; } = new();

    [JsonPropertyName("mercury")]
    public ZodiacInterpretationSet<MercuryInterpretationEntry> Mercury { get; init; } = new();

    [JsonPropertyName("venus")]
    public ZodiacInterpretationSet<VenusInterpretationEntry> Venus { get; init; } = new();

    [JsonPropertyName("mars")]
    public ZodiacInterpretationSet<MarsInterpretationEntry> Mars { get; init; } = new();

    public InterpretationEntry GetEntry(PremiumInterpretationPosition position, ZodiacSign sign)
    {
        return position switch
        {
            PremiumInterpretationPosition.Sun => Sun.GetBySign(sign),
            PremiumInterpretationPosition.Moon => Moon.GetBySign(sign),
            PremiumInterpretationPosition.Ascendant => Ascendant.GetBySign(sign),
            PremiumInterpretationPosition.Mercury => Mercury.GetBySign(sign),
            PremiumInterpretationPosition.Venus => Venus.GetBySign(sign),
            PremiumInterpretationPosition.Mars => Mars.GetBySign(sign),
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
