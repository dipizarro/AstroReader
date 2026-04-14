using AstroReader.Domain.Entities;

namespace AstroReader.Application.Interpretations.Premium;

public interface IInterpretationAnalyzer
{
    InterpretationAnalysisResult Analyze(NatalChart chart);
}
