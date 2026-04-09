namespace AstroReader.AstroEngine.Internal;

internal sealed record SwissHouseCalculation(
    double AscendantDegree,
    IReadOnlyDictionary<int, double> HouseCusps
);
