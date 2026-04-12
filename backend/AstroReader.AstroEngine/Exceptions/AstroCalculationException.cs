namespace AstroReader.AstroEngine.Exceptions;

public enum AstroCalculationErrorCode
{
    Configuration,
    Ephemerides,
    Wrapper,
    Calculation
}

/// <summary>
/// Excepción técnica del motor astral. Conserva detalle útil para diagnóstico interno
/// y expone un mensaje seguro para capas superiores.
/// </summary>
public sealed class AstroCalculationException : Exception
{
    public AstroCalculationException(
        AstroCalculationErrorCode code,
        string publicMessage,
        string diagnosticMessage,
        Exception? innerException = null)
        : base(diagnosticMessage, innerException)
    {
        Code = code;
        PublicMessage = publicMessage;
    }

    public AstroCalculationErrorCode Code { get; }

    public string PublicMessage { get; }
}
