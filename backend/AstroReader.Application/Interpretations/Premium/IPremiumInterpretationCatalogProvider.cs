using AstroReader.Domain.Enums;

namespace AstroReader.Application.Interpretations.Premium;

public interface IPremiumInterpretationCatalogProvider
{
    PremiumInterpretationCatalog GetCatalog();

    InterpretationEntry GetEntry(PremiumInterpretationPosition position, ZodiacSign sign);

    TEntry GetEntry<TEntry>(PremiumInterpretationPosition position, ZodiacSign sign)
        where TEntry : InterpretationEntry;
}
