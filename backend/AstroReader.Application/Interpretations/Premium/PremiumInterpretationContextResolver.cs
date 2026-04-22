using AstroReader.Domain.Entities;
using AstroReader.Domain.Enums;

namespace AstroReader.Application.Interpretations.Premium;

public sealed class PremiumInterpretationContextResolver : IPremiumInterpretationContextResolver
{
    private readonly IPremiumInterpretationCatalogProvider _catalogProvider;

    public PremiumInterpretationContextResolver(IPremiumInterpretationCatalogProvider catalogProvider)
    {
        _catalogProvider = catalogProvider;
    }

    public PremiumInterpretationContext Resolve(NatalChart chart, PremiumReaderProfileContext? readerProfile = null)
    {
        var sunSign = RequirePlanet(chart, Planet.Sun).Sign;
        var moonSign = RequirePlanet(chart, Planet.Moon).Sign;
        var mercurySign = RequirePlanet(chart, Planet.Mercury).Sign;
        var venusSign = RequirePlanet(chart, Planet.Venus).Sign;
        var marsSign = RequirePlanet(chart, Planet.Mars).Sign;
        var ascendantSign = RequireHouse(chart, 1).Sign;

        var catalog = _catalogProvider.GetCatalog();
        var coverage = PremiumInterpretationCoverageEvaluator.Evaluate(
            catalog,
            sunSign,
            moonSign,
            ascendantSign,
            mercurySign,
            venusSign,
            marsSign);

        return new PremiumInterpretationContext
        {
            Chart = chart,
            ReaderProfile = readerProfile,
            Coverage = coverage,
            SunSign = sunSign,
            MoonSign = moonSign,
            AscendantSign = ascendantSign,
            MercurySign = mercurySign,
            VenusSign = venusSign,
            MarsSign = marsSign,
            Sun = TryGetEntry<SunInterpretationEntry>(catalog, PremiumInterpretationPosition.Sun, sunSign),
            Moon = TryGetEntry<MoonInterpretationEntry>(catalog, PremiumInterpretationPosition.Moon, moonSign),
            Ascendant = TryGetEntry<AscendantInterpretationEntry>(catalog, PremiumInterpretationPosition.Ascendant, ascendantSign),
            Mercury = TryGetEntry<MercuryInterpretationEntry>(catalog, PremiumInterpretationPosition.Mercury, mercurySign),
            Venus = TryGetEntry<VenusInterpretationEntry>(catalog, PremiumInterpretationPosition.Venus, venusSign),
            Mars = TryGetEntry<MarsInterpretationEntry>(catalog, PremiumInterpretationPosition.Mars, marsSign)
        };
    }

    private static TEntry? TryGetEntry<TEntry>(
        PremiumInterpretationCatalog catalog,
        PremiumInterpretationPosition position,
        ZodiacSign sign)
        where TEntry : InterpretationEntry
    {
        return catalog.HasEntry(position, sign)
            ? catalog.GetEntry<TEntry>(position, sign)
            : null;
    }

    private static PlanetPosition RequirePlanet(NatalChart chart, Planet planet)
    {
        return chart.Planets.FirstOrDefault(x => x.Planet == planet)
            ?? throw new PremiumInterpretationAnalysisException(
                $"La carta no contiene la posición requerida de '{planet}' para resolver el contexto premium.");
    }

    private static HousePosition RequireHouse(NatalChart chart, int houseNumber)
    {
        return chart.Houses.FirstOrDefault(x => x.HouseNumber == houseNumber)
            ?? throw new PremiumInterpretationAnalysisException(
                $"La carta no contiene la casa requerida '{houseNumber}' para resolver el contexto premium.");
    }
}
