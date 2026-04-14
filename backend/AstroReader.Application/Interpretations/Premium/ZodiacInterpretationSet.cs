using System.Text.Json.Serialization;
using AstroReader.Domain.Enums;

namespace AstroReader.Application.Interpretations.Premium;

// La estructura explícita por signo hace que el JSON editorial sea legible
// y evita depender de claves mágicas dispersas en el resto del sistema.
public sealed class ZodiacInterpretationSet<TEntry> where TEntry : InterpretationEntry
{
    [JsonPropertyName("aries")]
    public TEntry Aries { get; init; } = default!;

    [JsonPropertyName("taurus")]
    public TEntry Taurus { get; init; } = default!;

    [JsonPropertyName("gemini")]
    public TEntry Gemini { get; init; } = default!;

    [JsonPropertyName("cancer")]
    public TEntry Cancer { get; init; } = default!;

    [JsonPropertyName("leo")]
    public TEntry Leo { get; init; } = default!;

    [JsonPropertyName("virgo")]
    public TEntry Virgo { get; init; } = default!;

    [JsonPropertyName("libra")]
    public TEntry Libra { get; init; } = default!;

    [JsonPropertyName("scorpio")]
    public TEntry Scorpio { get; init; } = default!;

    [JsonPropertyName("sagittarius")]
    public TEntry Sagittarius { get; init; } = default!;

    [JsonPropertyName("capricorn")]
    public TEntry Capricorn { get; init; } = default!;

    [JsonPropertyName("aquarius")]
    public TEntry Aquarius { get; init; } = default!;

    [JsonPropertyName("pisces")]
    public TEntry Pisces { get; init; } = default!;

    public TEntry GetBySign(ZodiacSign sign)
    {
        return sign switch
        {
            ZodiacSign.Aries => Aries,
            ZodiacSign.Taurus => Taurus,
            ZodiacSign.Gemini => Gemini,
            ZodiacSign.Cancer => Cancer,
            ZodiacSign.Leo => Leo,
            ZodiacSign.Virgo => Virgo,
            ZodiacSign.Libra => Libra,
            ZodiacSign.Scorpio => Scorpio,
            ZodiacSign.Sagittarius => Sagittarius,
            ZodiacSign.Capricorn => Capricorn,
            ZodiacSign.Aquarius => Aquarius,
            ZodiacSign.Pisces => Pisces,
            _ => throw new ArgumentOutOfRangeException(nameof(sign), sign, "Unsupported zodiac sign.")
        };
    }
}
