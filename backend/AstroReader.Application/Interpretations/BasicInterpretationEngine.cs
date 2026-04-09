using System.Linq;
using AstroReader.Application.Charts.DTOs;
using AstroReader.Domain.Entities;
using AstroReader.Domain.Enums;

namespace AstroReader.Application.Interpretations;

/// <summary>
/// Motor determinístico inicial. En el futuro, esto podría consultar una Base de Datos o a un Agente IA.
/// </summary>
public class BasicInterpretationEngine : IInterpretationEngine
{
    public ChartInterpretation GenerateBaseInterpretation(NatalChart chart)
    {
        var sun = GetPlanet(chart, Planet.Sun);
        var moon = GetPlanet(chart, Planet.Moon);
        var mercury = GetPlanet(chart, Planet.Mercury);
        var venus = GetPlanet(chart, Planet.Venus);
        var mars = GetPlanet(chart, Planet.Mars);
        var ascendantHouse = GetHouse(chart, 1);

        var relevantHouses = chart.Houses
            .Where(h => InterpretationCatalog.HouseThemes.ContainsKey(h.HouseNumber))
            .OrderBy(h => h.HouseNumber)
            .Select(h => new HouseInterpretationDto
            {
                HouseNumber = h.HouseNumber,
                Sign = h.Sign.ToString(),
                Title = GetHouseTitle(h.HouseNumber),
                Meaning = BuildHouseInterpretation(h.HouseNumber, h.Sign)
            })
            .ToList();

        var sunInterpretation = BuildPlanetInterpretation(sun, InterpretationCatalog.SunBySign, "Posicion solar no disponible.");
        var moonInterpretation = BuildPlanetInterpretation(moon, InterpretationCatalog.MoonBySign, "Posicion lunar no disponible.");
        var ascendantInterpretation = BuildHouseInterpretation(
            ascendantHouse,
            InterpretationCatalog.AscendantBySign,
            "Ascendente no disponible.");
        var mercuryInterpretation = BuildPlanetInterpretation(mercury, InterpretationCatalog.MercuryBySign, "Posicion de Mercurio no disponible.");
        var venusInterpretation = BuildPlanetInterpretation(venus, InterpretationCatalog.VenusBySign, "Posicion de Venus no disponible.");
        var marsInterpretation = BuildPlanetInterpretation(mars, InterpretationCatalog.MarsBySign, "Posicion de Marte no disponible.");
        var summary = BuildGeneralSummary(sun, moon, ascendantHouse, mercury, venus, mars);

        return new ChartInterpretation
        {
            Headline = BuildHeadline(sun, moon, ascendantHouse),
            Summary = summary,
            Core = new CoreInterpretation
            {
                Sun = sunInterpretation,
                Moon = moonInterpretation,
                Ascendant = ascendantInterpretation
            },
            PersonalPlanets = new PersonalPlanetsInterpretation
            {
                Mercury = mercuryInterpretation,
                Venus = venusInterpretation,
                Mars = marsInterpretation
            },
            Houses = relevantHouses,
            Profiles = []
        };
    }

    private static PlanetPosition? GetPlanet(NatalChart chart, Planet planet)
    {
        return chart.Planets.FirstOrDefault(p => p.Planet == planet);
    }

    private static HousePosition? GetHouse(NatalChart chart, int houseNumber)
    {
        return chart.Houses.FirstOrDefault(h => h.HouseNumber == houseNumber);
    }

    private static string BuildPlanetInterpretation(
        PlanetPosition? position,
        IReadOnlyDictionary<ZodiacSign, string> catalog,
        string fallback)
    {
        if (position is null)
        {
            return fallback;
        }

        var baseText = catalog.TryGetValue(position.Sign, out var interpretation)
            ? interpretation
            : fallback;

        return position.IsRetrograde
            ? $"{baseText} Esta energia opera en fase retrograda, volviendose mas introspectiva, revisora o internalizada."
            : baseText;
    }

    private static string BuildHouseInterpretation(
        HousePosition? house,
        IReadOnlyDictionary<ZodiacSign, string> catalog,
        string fallback)
    {
        if (house is null)
        {
            return fallback;
        }

        return catalog.TryGetValue(house.Sign, out var interpretation)
            ? interpretation
            : fallback;
    }

    private static string BuildHeadline(
        PlanetPosition? sun,
        PlanetPosition? moon,
        HousePosition? ascendantHouse)
    {
        if (sun is null || moon is null || ascendantHouse is null)
        {
            return "Tu carta ya ofrece una base real para leer identidad, emocionalidad y forma de presencia, aunque algunos factores centrales no esten disponibles.";
        }

        return $"Tu carta combina un Sol en {sun.Sign}, una Luna en {moon.Sign} y un Ascendente en {ascendantHouse.Sign}, formando una personalidad con identidad, sensibilidad y presencia bien diferenciadas.";
    }

    private static string BuildGeneralSummary(
        PlanetPosition? sun,
        PlanetPosition? moon,
        HousePosition? ascendantHouse,
        PlanetPosition? mercury,
        PlanetPosition? venus,
        PlanetPosition? mars)
    {
        var identity = sun is not null
            ? $"la identidad se apoya en {sun.Sign}"
            : "la identidad necesita una lectura solar mas completa";

        var emotional = moon is not null
            ? $"la emocionalidad se mueve desde {moon.Sign}"
            : "la emocionalidad necesita una lectura lunar mas completa";

        var presence = ascendantHouse is not null
            ? $"tu forma de entrar al mundo pasa por {ascendantHouse.Sign}"
            : "la forma de presencia necesita un ascendente disponible";

        var mercuryText = DescribePlanet(mercury, "Mercurio");
        var venusText = DescribePlanet(venus, "Venus");
        var marsText = DescribePlanet(mars, "Marte");

        return $"En conjunto, {identity}, {emotional} y {presence}. En lo mental sobresale {mercuryText}, en lo afectivo {venusText} y en la accion {marsText}, creando un perfil mas rico entre pensamiento, deseo y empuje.";
    }

    private static string DescribePlanet(PlanetPosition? position, string label)
    {
        if (position is null)
        {
            return $"{label.ToLowerInvariant()} no disponible";
        }

        return position.IsRetrograde
            ? $"{label} en {position.Sign} retrogrado"
            : $"{label} en {position.Sign}";
    }

    private static string BuildHouseInterpretation(int houseNumber, ZodiacSign sign)
    {
        if (!InterpretationCatalog.HouseThemes.TryGetValue(houseNumber, out var theme))
        {
            return $"La Casa {houseNumber} en {sign} aporta un tono particular a ese area de experiencia.";
        }

        return $"{theme} En tu carta, {sign} colorea esta casa con su estilo particular.";
    }

    private static string GetHouseTitle(int houseNumber)
    {
        return InterpretationCatalog.HouseTitles.TryGetValue(houseNumber, out var title)
            ? title
            : $"Casa {houseNumber}";
    }
}
