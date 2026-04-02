using AstroReader.Domain.Enums;

namespace AstroReader.Domain.Entities;

public record PlanetPosition
{
    public Planet Planet { get; init; }
    public ZodiacSign Sign { get; init; }
    public double SignDegree { get; init; } // 0 a 30 grados
    public double AbsoluteDegree { get; init; } // 0 a 360 grados
    public bool IsRetrograde { get; init; }
}
