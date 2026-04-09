namespace AstroReader.AstroEngine.Internal;

internal sealed record SwissPlanetCalculation(
    double[] Positions,
    int ReturnFlag,
    string ErrorText
);
