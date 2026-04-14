using System.Text.Json.Serialization;

namespace AstroReader.Application.Interpretations.Premium;

public sealed class PremiumInterpretationPlanetCatalog
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
}
