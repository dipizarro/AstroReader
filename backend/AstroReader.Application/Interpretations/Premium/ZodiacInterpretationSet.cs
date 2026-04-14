using System.Text.Json.Serialization;
using AstroReader.Domain.Enums;

namespace AstroReader.Application.Interpretations.Premium;

// La estructura explícita por signo hace que el JSON editorial sea legible
// y evita depender de claves mágicas dispersas en el resto del sistema.
public sealed class ZodiacInterpretationSet<TEntry> where TEntry : InterpretationEntry
{
    [JsonPropertyName("aries")]
    public TEntry? Aries { get; init; }

    [JsonPropertyName("taurus")]
    public TEntry? Taurus { get; init; }

    [JsonPropertyName("gemini")]
    public TEntry? Gemini { get; init; }

    [JsonPropertyName("cancer")]
    public TEntry? Cancer { get; init; }

    [JsonPropertyName("leo")]
    public TEntry? Leo { get; init; }

    [JsonPropertyName("virgo")]
    public TEntry? Virgo { get; init; }

    [JsonPropertyName("libra")]
    public TEntry? Libra { get; init; }

    [JsonPropertyName("scorpio")]
    public TEntry? Scorpio { get; init; }

    [JsonPropertyName("sagittarius")]
    public TEntry? Sagittarius { get; init; }

    [JsonPropertyName("capricorn")]
    public TEntry? Capricorn { get; init; }

    [JsonPropertyName("aquarius")]
    public TEntry? Aquarius { get; init; }

    [JsonPropertyName("pisces")]
    public TEntry? Pisces { get; init; }

    public bool HasEntry(ZodiacSign sign)
    {
        return sign switch
        {
            ZodiacSign.Aries => Aries is not null,
            ZodiacSign.Taurus => Taurus is not null,
            ZodiacSign.Gemini => Gemini is not null,
            ZodiacSign.Cancer => Cancer is not null,
            ZodiacSign.Leo => Leo is not null,
            ZodiacSign.Virgo => Virgo is not null,
            ZodiacSign.Libra => Libra is not null,
            ZodiacSign.Scorpio => Scorpio is not null,
            ZodiacSign.Sagittarius => Sagittarius is not null,
            ZodiacSign.Capricorn => Capricorn is not null,
            ZodiacSign.Aquarius => Aquarius is not null,
            ZodiacSign.Pisces => Pisces is not null,
            _ => throw new ArgumentOutOfRangeException(nameof(sign), sign, "Unsupported zodiac sign.")
        };
    }

    public TEntry GetBySign(ZodiacSign sign)
    {
        var entry = sign switch
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

        return entry
            ?? throw new PremiumInterpretationCatalogException(
                $"El catálogo premium todavía no contiene una entrada para el signo '{sign.ToString().ToLowerInvariant()}'.");
    }

    public IEnumerable<KeyValuePair<ZodiacSign, TEntry>> EnumerateAvailableEntries()
    {
        if (Aries is not null) yield return new KeyValuePair<ZodiacSign, TEntry>(ZodiacSign.Aries, Aries);
        if (Taurus is not null) yield return new KeyValuePair<ZodiacSign, TEntry>(ZodiacSign.Taurus, Taurus);
        if (Gemini is not null) yield return new KeyValuePair<ZodiacSign, TEntry>(ZodiacSign.Gemini, Gemini);
        if (Cancer is not null) yield return new KeyValuePair<ZodiacSign, TEntry>(ZodiacSign.Cancer, Cancer);
        if (Leo is not null) yield return new KeyValuePair<ZodiacSign, TEntry>(ZodiacSign.Leo, Leo);
        if (Virgo is not null) yield return new KeyValuePair<ZodiacSign, TEntry>(ZodiacSign.Virgo, Virgo);
        if (Libra is not null) yield return new KeyValuePair<ZodiacSign, TEntry>(ZodiacSign.Libra, Libra);
        if (Scorpio is not null) yield return new KeyValuePair<ZodiacSign, TEntry>(ZodiacSign.Scorpio, Scorpio);
        if (Sagittarius is not null) yield return new KeyValuePair<ZodiacSign, TEntry>(ZodiacSign.Sagittarius, Sagittarius);
        if (Capricorn is not null) yield return new KeyValuePair<ZodiacSign, TEntry>(ZodiacSign.Capricorn, Capricorn);
        if (Aquarius is not null) yield return new KeyValuePair<ZodiacSign, TEntry>(ZodiacSign.Aquarius, Aquarius);
        if (Pisces is not null) yield return new KeyValuePair<ZodiacSign, TEntry>(ZodiacSign.Pisces, Pisces);
    }
}
