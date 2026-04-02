using AstroReader.Application.Charts.DTOs;
using AstroReader.Domain.Entities;

namespace AstroReader.Application.Interpretations;

public interface IInterpretationEngine
{
    /// <summary>
    /// Genera una interpretación básica estructurada a partir de la carta natal de dominio.
    /// </summary>
    ChartInterpretation GenerateBaseInterpretation(NatalChart chart);
}
