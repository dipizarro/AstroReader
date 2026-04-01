using AstroReader.Application.Charts.DTOs;
using AstroReader.Application.Charts.Interfaces;

namespace AstroReader.Application.Charts.UseCases;

public class CalculateNatalChartUseCase : ICalculateNatalChartUseCase
{
    public CalculateChartResponse Execute(CalculateChartRequest request)
    {
        // TODO: Reemplazar este mock por el uso real de AstroEngine y la lógica del dominio
        
        return new CalculateChartResponse
        {
            Summary = new ChartSummary
            {
                Sun = "Tauro",
                Moon = "Leo",
                Ascendant = "Libra"
            },
            Planets =
            [
                new PlanetPosition { Name = "Sun", Sign = "Tauro", Degree = 19.4 },
                new PlanetPosition { Name = "Moon", Sign = "Leo", Degree = 7.8 },
                new PlanetPosition { Name = "Mercury", Sign = "Aries", Degree = 28.1 }
            ],
            Houses =
            [
                new HousePosition { House = 1, Sign = "Libra" },
                new HousePosition { House = 2, Sign = "Escorpio" }
            ],
            Interpretation = new ChartInterpretation
            {
                Headline = "Una personalidad estable con fuerte necesidad de expresión.",
                Sun = "Tu Sol en Tauro sugiere constancia, sentido práctico y conexión con lo tangible.",
                Moon = "Tu Luna en Leo aporta una vida emocional intensa, expresiva y creativa.",
                Ascendant = "Tu Ascendente en Libra te da una presencia armónica, sociable y diplomática."
            }
        };
    }
}
