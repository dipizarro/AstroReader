namespace AstroReader.Application.Interpretations.Premium;

public interface IInterpretationAnalyzer
{
    InterpretationAnalysisResult Analyze(PremiumInterpretationContext context);
}
