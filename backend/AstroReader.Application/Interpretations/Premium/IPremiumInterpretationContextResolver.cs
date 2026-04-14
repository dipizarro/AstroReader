using AstroReader.Domain.Entities;

namespace AstroReader.Application.Interpretations.Premium;

public interface IPremiumInterpretationContextResolver
{
    PremiumInterpretationContext Resolve(NatalChart chart);
}
