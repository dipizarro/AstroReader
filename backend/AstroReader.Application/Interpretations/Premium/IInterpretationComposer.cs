namespace AstroReader.Application.Interpretations.Premium;

public interface IInterpretationComposer
{
    PremiumInterpretationCompositionResult Compose(PremiumInterpretationContext context, InterpretationAnalysisResult analysis);
}
