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
        
        // Asume Casa 1 como el Ascendente
        var ascendantSign = chart.Houses.FirstOrDefault(h => h.HouseNumber == 1)?.Sign ?? ZodiacSign.Aries;

        return new ChartInterpretation
        {
            Headline = GetHeadline(sunSign, moonSign, ascendantSign),
            Sun = GetSunInterpretation(sunSign),
            Moon = GetMoonInterpretation(moonSign),
            Ascendant = GetAscendantInterpretation(ascendantSign)
        };
    }

    private string GetHeadline(ZodiacSign sun, ZodiacSign moon, ZodiacSign ascendant)
    {
        return $"Personalidad central estructurada por el Sol en {sun}, con un mundo emocional filtrado por {moon} y un enfoque hacia el mundo guiado por tu Ascendente {ascendant}.";
    }

    private string GetSunInterpretation(ZodiacSign sign) => sign switch
    {
        ZodiacSign.Aries => "Tu Sol en Aries te dota de gran iniciativa, energía impulsiva y vocación de liderazgo.",
        ZodiacSign.Taurus => "Tu Sol en Tauro sugiere constancia, sentido práctico y una fuerte conexión con el mundo material.",
        ZodiacSign.Gemini => "Tu Sol en Géminis te otorga curiosidad insaciable, dualidad mental y gran capacidad de comunicación.",
        ZodiacSign.Cancer => "Tu Sol en Cáncer indica que la familia, la memoria y la protección emocional son el centro de tu identidad.",
        ZodiacSign.Leo => "Tu Sol en Leo brilla con creatividad, necesidad de expresión personal y un carisma innato.",
        ZodiacSign.Virgo => "Tu Sol en Virgo te orienta hacia el servicio, la eficiencia, el análisis detallado y el orden.",
        ZodiacSign.Libra => "Tu Sol en Libra busca siempre el equilibrio, la armonía estética y se define a través de las relaciones.",
        ZodiacSign.Scorpio => "Tu Sol en Escorpio posee una intensidad magnética, buscando la transformación y la verdad oculta.",
        ZodiacSign.Sagittarius => "Tu Sol en Sagitario es el del eterno explorador, buscando la verdad, el optimismo y la filosofía de vida.",
        ZodiacSign.Capricorn => "Tu Sol en Capricornio te dota de ambición, disciplina, estructura y orientación al logro a largo plazo.",
        ZodiacSign.Aquarius => "Tu Sol en Acuario rompe moldes, es original, rebelde y orientado hacia el bienestar colectivo.",
        ZodiacSign.Pisces => "Tu Sol en Piscis te hace profundamente empático, soñador, místico y emocionalmente conectado al todo.",
        _ => "Posición solar indeterminada."
    };

    private string GetMoonInterpretation(ZodiacSign sign) => sign switch
    {
        ZodiacSign.Aries => "Emociones rápidas y reactivas. Necesitas pionerismo para sentirte seguro.",
        ZodiacSign.Taurus => "Emociones estables. Buscas confort, comida y seguridad tangible para nutrirte.",
        ZodiacSign.Gemini => "Tus emociones están atadas a la palabra. Necesitas hablar y analizar lo que sientes.",
        ZodiacSign.Cancer => "Mundo emocional muy rico y protector. Tu hogar es tu mayor refugio.",
        ZodiacSign.Leo => "Necesitas ser valorado y sentir atención afectiva para estar emocionalmente pleno.",
        ZodiacSign.Virgo => "Procesas tus emociones mediante el servicio y entendiendo la lógica de los detalles.",
        ZodiacSign.Libra => "Sientes plenitud cuando hay paz y armonía a tu alrededor. Rehuyes al conflicto.",
        ZodiacSign.Scorpio => "Sientes todo a niveles muy profundos. Emociones extremas, lealtad y pasión.",
        ZodiacSign.Sagittarius => "Seguridad a través de la libertad. Necesitas sentir que no estás atado emocionalmente.",
        ZodiacSign.Capricorn => "Emociones contenidas y responsables. Encuentras paz en tener todo bajo control.",
        ZodiacSign.Aquarius => "Sientes refugio en tus ideas y en tus amistades. Eres un tanto distante emocionalmente.",
        ZodiacSign.Pisces => "Eres una esponja emocional del entorno. Profunda intuición y sensibilidad artística.",
        _ => "Posición lunar indeterminada."
    };

    private string GetAscendantInterpretation(ZodiacSign sign) => sign switch
    {
        ZodiacSign.Aries => "Tu ascendente te hace ver emprendedor, directo y a veces precipitado.",
        ZodiacSign.Taurus => "Proyectas una imagen de paciencia, terquedad pacífica y gozo.",
        ZodiacSign.Gemini => "La gente percibe tu agilidad mental y tu necesidad constante de moverte y conversar.",
        ZodiacSign.Cancer => "Tu carta de presentación es amable, tímida y de alguien que acoge y cuida.",
        ZodiacSign.Leo => "Tu presencia no pasa desapercibida; proyectas autoconfianza y orgullo teatral.",
        ZodiacSign.Virgo => "Luces pulcro, detallista, prudente y siempre dispuesto a ayudar.",
        ZodiacSign.Libra => "Proyectas encanto, diplomacia y tienes la habilidad de socializar agradablemente con todos.",
        ZodiacSign.Scorpio => "Posees un aura misteriosa, penetrante y callada que magnetiza e intriga.",
        ZodiacSign.Sagittarius => "Tu primer acercamiento al mundo es optimista, risueño y muy expansivo.",
        ZodiacSign.Capricorn => "Te presentas maduro, serio, responsable y ganando el respeto casi inmediato.",
        ZodiacSign.Aquarius => "Proyectas excentricidad, intelecto brillante y una independencia radical.",
        ZodiacSign.Pisces => "La primera impresión que das es de suavidad, empatía y una mirada algo perdida o soñadora.",
        _ => "Ascendente indeterminado."
    };
}
