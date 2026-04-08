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
        var sunSign = chart.Planets.FirstOrDefault(p => p.Planet == Planet.Sun)?.Sign ?? ZodiacSign.Aries;
        var moonSign = chart.Planets.FirstOrDefault(p => p.Planet == Planet.Moon)?.Sign ?? ZodiacSign.Aries;
        var ascendantSign = chart.Houses.FirstOrDefault(h => h.HouseNumber == 1)?.Sign ?? ZodiacSign.Aries;
        var mercurySign = chart.Planets.FirstOrDefault(p => p.Planet == Planet.Mercury)?.Sign ?? ZodiacSign.Aries;
        var venusSign = chart.Planets.FirstOrDefault(p => p.Planet == Planet.Venus)?.Sign ?? ZodiacSign.Aries;
        var marsSign = chart.Planets.FirstOrDefault(p => p.Planet == Planet.Mars)?.Sign ?? ZodiacSign.Aries;
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

        var sunInterpretation = GetInterpretation(InterpretationCatalog.SunBySign, sunSign, "Posicion solar indeterminada.");
        var moonInterpretation = GetInterpretation(InterpretationCatalog.MoonBySign, moonSign, "Posicion lunar indeterminada.");
        var ascendantInterpretation = GetInterpretation(InterpretationCatalog.AscendantBySign, ascendantSign, "Ascendente indeterminado.");
        var mercuryInterpretation = GetInterpretation(InterpretationCatalog.MercuryBySign, mercurySign, "Mercurio indeterminado.");
        var venusInterpretation = GetInterpretation(InterpretationCatalog.VenusBySign, venusSign, "Venus indeterminada.");
        var marsInterpretation = GetInterpretation(InterpretationCatalog.MarsBySign, marsSign, "Marte indeterminado.");
        var summary = GetGeneralSummary(sunSign, moonSign, ascendantSign, mercurySign, venusSign, marsSign);

        return new ChartInterpretation
        {
            Headline = GetHeadline(sunSign, moonSign, ascendantSign),
            Summary = summary,
            GeneralSummary = summary,
            Sun = sunInterpretation,
            Moon = moonInterpretation,
            Ascendant = ascendantInterpretation,
            Mercury = mercuryInterpretation,
            Venus = venusInterpretation,
            Mars = marsInterpretation,
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

    private string GetHeadline(ZodiacSign sun, ZodiacSign moon, ZodiacSign ascendant)
    {
        return $"Tu carta combina un Sol en {sun}, una Luna en {moon} y un Ascendente en {ascendant}, formando una personalidad con identidad, sensibilidad y presencia muy marcadas.";
    }

    private string GetGeneralSummary(
        ZodiacSign sun,
        ZodiacSign moon,
        ZodiacSign ascendant,
        ZodiacSign mercury,
        ZodiacSign venus,
        ZodiacSign mars)
    {
        return $"En conjunto, la identidad se apoya en {sun}, la emocionalidad se mueve desde {moon} y tu forma de entrar al mundo pasa por {ascendant}. En lo mental sobresale Mercurio en {mercury}, en lo afectivo Venus en {venus} y en la accion Marte en {mars}, creando un perfil mas completo entre pensamiento, deseo y empuje.";
    }

    private static string GetInterpretation(
        IReadOnlyDictionary<ZodiacSign, string> catalog,
        ZodiacSign sign,
        string fallback)
    {
        return catalog.TryGetValue(sign, out var interpretation)
            ? interpretation
            : fallback;
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
