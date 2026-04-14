namespace AstroReader.Application.Interpretations.Premium;

public interface IPremiumInterpretationPreviewUseCase
{
    PremiumInterpretationPreviewResponse Execute(PremiumInterpretationPreviewRequest request);
}
