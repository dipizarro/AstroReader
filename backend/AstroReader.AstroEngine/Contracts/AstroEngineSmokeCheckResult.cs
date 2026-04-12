using AstroReader.AstroEngine.Exceptions;

namespace AstroReader.AstroEngine.Contracts;

public sealed record AstroEngineSmokeCheckResult
{
    public required bool IsHealthy { get; init; }

    public required string ActiveEngine { get; init; }

    public required string HouseSystem { get; init; }

    public required string EphemerisPath { get; init; }

    public required string Message { get; init; }

    public AstroCalculationErrorCode? ErrorCode { get; init; }

    public bool Skipped { get; init; }
}
