using AstroReader.Domain.Entities;

namespace AstroReader.Application.Interpretations.Premium;

public interface IInterpretationComposer
{
    PremiumInterpretationCompositionResult Compose(NatalChart chart, InterpretationAnalysisResult analysis);
}
