using AstroReader.Domain.Enums;

namespace AstroReader.Domain.ValueObjects;

public record ChartSummary(ZodiacSign Sun, ZodiacSign Moon, ZodiacSign Ascendant);
