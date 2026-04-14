namespace AstroReader.Application.Interpretations.Premium;

public sealed class PremiumInterpretationCatalogException : Exception
{
    public PremiumInterpretationCatalogException(string message)
        : base(message)
    {
    }

    public PremiumInterpretationCatalogException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
