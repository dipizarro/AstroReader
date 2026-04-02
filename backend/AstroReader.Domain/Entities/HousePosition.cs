using AstroReader.Domain.Enums;

namespace AstroReader.Domain.Entities;

public record HousePosition
{
    public int HouseNumber { get; init; } // 1 a 12
    public ZodiacSign Sign { get; init; }
    public double AbsoluteDegree { get; init; }
}
