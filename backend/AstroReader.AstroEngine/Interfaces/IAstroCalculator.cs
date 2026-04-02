using System;
using System.Collections.Generic;

namespace AstroReader.AstroEngine.Interfaces;

public record EngineCalculationRequest(DateTime UtcDate, double Latitude, double Longitude);

public record RawPlanetData(int PlanetId, double AbsoluteDegree, bool IsRetrograde);
public record RawHouseData(int HouseNumber, double AbsoluteDegree);

public record EngineCalculationResult(
    List<RawPlanetData> Planets, 
    List<RawHouseData> Houses
);

public interface IAstroCalculator
{
    EngineCalculationResult Calculate(EngineCalculationRequest request);
}
