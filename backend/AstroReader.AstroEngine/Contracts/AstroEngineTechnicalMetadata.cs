namespace AstroReader.AstroEngine.Contracts;

public sealed record AstroEngineTechnicalMetadata(
    string CalculationEngine,
    string? HouseSystemCode,
    bool UsesRealEngine,
    bool UsesCustomEphemerisPath,
    string? WrapperVersion
);
